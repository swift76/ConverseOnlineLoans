define([
    'knockout',
    'actions/payment.actions',
    'components/Modal/index',
    'services/helpers',
], function (
    ko,
    paymentActions,
    helpers
) {
        'use strict';
       
        var PaymentConfirmation = function Home(payload) {
            var self = this;
            self.message = ko.observable('Վճարման մշակում');
            self.continueLoading = ko.observable(true);
            paymentActions.verifyPayment(payload.data().orderId)
                .then(function (response) {
                    self.continueLoading(false);
                    if (response.data) {
                        payload.setCurrentView('processingLoan', {
                            applicationId: response.data,
                        });
                    } else {
                        self.message("Սխալ է տեղի ունեցել");
                    }
                })
                .catch(function (error) {
                    self.continueLoading(false);
                    self.message(error.response && error.response.data.Message)
                });

            return {
                message: self.message,
                continueLoading: self.continueLoading
            };
        };

        return PaymentConfirmation;
    });
