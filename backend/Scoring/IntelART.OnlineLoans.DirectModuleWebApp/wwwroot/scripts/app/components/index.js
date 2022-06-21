define(['knockout', './Header/index', './Footer/index', './Select/index'], function(
  ko,
  Header,
  Footer,
  Select
) {
  'use strict';
  // register components
  ko.components.register('app-header', Header);
  ko.components.register('app-footer', Footer);
  ko.components.register('ko.dropdown', Select);
});
