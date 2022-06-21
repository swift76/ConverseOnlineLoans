define(['knockout',
    'jquery',
    './loanMainApplication.vm',
    './activeCreditCardList.vm',
    './creditCardTypeList.vm',
    './bankBranchList.vm',
    './collectionWrapperList.vm',
    './contractDetails.vm',
    './statementDeliveryMethodList.vm'
],

    function (ko, $, LoanMainApplication, ActiveCreditCardList, CreditCardTypeList, BankBranchList, CollectionWrapperList, ContractDetails, StatementDeliveryMethodList) {
        return function (id, preApprovalApplication, isEditable) {
            var self = this;

            this.isEditable = isEditable;

            this.id = id || preApprovalApplication().id;
            this.validationErrors = ko.observableArray();
            this.preApprovalApplication = preApprovalApplication();
            this.mainApplication = new LoanMainApplication(id, preApprovalApplication, isEditable);
            this.loanDeliveryDetails = new ContractDetails(id, preApprovalApplication, null, isEditable);
            self.mainApplication.interest(null);
            self.mainApplication.loanPaymentPeriods(new CollectionWrapperList([]));

            this.statementDeliveryMethods = ko.observable(new StatementDeliveryMethodList());
            this.statementDeliveryMethod = ko.observable();

            this.averageMonthlyPayment = ko.observable();

            this.currencyName = ko.computed(function () {
                var currencyText = "";
                var currencies = self.preApprovalApplication.currencies();
                if (currencies.items()) {
                    for (var i = 0; i < currencies.items().length; i++) {
                        if (currencies.items()[i].CODE == self.preApprovalApplication.currency()) {
                            currencyText = currencies.items()[i].NAME;
                            break;
                        }
                    }
                }

                return currencyText;
            });
            var lastAmount = null;
            var lastInterest = null;
            var lastPeriod = null;
            var minimumAmountLimit = null;

            var currentServiceAmount = 0;
            var currentServiceInterest = 0;

            var opHandler = Math.random();

            ko.computed(function () {
                var amount = self.mainApplication.offeredLoanAmount();
                var interest = self.mainApplication.interest();
                var period = self.mainApplication.loanPaymentPeriod();
                var serviceInterest = currentServiceInterest;
                var serviceAmount = currentServiceAmount;
                if (amount && interest && period
                    && amount != null && interest != null && period != null) {
                    if (!(amount == lastAmount && interest == lastInterest && period == lastPeriod)) {
                        var loanTypeCode = self.preApprovalApplication.loanType();
                        var currency = self.preApprovalApplication.currency();
                        var requestId = Math.random();
                        opHandler = requestId;
                        $.get({
                            url: "/api/loan/RepaymentSchedule?loanTypeCode=" + loanTypeCode
                                + "&currency=" + currency
                                + "&interest=" + interest
                                + "&amount=" + amount
                                + "&duration=" + period
                                + "&serviceInterest=" + serviceInterest
                                + "&serviceAmount=" + serviceAmount,
                            context: self,
                            success: function (data) {
                                if (opHandler == requestId) {
                                    self.averageMonthlyPayment(numberWithCommas(data.MONTHLY_PAYMENT_AMOUNT) + "  " + self.currencyName());
                                }
                            },
                            dataType: 'json'
                        });
                    }
                }
                else {
                    opHandler = Math.random();
                    self.averageMonthlyPayment(null);
                }
                lastAmount = amount;
                lastInterest = interest;
                lastPeriod = period;
            });

            this.interest = ko.pureComputed(function () {
                var interest = null;
                var scoringResults = self.mainApplication.generalScoringResults();
                var amount = self.mainApplication.offeredLoanAmount();
                if (!amount) {
                    self.mainApplication.offeredLoanAmount(Number(self.maximumAmount().replace(/,/g, '')));
                    amount = self.mainApplication.offeredLoanAmount();
                }

                amount = Number(amount);

                if (amount
                    && scoringResults
                    && scoringResults.length) {
                    if (amount <= Number(self.maximumAmount().replace(/,/g, ''))) {
                        var index = null;
                        for (var i = 0; i < scoringResults.length; i++) {
                            if (amount <= scoringResults[i].AMOUNT) {
                                index = i;
                                break;
                            }
                        }
                        if (index == null) {
                            interest = null;
                            currentServiceAmount = null;
                            currentServiceInterest = null;
                        }
                        else {
                            interest = scoringResults[index].INTEREST;
                            currentServiceAmount = scoringResults[index].SERVICE_AMOUNT;
                            currentServiceInterest = scoringResults[index].SERVICE_INTEREST;
                        }
                    }
                }
                self.mainApplication.interest(interest);
                return interest;
            });

            this.maximumAmount = ko.pureComputed(function () {
                var value = 0;

                if (self.mainApplication) {
                    var scoringResults = self.mainApplication.generalScoringResults();
                    if (scoringResults
                        && scoringResults.length) {

                        for (var i = 0; i < scoringResults.length; i++) {
                            if (scoringResults[i].AMOUNT > value) {
                                value = scoringResults[i].AMOUNT;
                            }
                        }
                    }
                }
                return numberWithCommas(value);
            });

            this.cardDeliveryAddressOption = ko.observable();

            this.loadData = function (callback) {
                self.mainApplication.loadData(function () {
                    self.loanDeliveryDetails.loadData(callback);
                    self.statementDeliveryMethod(self.mainApplication.statementDeliveryMethod());
                });

                (function () {
                    $.get({
                        url: "/api/loan/LoanLimits?loanTypeCode=" + self.preApprovalApplication.loanType() + "&currency=" + self.preApprovalApplication.currency(),
                        context: self,
                        success: function (data) {
                            minimumAmountLimit = numberWithCommas(data.FROM_AMOUNT);
                        },
                        dataType: 'json'
                    });
                })();
            }

            this.validate = function (propertyName, model) {

                //If validation is done through one field, do not remove other fields` previous validation error messages
                if (propertyName)
                {
                    self.validationErrors(self.validationErrors().filter(el => el.propertyName != propertyName));
                    self.mainApplication.validationErrors(self.mainApplication.validationErrors().filter(el => el.propertyName != propertyName));
                    self.loanDeliveryDetails.validationErrors(self.loanDeliveryDetails.validationErrors().filter(el => el.propertyName != propertyName));
                    self.mainApplication.userProfile.validationErrors(self.mainApplication.userProfile.validationErrors().filter(el => el.propertyName != propertyName));
                }
                else
                {
                    self.validationErrors([]);
                    self.mainApplication.validationErrors([]);
                    self.loanDeliveryDetails.validationErrors([]);
                    self.mainApplication.userProfile.validationErrors([]);
                }

                if (!propertyName || propertyName == 'selectedPaymentPeriod')
                {
                    if (!self.mainApplication.loanPaymentPeriod()) {
                        self.mainApplication.validationErrors.push({ autoFocusDisabled: propertyName, propertyName: 'selectedPaymentPeriod', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
                    } else if (self.mainApplication.loanPaymentPeriod() && (self.mainApplication.loanPaymentPeriod() < self.mainApplication.scoringResult.TERM_FROM || self.mainApplication.loanPaymentPeriod() > self.mainApplication.scoringResult.TERM_TO)) {
                        self.mainApplication.validationErrors.push({ autoFocusDisabled: propertyName, propertyName: 'selectedPaymentPeriod', errorMessage: localization.errors["ALLOW_FIELD_ERROR"] + self.mainApplication.scoringResult.TERM_FROM + " - " + self.mainApplication.scoringResult.TERM_TO });
                    }
                }

                //TODO: Do we really need validation for this ?
                if (!propertyName || propertyName == 'selectedInterest')
                {
                    if (!self.mainApplication.interest()) {
                        self.mainApplication.validationErrors.push({ autoFocusDisabled: propertyName, propertyName: 'selectedInterest', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
                    }
                }

                if (!propertyName || propertyName == 'paymentDayMissing')
                {
                    if (!self.mainApplication.isRepayDayFixed()) {
                        if (!self.mainApplication.paymentDay()) {
                            self.mainApplication.validationErrors.push({ autoFocusDisabled: propertyName, propertyName: 'paymentDayMissing', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
                        } else if (self.mainApplication.repaymentDayFrom && self.mainApplication.repaymentDayTo &&
                            ((parseInt(self.mainApplication.paymentDay()) > parseInt(self.mainApplication.repaymentDayTo)) || (parseInt(self.mainApplication.paymentDay()) < parseInt(self.mainApplication.repaymentDayFrom)))) {
                            self.mainApplication.validationErrors.push({ autoFocusDisabled: propertyName, propertyName: 'paymentDayMissing', errorMessage: localization.errors["ALLOW_FIELD_ERROR"] + self.mainApplication.repaymentDayFrom + " - " + self.mainApplication.repaymentDayTo });
                        }
                    }
                }

                var minimumAmountNumber = parseInt(minimumAmountLimit.replace(/,/g, ""), 10);
                var maximumAmountNumber = parseInt(self.maximumAmount().replace(/,/g, ""), 10);

                if (!propertyName || propertyName == 'offeredLoanAmount')
                {
                    if (!self.mainApplication.offeredLoanAmount()) {
                        self.mainApplication.validationErrors.push({ autoFocusDisabled: propertyName, propertyName: 'offeredLoanAmount', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
                    } else if (minimumAmountNumber && maximumAmountNumber &&
                        ((parseInt(self.mainApplication.offeredLoanAmount()) > maximumAmountNumber) || (parseInt(self.mainApplication.offeredLoanAmount()) < minimumAmountNumber))) {
                        self.mainApplication.validationErrors.push({ autoFocusDisabled: propertyName, propertyName: 'offeredLoanAmount', errorMessage: localization.errors["ALLOW_FIELD_ERROR"] + minimumAmountLimit + " - " + self.maximumAmount() });
                    }
                }

                if (!propertyName || propertyName == 'birthPlace') {
                    if (!self.mainApplication.userProfile.birthCountry()) {
                        self.mainApplication.userProfile.validationErrors.push({ autoFocusDisabled: propertyName, propertyName: 'birthPlace', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
                    }
                }

                if (!propertyName || propertyName == 'citizenship')
                {
                    if (!self.mainApplication.userProfile.citizenship()) {
                        self.mainApplication.userProfile.validationErrors.push({ autoFocusDisabled: propertyName, propertyName: 'citizenship', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
                    }
                }

                if (!propertyName || propertyName == 'email')
                {
                    if (!self.mainApplication.userProfile.email()) {
                        self.mainApplication.userProfile.validationErrors.push({ autoFocusDisabled: propertyName, propertyName: 'email', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
                    } else if (!isValidEmailAddress(self.mainApplication.userProfile.email())) {
                        self.mainApplication.userProfile.validationErrors.push({ autoFocusDisabled: propertyName, propertyName: 'email', errorMessage: localization.errors["EMAIL_FORMAT_ERROR"] });
                    }
                }

                //residentialAddress
                if (!propertyName || ((propertyName == 'streetName' || propertyName == 'buildingNumber')
                    && model == self.mainApplication.userProfile.residentialAddress()))
                {
                    var isResidentalAddressValid = self.mainApplication.userProfile.residentialAddress().validate(true, propertyName);
                    if (!isResidentalAddressValid) {
                        // expand address area if there is any validation error
                        self.mainApplication.isShowAddres1(true);
                    }
                }

                //homeAddress
                if (!propertyName || ((propertyName == 'streetName' || propertyName == 'buildingNumber')
                    && model == self.mainApplication.userProfile.homeAddress())) {
                    var isHomeAddressValid = self.mainApplication.userProfile.homeAddress().validate(true, propertyName);
                    if (!isHomeAddressValid) {
                        // expand address area if there is any validation error
                        self.mainApplication.isShowAddres2(true);
                    }
                }

                var isDeliveryDetailsValid = self.loanDeliveryDetails.validate();

                return (self.validationErrors().length === 0) &&
                    (self.mainApplication.validationErrors().length === 0) &&
                    (self.mainApplication.userProfile.validationErrors().length === 0) &&
                    isResidentalAddressValid &&
                    isHomeAddressValid &&
                    isDeliveryDetailsValid;

            }

            this.saveData = function (callback, isSubmiting) {
                var self = this;
                if (isSubmiting) {
                    if (!self.validate()) {
                        if (callback && (typeof callback == "function"))
                            callback();
                        return;
                    }
                }

                self.mainApplication.statementDeliveryMethod(self.statementDeliveryMethod());

                obj = {
                    id: this.id,
                    cardDeliveryAddressOption: this.cardDeliveryAddressOption(),
                    preApprovalApplication: this.preApprovalApplication.getDataObject(),
                    loanDeliveryDetails: this.loanDeliveryDetails.getDataObject(),
                    mainApplication: this.mainApplication.getDataObject()
                }
                obj.mainApplication.SUBMIT = isSubmiting;

                $.post({
                    url: "/api/loan/Applications/" + self.id + "/Full",
                    context: self,
                    data: JSON.stringify(obj),
                    success: function (data) {
                        $.get({
                            url: "/api/loan/Applications/" + self.id,
                            context: self,
                            success: function (data) {
                                preApprovalApplication().state(data.STATUS_STATE)
                            },
                            error: function (err) {
                                this.refresh();
                                if (callback) {
                                    callback(err);
                                }
                            },
                            dataType: 'json'
                        });

                        if (callback) {
                            callback();
                        }
                    },
                    error: function (err) {
                        if (callback) {
                            callback(err);
                        }
                    },
                    //dataType: 'json'
                });
            }
        }
    });

