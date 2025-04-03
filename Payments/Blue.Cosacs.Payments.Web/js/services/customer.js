/*global _, moment, module */
var customer = function ($http, $q, jsonUrlUtils) {
    'use strict';

    /**
     * This function returns a promise
     */
    var getById = function (customerId) {
        return $q(function (resolve) {
            $http.get('/Payments/DataMocks/Credit/Customer/CustomerSearchById_1.json').
                success(function (data) {
                    resolve(data);
                });
        });
    };

    return {
        getById: getById
    };
};

customer.$inject = ['$http', '$q', 'jsonUrlUtils'];
module.exports = customer;
