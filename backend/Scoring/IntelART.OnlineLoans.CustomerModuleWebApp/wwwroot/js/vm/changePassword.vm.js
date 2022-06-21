define(['knockout', 'jquery', '../util/notificationDialog'], function(ko, $, NotificationDialog) {
  return function(navigationHelper) {
    var self = this;

    this.validationErrors = ko.observableArray();
    this.oldPassword = ko.observable();
    this.newPassword = ko.observable();
    this.newPassword2 = ko.observable();

    this.validate = function() {
      self.validationErrors([]);

      if (self.oldPassword() == null || self.oldPassword() == '') {
        self.validationErrors.push({
          propertyName: 'oldPassword',
          errorMessage: 'Հին գաղտնաբառը պետք է մուտքագրված լինի',
        });
      }
      if (self.newPassword() == null || self.newPassword() == '') {
        self.validationErrors.push({
          propertyName: 'newPassword',
          errorMessage: 'Նոր գաղտնաբառը պետք է մուտքագրված լինի',
        });
      }
      if (self.newPassword2() == null || self.newPassword2() == '') {
        self.validationErrors.push({
          propertyName: 'newPassword2',
          errorMessage: 'Նոր գաղտնաբառի կրկնությունը պետք է մուտքագրված լինի',
        });
      } else {
        if (self.newPassword() != self.newPassword2()) {
          self.validationErrors.push({
            propertyName: 'newPassword2',
            errorMessage: 'Մուտքագրված նոր գաղտնաբառը և դրա կրկնությունը չեն համընկնում',
          });
        }
      }

      return self.validationErrors().length === 0;
    };

    this.save = function(callback) {
      if (!self.validate()) {
        callback();
        return;
      }
      var obj = {
        oldPassword: self.oldPassword(),
        newPassword: self.newPassword(),
        newPasswordRepeat: self.newPassword2(),
      };
      // TODO
      $.ajax({
        type: 'PUT',
        url: '/api/customer/Profile/login',
        context: this,
        data: JSON.stringify(obj),
        success: function(data) {
          dialog = new NotificationDialog({
            message: 'Գաղտնաբառը հաջողությամբ փոփոխվել է:',
            title: 'Գաղտնաբառի փոփոխում',
            messageClass: 'dialogSuccess',
            visibleButton: 'false',
            close: function() {
              navigationHelper.navigateToApplicationList();
            },
          });
          dialog.show();

          // clean the input fields
          self.oldPassword('');
          self.newPassword('');
          self.newPassword2('');

          if (callback) {
            callback();
          }
        },
        error: function(err) {
          if (err.responseJSON.ErrorCode === 'ERR-0029') {
            self.validationErrors.push({
              propertyName: 'oldPassword',
              errorMessage: err.responseJSON.Message,
            });
          } else {
            self.validationErrors.push({
              propertyName: 'newPassword',
              errorMessage: err.responseJSON.Message,
            });
          }

          if (callback) {
            callback(err);
          }
        },
      });
    };

    this.loadData = function(callback) {
      callback();
    };
  };
});
