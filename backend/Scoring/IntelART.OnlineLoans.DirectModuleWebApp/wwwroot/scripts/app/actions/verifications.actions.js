define(['services/api', 'qs'], function(axios, qs) {
  'use strict';
  var url = '/loan/Applications';
  return {
    verifyPhone: function(phone) {
      return axios.post(url + '/SendMobilePhoneAuthorization', phone).catch(function(error) {
        alert(error.response && error.response.data.Message);
        throw new Error(error);
      });
    },
    verifySMSCode: function(data) {
      return axios.post(url + '/CheckMobilePhoneAuthorization', data);
    },
  };
});
