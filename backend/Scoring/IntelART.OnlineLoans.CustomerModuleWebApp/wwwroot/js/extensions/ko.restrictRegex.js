define(['knockout', 'jquery', 'regexpFormat'], function (ko, $) {
    ko.bindingHandlers['restrictRegex'] = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var value = valueAccessor();
            $(element).regexpFormat(value);
        },
    };
});