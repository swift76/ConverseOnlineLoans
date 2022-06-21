define([
  "knockout",
  "jquery",
  "./activeCreditCardList.vm",
  "../util/notificationDialog",
], function (ko, $, ActiveCreditCardList, NotificationDialog) {
  return function (id, preApprovalStage, navigationHelper, isEditable) {
    var self = this;
    // var isConverseCustomer = false;

    this.status = ko.observable();

    this.isEditable = isEditable;

    this.id = id;
    this.loanType = ko.observable();

    this.activeCreditCards = ko.computed(function () {
      return new ActiveCreditCardList(self.id);
    });

    this.activeCardListVisible = ko.computed(function () {
      return (
        self.activeCreditCards() &&
        self.activeCreditCards().items() &&
        self.activeCreditCards().items().length > 0
      );
    });

    this.loanAmount = ko.observable();
    this.loanDuration = ko.observable();
    this.interest = ko.observable();
    this.realInterest = ko.observable();
    this.monthlyPayment = ko.observable();
    this.firstPayment = ko.observable();
    this.firstPrincipalAmountPayment = ko.observable();
    this.totalPayment = ko.observable();
    this.totalInterestAmount = ko.observable();

    this.agreedWithTerms = ko.observable(false);

    this.personalSheetLink =
      "/api/loan/Applications/" + this.id + "/Documents/DOC_INDIVIDUAL_SHEET";
    this.loanContractLink =
      "/api/loan/Applications/" + this.id + "/Documents/DOC_LOAN_CONTRACT";

    this.loadData = function (callback) {
      $.get({
        url: "/api/loan/Applications/" + self.id + "/Summary",
        context: self,
        success: function (data) {
          if (data) {
            self.status(data.STATUS_STATE);
            self.loanType(data.LOAN_TYPE_NAME);
            self.loanAmount(numberWithCommas(data.FINAL_AMOUNT));
            self.loanDuration(data.DURATION);
            self.interest(data.INTEREST);
            self.realInterest(data.REAL_INTEREST);
            self.monthlyPayment(numberWithCommas(data.MONTHLY_PAYMENT));
            self.firstPayment(numberWithCommas(data.FIRST_PAYMENT));
            self.firstPrincipalAmountPayment(
              numberWithCommas(data.FIRST_PRINCIPAL_PAYMENT)
            );
            self.totalPayment(numberWithCommas(data.TOTAL_PAYMENT));
            self.totalInterestAmount(
              numberWithCommas(data.TOTAL_INTEREST_PAYMENT)
            );
          }
          //   $.get({
          //     url: "/api/loan/Applications/" + self.id + "/IsConverseCustomer",
          //     context: self,
          //     success: function (data) {
          //       isConverseCustomer = data;
          //     },
          //     error: function (err) {
          //       if (callback) {
          //         callback(err);
          //       }
          //     },
          //     dataType: "json",
          //   });
          if (callback) {
            callback();
          }
        },
        dataType: "json",
      });
    };

    this.action = function (name, callback) {
      if (name == "submitContract") {
        self.submit(callback);
      }
    };

    this.submit = function (callback) {
      var dialog = new NotificationDialog({
        message: localization.strings["MESSAGE.VISIT_BANK_BRANCH"],
        title: "Մոտեցեք մասնաճյուղ",
        messageClass: "dialogSuccess",
        visibleButton: "false",
        close: function () {
          navigationHelper.navigateToApplicationList();
        },
      });
      dialog.show();

      if (self.activeCardListVisible()) {
        dialog.setTitle("Կատարվում է հարցում");
        dialog.setMessageClass("dialogLoader");
        dialog.setMessage(
          "Կատարվում են բանկային ձևակերպումները, խնդրում ենք սպասել, դա կարող է տևել մի քանի րոպե:"
        );
      }

      $.ajax({
        type: "POST",
        url: "/api/loan/Applications/Submitted/" + self.id,
        context: self,
        data: JSON.stringify({}),
        success: function (data) {
          self.loadData(callback);
          if (self.activeCardListVisible()) {
            var noResponseTimeout = 0;
            var newApplicationRequest = setInterval(function () {
              $.get({
                url: "/api/loan/Applications/" + self.id,
                context: self,
                success: function (data) {
                  var hasResult = false;
                  if (data && data.STATUS_STATE == "APPROVAL_REVIEW") {
                    hasResult = true;
                    dialog.setTitle("Կատարվել է հարցում");
                    dialog.setMessageClass("dialogSuccess");
                    dialog.setMessage(
                      localization.strings["MESSAGE.VISIT_BANK_BRANCH"]
                    );
                    clearInterval(newApplicationRequest);
                  } else if (data && data.PRINT_EXISTS) {
                    hasResult = true;
                    dialog.setTitle("Կատարվել է հարցում");
                    dialog.setMessageClass("dialogSuccess");
                    dialog.setMessage(
                      'Վարկի գումարը մուտքագրվել է Ձեր բանկային հաշվին։ Վարկային պայմանագիրը հասանելի է "Հայտեր" էջի "Գործողություն" հատվածում: Շնորհակալություն "Կոնվերս Բանկ" ՓԲԸ-ի ծառայություններից օգտվելու համար։'
                    );
                    clearInterval(newApplicationRequest);
                  }
                  noResponseTimeout += 10000;
                  if (!hasResult && noResponseTimeout >= 5 * 1000 * 60) {
                    dialog.setTitle("Կատարվել է հարցում");
                    dialog.setMessageClass("dialogNotify");
                    dialog.setMessage(
                      "Առկա է տեխնիկական խնդիր, խնդրում ենք փորձել ավելի ուշ։ Ձեր հայտը պահպանվել է Հայտեր էջում։"
                    );
                    clearInterval(newApplicationRequest);
                  }
                  if (callback) {
                    callback();
                  }
                },
                dataType: "json",
              });
            }, 10000);
          }
        },
        error: function (data) {
          if (callback) {
            callback(data);
          }
        },
      });
    };
  };
});
