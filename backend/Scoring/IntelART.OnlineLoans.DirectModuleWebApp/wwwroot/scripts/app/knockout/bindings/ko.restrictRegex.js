define(['knockout', 'jquery', 'regexpFormat'], function(ko, $) {
  ko.bindingHandlers['restrictRegex'] = {
    init: function(element, valueAccessor) {
      var value = valueAccessor();
      $(element).regexpFormat(value);
    },
  };
});
