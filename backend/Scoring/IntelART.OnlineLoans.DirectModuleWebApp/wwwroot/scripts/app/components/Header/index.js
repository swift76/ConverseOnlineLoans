define(['knockout'], function(ko) {
  'use strict';
  var Header = function() {};
  return {
    viewModel: Header,
    template: { require: 'text!components/Header/index.html' },
  };
});
