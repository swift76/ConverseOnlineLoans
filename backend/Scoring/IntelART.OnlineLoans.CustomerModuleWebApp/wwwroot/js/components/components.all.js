define(['knockout'], function (ko) {
    ko.components.register('address-details', {
        template: { require: 'text!./components/addressDetailsComponent.html' }
    });
    ko.components.register('personal-contacts', {
        template: { require: 'text!./components/personalContactsComponents.html' }
    });
    ko.components.register('family-details', {
        template: { require: 'text!./components/familyDetailsComponent.html' }
    });

    ko.components.register('work-details', {
        template: { require: 'text!./components/workDetails.html' }
    });
    ko.components.register('id-document', {
        template: { require: 'text!./components/idDocumentComponent.html' }
    });
    ko.components.register('personal-details', {
        template: { require: 'text!./components/personalDetailsComponent.html' }
    });
    ko.components.register('english-name-surname', {
        template: { require: 'text!./components/englishNameSurnameComponent.html' }
    });
    ko.components.register('citizenship-details', {
        template: { require: 'text!./components/citizenshipDetailsComponent.html' }
    });
});