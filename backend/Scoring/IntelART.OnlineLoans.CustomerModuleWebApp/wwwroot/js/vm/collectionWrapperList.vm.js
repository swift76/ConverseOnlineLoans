define(['knockout'], function (ko) {

    CollectionWrapperList = function (entities) {
        var self = this;
        this.entities = entities;
        this.items = ko.onDemandObservable(self.getItems, this);
    }

    CollectionWrapperList.prototype.getItems = function () {
        var self = this;
        self.items(self.entities);
        self.items.loaded(true);
    };

    return CollectionWrapperList;
});