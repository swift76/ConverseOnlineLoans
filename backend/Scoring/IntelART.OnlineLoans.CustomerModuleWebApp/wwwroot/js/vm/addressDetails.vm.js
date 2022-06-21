define(['knockout', 'jquery', './countryList.vm', './stateList.vm', './cityList.vm', './addressCountryList.vm'],
    function (ko, $, CountryList, StateList, CityList, AddressCountryList) {
        return function (country, state, city, street, building, apartment, isEditable) {
            var self = this;
            this.validationErrors = ko.observableArray();
            this.countryCode = ko.observable(country);
            this.stateCode = ko.observable(state);
            this.cityCode = ko.observable(city);
            this.streetName = ko.observable(street);
            this.buildingNumber = ko.observable(building);
            this.apartmentNumber = ko.observable(apartment);

            this.isEditable = isEditable;

            this.countries = ko.observable(new CountryList());
            this.addressCountries = ko.observable(new AddressCountryList());

            this.states = ko.pureComputed(
                function () {
                    return new StateList(self.countryCode());
                });
            this.cities = ko.pureComputed(
                function () {
                    return new CityList(self.stateCode());
                });

            this.validate = function (isRequired, propertyName) {
                var self = this;

                //If validation is done through one field, do not remove other fields` validation error messages
                if (propertyName)
                {
                    self.validationErrors(self.validationErrors().filter(el => el.propertyName != propertyName));
                }
                else
                {
                    self.validationErrors([]);
                }


                if (!self.countryCode().trim() && isRequired && (!propertyName || propertyName == 'countryCode'))
                {
                    self.validationErrors.push({ autoFocusDisabled: propertyName,  propertyName: 'countryCode', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
                }

                if (!self.stateCode().trim() && isRequired && (!propertyName || propertyName == 'stateCode')) {
                    self.validationErrors.push({ autoFocusDisabled: propertyName,  propertyName: 'stateCode', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
                }

                if (!self.cityCode().trim() && isRequired && (!propertyName || propertyName == 'cityCode')) {
                    self.validationErrors.push({ autoFocusDisabled: propertyName,  propertyName: 'cityCode', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
                }

                if (!self.streetName().trim() && isRequired && (!propertyName || propertyName == 'streetName')) {
                    self.validationErrors.push({ autoFocusDisabled: propertyName, propertyName: 'streetName', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
                }

                if (!self.buildingNumber().trim() && isRequired && (!propertyName || propertyName == 'buildingNumber')) {
                    self.validationErrors.push({ autoFocusDisabled: propertyName,  propertyName: 'buildingNumber', errorMessage: localization.errors["REQUIRED_FIELD_ERROR"] });
                }


                return (self.validationErrors().length === 0);
            }
        }
});