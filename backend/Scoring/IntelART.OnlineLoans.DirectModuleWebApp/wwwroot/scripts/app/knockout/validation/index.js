define(['require', 'knockout', 'locale/validation', 'knockout.validation'], function(
  require,
  ko,
  localAM
) {
  'use strict';

  // declar armenian locale
  ko.validation.defineLocale('hy-AM', localAM);
  // add local to knockout
  ko.validation.locale('hy-AM');

  // init knockout validation with options
  ko.validation.init(
    {
      decorateInputElement: true,
      registerExtenders: true,
      messagesOnModified: true,
      insertMessages: false,
      parseInputAttributes: true,
      messageTemplate: null,
      errorElementClass: 'error-validate',
      errorMessageClass: 'invalid-feedback',
    },
    true
  );

  require(['./rules/index'], function() {
    ko.validation.registerExtenders();
  });
});
