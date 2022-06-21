define(['knockout', 'jquery'], function (ko, $) {

    CreditCardTypeList = function (id) {
        var self = this;
        this.items = ko.onDemandObservable(function () { self.getItems(id) }, this);
    }

    CreditCardTypeList.prototype.getItems = function (id) {
        var self = this;
        $.get({
            url: "/api/loan/Applications/" + id + "/CreditCardTypes",
            context: self,
            success: function (data) {
                self.items(data);
                self.items.loaded(true);
            },
            dataType: 'json'
        });
    };

    return CreditCardTypeList;
});