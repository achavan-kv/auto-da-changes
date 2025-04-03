/*global angular, require */
angular.module('Payments.services', [])
    .factory('customer', require('./customer'))
    .factory('jsonUrlUtils', require('./jsonUrlUtils'))
    .factory('stringUtils', require('./stringUtils'))
    .factory('promiseDataService', require('./promiseDataService'));
