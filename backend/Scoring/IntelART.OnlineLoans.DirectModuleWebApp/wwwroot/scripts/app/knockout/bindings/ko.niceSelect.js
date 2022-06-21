define(['knockout', 'jquery', 'niceSelect'], function(ko, $) {
  ko.bindingHandlers['niceSelect'] = {
    init: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
      var value = valueAccessor();
      $(element).niceSelect();
    },
    update: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
      $(element).niceSelect('update');
    },
  };
});
