'use strict';
var common = require('./common');

var customerSearchController = function($scope) {
    $scope.title = "Customer Search";
    $scope.formatDate = function(date) {
        return moment(date).format("DD MMMM YYYY");
    };
};

customerSearchController.$inject = ['$scope'];
module.exports = customerSearchController;
