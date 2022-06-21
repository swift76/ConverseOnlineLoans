define(['knockout', 'lookupDirectory'], function (ko, LookupDirectory) {

    IdDocumentTypeList = function () {
        var self = this;
        this.items = ko.onDemandObservable(self.getItems, this);
    }

    IdDocumentTypeList.prototype.getItems = function (state) {
        var self = this;
        (new LookupDirectory()).getIdentificationDocumentTypes(function (documentTypes) {
            self.items(documentTypes);
            self.items.loaded(true);
        });
    };

    return IdDocumentTypeList;
});