define(['knockout'], function (ko) {
    ObjectLoader = function (constructor) {
        var c = constructor;
        var self = this;
        this.object = ko.onDemandObservable(function () {
            var obj = new c();
            obj.loadData(function () {
                self.object(obj);
            })
        }, this);

        this.isBusy = ko.observable(false);

        this.save = function () {
            if (self.object
                && self.object.loaded()
                && self.object().saveData) {
                self.isBusy(true);
                self.object().saveData(function () {
                    self.isBusy(false);
                });
            }
        }
    }

    return ObjectLoader;
});