define(['knockout', 'jquery'], function (ko, $) {
    return function (model) {
        var viewModel = {isShow: false};

        this.show = function () {
            var body = $("body")[0];
            var div = document.createElement("div");
            div.style.display = "none";
            document.body.appendChild(div);
            var dialog = null;
            var getDialog = function () { return dialog; };
            var buttonText = "Փակել";
            var confirmAction = function () { };

            viewModel = {
                message: ko.observable(),
                title: ko.observable(),
                actions: [],
                isShow: true,
                confirm: function () {
                    var d = getDialog();
                    if (d) {
                        d.modal("hide");
                    }
                    try {
                        confirmAction();
                    }
                    catch (err) {
                    }
                },
                hide: function () {
                    var d = getDialog();
                    if (d) {
                        d.modal("hide");
                    }
                }
                
            };

            defaultAction = {
                buttonText: ko.observable(buttonText),
                isEnabled: true,
                action: function () {
                    var d = getDialog();
                    if (d) {
                        d.modal("hide");
                    }
                    try {
                        confirmAction();
                    }
                    catch (err) {
                    }
                }
            }

            if (model) {
                
                if (model.buttonText) {
                    buttonText = model.buttonText;
                    defaultAction.buttonText = model.buttonText;
                }
                if (model.message) {
                    viewModel.message(model.message);
                }
                if (model.title) {
                    viewModel.title(model.title);
                }
                if (model.actions) {
                    viewModel.confirmAction = model.confirm;
                    viewModel.closeAction = model.actions.closeAction;
                } else  if (model.confirm) {
                    confirmAction = model.confirm;
                    viewModel.confirmAction = model.confirm;
                    viewModel.actions = [defaultAction];
                   
                }
                else {
                     // Inject default action
                    viewModel.actions = [defaultAction];
                }
              
                
            }

            ko.renderTemplate("notificationDialogBoxOld", viewModel, {
                afterRender: function (renderedElement) {
                    dialog = $(renderedElement);
                    dialog.modal("show");
                    dialog.on("hidden.bs.modal", function () {
                        dialog.each(function (index, element) { ko.cleanNode(element); });
                        dialog.remove();
                        try {
                            if (viewModel.closeAction) {
                                viewModel.closeAction();
                            }
                        }
                        catch (err) {
                        }
                    });
                }
            }, div, "replaceNode");
        }
        this.hide = function () {
            viewModel.hide();
        }
        this.setMessage = function (msg) {
            viewModel.message(msg);
        }
        this.isShow = function () {
            return viewModel.isShow;
        }
    }
});