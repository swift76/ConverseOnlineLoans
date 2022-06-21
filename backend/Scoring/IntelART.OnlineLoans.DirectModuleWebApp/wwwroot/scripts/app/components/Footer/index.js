define(['knockout'], function(ko) {
  'use strict';
  var Footer = function Footer() {
    return {
      address: 'Գլխավոր գրասենյակ, ՀՀ, 0010, ք. Երևան, Վ. Սարգսյան 26/1',
      phone: '(374 10) 511 211',
      email: 'post@conversebank.am',
    };
  };

  return {
    viewModel: Footer,
    template: { require: 'text!components/Footer/index.html' },
  };
});
