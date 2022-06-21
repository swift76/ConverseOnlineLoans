define(['knockout', 'jquery', './idDocTypeList.vm'], function (ko, $, IdDocumentTypeList) {
    IdDocumentDetails = function (ssn, docType, docNum, validFrom, validTo, issuer, isEditable) {
        this.validationErrors = ko.observableArray();

        this.ssn = ko.observable(ssn);
        this.identificationDocumentTypeCode = ko.observable(docType);
        this.identificationDocumentNumber = ko.observable(docNum);
        this.identificationDocumentValidFrom = ko.observable(new Date(validFrom));
        this.identificationDocumentValidTo = ko.observable(new Date(validTo));
        this.identificationDocumentIssuer = ko.observable(issuer);
        this.identificationDocumentTypes = ko.observable(new IdDocumentTypeList());

        this.isEditable = isEditable;

        this.validate = function (isRequired) {
            var self = this;
            self.validationErrors([]);

            if (!self.identificationDocumentTypeCode() && isRequired) {
                self.validationErrors.push({ propertyName: 'idTypeCode', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
            }

            if ((!self.identificationDocumentNumber() || self.identificationDocumentNumber().trim() == "") && isRequired) {
                self.validationErrors.push({ propertyName: 'idNumber', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
            } else if (self.identificationDocumentTypeCode() == "2" && !isValidIdCard(self.identificationDocumentNumber())) {
                self.validationErrors.push({ propertyName: 'idNumber', errorMessage: localization.errors["WRONG_DOCUMENT_NUMBER"] });
            } else if (self.identificationDocumentTypeCode() !== "2" && !isValidPassport(self.identificationDocumentNumber())) {
                self.validationErrors.push({ propertyName: 'idNumber', errorMessage: localization.errors["WRONG_DOCUMENT_NUMBER"] });
            }

            ////if (!self.identificationDocumentValidFrom() && isRequired) {
            ////    self.validationErrors.push({ propertyName: 'idValidFrom', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
            ////    //} else if (!isValidDateYearFormat(self.identificationDocumentValidFrom())) {
            ////    //    self.validationErrors.push({ propertyName: 'idValidFrom', errorMessage: localization.errors["DATE_YEAR_FORMAT_ERROR"] });
            ////} else if (!isDateValid(self.identificationDocumentValidFrom(), null, new Date())) {
            ////    self.validationErrors.push({ propertyName: 'idValidFrom', errorMessage: localization.errors["DATE_DOCUMENT_GIVEN_FORMAT_ERROR"] });
            ////} //else if (!isValidDateFormat(self.identificationDocumentValidFrom()) || !isValidDate(self.identificationDocumentValidFrom(), null, null)) {
            //////   self.validationErrors.push({ propertyName: 'idValidFrom', errorMessage: localization.errors["DATE_FORMAT_ERROR"] });
            //////}

            ////if (!self.identificationDocumentValidTo() && isRequired) {
            ////    self.validationErrors.push({ propertyName: 'idValidTo', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
            ////    //} else if (!isValidDateYearFormat(self.identificationDocumentValidTo())) {
            ////    //    self.validationErrors.push({ propertyName: 'idValidTo', errorMessage: localization.errors["DATE_YEAR_FORMAT_ERROR"] });
            ////} else if (!isDateValid(self.identificationDocumentValidTo(), new Date(), null)) {
            ////    self.validationErrors.push({ propertyName: 'idValidTo', errorMessage: localization.errors["DATE_DOCUMENT_EXPIRED_FORMAT_ERROR"] });
            ////} //else if (!isValidDateFormat(self.identificationDocumentValidTo()) || !isValidDate(self.identificationDocumentValidTo(), null, null)) {
            //////    self.validationErrors.push({ propertyName: 'idValidTo', errorMessage: localization.errors["DATE_FORMAT_ERROR"] });
            //////}

            ////if (!self.identificationDocumentIssuer() && isRequired) {
            ////    self.validationErrors.push({ propertyName: 'idIssuer', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
            ////} else if (!isValidNumber(self.identificationDocumentIssuer()) || self.identificationDocumentIssuer().trim().length !== 3) {
            ////    self.validationErrors.push({ propertyName: 'idIssuer', errorMessage: localization.errors["NUMBER_FORMAT_ERROR"] });
            ////}

            return (self.validationErrors().length === 0);
        }
    }

    return IdDocumentDetails;
});