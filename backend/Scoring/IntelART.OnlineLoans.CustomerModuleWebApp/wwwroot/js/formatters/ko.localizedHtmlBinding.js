define(['knockout'], function (ko) {
    ko.bindingHandlers.localizedHtml = {
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var val = ko.unwrap(valueAccessor());
            if (typeof localization !== 'undefined'
                && localization.strings
                && localization.strings[val]) {
                val = localization.strings[val];
            }
            ko.bindingHandlers.html.update(element, function () { return val; });
        }
    };
});