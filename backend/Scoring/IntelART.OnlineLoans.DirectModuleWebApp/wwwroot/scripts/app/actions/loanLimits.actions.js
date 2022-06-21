define(['services/api'], function(axios) {
  'use strict';
  return {
    /// Implements GET /LoanLimits?loanTypeCode={loanTypeCode}&currency={currency}
    /// Returns lower and upper loan limits for the given loan type and currency
    getLoanLimits: function(typeCode, currency) {
      return axios
        .get('/loan/LoanLimits?loanTypeCode=' + typeCode + '&currency=' + currency)
        .then(function(response) {
          return response.data;
        })
        .catch(function(error) {
          alert(error.response && error.response.data.Message);
          throw new Error(error);
        });
    },
  };
});
