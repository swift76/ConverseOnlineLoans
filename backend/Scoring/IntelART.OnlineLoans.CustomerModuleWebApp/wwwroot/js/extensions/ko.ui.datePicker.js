define(['knockout', 'jquery', 'datepicker'], function (ko, $) {
    ko.bindingHandlers['ui-datepicker'] = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var value = valueAccessor();
            $(element).change(function () {
                var val = $(this).datepicker("getDate");
                value(val);
            })
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var val = ko.unwrap(valueAccessor());
            ////$(element).datepicker("setDate", new Date(data.BIRTH_DATE));
            $(element).datepicker("setDate", val);
        }
    };
});