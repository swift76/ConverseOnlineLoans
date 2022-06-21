define(['knockout', 'lookupDirectory'], function (ko, LookupDirectory) {

    StatementDeliveryMethodList = function () {
        this.items = ko.onDemandObservable(this.getItems, this);
    }

    StatementDeliveryMethodList.prototype.getItems = function () {
        var self = this;
        (new LookupDirectory()).getBankStatementDeliveryMethods(function (statementDeliveryMethods) {
            self.items(statementDeliveryMethods);
            self.items.loaded(true);
        });
    };

    return StatementDeliveryMethodList;
});
