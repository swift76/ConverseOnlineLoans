define(['knockout', 'jquery'], function (ko, $) {
    ko.bindingHandlers['disabled'] = {
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var val = ko.unwrap(valueAccessor());
            if (element.tagName == 'INPUT'
                || element.tagName == 'TEXTAREA'
                || element.tagName == 'BUTTON'
                || element.tagName == 'SELECT') {
                $(element).prop('disabled', val);
            }
            $(element).find('input, textarea, button, select').prop('disabled', val);
        }
    };
});