/*global angular, require */
angular.module('Payments.controllers', [])
    .controller('searchController', require('./searchController'))
    .controller('paymentsController', require('./paymentsController'))
    .controller('bankMaintenanceController', require('./bankMaintenanceController'))
    .controller('paymentSetupController', require('./paymentSetupController'))
    .controller('paymentMapController', require('./paymentMapController'))
    .controller('exchangeRateController', require('./exchangeRateController'));


