define(['knockout'], function(ko) {
  'use strict';
  ko.validation.rules['checked'] = {
    validator: function(val) {
      if (!val) {
        return false;
      }
      return true;
    },
    message: 'Շարունակելու համար անհրաժեշտ է տալ համաձայնություն',
  };
});
