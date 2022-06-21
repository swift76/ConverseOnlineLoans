define(['knockout', 'jquery', 'jquery.dataTables'], function (ko, $) {
    ko.bindingHandlers['ui-DataTable'] = {
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var table = $(element);
            if (!table.is('table')) {
                table = table.parent('table');
            }
            if (table.is('table')) {
                table.DataTable({
                    destroy: true,
                    "paging": false,
                    'createdRow': function (row, data, dataIndex) {
                        $(row).attr('index', dataIndex);
                    }
                });
            }
        }
    };
});