define(['knockout', 'jquery'], function (ko, $) {
    ko.bindingHandlers["confirmation"] = {
        init: function (element, valueAccessor) {
            var value = valueAccessor();
            var msg = "Հաստատեք, որ ցանկանում եք շարունակել։";
            var title = "Հաստատում";
            var confirmButton = "Հաստատում";
            var confirmAction = function () { };
            if (value) {
                if (value.message) {
                    msg = value.message;
                }
                if (value.confirm) {
                    confirmAction = value.confirm;
                }
                if (value.title) {
                    title = value.title;
                }
                if (value.confirmButton) {
                    confirmButton = value.confirmButton;
                }
            }
            $(element).click(function (event) {
                event.stopPropagation();
                var body = $("body")[0];
                var div = document.createElement("div");
                div.style.display = "none";
                document.body.appendChild(div);
                var dialog = null;
                var getDialog = function () { return dialog; };
                ko.renderTemplate("confirmationDialog", {
                    message: msg, title: title, confirmButton: confirmButton, confirm: function () {
                        confirmAction();
                        var d = getDialog();
                        if (d) {
                            d.modal("hide");
                        }
                    }
                }, {
                    afterRender: function (renderedElement) {
                        dialog = $(renderedElement);
                        dialog.modal("show");
                        dialog.on("hidden.bs.modal", function () {
                            dialog.each(function (index, element) { ko.cleanNode(element); });
                            dialog.remove();
                        });
                    }
                }, div, "replaceNode")
            })
        },
        update: function (element, valueAccessor) {
        }
    }
});