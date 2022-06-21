define(['knockout', 'lookupDirectory'], function (ko, LookupDirectory) {

    AddressCountryList = function () {
        this.items = ko.onDemandObservable(this.getItems, this);
    }

    AddressCountryList.prototype.getItems = function () {
        var self = this;
        (new LookupDirectory()).getAddressCountries(function (addressCountries) {
            self.items(addressCountries);
            self.items.loaded(true);
        });
    };

    return AddressCountryList;
});