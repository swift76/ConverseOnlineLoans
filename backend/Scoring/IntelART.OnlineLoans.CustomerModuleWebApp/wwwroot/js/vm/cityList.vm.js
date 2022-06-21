define(['knockout', 'lookupDirectory'], function (ko, LookupDirectory) {

    CityList = function (state) {
        var self = this;
        this.items = ko.onDemandObservable(function () { self.getItems(state) }, this);
    }

    CityList.prototype.getItems = function (state) {
        var self = this;
        (new LookupDirectory()).getCities(state, function (cities) {
            self.items(cities);
            self.items.loaded(true);
        });
    };

    return CityList;
});