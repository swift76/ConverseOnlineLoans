define(['knockout', 'jquery', './addressDetails.vm', './countryList.vm', './idDocDetails.vm', './familyStatusDetails.vm', './workDetails.vm'],
    function (ko, $, AddressDetails, CountryList, IdDocumentDetails, FamilyStatusDetails, WorkDetails) {
    UserProfile = function (isEditable) {
        var self = this;

        this.validationErrors = ko.observableArray();
        this.nameEn = ko.observable();
        this.lastNameEn = ko.observable();
        this.nameAm = ko.observable();
        this.lastNameAm = ko.observable();
        this.patronymicNameAm = ko.observable();
        this.birthDate = ko.observable();
        this.birthCountry = ko.observable();
        this.citizenship = ko.observable();
        this.cellphone = ko.observable();
        this.phone = ko.observable();
        this.email = ko.observable();
        this.identificationDocument = ko.observable();
        this.residentialAddress = ko.observable();
        this.homeAddress = ko.observable();
        this.countries = ko.observable(new CountryList());
        this.InitialEmail = ko.observable();

        this.emailAlreadyExists = ko.computed(function () {
            return (self.InitialEmail() && self.InitialEmail().trim() != "");
        });

        if (isEditable) {
            this.isEditable = isEditable;
        }
        else {
            this.isEditable = ko.observable(true);
        }


        this.familyStatus = ko.observable(new FamilyStatusDetails(null, this.isEditable));
        this.workDetails = ko.observable(new WorkDetails(null, null, null, null, null, this.isEditable));
        this.validate = function () {
            var self = this;
            self.validationErrors([]);

            if (!self.nameEn() || self.nameEn().trim() == "") {
                self.validationErrors.push({ propertyName: 'nameEn', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
            }

            if (!self.lastNameEn() || self.lastNameEn().trim() == "") {
                self.validationErrors.push({ propertyName: 'lastNameEn', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
            }

            if (!self.nameAm() || self.nameAm().trim() == "") {
                self.validationErrors.push({ propertyName: 'nameAm', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
            }

            if (!self.lastNameAm() || self.lastNameAm().trim() == "") {
                self.validationErrors.push({ propertyName: 'lastNameAm', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
            }

            if (!self.patronymicNameAm() || self.patronymicNameAm().trim() == "") {
                self.validationErrors.push({ propertyName: 'patronymicNameAm', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
            }

            if (!self.birthDate()) {
                //self.validationErrors.push({ propertyName: 'birthDate', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
                //} else if (!isValidDateYearFormat(self.birthDate())) {
                //    self.validationErrors.push({ propertyName: 'birthDate', errorMessage: localization.errors["DATE_YEAR_FORMAT_ERROR"] });
            } else if (!isDateValid(self.birthDate(), null, new Date())) {
                self.validationErrors.push({ propertyName: 'birthDate', errorMessage: localization.errors["DATE_BIRTHDATE_FORMAT_ERROR"] });
            } //else if (!isValidDateFormat(self.birthDate()) || !isValidDate(self.birthDate(), null, null)) {
            //    self.validationErrors.push({ propertyName: 'birthDate', errorMessage: localization.errors["DATE_FORMAT_ERROR"] });
            //}

            if (!self.email() || self.email().trim() == "") {
                self.validationErrors.push({ propertyName: 'email', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
            } else if (!isValidEmailAddress(self.email())) {
                self.validationErrors.push({ propertyName: 'email', errorMessage: localization.errors["EMAIL_FORMAT_ERROR"] });
            }

            var isIDSectionValid = self.identificationDocument().validate(false);

            return (self.validationErrors().length === 0) && isIDSectionValid;
        }

        this.saveData = function (callback) {
            var self = this;
            if (!self.validate()) {
                callback();
                return;
            }

            var obj = {
                FIRST_NAME_EN: self.nameEn(),
                LAST_NAME_EN: self.lastNameEn(),
                FIRST_NAME: self.nameAm(),
                LAST_NAME: self.lastNameAm(),
                PATRONYMIC_NAME: self.patronymicNameAm(),
                BIRTH_DATE: toUtcMidnight(self.birthDate()),
                BIRTH_PLACE_CODE: self.birthCountry(),
                CITIZENSHIP_CODE: self.citizenship(),
                MOBILE_PHONE: self.cellphone(),
                FIXED_PHONE: self.phone(),
                EMAIL: self.email(),
                SOCIAL_CARD_NUMBER: self.identificationDocument().ssn(),
                DOCUMENT_TYPE_CODE: self.identificationDocument().identificationDocumentTypeCode(),
                DOCUMENT_NUMBER: self.identificationDocument().identificationDocumentNumber(),
                DOCUMENT_GIVEN_DATE: toUtcMidnight(self.identificationDocument().identificationDocumentValidFrom()),
                DOCUMENT_EXPIRY_DATE: toUtcMidnight(self.identificationDocument().identificationDocumentValidTo()),
                DOCUMENT_GIVEN_BY: self.identificationDocument().identificationDocumentIssuer(),
                REGISTRATION_COUNTRY_CODE: self.residentialAddress().countryCode(),
                REGISTRATION_STATE_CODE: self.residentialAddress().stateCode(),
                REGISTRATION_CITY_CODE: self.residentialAddress().cityCode(),
                REGISTRATION_STREET: self.residentialAddress().streetName(),
                REGISTRATION_BUILDNUM: self.residentialAddress().buildingNumber(),
                REGISTRATION_APARTMENT: self.residentialAddress().apartmentNumber(),
                CURRENT_COUNTRY_CODE: self.homeAddress().countryCode(),
                CURRENT_STATE_CODE: self.homeAddress().stateCode(),
                CURRENT_CITY_CODE: self.homeAddress().cityCode(),
                CURRENT_STREET: self.homeAddress().streetName(),
                CURRENT_BUILDNUM: self.homeAddress().buildingNumber(),
                CURRENT_APARTMENT: self.homeAddress().apartmentNumber(),
            }
            $.ajax({
                type: 'POST',
                url: "/api/customer/Profile",
                context: this,
                data: JSON.stringify(obj),
                success: function (data) {
                    if (callback) {
                        callback();
                    }
                },
                error: function (err) {
                    if (callback) {
                        callback(err);
                    }
                },
                ////dataType: 'json'
            });
        }
    }

    UserProfile.prototype.loadData = function (callback) {
        var self = this;
        $.ajax({
            type: 'GET',
            url: "/api/customer/Profile",
            context: self,
            success: function (data) {
                self.nameEn(data.FIRST_NAME_EN);
                self.lastNameEn(data.LAST_NAME_EN);
                self.nameAm(data.FIRST_NAME);
                self.lastNameAm(data.LAST_NAME);
                self.patronymicNameAm(data.PATRONYMIC_NAME);
                self.birthDate(new Date(data.BIRTH_DATE));
                self.birthCountry(data.BIRTH_PLACE_CODE);
                self.citizenship(data.CITIZENSHIP_CODE);
                self.cellphone(data.MOBILE_PHONE);
                self.email(data.EMAIL);

                self.identificationDocument(
                    new IdDocumentDetails(
                        data.SOCIAL_CARD_NUMBER,
                        data.DOCUMENT_TYPE_CODE,
                        data.DOCUMENT_NUMBER,
                        data.DOCUMENT_GIVEN_DATE,
                        data.DOCUMENT_EXPIRY_DATE,
                        data.DOCUMENT_GIVEN_BY,
                        self.isEditable
                    ));

                self.residentialAddress(
                    new AddressDetails(
                        data.REGISTRATION_COUNTRY_CODE,
                        data.REGISTRATION_STATE_CODE,
                        data.REGISTRATION_CITY_CODE,
                        data.REGISTRATION_STREET,
                        data.REGISTRATION_BUILDNUM,
                        data.REGISTRATION_APARTMENT,
                        self.isEditable
                    ));
                self.homeAddress(
                    new AddressDetails(
                        data.CURRENT_COUNTRY_CODE,
                        data.CURRENT_STATE_CODE,
                        data.CURRENT_CITY_CODE,
                        data.CURRENT_STREET,
                        data.CURRENT_BUILDNUM,
                        data.CURRENT_APARTMENT,
                        self.isEditable
                    ));

                this.familyStatus(new FamilyStatusDetails(data.FAMILY_STATUS_CODE, self.isEditable));
                this.workDetails(new WorkDetails(data.COMPANY_NAME, data.COMPANY_PHONE, data.POSITION, data.MONTHLY_INCOME_CODE, data.WORKING_EXPERIENCE_CODE, self.isEditable));


                if (callback) {
                    callback();
                }
            },
            dataType: 'json'
        });
    };

    return UserProfile;
});