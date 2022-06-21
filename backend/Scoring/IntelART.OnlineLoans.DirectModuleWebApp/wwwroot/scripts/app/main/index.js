define(['knockout', 'components/index'], function (ko) {
    var App = function App() {
        var self = this;
        var moduleName = ko.observable('home');
        var moduleData = ko.observable({});

        var setCurrentView = function (name, data) {
            moduleName(name);
            moduleData(data);
        };

        var urlParams = new URLSearchParams(location.search);
        if (urlParams.has('orderId')) {
            setCurrentView('paymentConfirmation', {
                orderId: urlParams.get('orderId')
            });
        }

        return {
            moduleName: moduleName,
            modulePayload: {
                setCurrentView: setCurrentView,
                data: moduleData,
            },
            moduleAfterRender: function (element, data) {
                // console.log('afterModelRender');
            },
            modulClean: function () {
                // console.log('clean');
            },
        };
    };

    return {
        viewModel: App,
        template: { require: 'text!app/main/index.html' },
    };
});
