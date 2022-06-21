define([
  'knockout',
  'actions/application.actions',
  'actions/loanLimits.actions',
  'constants/statuses',
], function(ko, applicationActions, loanActions, STATUSES) {
  'use strict';

  var ProcessingLoan = function Processing(payload) {
    var self = this;
    var moduleData = payload.data();
    var hasResult = false;
    var autoScoringWaitingMinutes = 3;
    var autoScoringWaitTimeout = autoScoringWaitingMinutes * 60 * 1000;
    var scoringPollingInterval = 1000;
    var ID = moduleData.applicationId;

    self.continueLoading = ko.observable(false);
    self.applicationData = {};
    self.viewStatus = ko.observable('loading');
    self.title = ko.observable('Ձեր հարցումը մշակվում է');
    self.msg = ko.observable('');
    self.displayAmount = ko.observable();

    self.continueLoan = function continueLoan() {
      var appData = self.applicationData;
      self.continueLoading(true);
      Promise.all([
        loanActions.getLoanLimits(appData.LOAN_TYPE_ID, appData.CURRENCY_CODE),
        applicationActions.getGeneralScoringResults(appData.ID),
        applicationActions.getParameters(appData.LOAN_TYPE_ID),
      ]).then(function(values) {
        self.continueLoading(false);
        payload.setCurrentView('loanRanges', {
          applicationData: appData,
          loanLimits: values[0],
          scoringResult: values[1],
          parameters: values[2],
        });
      });
    };

    var setRejectedView = function(title, refusalReason) {
      self.viewStatus('rejected');
      self.title(title || 'ՎԱՐԿԸ ՄԵՐԺՎԱԾ Է');
      self.msg(refusalReason || 'Մանրամասների համար խնդրում ենք կապ հաստատել մեզ հետ:');
    };

    var setApprovedView = function() {
      self.viewStatus('approved');
      self.title('Հաստատվել է վարկի առավելագույն սահմանաչափ');
    };

    var getApplicationById = function() {
      return applicationActions.getApplicationById(ID);
    };

    getApplicationById().then(function(response) {
      var timeout = 0;

      var checkLoanStatus = function(data) {
        var status = data.STATUS_STATE;
        var refusalReason = data.REFUSAL_REASON;
        switch (status) {
          case STATUSES.PENDING_PRE_APPROVAL:
            timeout += scoringPollingInterval;
            setTimeout(function() {
              getApplicationById().then(function(response) {
                if (timeout > autoScoringWaitTimeout) {
                  setRejectedView(
                    'Կատարվել է հարցում',
                    'Առկա է տեխնիկական խնդիր, խնդրում ենք փորձել ավելի ուշ։'
                  );
                } else if (!hasResult) {
                  self.displayAmount(response.DISPLAY_AMOUNT);
                  checkLoanStatus(response);
                }
              });
            }, scoringPollingInterval);
            break;
          case STATUSES.PRE_APPROVAL_SUCCESS:
            hasResult = true;
            self.applicationData = data;
            setApprovedView();
            break;
          case STATUSES.PRE_APPROVAL_FAIL:
          case STATUSES.PRE_APPROVAL_REVIEW:
            hasResult = true;
            setRejectedView(null, refusalReason);
            break;
          case STATUSES.NEW:
            if (refusalReason === '') hasResult = true;
            setRejectedView(null, refusalReason);
            break;
          default:
            break;
        }
      };

      checkLoanStatus(response);
    });
  };

  return ProcessingLoan;
});
