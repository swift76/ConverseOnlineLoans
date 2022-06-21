requirejs.config({
  baseUrl: '/scripts/lib',
  generateSourceMaps: true,
  paths: {
    app: '../app',
    components: '../app/components',
    actions: '../app/actions',
    services: '../app/services',
    koHelpers: '../app/knockout',
    constants: '../app/constants',
    locale: '../app/locale',
  },
  shim: {
    regexpFormat: {
      deps: ['jquery'],
    },
    modal: {
      deps: ['jquery'],
    },
    niceSelect: {
      deps: ['jquery'],
    },
  },
  waitSeconds: 15,
});

requirejs(
  [
    'jquery',
    'knockout',
    'app/main/index',
    'knockout-amd-helpers',
    'koHelpers/bindings/index',
    'koHelpers/validation/index',
    'koHelpers/extenders/index',
  ],
  function($, ko, App) {
    // TODO: remove later next line (added for access knockout from console)
    window.ko = ko;
    // overide default amd settings
    ko.bindingHandlers.module.baseDir = 'app/modules';
    // ko.bindingHandlers.module.loader =
    ko.amdTemplateEngine.defaultPath = 'app/templates';
    ko.amdTemplateEngine.defaultSuffix = '.html';
    // register main application
    ko.components.register('app-root', App);
    // start bindings
    setTimeout(function() {
      ko.applyBindings(undefined, document.body);
    }, 0);
  }
);
