define([
  'knockout',
  'jquery',
  './loanTypeList.vm',
  './industriesList.vm',
  './currenciesList.vm',
  './listInitialCaption.vm',
  './collectionWrapperList.vm',
  'helpers',
  'lookupDirectory',
  '../util/notificationDialog',
], function(
  ko,
  $,
  LoanTypeList,
  IndustriesList,
  CurrenciesList,
  ListInitialCaption,
  CollectionWrapperList,
  helpers,
  LookupDirectory,
  NotificationDialog
) {
  LoanMainApplication = function(id, preApprovalStage, isEditable) {
    var self = this;

    this.isEditable = isEditable;

    this.id = id || preApprovalStage().id;
    this.state = ko.observable();
    this.loanPaymentPeriods = ko.observable();

    this.loanPaymentPeriod = ko.observable();
    this.loanPaymentPeriodDisabled = ko.observable();
    this.validationErrors = ko.observableArray();
    var userProfile = new UserProfile(this.isEditable);
    this.loanType = ko.observable();

    this.statementDeliveryMethod = ko.observable();

    this.clientCode = ko.observable();
    this.isDataComplete = ko.observable();
    this.listInitialCaption = ko.observable(new ListInitialCaption(''));
    this.agreedToTerms = ko.observable(false);
    this.isPassportUploaded = ko.observable(false);
    this.isSocCardUploaded = ko.observable(false);
    this.passportUrl = ko.observable('');
    this.isSocCardUrl = ko.observable('');
    this.isRepayDayFixed = ko.observable(false);
    this.scoringResult = ko.observable();
    this.scoringResultAllowText = ko.observable();
    this.paymentDayAllowText = ko.observable('');
    this.generalScoringResults = ko.observable();
    this.repaymentDayFrom = null;
    this.repaymentDayTo = null;

    this.isShowAddres1 = ko.observable(false);
    this.isShowAddres2 = ko.observable(false);

    this.isSameAddress = ko.observable(false);

    this.toggleAddres1 = function() {
      this.isShowAddres1(!this.isShowAddres1());
    };
    this.toggleAddres2 = function() {
      this.isShowAddres2(!this.isShowAddres2());
    };

    ko.computed(function() {
      if (self.isSameAddress() === true) {
        //copy residential address to home address
        userProfile.homeAddress().countryCode(userProfile.residentialAddress().countryCode());
        userProfile.homeAddress().stateCode(userProfile.residentialAddress().stateCode());
        userProfile.homeAddress().cityCode(userProfile.residentialAddress().cityCode());
        userProfile.homeAddress().streetName(userProfile.residentialAddress().streetName());
        userProfile.homeAddress().buildingNumber(userProfile.residentialAddress().buildingNumber());
        userProfile
          .homeAddress()
          .apartmentNumber(userProfile.residentialAddress().apartmentNumber());
      }
    });

    let getDocumentUrl = function(id, documentTypeCode) {
      return '/api/loan/Applications/' + self.id + '/Documents/' + documentTypeCode;
    };

    this.wasDocumentUploaded = function(id, callback) {
      $.get({
        url: '/api/loan/Applications/' + id + '/Documents',
        context: self,
        success: function(data) {
          if (data) {
            self.isPassportUploaded(data.indexOf('DOC_PASSPORT') >= 0);
            self.isSocCardUploaded(data.indexOf('DOC_SOC_CARD') >= 0);
            if (self.isPassportUploaded()) {
              self.passportUrl(getDocumentUrl(id, 'DOC_PASSPORT'));
            }

            if (self.isSocCardUploaded()) {
              self.isSocCardUrl(getDocumentUrl(id, 'DOC_SOC_CARD'));
            }

            if (callback) {
              callback(true);
            }
          }
        },

        dataType: 'json',
      });
    };

    this.uploadSuccess = function() {
      self.wasDocumentUploaded(id);
    };

    this.uploadError = function(error) {
      if (!error) {
        return;
      }

      self.fileMaxSizeExceeded();
    };

    let restrictions = { fileMaxSize: FILE_MAX_SIZE };

    this.passportUploader = new helpers.FileUploader(
      getDocumentUrl(id, 'DOC_PASSPORT'),
      {
        uploadSuccess: this.uploadSuccess,
        uploadError: this.uploadError,
      },
      restrictions
    );

    this.socCardUploader = new helpers.FileUploader(
      getDocumentUrl(id, 'DOC_SOC_CARD'),
      {
        uploadSuccess: this.uploadSuccess,
        uploadError: this.uploadError,
      },
      restrictions
    );

    // #region english-name-surname

    this.userProfile = userProfile;
    // #endregion

    var buildLoanMainApplication = function(data) {
      if (data) {
        if (data.STATUS_STATE) {
          self.state(data.STATUS_STATE);
        }

        if (data.INTEREST) {
          self.interest(data.INTEREST);
        }

        if (data.PERIOD_TYPE_CODE) {
          self.loanPaymentPeriod(data.PERIOD_TYPE_CODE);
        }

        if (data.REPAY_DAY) {
          self.paymentDay(data.REPAY_DAY);
          self.paymentDayAllowText(self.repaymentDayFrom + ' - ' + self.repaymentDayTo);
        }

        if (data.COMMUNICATION_TYPE_CODE) {
          self.statementDeliveryMethod(data.COMMUNICATION_TYPE_CODE);
        }

        // #region filling in user profile data
        self.userProfile.nameEn(data.FIRST_NAME_EN);
        self.userProfile.lastNameEn(data.LAST_NAME_EN);
        self.userProfile.birthCountry(data.BIRTH_PLACE_CODE);
        self.userProfile.citizenship(data.CITIZENSHIP_CODE);
        self.userProfile.cellphone(data.MOBILE_PHONE_1);
        self.userProfile.phone(data.FIXED_PHONE);
        self.userProfile.email(data.EMAIL);
        self.userProfile.residentialAddress().countryCode(data.REGISTRATION_COUNTRY_CODE);
        self.userProfile.residentialAddress().stateCode(data.REGISTRATION_STATE_CODE);
        self.userProfile.residentialAddress().cityCode(data.REGISTRATION_CITY_CODE);
        self.userProfile.residentialAddress().streetName(data.REGISTRATION_STREET);
        self.userProfile.residentialAddress().buildingNumber(data.REGISTRATION_BUILDNUM);
        self.userProfile.residentialAddress().apartmentNumber(data.REGISTRATION_APARTMENT);
        self.userProfile.homeAddress().countryCode(data.CURRENT_COUNTRY_CODE);
        self.userProfile.homeAddress().stateCode(data.CURRENT_STATE_CODE);
        self.userProfile.homeAddress().cityCode(data.CURRENT_CITY_CODE);
        self.userProfile.homeAddress().streetName(data.CURRENT_STREET);
        self.userProfile.homeAddress().buildingNumber(data.CURRENT_BUILDNUM);
        self.userProfile.homeAddress().apartmentNumber(data.CURRENT_APARTMENT);
        self.userProfile.familyStatus().familyStatusCode(data.FAMILY_STATUS_CODE);
        self.userProfile.workDetails().companyName(data.COMPANY_NAME);
        self.userProfile.workDetails().companyPhone(data.COMPANY_PHONE);
        self.userProfile.workDetails().position(data.POSITION);
        self.userProfile.workDetails().incomeRangeCode(data.MONTHLY_INCOME_CODE);
        self.userProfile.workDetails().experienceDurationCode(data.WORKING_EXPERIENCE_CODE);

        self.userProfile.InitialEmail(data.EMAIL);

        if (data.FINAL_AMOUNT) {
          self.offeredLoanAmount(data.FINAL_AMOUNT);
        }

        // #endregion filling in user profile data
      }
    };

    this.loadData = function(callback) {
      var self = this;
      if (self.id) {
        $.get({
          url: '/api/loan/Applications/' + self.id + '/Main',
          context: self,
          success: function(data) {
            self.userProfile.loadData(function() {
              buildLoanMainApplication(data);

              if (
                self.userProfile.homeAddress().streetName() == null ||
                self.userProfile.homeAddress().buildingNumber() == null ||
                self.userProfile.homeAddress().apartmentNumber() == null
              ) {
                self.isShowAddres2(true);
              }

              if (
                self.userProfile.residentialAddress().streetName() == null ||
                self.userProfile.residentialAddress().buildingNumber() == null ||
                self.userProfile.residentialAddress().apartmentNumber() == null
              ) {
                self.isShowAddres1(true);
              }

              if (callback) {
                callback(false);
              }
            });

            self.wasDocumentUploaded(self.id);
            if (callback) {
              callback(false);
            }
          },
          dataType: 'json',
        });

        (function() {
          $.get({
            url: '/api/loan/Applications/' + self.id + '/GeneralScoringResults',
            context: self,
            success: function(data) {
              self.generalScoringResults(data);
            },
            dataType: 'json',
          });
        })();

        $.get({
          url: '/api/loan/Applications/' + self.id,
          context: self,
          success: function(data) {
            if (data) {
              self.preApprovalApplicationData = data;

              if (self.preApprovalApplicationData.CLIENT_CODE) {
                self.clientCode(self.preApprovalApplicationData.CLIENT_CODE.replace(/\s/g, ''));
              }

              if (self.preApprovalApplicationData.IS_DATA_COMPLETE) {
                self.isDataComplete(data.IS_DATA_COMPLETE);
              }

              if (self.preApprovalApplicationData.LOAN_TYPE_ID) {
                self.loanType(data.LOAN_TYPE_ID);
                (function() {
                  new LookupDirectory().getLoanParameters(self.loanType(), function(parameters) {
                    self.isRepayDayFixed(parameters.IS_REPAY_DAY_FIXED);
                    self.repaymentDayFrom = parameters.REPAYMENT_DAY_FROM;
                    self.repaymentDayTo = parameters.REPAYMENT_DAY_TO;
                    self.paymentDayAllowText(self.repaymentDayFrom + ' - ' + self.repaymentDayTo);
                  });
                })();
              }
            }
          },

          dataType: 'json',
        });
      }
    };

    var prepareMainApplicationObject = function() {
      var obj = {
        FINAL_AMOUNT: self.offeredLoanAmount(),
        INTEREST: self.interest(),
        FIRST_NAME_EN: self.userProfile.nameEn(),
        LAST_NAME_EN: self.userProfile.lastNameEn(),
        REPAY_DAY: self.paymentDay(),
        PERIOD_TYPE_CODE: self.loanPaymentPeriod(),
        BIRTH_PLACE_CODE: self.userProfile.birthCountry(),
        CITIZENSHIP_CODE: self.userProfile.citizenship(),
        MOBILE_PHONE_1: self.userProfile.cellphone(),
        FIXED_PHONE: self.userProfile.phone(),
        EMAIL: self.userProfile.email(),
        REGISTRATION_COUNTRY_CODE: self.userProfile.residentialAddress().countryCode(),
        REGISTRATION_STATE_CODE: self.userProfile.residentialAddress().stateCode(),
        REGISTRATION_CITY_CODE: self.userProfile.residentialAddress().cityCode(),
        REGISTRATION_STREET: self.userProfile.residentialAddress().streetName(),
        REGISTRATION_BUILDNUM: self.userProfile.residentialAddress().buildingNumber(),
        REGISTRATION_APARTMENT: self.userProfile.residentialAddress().apartmentNumber(),
        CURRENT_COUNTRY_CODE: self.userProfile.homeAddress().countryCode(),
        CURRENT_STATE_CODE: self.userProfile.homeAddress().stateCode(),
        CURRENT_CITY_CODE: self.userProfile.homeAddress().cityCode(),
        CURRENT_STREET: self.userProfile.homeAddress().streetName(),
        CURRENT_BUILDNUM: self.userProfile.homeAddress().buildingNumber(),
        CURRENT_APARTMENT: self.userProfile.homeAddress().apartmentNumber(),
        COMPANY_NAME: self.userProfile.workDetails().companyName(),
        COMPANY_PHONE: self.userProfile.workDetails().companyPhone(),
        POSITION: self.userProfile.workDetails().position(),
        MONTHLY_INCOME_CODE: self.userProfile.workDetails().incomeRangeCode(),
        WORKING_EXPERIENCE_CODE: self.userProfile.workDetails().experienceDurationCode(),
        FAMILY_STATUS_CODE: self.userProfile.familyStatus().familyStatusCode(),
        AGREED_WITH_TERMS: true,
        LOAN_TEMPLATE_CODE: self.scoringResult.TEMPLATE_CODE,
        COMMUNICATION_TYPE_CODE: self.statementDeliveryMethod(),
      };

      if (self.preApprovalApplicationData) {
        LOAN_TYPE_ID: self.preApprovalApplicationData.LOAN_TYPE_ID;
        LOAN_TYPE_STATE: self.preApprovalApplicationData.LOAN_TYPE_STATE;
      }

      if (self.id) {
        obj.ID = self.id;
      }

      return obj;
    };

    this.getDataObject = function() {
      return prepareMainApplicationObject();
    };

    this.fileMaxSizeExceeded = function() {
      var dialog = new NotificationDialog({
        message: 'MESSAGE.FILE_MAX_SIZE_EXCEEDED',
        title: 'Կցվող փաստաթղթի չափի գերազանցում',
        messageClass: 'dialogNotify',
        visibleButton: 'false',
        confirm: function() {
          navigationHelper.navigateToApplicationList();
        },
      });
      dialog.show();
    };

    this.saveData = function(callback, isSubmiting) {
      var self = this;
      if (isSubmiting) {
        if (!self.validate()) {
          if (callback && typeof callback == 'function') callback();
          return;
        }
      }

      obj = prepareMainApplicationObject();
      obj.SUBMIT = isSubmiting;

      $.post({
        url: '/api/loan/Applications/' + self.id + '/Main',
        context: self,
        data: JSON.stringify(obj),
        success: function(data) {
          $.get({
            url: '/api/loan/Applications/' + self.id + '/Main',
            context: self,
            success: function(data) {
              preApprovalStage().state(data.STATUS_STATE);
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
        error: function(err) {
          if (callback) {
            callback(err);
          }
        },
        //dataType: 'json'
      });
    };

    var getLoanPaymentPeriods = function(interest, loanTypeId) {
      var options = [];
      if (!interest) {
        options.push({ CODE: '', NAME: '' });
      } else {
        var start = interest.TERM_FROM;
        while (start <= interest.TERM_TO) {
          options.push({ CODE: start, NAME: start + ' ամիս' });
          start += 6;
        }

        var isOverdraft = false;
        //TODO VAS(2018-04-07): open when be available LOAN_TYPES and selected one
        //for (var i = 0; i < LOAN_TYPES.length; i++) {
        //    if (LOAN_TYPES[i].CODE == loanTypeId) {
        //        isOverdraft = LOAN_TYPES[i].IS_OVERDRAFT;
        //        break;
        //    }
        //}

        if (isOverdraft) options.push({ CODE: 0, NAME: 'Անժամկետ' });
      }

      return options;
    };

    this.interest = ko.observable();
    let subscription = this.interest.subscribe(function(newValue) {
      if (self.listInitialCaption().defaultCaptionEnabled()) {
        self.listInitialCaption().defaultCaptionEnabled(false);
      }

      if (self.generalScoringResults()) {
        let scoringResults = self.generalScoringResults();

        for (var i = 0; i < scoringResults.length; i++) {
          if (scoringResults[i]['INTEREST'] === newValue) {
            self.scoringResult = scoringResults[i];
            //If user has only one choice of loan payment period, set that choice by default
            if (
              self.scoringResult.TERM_FROM === self.scoringResult.TERM_TO &&
              !self.loanPaymentPeriod()
            ) {
              self.scoringResultAllowText = '';
              self.loanPaymentPeriod(self.scoringResult.TERM_FROM);
              self.loanPaymentPeriodDisabled(true);
            } else {
              self.scoringResultAllowText =
                ' (' + self.scoringResult.TERM_FROM + ' - ' + self.scoringResult.TERM_TO + ') ';
              self.loanPaymentPeriodDisabled(false);
            }
          }
        }

        if (self.scoringResult) {
          self.loanPaymentPeriods(
            new CollectionWrapperList(getLoanPaymentPeriods(self.scoringResult))
          );

          ////self.offeredLoanAmount(self.scoringResult.AMOUNT);
        }
      }
    });

    // Skip the limits for now
    //this.paymentDay = ko.observable().extend({ numeric: { min: 1, max: 31 } });
    this.paymentDay = ko.observable();

    this.showMonthlyPaymentText = ko.observable(false);
    this.averageMonthlyPayment = ko.observable(0);
    this.calculateMonthlyPayment = function() {
      var interest = self.interest();
      var amount = self.offeredLoanAmount();
      var duration = self.loanPaymentPeriod();
      var serviceInterest = 0;
      var serviceAmount = 0;
      $.get({
        url:
          '/api/loan/RepaymentSchedule?interest=' +
          interest +
          '&amount=' +
          amount +
          '&duration=' +
          period +
          '&serviceInterest=' +
          serviceInterest +
          '&serviceAmount=' +
          serviceAmount,
        context: self,
        success: function(data) {
          self.showMonthlyPaymentText(true);
          self.averageMonthlyPayment(numberWithCommas(data.MONTHLY_PAYMENT_AMOUNT));
        },
        dataType: 'json',
      });
    };

    this.interests = ko.computed(
      {
        read: function() {
          if (!self.generalScoringResults()) {
            return new CollectionWrapperList([]);
          }

          var entities = self.generalScoringResults().map(function(item) {
            return {
              CODE: item.INTEREST,
              NAME: item.INTEREST,
            };
          });

          return new CollectionWrapperList(entities);
        },
        deferEvaluation: true,
      },
      this
    );

    this.offeredLoanAmount = ko.observable(0);

    this.validate = function() {
      var self = this;
      self.validationErrors([]);

      if (!self.interest()) {
        self.validationErrors.push({
          propertyName: 'selectedInterest',
          errorMessage: localization.errors['REQUIRED_FIELD_ERROR'],
        });
      }

      if (!self.loanPaymentPeriod()) {
        self.validationErrors.push({
          propertyName: 'selectedPaymentPeriod',
          errorMessage: localization.errors['REQUIRED_FIELD_ERROR'],
        });
      }

      if (!self.paymentDay()) {
        self.validationErrors.push({
          propertyName: 'paymentDayMissing',
          errorMessage: localization.errors['REQUIRED_FIELD_ERROR'],
        });
      } else if (
        self.repaymentDayFrom &&
        self.repaymentDayTo &&
        (parseInt(self.paymentDay()) > parseInt(self.repaymentDayTo) ||
          parseInt(self.paymentDay()) < parseInt(self.repaymentDayFrom))
      ) {
        self.validationErrors.push({
          propertyName: 'paymentDayMissing',
          errorMessage:
            localization.errors['ALLOW_FIELD_ERROR'] +
            self.repaymentDayFrom +
            ' - ' +
            self.repaymentDayTo,
        });
      }

      //if (!self.userProfile.nameEn()) {
      //    self.userProfile.validationErrors.push({ propertyName: 'nameEn', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
      //}

      //if (!self.userProfile.lastNameEn()) {
      //    self.userProfile.validationErrors.push({ propertyName: 'lastNameEn', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
      //}

      if (!self.userProfile.birthCountry()) {
        self.userProfile.validationErrors.push({
          propertyName: 'birthPlace',
          errorMessage: localization.errors['REQUIRED_FIELD_ERROR'],
        });
      }

      var isResidentalAddressValid = self.userProfile.residentialAddress().validate(true);
      var isHomeAddressValid = self.userProfile.homeAddress().validate(true);
      var isFamilyStatusValid = self.userProfile.familyStatus().validate(true);
      var wereDocumentsUploaded = self.isPassportUploaded() && self.isSocCardUploaded();

      if (!isResidentalAddressValid) {
        // expand address area if there is any validation error
        self.isShowAddres1(true);
      }

      if (!isHomeAddressValid) {
        // expand address area if there is any validation error
        self.isShowAddres2(true);
      }

      return (
        self.validationErrors().length === 0 &&
        isResidentalAddressValid &&
        isFamilyStatusValid &&
        wereDocumentsUploaded
      );
    };
  };

  return LoanMainApplication;
});
