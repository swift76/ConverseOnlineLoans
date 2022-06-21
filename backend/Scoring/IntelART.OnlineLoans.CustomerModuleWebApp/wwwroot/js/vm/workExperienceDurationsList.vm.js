define(['knockout', 'lookupDirectory'], function (ko, LookupDirectory) {

    WorkExperienceDurationsList = function () {
        var self = this;
        this.items = ko.onDemandObservable(self.getItems, this);
    }

    WorkExperienceDurationsList.prototype.getItems = function () {
        var self = this;
        (new LookupDirectory()).getWorkExperienceDurations(function (data) {
            self.items(data);
            self.items.loaded(true);
        });
    };

    return WorkExperienceDurationsList;
});