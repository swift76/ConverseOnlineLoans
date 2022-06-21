define(['knockout', 'jquery', 'services/helpers', 'modal'], function(ko, $, helpers) {
  'use strict';

  var createModalElement = function createModalElement(templateName, viewModel) {
    var temporaryDiv = helpers.addHiddenDivToBody();
    var deferredElement = $.Deferred();
    ko.renderTemplate(
      templateName,
      viewModel,
      {
        afterRender: function(nodes) {
          var elements = nodes.filter(function(node) {
            return node.nodeType === 1;
          });
          deferredElement.resolve(elements[0]);
        },
      },
      temporaryDiv,
      'replaceNode'
    );
    return deferredElement;
  };

  var addModalHelperToViewModel = function addModalHelperToViewModel(
    viewModel,
    deferredModalResult,
    context
  ) {
    viewModel.modal = {
      close: function(result) {
        if (typeof result !== 'undefined') {
          deferredModalResult.resolveWith(context, [result]);
        } else {
          deferredModalResult.rejectWith(context, []);
        }
      },
    };
  };

  var showJqueryModal = function showJqueryModal($ui) {
    $ui.modal({
      fadeDuration: 100,
      showClose: false,
      clickClose: false,
    });
  };

  var closeOnResultComplete = function closeOnResultComplete(deferredModalResult, $ui) {
    deferredModalResult.always(function() {
      $.modal.close();
    });
  };

  var bindModalEvents = function bindModalEvents($ui) {
    $ui.on($.modal.OPEN, function() {
      $('body').addClass('modal-open');
    });
    $ui.on($.modal.CLOSE, function() {
      $('body').removeClass('modal-open');
      $ui.each(function(index, element) {
        ko.cleanNode(element);
      });
      $ui.remove();
    });
  };

  var openModal = function openModal(options) {
    if (typeof options === 'undefined') throw new Error('An options argument is required.');
    if (typeof options.viewModel !== 'object') throw new Error('options.viewModel is required.');

    var viewModel = options.viewModel;
    var template = options.template || viewModel.template;
    var context = options.context;

    if (!template) throw new Error('options.template or options.viewModel.template is required.');

    return createModalElement(template, viewModel)
      .pipe($) // jQueryify the DOM element
      .pipe(function($ui) {
        var deferredModalResult = $.Deferred();
        addModalHelperToViewModel(viewModel, deferredModalResult, context);
        showJqueryModal($ui);
        closeOnResultComplete(deferredModalResult, $ui);
        bindModalEvents($ui);
        return deferredModalResult;
      });
  };

  return {
    open: openModal,
  };
});
