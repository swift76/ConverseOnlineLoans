define(['knockout'], function (ko) {
    ko.bindingHandlers.localizedText = {
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var val = ko.unwrap(valueAccessor());
            if (typeof localization !== 'undefined'
                && localization.strings
                && localization.strings[val]) {
                val = localization.strings[val];
            }
            ko.bindingHandlers.text.update(element, function () { return val; });
        }
    };
});