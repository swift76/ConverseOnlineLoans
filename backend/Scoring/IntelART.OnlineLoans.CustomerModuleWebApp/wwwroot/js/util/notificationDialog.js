define(['knockout', 'jquery'], function(ko, $) {
  return function(model) {
    var viewModel;

    this.show = function() {
      var body = $('body')[0];
      var div = document.createElement('div');
      div.style.display = 'none';
      document.body.appendChild(div);
      var dialog = null;
      var getDialog = function() {
        return dialog;
      };

      var msg = '';
      var title = '';
      var confirmButtonText = 'Շարունակել';

      var messageClass = 'dialogLoader';
      var visibleButton = true;
      var visibleFirstButton = true;
      var confirmAction = function() {};
      var closeAction = function() {};
      defaultAction = {
        isEnabled: true,
        action: function() {
          var d = getDialog();
          if (d) {
            d.modal('hide');
          }
          try {
            confirmAction();
          } catch (err) {}
        },
      };
      if (model) {
        if (model.message) {
          msg = model.message;
        }
        if (model.title) {
          title = model.title;
        }
        if (model.confirm) {
          confirmAction = model.confirm;
        }
        if (model.close) {
          closeAction = model.close;
        }
        if (model.confirmButtonText) {
          confirmButtonText = model.confirmButtonText;
        }
        if (model.visibleButton) {
          visibleButton = model.visibleButton == 'true';
        }
        if (model.visibleFirstButton) {
          visibleFirstButton = model.visibleFirstButton == 'true';
        }
        if (model.messageClass) {
          messageClass = model.messageClass;
        }
        if (model.actions) {
          confirmAction = model.confirm;
          closeAction = model.actions.closeAction;
        } else if (model.confirm) {
          confirmAction = model.confirm;
          actions = [defaultAction];
        } else {
          // Inject default action
          actions = [defaultAction];
        }
      }

      viewModel = {
        message: ko.observable(msg),
        title: ko.observable(title),
        isComplete: ko.observable(false),
        confirmButtonText: ko.observable(confirmButtonText),
        visibleButton: ko.observable(visibleButton),
        visibleFirstButton: ko.observable(visibleFirstButton),
        messageClass: ko.observable(messageClass),
        actions: [],
        isShow: true,
        confirm: function() {
          closeAction = function() {};
          var d = getDialog();
          if (d) {
            d.modal('hide');
          }
          try {
            confirmAction();
          } catch (err) {}
        },
        hide: function() {
          var d = getDialog();
          if (d) {
            d.modal('hide');
          }
        },
      };

      ko.renderTemplate(
        'notificationDialogBox',
        viewModel,
        {
          afterRender: function(renderedElement) {
            dialog = $(renderedElement);
            dialog.modal('show');
            dialog.on('hidden.bs.modal', function() {
              dialog.each(function(index, element) {
                ko.cleanNode(element);
              });
              dialog.remove();
              try {
                closeAction();
              } catch (err) {}
            });
          },
        },
        div,
        'replaceNode'
      );
    };

    this.notifyComplete = function() {
      viewModel.isComplete(true);
    };

    this.setMessage = function(msg) {
      viewModel.message(msg);
    };

    this.setTitle = function(title) {
      viewModel.title(title);
    };

    this.hide = function() {
      viewModel.hide();
    };

    this.isShow = function() {
      return viewModel.isShow;
    };

    this.setMessageClass = function(messageClass) {
      viewModel.messageClass(messageClass);
    };

    this.setButtonVisible = function(visibleButton) {
      viewModel.visibleButton(visibleButton == 'true');
    };

    this.setConfirmButtonText = function(buttonText) {
      viewModel.confirmButtonText(buttonText);
    };
  };
});
