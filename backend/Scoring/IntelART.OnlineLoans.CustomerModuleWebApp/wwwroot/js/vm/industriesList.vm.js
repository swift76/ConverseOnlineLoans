define(['knockout', 'lookupDirectory'], function (ko, LookupDirectory) {

    IndustriesList = function () {
        var self = this;
        this.items = ko.onDemandObservable(self.getItems, this);
    }

    IndustriesList.prototype.getItems = function () {
        var self = this;
        (new LookupDirectory()).getIndustries(function (data) {
            self.items(data);
            self.items.loaded(true);
        });
    };

    return IndustriesList;
});