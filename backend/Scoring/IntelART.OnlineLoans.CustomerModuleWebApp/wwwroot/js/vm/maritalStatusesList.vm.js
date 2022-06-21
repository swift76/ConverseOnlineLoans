define(['knockout', 'lookupDirectory'], function (ko, LookupDirectory) {

    MaritalStatusesList = function () {
        var self = this;
        this.items = ko.onDemandObservable(self.getItems, this);
    }

    MaritalStatusesList.prototype.getItems = function () {
        var self = this;
        (new LookupDirectory()).getMaritalStatuses(function (data) {
            self.items(data);
            self.items.loaded(true);
        });
    };

    return MaritalStatusesList;
});