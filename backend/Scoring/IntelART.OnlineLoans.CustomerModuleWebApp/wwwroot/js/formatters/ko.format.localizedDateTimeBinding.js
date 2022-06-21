define(['knockout', 'moment'], function (ko, moment) {
    ko.bindingHandlers.formattedDate = {
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var val = ko.unwrap(valueAccessor());
            val = moment(val).format('L');
            ko.bindingHandlers.text.update(element, function () { return val; });
        }
    };
});