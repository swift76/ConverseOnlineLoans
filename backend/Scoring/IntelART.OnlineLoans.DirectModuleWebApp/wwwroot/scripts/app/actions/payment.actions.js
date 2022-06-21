define(['services/api', 'qs'], function (axios, qs) {
    'use strict';
    var url = '/Loan/ArcaPayment';
    return {
        verifyPayment: function (orderId) {
            return axios.get(url+'/' + orderId);
        }
    };
});
