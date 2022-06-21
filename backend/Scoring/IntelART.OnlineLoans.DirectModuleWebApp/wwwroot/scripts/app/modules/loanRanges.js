define([
  'knockout',
  'wNumb',
  'actions/application.actions',
  'actions/directory.actions',
  'services/helpers',
  'constants/options',
  'components/Modal/index',
], function(ko, wNumb, applicationActions, directoryActions, helpers, OPTIONS, modal) {
  'use strict';

  var CreeditRanges = function CreeditRanges(payload) {
    var self = this;
    var moduleData = payload && payload.data();
    var applicationData = moduleData.applicationData;
    var loanLimits = moduleData.loanLimits;
    var scoringResult = moduleData.scoringResult;
    var parameters = moduleData.parameters;

    var ID = applicationData.ID;
    var getScoringResultByAmount = function(offeredAmount) {
      if (self.generalScoringResults()) {
        var amount;
        if (offeredAmount <= self.minimalAmount() && offeredAmount >= self.minimalAmountLimit()) {
          amount = self.minimalAmount();
        } else {
          amount = self.maximalAmount();
        }

        return self.generalScoringResults().find(function(item) {
          return item.AMOUNT === amount;
        });
      } else {
        return null;
      }
    };
    self.continueLoading = ko.observable(false);
    self.offeredAmountFormat = { mark: '.', thousand: ',', decimals: 0 };
    self.paymentPeriodFormat = { decimals: 3, thousand: '.', suffix: ' ամիս' };
    var offeredFormat = wNumb(self.offeredAmountFormat);
    var periodFormat = wNumb(self.paymentPeriodFormat);
    // api calls
    self.statementDeliveryMethods = directoryActions.getCommunicationTypes;
    self.getBankBranches = directoryActions.getBankBranches;
    self.getActiveCards = function() {
      return applicationActions.getActiveCards(ID);
    };
    self.getCreditCardTypes = function() {
      return applicationActions.getCreditCardTypes(ID);
    };
    self.getMoneyMethods = OPTIONS.GET_MONEY_OPTIONS;
    self.generalScoringResults = ko.observable(scoringResult);
    self.interest = ko.observable(1);
    self.offeredLoanAmount = ko.observable(offeredFormat.from(applicationData.DISPLAY_AMOUNT));
    self.minimalAmountLimit = ko.observable(loanLimits.FROM_AMOUNT);
    self.offeredLoanAmount.subscribe(function(newValue) {
      var scorring = getScoringResultByAmount(offeredFormat.from(newValue));
      if (scorring) {
        self.interest(scorring.INTEREST);
        self.paymentPeriodMax(scorring.TERM_TO);
        self.paymentPeriod(scorring.TERM_TO);
      }
    });
    self.minimalAmount = ko.observable(helpers.getMinValueInArr(scoringResult, 'AMOUNT'));
    self.maximalAmount = ko.observable(helpers.getMaxValueInArr(scoringResult, 'AMOUNT'));
    self.paymentPeriod = ko.observable(24);
    self.paymentPeriodMin = ko.observable(1);
    self.paymentPeriodMax = ko.observable(2);
    self.paymentDay = ko.observable(parameters.REPAYMENT_DAY_FROM);
    self.paymentDayMin = ko.observable(parameters.REPAYMENT_DAY_FROM);
    self.paymentDayMax = ko.observable(parameters.REPAYMENT_DAY_TO);
    self.monthlyRepayment = ko.computed(function() {
      var result = helpers.calc(
        periodFormat.from(self.paymentPeriod()),
        offeredFormat.from(self.offeredLoanAmount()),
        self.interest()
      );
      return helpers.formatMoney(result);
    }, self);

    // values
    var statementDeliveryMethod = ko.observable().extend({ required: true });
    var getLoanMethod = ko.observable().extend({ required: true });

    var getExpertInEmail = ko.observable().extend({
      email: true,
      required: {
        onlyIf: function() {
          return Number(statementDeliveryMethod()) === 1;
        },
      },
    });
    var getExpertInRegAddress = ko.observable().extend({
      required: {
        onlyIf: function() {
          return Number(statementDeliveryMethod()) === 2;
        },
      },
    });
    var getExpertInHomeAddress = ko.observable().extend({
      required: {
        onlyIf: function() {
          return Number(statementDeliveryMethod()) === 3;
        },
      },
    });
    var getMoneyInBank = ko.observable().extend({
      required: {
        onlyIf: function() {
          return Number(getLoanMethod()) === 1;
        },
      },
    });
    var newCardType = ko.observable().extend({
      required: {
        onlyIf: function() {
          return Number(getLoanMethod()) === 2;
        },
      },
    });
    var newCardCode = ko.observable().extend({
      required: {
        onlyIf: function() {
          return Number(getLoanMethod()) === 2;
        },
      },
    });
    var newCardInBank = ko.observable(true);
    var newCardReceiveBankLocation = ko.observable().extend({
      required: {
        onlyIf: function() {
          return Number(getLoanMethod()) === 2 && newCardInBank() === true;
        },
      },
    });
    var newCardReceiveAddress = ko.observable().extend({
      required: {
        onlyIf: function() {
          return Number(getLoanMethod()) === 2 && newCardInBank() === false;
        },
      },
    });
    var getMoneyInExistingCard = ko.observable().extend({
      required: {
        onlyIf: function() {
          return Number(getLoanMethod()) === 3;
        },
      },
    });

    self.values = ko.validatedObservable({
      offeredLoanAmount: self.offeredLoanAmount,
      paymentPeriod: self.paymentPeriod,
      paymentDay: self.paymentDay,
      statementDeliveryMethod: statementDeliveryMethod,
      getLoanMethod: getLoanMethod,
      getExpertInEmail: getExpertInEmail,
      getExpertInRegAddress: getExpertInRegAddress,
      getExpertInHomeAddress: getExpertInHomeAddress,
      getMoneyInBank: getMoneyInBank,
      newCardReceiveBankLocation: newCardReceiveBankLocation,
      newCardInBank: newCardInBank,
      newCardType: newCardType,
      newCardCode: newCardCode,
      getMoneyInExistingCard: getMoneyInExistingCard,
      newCardReceiveAddress: newCardReceiveAddress,
      agreedToTerms: ko.observable(false).extend({
        checked: true,
      }),
    });

    self.openTermModal = function() {
      modal.open({
        viewModel: { template: 'modals/termAndConditionModal' },
        context: self,
      });
    };

    var getLoanGettingCode = function() {
      var loanMethod = Number(self.values().getLoanMethod());
      return loanMethod === 2 || loanMethod === 3 ? 2 : loanMethod;
    };

    var getBankBranchCode = function() {
      return Number(self.values().getLoanMethod()) === 1
        ? self.values().getMoneyInBank()
        : self.values().newCardReceiveBankLocation();
    };

    var getLoanDeliveryDetails = function() {
      var values = self.values();
      return {
        LOAN_GETTING_OPTION_CODE: getLoanGettingCode(),
        COMMUNICATION_TYPE_CODE: values.statementDeliveryMethod(),
        CUSTOMER_EMAIL: values.getExpertInEmail(),
        CURRENT_ADDRESS: values.getExpertInHomeAddress(),
        CUSTOMER_ADDRESS: values.getExpertInRegAddress(),
        EXISTING_CARD_CODE: values.getMoneyInExistingCard(),
        IS_NEW_CARD: Number(values.getLoanMethod()) === 2,
        CREDIT_CARD_TYPE_CODE: values.newCardType(),
        IS_CARD_DELIVERY: !values.newCardInBank(),
        BANK_BRANCH_CODE: getBankBranchCode(),
        CARD_RECOVERY_CODE: values.newCardCode(),
        AGREED_WITH_TERMS: values.agreedToTerms(),
        CARD_DELIVERY_ADDRESS: values.newCardReceiveAddress(),
        SUBMIT: false,
      };
    };

    var getMainApplicationOptions = function() {
      return {
        FINAL_AMOUNT: wNumb(offeredFormat).from(self.offeredLoanAmount()),
        PERIOD_TYPE_CODE: wNumb(periodFormat).from(self.paymentPeriod()),
        REPAY_DAY: Number(self.paymentDay()),
        INTEREST: self.interest(),
        SUBMIT: true,
      };
    };

    self.handleSubmit = function() {
      self.continueLoading(true);
      applicationActions
        .postApplicationFull(ID, {
          id: ID,
          preApprovalApplication: {},
          loanDeliveryDetails: getLoanDeliveryDetails(),
          mainApplication: getMainApplicationOptions(),
        })
        .then(function() {
          applicationActions.postApplicationSubmitted(ID).then(function() {
            self.continueLoading(false);
            payload.setCurrentView('thankYouMsg');
          });
        });
    };
  };

  return CreeditRanges;
});
