define(['axios', 'constants/api'], function(axios, API) {
  'use strict';
  var instance = axios.create({
    baseURL: `${API.BASE_URL}`,
  });

  instance.interceptors.response.use(
    response => {
      return response;
    },
    err => {
      return new Promise((resolve, reject) => {
        if (err.response.status === 401) {
          console.log('logout');
        }
        throw err;
      });
    }
  );
  // TODO: remove
  window.instance = instance;
  return instance;
});
