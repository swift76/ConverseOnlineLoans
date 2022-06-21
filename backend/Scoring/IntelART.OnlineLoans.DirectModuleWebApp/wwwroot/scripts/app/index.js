define(['knockout'], function(ko) {
  'use strict';
  var App = function App() {
    var self = this;

    return {};
  };

  return {
    viewModel: App,
    template: { require: 'text!templates/app-root' },
  };
});
