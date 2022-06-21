define(['knockout', 'jquery', 'nouislider', 'wNumb'], function(ko, $, noUiSlider, wNumb) {
  var createPips = function(min, max, format) {
    return {
      mode: 'values',
      values: [min, max],
      density: -1,
      format: wNumb(format),
    };
  };
  ko.bindingHandlers.nouislider = {
    init: function(element, valueAccessor, allBindingsAccessor) {
      var sliderOptions = allBindingsAccessor().sliderOptions || {};
      var options = ko.unwrap(valueAccessor());
      var defaultOptions = {
        animate: true,
        tooltips: true,
        connect: [true, false],
        pips: createPips(null, null, sliderOptions.format),
      };
      sliderOptions = $.extend(true, defaultOptions, sliderOptions);

      sliderOptions.range = {};
      sliderOptions.format = wNumb(sliderOptions.format);

      if (ko.unwrap(options.start)) {
        sliderOptions.start = ko.unwrap(options.start);
      }
      if (ko.unwrap(options.min)) {
        sliderOptions.range.min = ko.unwrap(options.min);
        sliderOptions.pips.values[0] = ko.unwrap(options.min);
      }
      if (ko.unwrap(options.max)) {
        sliderOptions.range.max = ko.unwrap(options.max);
        sliderOptions.pips.values[1] = ko.unwrap(options.max);
      }
      if (ko.unwrap(options.step)) {
        sliderOptions.step = ko.unwrap(options.step);
      }
      if (ko.unwrap(options.enabled)) {
        sliderOptions.disabled = !ko.unwrap(options.enabled);
      }

      noUiSlider.create(element, sliderOptions);

      element.noUiSlider.on('set', function(values, handle) {
        var observable = valueAccessor();
        observable.start(values[handle]);
      });

      element.noUiSlider.on('update', function(values, handle) {
        var observable = valueAccessor();
        observable.start(values[handle]);
      });

      ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
        element.noUiSlider.destroy();
      });
    },
    update: function(element, valueAccessor, allBindingsAccessor) {
      var newOptions = ko.unwrap(valueAccessor());
      var sOptions = allBindingsAccessor().sliderOptions || {};
      element.noUiSlider.updateOptions(
        {
          start: newOptions.start(),
          range: {
            min: newOptions.min,
            max: newOptions.max,
          },
          pips: createPips(newOptions.min, newOptions.max, sOptions.format),
        },
        true
      );
    },
  };
});
