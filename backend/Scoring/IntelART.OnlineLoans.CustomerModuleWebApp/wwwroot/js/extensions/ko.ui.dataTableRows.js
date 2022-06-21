define(['knockout', 'jquery', 'jquery.dataTables'], function (ko, $) {
    ko.bindingHandlers['ui-DataTableRows'] = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel, context) {
            return ko.bindingHandlers.foreach.init(element, valueAccessor, allBindingsAccessor, viewModel, context);
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var table = $(element).parent('table');
            var value = valueAccessor();
            if (value
                && value.loaded
                && value.loaded()
                && value()) {
                if (table.is('table')) {
                    table.DataTable().clear();
                    table.DataTable().destroy();
                    ko.bindingHandlers.foreach.update(element, valueAccessor, allBindings, viewModel, bindingContext);
                    table.DataTable({ 
                        paging: false,
                        searching: false,
                        info:     false,
                        oLanguage: {
                            sEmptyTable: "Հայտեր առկա չեն",
                        }
                    });
                }
            }
        }
    };
});