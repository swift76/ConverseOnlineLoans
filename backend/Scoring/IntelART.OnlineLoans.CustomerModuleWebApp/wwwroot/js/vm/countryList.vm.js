define(['knockout', 'lookupDirectory'], function (ko, LookupDirectory) {

    CountryList = function () {
        this.items = ko.onDemandObservable(this.getItems, this);
    }

    CountryList.prototype.getItems = function () {
        var self = this;
        (new LookupDirectory()).getCountries(function (countries) {
            self.items(countries);
            self.items.loaded(true);
        });
    };

    return CountryList;
});