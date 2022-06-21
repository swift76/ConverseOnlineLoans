require.config({
    baseUrl: "/js",
    paths: {
        "jquery": "jquery-1.12.4",
        'knockout-amd-helpers': 'lib/knockout-amd-helpers.min',
        'knockout': 'lib/knockout-3.4.2',
        'jquery.dataTables': 'jquery.dataTables.min',
        'moment': 'lib/moment.min',
        'datepicker': 'lib/datepicker.min',
        'numeric': 'lib/jquery.alphanum',
        'text': 'lib/text',
        'objectLoader': 'util/objectLoader',
        'lookupDirectory': 'util/lookupDirectory',
		'helpers': 'util/helpers',
        'bootstrap': 'bootstrap.min',
        'regexpFormat': 'lib/jquery.regexpFormat',
    },
    shim: {
        'bootstrap': {
            deps: ['jquery']
        }
    },
    ////urlArgs: "bust=" + (new Date()).getTime(),
    waitSeconds: 15
});

require([
    'knockout',
    './vm/rootViewModel.vm',
    'jquery',
    'helpers',
    'bootstrap',
    'jquery.number.min',
    'knockout-amd-helpers',
    './extensions/ko.ext.all',
    './converters/ko.cnv.all',
    './formatters/ko.format.all',
    './components/components.all',
],
    function (ko, RootViewModel, $, Helpers) {
        $(function () {
            ko.amdTemplateEngine.defaultPath = "/views";
            ko.amdTemplateEngine.defaultSuffix = ".html";
            ko.amdTemplateEngine.defaultRequireTextPluginName = "text";

            ko.applyBindings(new RootViewModel());
            Helpers.getFileMaxSize(function () { });

        });
    }
);
