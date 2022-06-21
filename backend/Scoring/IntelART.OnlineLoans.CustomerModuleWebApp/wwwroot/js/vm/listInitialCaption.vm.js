define(['knockout'],
    function (ko) {
        return function (initialCaption, isInitiallyEnabled) {
            var self = this;
            if (isInitiallyEnabled == undefined) {
                isInitiallyEnabled = true;
            }

            this.defaultCaption = ko.observable(initialCaption);
            this.defaultCaptionEnabled = ko.observable(isInitiallyEnabled);
            this.dynamicOptionsCaption = ko.computed(function () {
                return self.defaultCaptionEnabled()
                    ? self.defaultCaption()
                    : null;
            });
        }
});