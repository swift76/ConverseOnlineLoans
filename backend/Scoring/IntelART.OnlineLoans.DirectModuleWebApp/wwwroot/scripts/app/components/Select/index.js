define(['jquery', 'knockout'], function($, ko) {
  'use strict';
  var Select = function(params) {
    var self = this;

    self.params = params;

    var getItems = function() {
      if (params.get) {
        params.get().then(function(data) {
          self.items(data);
          self.items.loaded(true);
        });
      }
      if (params.options) {
        self.items(params.options);
        self.items.loaded(true);
      }
    };

    self.items = ko.onDemandObservable(getItems, self);

    self.optionsAfterRender = function(option, item) {
      ko.applyBindingsToNode(option, { disable: !item }, item);
    };
  };
  return {
    viewModel: Select,
    template: { require: 'text!components/Select/index.html' },
  };
});
