define(['knockout', 'actions/verifications.actions'], function(ko, Actions) {
  'use strict';
  var ConfirmPhone = function ConfirmPhone(phone) {
    var self = this;
    self.template = 'modals/confirmPhoneModal';
    self.verifyErrorMsg = ko.observable('');
    self.verifySuccessMsg = ko.observable('');
    self.canTry = ko.observable(false);
    self.canNotSubmit = ko.observable(false);
    self.confirmSMSCodeLoading = ko.observable(false);

    self.values = ko.validatedObservable({
      phone: phone,
      code: ko.observable('').extend({ required: true, pattern: /^[0-9]{4,4}$/ }),
    });

    // == METHODS

    // substring phone for show into modal
    self.phonoToShow = function() {
      if (phone) {
        var NCode = phone.substring(0, 2);
        var SNumber = phone.substring(2, 8).replace(/^.{3}/g, '***');
        return '(+374 ' + NCode + ') ' + SNumber;
      }
      return '';
    };

    // verify action
    self.verify = function() {
      self.confirmSMSCodeLoading(true);
      Actions.verifySMSCode({
        MOBILE_PHONE: self.values().phone,
        SMS_CODE: self.values().code(),
      })
        .then(function(response) {
          self.confirmSMSCodeLoading(false);
          self.verifySuccessMsg('վերիֆիկացման կոդը ճիշտ է');
          self.verifyErrorMsg('');
          setTimeout(() => {
            self.modal.close(self.values().code());
          }, 1000);
        })
        .catch(function(error) {
          self.confirmSMSCodeLoading(false);
          if (error.response) {
            self.canTry(error.response.data.ErrorCode !== 'ERR-0008');
            self.canNotSubmit(error.response.data.ErrorCode === 'ERR-0008');
            self.verifyErrorMsg(error.response.data.Message);
          }
        });
    };

    // get sms again
    self.sendCodeAgain = function() {
      self.verifyErrorMsg('');
      self.values().code('');
      self.values().code.clearError();
      Actions.verifyPhone(self.values().phone);
    };
  };

  return ConfirmPhone;
});
