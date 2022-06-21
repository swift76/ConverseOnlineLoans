define([
  'knockout',
  'actions/verifications.actions',
  'actions/application.actions',
  'actions/directory.actions',
  'components/Modal/index',
  './confirmPhoneModal',
  'services/helpers',
], function(
  ko,
  varifyActions,
  applicationActions,
  directoryActions,
  modal,
  ConfirmPhoneModalVm,
  helpers
) {
  'use strict';
  var Home = function Home(payload) {
    var values = ko.validatedObservable({
      firstName: ko.observable().extend({ required: true }),
      patronicName: ko.observable(''),
      lastname: ko.observable().extend({ required: true }),
      documentTypeCode: ko.observable().extend({ required: true }),
      documentNumber: ko.observable(),
      email: ko.observable().extend({ required: true, email: true }),
      phone: ko.observable().extend({
        required: true,
        pattern: /^((?!(0))[0-9]{8})$/,
      }),
      phoneCode: ko.observable().extend({ required: true, pattern: /^[0-9]{4,4}$/ }),
      socialCardNum: ko.observable().extend({ required: true, pattern: /^([0-9]{10,10})$/ }),
      agreedToTerms: ko.observable(false).extend({
        checked: true,
      }),
      day: ko.observable().extend({ required: true }),
      month: ko.observable().extend({ required: true }),
      year: ko.observable().extend({ required: true }),
    });

    var continueLoading = ko.observable(false);
    var getSMSCodeLoading = ko.observable(false);

    var birthDate = ko.computed(function() {
      return values().month() + '/' + values().day() + '/' + values().year();
    });

    var docNoPattern = ko.pureComputed(function() {
      return values().documentTypeCode() === '2' ? /^[0-9]{9,9}$/ : /[A-Z]{2}[0-9]{7}/;
    });

    values().documentNumber.extend({
      required: true,
      pattern: docNoPattern,
    });

    var verifyPhone = function verifyPhone() {
      getSMSCodeLoading(true);
      varifyActions
        .verifyPhone(values().phone())
        .then(function() {
          getSMSCodeLoading(false);
          modal
            .open({
              viewModel: new ConfirmPhoneModalVm(values().phone()),
              context: self,
            })
            .then(function(data) {
              values().phoneCode(data);
            })
            .catch(function(error) {
              console.error(error);
            });
        })
        .catch(function(error) {
          getSMSCodeLoading(false);
        });
    };

    var handleSubmit = function handleSubmit() {
      var extValues = values();
      if (values.isValid()) {
        continueLoading(true);
        var mapDataToServer = {
          FIRST_NAME_AM: extValues.firstName(),
          LAST_NAME_AM: extValues.lastname(),
          PATRONYMIC_NAME_AM: extValues.patronicName(),
          BIRTH_DATE: helpers.toUtcMidnight(new Date(birthDate())),
          SOCIAL_CARD_NUMBER: extValues.socialCardNum(),
          DOCUMENT_TYPE_CODE: extValues.documentTypeCode(),
          DOCUMENT_NUMBER: extValues.documentNumber(),
          REGISTRATION_MOBILE_PHONE: extValues.phone(),
          REGISTRATION_EMAIL: extValues.email(),
        };

        applicationActions.postApplication(mapDataToServer).then(function(data) {
          continueLoading(false);
          if (data.registeredApplicationId) {
            payload.setCurrentView('processingLoan', {
              applicationId: data.registeredApplicationId,
            });
          } else if (data.PaymentFormUrl) {
            //TODO: Improve message UI
            alert(
              'Խնդրում ենք Ձեր քարտով կատարել 1դ․ արժողությամբ վճարում, մուտքագրված տվյլաների ճշգրտությունը ստուգելու համար։'
            );
            window.location.href = data.PaymentFormUrl;
          } else {
            // TODO: user friendly error message
            alert('Something went wrong');
          }
        });
      } else {
        values.errors.showAllMessages();
      }
    };

    var enterKeyPhoneCode = function() {
      if (event.which == 13) {
        verifyPhone();
        return false;
      }
      return true;
    };

    return {
      values: values,
      dayArray: helpers.buildDaysArray(),
      monthArray: helpers.buildMonthArray(),
      buildYearArray: helpers.buildYearArray(),
      continueLoading: continueLoading,
      getSMSCodeLoading: getSMSCodeLoading,
      getIdDocTypes: directoryActions.getIdDocumentTypes,
      verifyPhone: verifyPhone,
      handleSubmit: handleSubmit,
      enterKeyPhoneCode: enterKeyPhoneCode,
    };
  };

  return Home;
});
