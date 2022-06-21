define(["knockout", "jquery"], function (ko, $) {
  ActiveCreditCardList = function (id) {
    var self = this;
    this.items = ko.onDemandObservable(function () {
      self.getItems(id);
    }, this);
  };

  ActiveCreditCardList.prototype.getItems = function (id) {
    var self = this;
    $.get({
      url: "/api/loan/Applications/" + id + "/ActiveCards",
      context: self,
      success: function (data) {
        var cards = [];
        for (var i = 0; i < data.length; i++) {
          cards[i] = {
            CODE: data[i].CardNumber,
            NAME: data[i].CardDescription,
          };
        }
        self.items(cards);
        self.items.loaded(true);
      },
      dataType: "json",
    });
  };

  return ActiveCreditCardList;
});
