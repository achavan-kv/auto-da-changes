'use strict';
function get($http, url) {

    var request = $http({
        method: "GET",
        url: url,
        cache: true
    });
    return (request);
}

var settingsService = function ($http) {
    return {
        credit: function () {
            return get($http, '/credit/api/settings')
        },
        payment: function () {
            return get($http, 'payments/api/PaymentSetup/GetActivePaymentMethods')
        },
        fieldsSettings: function() {
            return get($http, '/credit/api/customizeFields');
        }
    }
};
settingsService.$inject = ['$http'];
module.exports = settingsService;