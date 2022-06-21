define(['knockout', 'jquery', './yearMonthList.vm'],

    function (ko, $, YearMonthList) {

    CardIdentification = function (id, preApprovalStage, reloadApplicationMetadata, isEditable) {
        var self = this;

        this.isEditable = isEditable;

        this.id = id || preApprovalStage().id;
        this.validationErrors = ko.observableArray();
        this.state = ko.observable();
        this.cardNumber = ko.observable();
        this.cardExpireMonths = ko.observable(new YearMonthList(1, 12));
        this.cardExpireMonth = ko.observable();
        this.cardExpireYears = ko.observable(new YearMonthList((new Date()).getFullYear(), 14));
        this.cardExpireYear = ko.observable();
        this.smsCode = ko.observable();

        this.pendingPhoneVerification = ko.pureComputed(function () {
            return !self.isEditable() || preApprovalStage().state() == "PHONE_VERIFICATION_PENDING";
        });

        this.loadData = function (callback) {
            if (callback) {
                callback();
            }
        }

        this.action = function (name, callback) {
            var self = this;
            if (name == 'checkCard') {
                self.checkCard(callback);
            } else if (name == 'sendSMS') {
                self.sendSMS(callback);
            } else if (name == 'submitSMSCode') {
                self.submitSMSCode(callback);
            }
        }
        
        this.validate = function () {
            var self = this;
            self.validationErrors([]);
            if (!self.pendingPhoneVerification()) {
                if (!self.cardNumber() || self.cardNumber().trim() == "") {
                    self.validationErrors.push({ propertyName: 'cardNumber', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
                } 

                if (!self.cardExpireMonth()) {
                    self.validationErrors.push({ propertyName: 'cardExpireMonth', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
                }

                if (!self.cardExpireYear()) {
                    self.validationErrors.push({ propertyName: 'cardExpireYear', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
                }

                if (self.cardNumber() && self.cardNumber().trim() !== "" &&
                    self.cardExpireMonth() && self.cardExpireYear() && self.cardNumber().length < 16) {
                    self.validationErrors.push({ propertyName: 'cardGroup', errorMessage: localization.errors["CARD_NUMBER_FORMAT_ERROR"] });
                }
            } else {
                if (!self.smsCode() || self.smsCode().trim() == "") {
                    self.validationErrors.push({ propertyName: 'smsCode', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
                }
            }

            return (self.validationErrors().length === 0);
        }

        this.checkCard = function (callback) {
            var self = this;
            if (!self.validate()) {
                return;
            }

            var date = new Date(parseInt(this.cardExpireYear()), parseInt(this.cardExpireMonth()), 0);
            
            var cardObj = {
                cardNumber: this.cardNumber(),
                expiryDate: toUtcMidnight(date)
            };
            $.post({
                url: "/api/loan/Applications/" + self.id + "/ValidateCard",
                context: self,
                data: JSON.stringify(cardObj),
                success: function (data) {
                    reloadApplicationMetadata();
                    if (callback) {
                        callback();
                    }
                },
                error: function (err) {
                    self.validationErrors.push({ propertyName: 'cardGroup', errorMessage: err.responseJSON.Message });
                    reloadApplicationMetadata();
                    if (callback) {
                        callback(err);
                    }
                },
                dataType: 'json'
            });
        }

        this.sendSMS = function (callback) {
            var self = this;
            $.post({
                url: "/api/loan/Applications/" + self.id + "/CreditCardAuthorization",
                context: self,
                data: "",
                success: function (data) {
                    this.smsCode("");
                    if (callback) {
                        callback();
                    }
                },
                error: function (err) {
                    self.validationErrors.push({ propertyName: 'SMSGroup', errorMessage: err.responseJSON.Message });
                    reloadApplicationMetadata();
                    if (callback) {
                        callback(err);
                    }
                },
            });
        }

        this.submitSMSCode = function (callback) {
            var self = this;
            if (!self.validate()) {
                callback();
                return;
            }

            $.post({
                url: "/api/loan/Applications/" + self.id + "/CheckCreditCardAuthorization",
                context: self,
                data: self.smsCode(),
                success: function (data) {
                    reloadApplicationMetadata();
                    if (callback) {
                        callback();
                    }
                },
                error: function (err) {
                    self.validationErrors.push({ propertyName: 'SMSGroup', errorMessage: err.responseJSON.Message });
                    reloadApplicationMetadata();
                    if (callback) {
                        callback(err);
                    }
                },
            });
        }
    }

    return CardIdentification;
});