define(['knockout', 'jquery', 'numeric'], function (ko, $) {
    ko.bindingHandlers['numericInput'] = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var value = valueAccessor();
            $(element).numeric({ allowThouSep: false, allowDecSep: false, allowMinus: false });
            $(element).number(value.isNumber);
            $(element).change(function () {
                var val = $(this).val();
                value.value(val);
            });
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var val = ko.unwrap(valueAccessor());
            if (val.value()) {
                $(element).val(val.value());
            }
        },
    };
});