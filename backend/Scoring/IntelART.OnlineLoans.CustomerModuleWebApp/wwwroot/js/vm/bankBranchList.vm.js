define(['knockout', 'lookupDirectory'], function (ko, LookupDirectory) {

    BankBranchList = function () {
        this.items = ko.onDemandObservable(this.getItems, this);
    }

    BankBranchList.prototype.getItems = function () {
        var self = this;
        (new LookupDirectory()).getBankBranches(function (bankBranches) {
            self.items(bankBranches);
            self.items.loaded(true);
        });
    };

    return BankBranchList;
});