define(['knockout'], function(ko) {
  'use strict';
  var ThankYouMsg = function ThankYouMsg(payload) {
    this.title = ko.observable('ՇՆՈՐՀԱԿԱԼՈՒԹՅՈՒՆ');
    this.message = ko.observable('Կոնվերս Բանկը ընտրելու համար');
  };

  return ThankYouMsg;
});
