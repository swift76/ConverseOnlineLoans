define(['knockout'], function(ko) {
  ko.bindingHandlers.delayBind = {
    update: function(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
      var value = valueAccessor(),
        waitUntil = ko.utils.unwrapObservable(value.waitUntil);
      if (waitUntil && !element.delayBindInit) {
        ko.applyBindingsToNode(
          element,
          function() {
            return ko.bindingProvider['instance']['parseBindingsString'](
              value.bind,
              bindingContext
            );
          },
          bindingContext
        );
        element.delayBindInit = true;
      }
    },
  };
});
