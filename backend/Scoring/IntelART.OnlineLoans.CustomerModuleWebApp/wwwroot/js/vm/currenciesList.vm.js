define(['knockout', 'lookupDirectory'], function (ko, LookupDirectory) {

    CurrencyList = function (loanType) {
        var self = this;
        this.items = ko.onDemandObservable(function () { self.getItems(loanType) }, this);
    }

    CurrencyList.prototype.getItems = function (loanType) {
        var self = this;
        (new LookupDirectory()).getCurrencies(loanType, function (currencies) {
            self.items(currencies);
            self.items.loaded(true);
        });
    };

    return CurrencyList;
});