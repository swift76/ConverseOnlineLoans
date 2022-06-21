define(['knockout'], function (ko) {

    YearMonthList = function (startFrom, numberOfItems) {
        var self = this;
        this.objectsList = ko.observableArray();
        for (var i = startFrom; i < startFrom + numberOfItems; i++) {
            var optionsText = (i < 10) ? ('0' + i) : i;
            var object = {
                NAME: optionsText,
                CODE: i
            };
            this.objectsList.push(object);
        }
    }

    return YearMonthList;
});