define(['knockout', 'lookupDirectory'], function (ko, LookupDirectory) {

    MonthlyIncomeRangesList = function () {
        var self = this;
        this.items = ko.onDemandObservable(self.getItems, this);
    }

    MonthlyIncomeRangesList.prototype.getItems = function () {
        var self = this;
        (new LookupDirectory()).getMonthlyIncomeRanges(function (data) {
            self.items(data);
            self.items.loaded(true);
        });
    };

    return MonthlyIncomeRangesList;
});