define(['knockout'], function (ko) {
    ko.extenders.numeric = function (target, option) {
        //create a writable computed observable to intercept writes to our observable

        var result = ko.pureComputed({
            read: target,  //always return the original observables value
            write: function (newValue) {
                let current = target();
                if (! /^[0-9]+$/.test(newValue)) {
                    if (!newValue) {
                        target(newValue);
                        return;
                    }

                    target.notifySubscribers(current);
                    return;
                }

                if (!isNaN(newValue) && (option.min <= newValue) && (newValue <= option.max)) {
                    target(+newValue);
                }
                else {

                    target.notifySubscribers(current);
                }

            }
        }).extend({ notify: 'always' });

        result(target());

        //return the new computed observable
        return result;
    };
});