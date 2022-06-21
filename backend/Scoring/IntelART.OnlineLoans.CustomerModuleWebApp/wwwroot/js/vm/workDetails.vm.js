define(['knockout', './monthlyIncomeRangesList.vm', './workExperienceDurationsList.vm', './listInitialCaption.vm'],
    function (ko, MonthlyIncomeRangesList, WorkExperienceDurationsList, ListInitialCaption) {
        return function (companyName, companyPhone, position, incomeRange, experienceDuration, isEditable) {
            var self = this;
            this.isEditable = isEditable;
            this.companyName = ko.observable(companyName);
            this.companyPhone = ko.observable(companyPhone);
            this.position = ko.observable(position);
            this.incomeRangeCode = ko.observable(incomeRange);
            this.experienceDurationCode = ko.observable(experienceDuration);
            //let subscription = this.familyStatusCode.subscribe(function () {
            //    self.listInitialCaption().defaultCaptionEnabled(false);
            //});

            this.monthlyIncomingRanges = ko.observable(new MonthlyIncomeRangesList());
            this.workExperienceDurations = ko.observable(new WorkExperienceDurationsList());
            //this.listInitialCaption = ko.observable(new ListInitialCaption(" ", true));
        }
});