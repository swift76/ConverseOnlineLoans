define(['knockout', 'jquery'], function (ko, $) {
    ko.bindingHandlers['placeholder-range'] = {
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var val = ko.unwrap(valueAccessor());
            var output = '';
            if (val
                && val.min()
                && val.max()) {
                output = $.number(val.min());
                output = output + ' - ';
                output = output + $.number(val.max());
            }
            ko.bindingHandlers.attr.update(element, function () { return {placeholder: output}; });
        }
    };
});