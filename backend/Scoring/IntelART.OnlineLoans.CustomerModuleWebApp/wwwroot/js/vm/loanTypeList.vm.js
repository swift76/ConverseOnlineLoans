define(['knockout', 'lookupDirectory'], function (ko, LookupDirectory) {

    LoanTypeList = function () {
        var self = this;
        this.items = ko.onDemandObservable(self.getItems, this);
    }

    LoanTypeList.prototype.getItems = function () {
        var self = this;
        (new LookupDirectory()).getLoanTypes(function (data) {
            self.items(data);
            self.items.loaded(true);
        });
    };

    return LoanTypeList;
});