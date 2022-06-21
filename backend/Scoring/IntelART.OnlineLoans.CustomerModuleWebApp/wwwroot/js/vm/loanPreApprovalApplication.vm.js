define([
  'knockout',
  'lookupDirectory',
  'jquery',
  './loanTypeList.vm',
  './industriesList.vm',
  './currenciesList.vm',
  '../util/notificationDialog',
], function(
  ko,
  LookupDirectory,
  $,
  LoanTypeList,
  IndustriesList,
  CurrenciesList,
  NotificationDialog
) {
  LoanPreApprovalApplication = function(id, navigationHelper, isEditable) {
    var self = this;
    var autoScoringWaitingMinutes = 3;
    var autoScoringWaitTimeout = autoScoringWaitingMinutes * 60 * 1000;
    var scoringPollingInterval = 1000;

    this.isEditable = isEditable;

    ////var isEditable = function () { return true; }

    ////this.isEditable = ko.pureComputed(
    ////    function () {
    ////        return isEditable();
    ////    }
    ////);

    ////this.SetEditabilityIndicator = function (editabilityIndicator) {
    ////    isEditable = editabilityIndicator;
    ////}

    this.id = id;
    this.validationErrors = ko.observableArray();
    this.loanType = ko.observable();
    this.loanTypeState = ko.observable();
    this.currency = ko.observable();
    this.amount = ko.observable();
    this.nameAm = ko.observable();
    this.lastNameAm = ko.observable();
    this.patronymicNameAm = ko.observable();
    this.birthDate = ko.observable();
    this.state = ko.observable();
    this.industry = ko.observable();
    this.identificationDocument = ko.observable();
    this.expireDayCount = ko.observable();
    this.repeatCount = ko.observable();
    this.repeatDayCount = ko.observable();

    this.loanTypes = ko.observable(new LoanTypeList());
    this.industries = ko.observable(new IndustriesList());
    this.currencies = ko.pureComputed(function() {
      return new CurrenciesList(self.loanType());
    });

    this.agreedToTerms = ko.observable(false);

    var preprarePreapprovalApplicationObject = function() {
      var obj = {
        LOAN_TYPE_ID: self.loanType(),
        INITIAL_AMOUNT: self.amount(),
        CURRENCY_CODE: self.currency(),
        FIRST_NAME_AM: self.nameAm(),
        LAST_NAME_AM: self.lastNameAm(),
        PATRONYMIC_NAME_AM: self.patronymicNameAm(),
        BIRTH_DATE: toUtcMidnight(self.birthDate()),
        SOCIAL_CARD_NUMBER: self.identificationDocument().ssn(),
        DOCUMENT_TYPE_CODE: self.identificationDocument().identificationDocumentTypeCode(),
        DOCUMENT_NUMBER: self.identificationDocument().identificationDocumentNumber(),
        DOCUMENT_GIVEN_DATE: toUtcMidnight(
          self.identificationDocument().identificationDocumentValidFrom()
        ),
        DOCUMENT_EXPIRY_DATE: toUtcMidnight(
          self.identificationDocument().identificationDocumentValidTo()
        ),
        DOCUMENT_GIVEN_BY: self.identificationDocument().identificationDocumentIssuer(),
        ORGANIZATION_ACTIVITY_CODE: self.industry(),
        AGREED_WITH_TERMS: self.agreedToTerms(),
      };

      if (self.id) {
        obj.ID = self.id;
      }

      return obj;
    };

    var polledState = null;

    var waitNotificationConfirm = function() {
      if (polledState) {
        self.state(polledState);
      }
    };

    var checkApplicationStatusState = function(data) {
      var hasResult = false;
      var dialog = null;
      var displayAmount = null;
      var timeout = 0;

      if (data.STATUS_STATE == 'PENDING_PRE_APPROVAL') {
        dialog = new NotificationDialog({
          message: 'Գործընթացը կարող է տևել մի քանի րոպե:',
          title: 'Կատարվում է հարցում՝ խնդրում ենք սպասել',
          confirm: function() {
            waitNotificationConfirm();
          },
          close: function() {
            timeout = 2 * autoScoringWaitTimeout;
            navigationHelper.navigateToApplicationList();
          },
        });
        dialog.show();
      }
      var checkStatusState = function(statusState, refusalReason) {
        polledState = statusState;
        if (dialog) {
          if (statusState == 'PENDING_PRE_APPROVAL') {
            timeout += scoringPollingInterval;
            setTimeout(function() {
              $.get({
                url: '/api/loan/Applications/' + data.ID,
                context: self,
                success: function(data) {
                  if (data) {
                    if (timeout > autoScoringWaitTimeout) {
                      dialog.setButtonVisible('false');
                      dialog.setMessageClass('dialogNotify');
                      dialog.setTitle('Կատարվել է հարցում');
                      dialog.setMessage(
                        'Առկա է տեխնիկական խնդիր, խնդրում ենք փորձել ավելի ուշ։ Ձեր հայտը պահպանվել է Հայտեր էջում։'
                      );
                    } else if (!hasResult) {
                      displayAmount = data.DISPLAY_AMOUNT;
                      checkStatusState(data.STATUS_STATE, data.REFUSAL_REASON);
                    }
                  }
                },
                dataType: 'json',
              });
            }, scoringPollingInterval);
          } else if (statusState == 'PRE_APPROVAL_SUCCESS') {
            hasResult = true;
            dialog.setButtonVisible('true');
            dialog.setMessageClass('dialogSuccess');
            dialog.setTitle('Հաստատում');
            dialog.setMessage(
              'Ձեր հայտը հաստատվել է ' +
                displayAmount +
                ' ՀՀ դրամ գումարի չափով։ Վարկի պայմաններին ծանոթանալու համար խնդրում ենք սեղմել շարունակել կոճակը։ Ձեր հայտն ավտոմատ կչեղարկվի այն լրացնելուց հետո ' +
                self.expireDayCount() +
                '-րդ օրացուցային օրը, ուստի խնդրում ենք ավարտել վարկի ձևակերպման գործընթացը նշված ժամկետում։'
            );
            dialog.notifyComplete();
          } else if (statusState == 'CANCELLED') {
            hasResult = true;
            dialog.setButtonVisible('false');
            dialog.setMessageClass('dialogNotify');
            dialog.setTitle('Չեղարկում');
            dialog.setMessage('Ձեր հայտը չեղարկվել է։');
          } else if (statusState == 'PRE_APPROVAL_FAIL') {
            hasResult = true;
            dialog.setButtonVisible('false');
            dialog.setMessageClass('dialogNotify');
            dialog.setTitle('Մերժում');
            dialog.setMessage(
              'Հարգելի հաճախորդ, Ձեր հայտը ներկայումս չի կարող բավարարվել: Մերժման պատճառը վերացնելուց հետո կարող եք կրկին դիմել։'
            );
          } else if (statusState == 'PRE_APPROVAL_REVIEW') {
            hasResult = true;
            dialog.setButtonVisible('false');
            dialog.setMessageClass('dialogNotify');
            var newdate = new Date(data.CREATION_DATE);
            newdate.setDate(newdate.getDate() + self.expireDayCount());
            var expireDate =
              newdate.getDate() + '/' + (newdate.getMonth() + 1) + '/' + newdate.getFullYear();
            dialog.setTitle('Մոտեցեք մասնաճյուղ մինչև ' + expireDate);
            dialog.setMessage(
              'Վարկի վերաբերյալ որոշում կայացնելու համար անհրաժեշտ են լրացուցիչ տվյալներ։ Խնդրում ենք անձը հաստատող փաստաթղթով, հանրային ծառայության համարանիշով և աշխատավարձի տեղեկանքով մոտենալ "Կոնվերս Բանկ" ՓԲԸ-ի <a href=\'https://www.conversebank.am/hy/branches/\'>ցանկացած մասնաճյուղ</a> ։'
            );
          } else if (statusState == 'NEW' && refusalReason == 'Սխալ փաստաթղթի տվյալներ') {
            hasResult = true;
            dialog.setButtonVisible('true');
            dialog.setConfirmButtonText('Խմբագրել');
            dialog.setMessageClass('dialogNotify');
            dialog.setTitle('Տվյալների անհամապատասխանություն');
            dialog.setMessage(
              'Հարգելի հաճախորդ, Ձեր հայտը չի կարող մշակվել: Խնդրում ենք ստուգել լրացված տվյալների ճշտությունը, խմբագրել և շարունակել։'
            );
            dialog.notifyComplete();
          }
        }
      };
      checkStatusState(data.STATUS_STATE, data.REFUSAL_REASON);
    };

    this.loadData = function(callback) {
      var self = this;
      new LookupDirectory().getGeneralLoanSettings(function(result) {
        self.expireDayCount(result.EXPIRE_DAY_COUNT);
        self.repeatCount(result.REPEAT_COUNT + 1);
        self.repeatDayCount(result.REPEAT_DAY_COUNT);
      });
      if (self.id) {
        $.get({
          url: '/api/loan/Applications/' + self.id,
          context: self,
          success: function(data) {
            if (data) {
              self.loanType = ko.observable(data.LOAN_TYPE_ID);
              self.loanTypeState = ko.observable(data.LOAN_TYPE_STATE);
              self.amount = ko.observable(data.INITIAL_AMOUNT);
              self.currency = ko.observable(data.CURRENCY_CODE);
              self.nameAm(data.FIRST_NAME_AM);
              self.lastNameAm(data.LAST_NAME_AM);
              self.patronymicNameAm(data.PATRONYMIC_NAME_AM);
              self.state(data.STATUS_STATE);
              if (data.BIRTH_DATE) {
                self.birthDate(new Date(data.BIRTH_DATE));
              }
              self.identificationDocument(
                new IdDocumentDetails(
                  data.SOCIAL_CARD_NUMBER,
                  data.DOCUMENT_TYPE_CODE,
                  data.DOCUMENT_NUMBER,
                  data.DOCUMENT_GIVEN_DATE,
                  data.DOCUMENT_EXPIRY_DATE,
                  data.DOCUMENT_GIVEN_BY,
                  self.isEditable
                )
              );
              self.industry(data.ORGANIZATION_ACTIVITY_CODE);
              self.agreedToTerms(data.AGREED_WITH_TERMS);
              checkApplicationStatusState(data);
            }

            if (callback) {
              callback();
            }
          },
          dataType: 'json',
        });
      } else {
        var profile = new UserProfile();
        var self = this;
        profile.loadData(function() {
          if (callback) {
            self.identificationDocument(profile.identificationDocument());
            self.nameAm(profile.nameAm());
            self.lastNameAm(profile.lastNameAm());
            self.patronymicNameAm(profile.patronymicNameAm());
            self.birthDate(profile.birthDate());
            callback();
          }
        });
      }
    };

    this.action = function(name, callback) {
      var self = this;
      if (name == 'save') {
        self.saveData(false, callback);
      } else if (name == 'submit') {
        if (self.validate()) {
          self.saveData(true, callback);
        } else {
          callback();
        }
      }
    };

    this.validate = function() {
      var self = this;
      self.validationErrors([]);

      if (!self.loanType()) {
        self.validationErrors.push({
          propertyName: 'loanType',
          errorMessage: localization.errors['REQUIRED_FIELD_ERROR'],
        });
      }

      if (!self.currency()) {
        self.validationErrors.push({
          propertyName: 'currency',
          errorMessage: localization.errors['REQUIRED_FIELD_ERROR'],
        });
      }

      var isIDSectionValid = self.identificationDocument().validate(true);

      return self.validationErrors().length === 0 && isIDSectionValid;
    };

    this.getDataObject = function() {
      return preprarePreapprovalApplicationObject();
    };

    this.saveData = function(isSubmit, callback) {
      var self = this;
      obj = preprarePreapprovalApplicationObject();
      obj.SUBMIT = isSubmit;
      $.post({
        url: '/api/loan/Applications',
        context: self,
        data: JSON.stringify(obj),
        success: function(id) {
          if (id) {
            self.id = id;
          }

          $.get({
            url: '/api/loan/Applications/' + id,
            context: self,
            success: function(data) {
              checkApplicationStatusState(data);
            },
            error: function(err) {
              this.refresh();
              if (callback) {
                callback(err);
              }
            },
            dataType: 'json',
          });
          if (callback) {
            callback();
          }
        },
        error: function(xhr, status, error) {
          var err = eval('(' + xhr.responseText + ')');
          // this.refresh();
          if (callback) {
            callback(err);
          }
          if (err.ErrorCode == 'ERR-0200') {
            dialog = new NotificationDialog({
              message:
                'Նշված համակարգով ' +
                self.repeatDayCount() +
                ' օրվա ընթացքում հնարավոր է դիմել վարկի միայն ' +
                self.repeatCount() +
                ' անգամ։ Խնդրում ենք վարկային հայտը ուղարկել ' +
                self.repeatDayCount() +
                ' օր անց։ Շնորհակալություն "Կոնվերս Բանկ" ՓԲԸ-ի ծառայություններից օգտվելու համար։',
              title: 'Կատարվել է հարցում',
              messageClass: 'dialogNotify',
              visibleButton: 'false',
              close: function() {
                navigationHelper.navigateToApplicationList();
              },
            });
            dialog.show();
          } else if (err.ErrorCode == 'ERR-0201') {
            dialog = new NotificationDialog({
              message: 'Բանկի աշխատակիցներին արգելվում է օգտվել առցանց վարկավորման համակարգերից:',
              title: 'Կատարվել է հարցում',
              messageClass: 'dialogNotify',
              visibleButton: 'false',
              close: function() {
                navigationHelper.navigateToApplicationList();
              },
            });
            dialog.show();
          }
        },
        dataType: 'json',
      });
    };
  };

  return LoanPreApprovalApplication;
});
