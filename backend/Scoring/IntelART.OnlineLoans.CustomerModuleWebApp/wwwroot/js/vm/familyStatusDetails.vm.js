define(['knockout', './maritalStatusesList.vm', './listInitialCaption.vm'],
    function (ko, MaritalStatuses, ListInitialCaption) {
        return function (familyStatus, isEditable) {
            var self = this;
            this.validationErrors = ko.observableArray();
            this.isEditable = isEditable;
            this.familyStatusCode = ko.observable(familyStatus);
            let subscription = this.familyStatusCode.subscribe(function () {
                self.listInitialCaption().defaultCaptionEnabled(false);
            });

            this.maritalStatuses = ko.observable(new MaritalStatuses());
            this.listInitialCaption = ko.observable(new ListInitialCaption(" ", true));

            this.validate = function (isRequired) {

                if (!self.familyStatusCode() && isRequired) {
                    self.validationErrors.push({ propertyName: 'familyStatusCode', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
                }

                return (self.validationErrors().length === 0);
            }
        }
});