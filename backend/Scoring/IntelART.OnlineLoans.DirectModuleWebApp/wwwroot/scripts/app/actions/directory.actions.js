define(['services/api', 'constants/api'], function(axios, API) {
  'use strict';
  return {
    /// Gets the list of countries
    // getCountries: function() {
    //   return axios.get(API.DIRECTORY_URL + '/Countries').then(response => {
    //     return response.data;
    //   });
    // },
    /// Gets the list of countries
    // getAddressCountries: function() {
    //   return axios.get(API.DIRECTORY_URL + '/AddressCountries').then(response => {
    //     return response.data;
    //   });
    // },
    /// Gets the list of states
    // getStates: function() {
    //   return axios.get(API.DIRECTORY_URL + '/States').then(response => {
    //     return response.data;
    //   });
    // },
    /// Gets the list of cities for a given state
    // getCities: function(code) {
    //   return axios.get(API.DIRECTORY_URL + '/States/' + code + '/Cities').then(response => {
    //     return response.data;
    //   });
    // },
    /// Gets the list of possible document types
    getIdDocumentTypes: function(code) {
      return axios.get(API.DIRECTORY_URL + '/IdDocumentTypes').then(response => {
        return response.data;
      });
    },
    /// Gets the list of possible options to receive branches of the bank
    getBankBranches: function(code) {
      return axios.get(API.DIRECTORY_URL + '/BankBranches').then(response => {
        return response.data;
      });
    },
    /// Gets the list of possible values for the working experience durations
    // getWorkExperienceDurations: function(code) {
    //   return axios.get(API.DIRECTORY_URL + '/WorkExperienceDurations').then(response => {
    //     return response.data;
    //   });
    // },
    /// Gets the list of possible communication types
    getCommunicationTypes: function() {
      return axios.get(API.DIRECTORY_URL + '/CommunicationTypes').then(response => {
        return response.data;
      });
    },
  };
});
