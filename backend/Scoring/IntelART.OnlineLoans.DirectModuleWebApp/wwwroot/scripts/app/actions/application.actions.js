define(['services/api', 'qs'], function(axios, qs) {
  'use strict';
  var url = '/loan/Applications';

  return {
    /// Implements POST /Applications
    /// Creates a new initial application
    postApplication: function(data) {
      return axios
        .post(url, data)
        .then(function(response) {
          return response.data;
        })
        .catch(function(error) {
          alert(error.response && error.response.data.Message);
          throw new Error(error);
        });
    },

    /// Implements GET /Applications/{id}
    /// Returns initial application with the given id
    getApplicationById: function(id) {
      return axios
        .get(url + '/' + id)
        .then(function(response) {
          return response.data;
        })
        .catch(function(error) {
          alert(error.response && error.response.data.Message);
          throw new Error(error);
        });
    },

    /// Implements GET /Applications/{id}/GeneralScoringResults
    /// Returns scoring results for the main application with the given id
    getGeneralScoringResults: function(id) {
      return axios
        .get(url + '/' + id + '/GeneralScoringResults')
        .then(function(response) {
          return response.data;
        })
        .catch(function(error) {
          alert(error.response && error.response.data.Message);
          throw new Error(error);
        });
    },

    /// Controller class to implement the API methods required for getting the
    /// loan Parameters available as a result of the successful scoring
    getParameters: function(loanTypeCode) {
      return axios
        .get('/loan/Parameters', { params: { loanTypeCode: loanTypeCode } })
        .then(function(response) {
          return response.data;
        })
        .catch(function(error) {
          alert(error.response && error.response.data.Message);
          throw new Error(error);
        });
    },

    /// Implements GET /Applications/{id}/Summary
    /// Returns agreed application with the given id
    getApplicationSummary: function(id) {
      return axios
        .get(url + '/' + id + '/Summary')
        .then(function(response) {
          return response.data;
        })
        .catch(function(error) {
          alert(error.response && error.response.data.Message);
          throw new Error(error);
        });
    },

    /// Implements POST /Applications/{id}/Full
    /// Creates, saves, or submits main application with the given id,
    /// along with its full data
    postApplicationFull: function(id, data) {
      return axios
        .post(url + '/' + id + '/Full', data)
        .then(function(response) {
          return response.data;
        })
        .catch(function(error) {
          alert(error.response && error.response.data.Message);
          throw new Error(error);
        });
    },

    /// Implements GET /Applications/{id}/Main
    /// Returns main application with the given id
    getApplicationMain: function(id) {
      return axios
        .get(url + '/' + id + '/Main')
        .then(function(response) {
          return response.data;
        })
        .catch(function(error) {
          alert(error.response && error.response.data.Message);
          throw new Error(error);
        });
    },

    /// Implements POST /Applications/Submitted/{id}
    /// Cancels the application with the given id by a customer
    postApplicationSubmitted: function(id) {
      return axios
        .post(url + '/Submitted/' + id, {})
        .then(function(response) {
          return response.data;
        })
        .catch(function(error) {
          alert(error.response && error.response.data.Message);
          throw new Error(error);
        });
    },

    /// Gets the list of active cards
    getActiveCards: function(id) {
      return axios
        .get(url + '/' + id + '/ActiveCards')
        .then(function(response) {
          return response.data;
        })
        .catch(function(error) {
          alert(error.response && error.response.data.Message);
          throw new Error(error);
        });
    },
    /// Gets the list of possible options to receive credit card types
    getCreditCardTypes: function(id) {
      return axios
        .get(url + '/' + id + '/CreditCardTypes')
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
