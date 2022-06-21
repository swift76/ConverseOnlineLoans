define(['knockout'], function (ko) {
    ko.bindingHandlers['cnv-loanTypeHint'] = {
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var val = "LOANTYPE.HINT." + ko.unwrap(valueAccessor());
            var hint = '';
            if (typeof localization !== 'undefined'
                && localization.strings
                && localization.strings[val]) {
                hint = localization.strings[val];
            }
            ko.bindingHandlers.html.update(element, function () { return hint; });
        }
    };
});