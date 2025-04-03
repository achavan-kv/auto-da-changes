/*global angular, require */
angular.module('NonStocks.controllers', [])
    .controller('nonStockController', require('./nonStockController'))
    .controller('productLinkController', require('./productLinkController'))
    .controller('searchController', require('./searchController'))
    .controller('promotionsController', require('./promotionsController'));
