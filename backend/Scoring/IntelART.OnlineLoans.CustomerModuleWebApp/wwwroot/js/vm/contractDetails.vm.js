define([
  "knockout",
  "jquery",
  "./activeCreditCardList.vm",
  "./creditCardTypeList.vm",
  "./bankBranchList.vm",
  "lookupDirectory",
  "../util/notificationDialog",
], function (
  ko,
  $,
  ActiveCreditCardList,
  CreditCardTypeList,
  BankBranchList,
  LookupDirectory,
  NotificationDialog
) {
  ContractDetails = function (
    id,
    preApprovalStage,
    navigationHelper,
    isEditable
  ) {
    var self = this;

    this.isEditable = isEditable;

    this.id = id;
    this.validationErrors = ko.observableArray();
    this.loanType = ko.observable(preApprovalStage().loanType());
    this.state = ko.observable(preApprovalStage().state());
    this.activeCreditCard = ko.observable();
    this.creditCardType = ko.observable();
    this.bankBranch = ko.observable();
    this.activeCreditCards = ko.computed(function () {
      return new ActiveCreditCardList(self.id);
    });

    this.cardDeliveryHintText = ko.computed(function () {
      if (
        self.activeCreditCards() &&
        self.activeCreditCards().items() &&
        self.activeCreditCards().items().length > 0
      ) {
        return localization.strings["STEP4.CARD.HINT.1"];
      } else {
        return localization.strings["STEP4.CARD.HINT.2"];
      }
    });

    this.activeCardListVisible = ko.computed(function () {
      return (
        self.activeCreditCards() &&
        self.activeCreditCards().items() &&
        self.activeCreditCards().items().length > 0
      );
    });

    console.log("activeCardListVisible", this.activeCardListVisible());
    console.log("activeCreditCards()", this.activeCreditCards().items());

    this.creditCardTypes = ko.computed(function () {
      return new CreditCardTypeList(self.id);
    });

    this.bankBranches = ko.computed(function () {
      return new BankBranchList();
    });

    this.newCardCheckbox = ko.observable(false);

    this.orderNewCardFormEnable = ko.computed(function () {
      return self.newCardCheckbox() === false;
    });

    this.isCardDelivered = ko.observable("true");

    this.isCardDeliveryAddressVisible = ko.pureComputed(function () {
      return self.isCardDelivered() === "true";
    });

    this.takeMoneyFromBank = ko.observable("true");
    //TODO: VAS: rename the property, now the meaning is oposite.

    this.willTakeMoneyFromBankVisbility = ko.computed(function () {
      return self.takeMoneyFromBank() === "true";
    });

    this.cardDeliveryAddress = ko.observable();
    this.contractTermsCheckbox = ko.observable(false);

    this.isSubmitButtonEnable = ko.pureComputed(function () {
      return !self.isEditable() || self.contractTermsCheckbox() === false;
    });

    this.isPersonalSheetVisible = ko.computed(function () {
      return preApprovalStage().currency() === "AMD";
    });

    // this.isConverseCustomer = ko.observable(false);
    this.isApplicationEditable = ko.computed(function () {
      return self.state() === "COMPLETED" || self.state() === "AGREED";
    });

    this.personalSheetLink =
      "/api/loan/Applications/" + this.id + "/Documents/DOC_INDIVIDUAL_SHEET";
    this.loanContractLink =
      "/api/loan/Applications/" + this.id + "/Documents/DOC_LOAN_CONTRACT";
    this.arbitrageAgreementLink =
      "/api/loan/Applications/" +
      this.id +
      "/Documents/DOC_ARBITRAGE_AGREEMENT";

    this.isOnlyInCashAvailable = ko.computed(function () {
      if (!self.activeCardListVisible()) {
        self.takeMoneyFromBank("false");
        return true;
      } else {
        self.takeMoneyFromBank("true");
        return false;
      }
    });

    this.loadData = function (callback) {
      var self = this;
      if (
        self.state() === "COMPLETED" ||
        self.state() === "AGREED" ||
        self.state() === "PRE_APPROVAL_SUCCESS"
      ) {
        $.get({
          url: "/api/loan/Applications/" + self.id + "/Agreed",
          context: self,
          success: function (data) {
            if (data) {
              self.contractTermsCheckbox(true);
              if (data.IS_NEW_CARD) {
                self.newCardCheckbox(true);
              }
              self.isCardDelivered(data.IS_CARD_DELIVERY.toString());
              if (data.CARD_DELIVERY_ADDRESS) {
                self.cardDeliveryAddress = ko.observable(
                  data.CARD_DELIVERY_ADDRESS
                );
              }
              if (data.EXISTING_CARD_CODE) {
                self.activeCreditCard = ko.observable(data.EXISTING_CARD_CODE);
              }
              if (data.CREDIT_CARD_TYPE_CODE) {
                self.creditCardType = ko.observable(data.CREDIT_CARD_TYPE_CODE);
              }
              if (data.BANK_BRANCH_CODE) {
                self.bankBranch = ko.observable(data.BANK_BRANCH_CODE);
              }
              if (data.LOAN_GETTING_OPTION_CODE) {
                self.takeMoneyFromBank(
                  data.LOAN_GETTING_OPTION_CODE === "1" ? "false" : "true"
                );
              }

              // $.get({
              //   url:
              //     "/api/loan/Applications/" + self.id + "/IsConverseCustomer",
              //   context: self,
              //   success: function (data) {
              //     self.isConverseCustomer(data);
              //   },
              //   error: function (err) {
              //     if (callback) {
              //       callback(err);
              //     }
              //   },
              //   dataType: "json",
              // });

              // TODO: the page, including the links, should be disabled here
            }
            if (callback) {
              callback();
            }
          },
          dataType: "json",
        });
      } else if (callback) {
        callback();
      }
    };

    this.action = function (name, callback) {
      var self = this;
      if (name == "personalSheetLink" || name == "loanContractLink") {
        self.saveConsumerLoanContract(name, callback);
      } else if (name == "submitContract") {
        self.submitContractDetails(callback);
      }
    };

    this.getDataObject = function () {
      var obj = {
        EXISTING_CARD_CODE: self.activeCreditCard(),
        IS_NEW_CARD: self.newCardCheckbox(),
        CREDIT_CARD_TYPE_CODE: self.creditCardType(),
        IS_CARD_DELIVERY: self.isCardDelivered(),
        CARD_DELIVERY_ADDRESS: self.cardDeliveryAddress(),
        BANK_BRANCH_CODE: self.bankBranch(),
        LOAN_TYPE_ID: self.loanType(),
        LOAN_GETTING_OPTION_CODE: self.takeMoneyFromBank() === "false" ? 1 : 2,
        AGREED_WITH_TERMS: true,
        SUBMIT: false,
      };

      return obj;
    };

    this.saveConsumerLoanContract = function (link, callback) {
      var self = this;
      // maybe not needed
      if (!this.id) {
        return false;
      }
      var url = self[link];
      var obj = {
        EXISTING_CARD_CODE: self.activeCreditCard(),
        IS_NEW_CARD: self.orderNewCardFormEnable(),
        CREDIT_CARD_TYPE_CODE: self.creditCardType(),
        IS_CARD_DELIVERY: self.isCardDeliveryAddressVisible(),
        CARD_DELIVERY_ADDRESS: self.cardDeliveryAddress(),
        BANK_BRANCH_CODE: self.bankBranch(),
        LOAN_TYPE_ID: self.loanType(),
        LOAN_GETTING_OPTION_CODE: self.takeMoneyFromBank() === "false" ? 1 : 2,
        AGREED_WITH_TERMS: true,
        SUBMIT: false,
      };
      $.post({
        url: "/api/loan/Applications/" + this.id + "/Agreed",
        context: self,
        data: JSON.stringify(obj),
        success: function (data) {
          openInNewTab(url);
          if (callback) {
            callback();
          }
        },
        error: function (err) {
          if (callback) {
            callback(err);
          }
        },
      });

      return false;
    };

    this.validate = function () {
      var self = this;

      if (
        !self.newCardCheckbox() &&
        !self.activeCreditCard() &&
        self.willTakeMoneyFromBankVisbility()
      ) {
        self.validationErrors.push({
          propertyName: "activeCreditCard",
          errorMessage: localization.errors["CARD_REQUIRED_ERROR"],
        });
      }

      if (self.newCardCheckbox() && self.willTakeMoneyFromBankVisbility()) {
        if (!self.creditCardType()) {
          self.validationErrors.push({
            propertyName: "creditCardType",
            errorMessage: localization.errors["CARD_TYPE_REQUIRED_ERROR"],
          });
        }

        if (
          self.isCardDeliveryAddressVisible() &&
          (!self.cardDeliveryAddress() ||
            self.cardDeliveryAddress().trim() == "")
        ) {
          self.validationErrors.push({
            propertyName: "cardDeliveryAddress",
            errorMessage: localization.errors["REQUIRED_FIELD_ERROR"],
          });
        }

        if (!self.isCardDeliveryAddressVisible() && !self.bankBranch()) {
          self.validationErrors.push({
            propertyName: "bankBranch",
            errorMessage: localization.errors["BRANCH_REQUIRED_ERROR"],
          });
        }
      }

      return self.validationErrors().length === 0;
    };

    this.submitContractDetails = function (callback) {
      var self = this;
      if (!self.validate()) {
        callback();
        return;
      }

      var obj = {
        EXISTING_CARD_CODE: "",
        IS_NEW_CARD: false,
        CREDIT_CARD_TYPE_CODE: "",
        IS_CARD_DELIVERY: false,
        CARD_DELIVERY_ADDRESS: "",
        LOAN_GETTING_OPTION_CODE: 2,
        BANK_BRANCH_CODE: "",
      };

      if (!self.newCardCheckbox()) {
        if (self.activeCreditCard()) {
          obj.EXISTING_CARD_CODE = self.activeCreditCard();
        }
      } else {
        obj.IS_NEW_CARD = true;

        if (self.creditCardType()) {
          obj.CREDIT_CARD_TYPE_CODE = self.creditCardType();
        }

        obj.IS_CARD_DELIVERY = self.isCardDeliveryAddressVisible();

        if (obj.IS_CARD_DELIVERY) {
          obj.CARD_DELIVERY_ADDRESS = self.cardDeliveryAddress();
        } else {
          obj.BANK_BRANCH_CODE = self.bankBranch();
        }
      }

      obj.AGREED_WITH_TERMS = true;
      obj.LOAN_TYPE_ID = self.loanType();
      obj.SUBMIT = true;
      var dialog = new NotificationDialog({
        message:
          "Կատարվում են բանկային ձևակերպումները, խնդրում ենք սպասել, դա կարող է տևել մի քանի րոպե:",
        title: "Կատարվում է հարցում",
        confirmButtonText: "Ավարտել",
        messageClass: "dialogLoader",
        confirm: function () {
          navigationHelper.navigateToApplicationList();
        },
      });
      dialog.show();

      $.post({
        url: "/api/loan/Applications/" + this.id + "/Agreed",
        context: self,
        data: JSON.stringify(obj),
        success: function (data) {
          var noResponseTimeout = 0;
          var newApplicationRequest = setInterval(function () {
            $.get({
              url: "/api/loan/Applications/" + self.id,
              context: self,
              success: function (data) {
                var hasResult = false;
                if (data && data.STATUS_STATE === "COMPLETED") {
                  hasResult = true;
                  dialog.setMessage(
                    'Վարկի գումարը մուտքագրվել է Ձեր բանկային հաշվին։ Վարկային պայմանագիրը հասանելի է "Հայտեր" էջի "Գործողություն" հատվածում: Շնորհակալություն "Կոնվերս Բանկ" ՓԲԸ-ի ծառայություններից օգտվելու համար։'
                  );
                  dialog.notifyComplete();
                }
                noResponseTimeout += 10000;
                if (!hasResult && noResponseTimeout >= 3 * 1000 * 60) {
                  dialog.setMessage(
                    "Առկա է տեխնիկական խնդիր, խնդրում ենք փորձել ավելի ուշ։ Ձեր հայտը պահպանվել է Հայտեր էջում։"
                  );
                }
                if (callback) {
                  callback();
                }
              },
              dataType: "json",
            });
          }, 10000);
        },
        error: function (err) {
          if (callback) {
            callback(err);
          }
        },
        dataType: "json",
      });
    };
  };

  return ContractDetails;
});
