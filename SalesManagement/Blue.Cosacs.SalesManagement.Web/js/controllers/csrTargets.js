'use strict';

var csrTargetsController = function ($scope, $http, UsersService, CommonService) {
    $scope.currentYear = {};
    $scope.currentYear.monthlySales = null;
    $scope.currentYear.weeklySales = null;
    $scope.currentYear.annualSales = null;

    $http({
        url: '/SalesManagement/api/SalesPersonTargets/',
        method: "GET"
    })
             .then(function (resp) {

                 var year = new Date().getFullYear();

                 var results = _.filter(resp.data, function (current) {
                     return current.Year < year;
                 });

                 var currentYearData = _.filter(resp.data, function (current) {
                     return current.Year === year;
                 });

                 var result = _.pluck(currentYearData, 'TargetYear');

                 if (result.length > 0) {

                     $scope.currentYear.monthlySales = (result / 12).toFixed(2);
                     $scope.currentYear.weeklySales = (result / 52).toFixed(2);
                     $scope.currentYear.annualSales = result;
                 }

                 $scope.getAnnualTargetHistory = results;
             });

    $scope.save = function () {
        $http({
            url: '/SalesManagement/api/SalesPersonTargets/SaveSalesPersonTargets',
            method: "GET",
            params: {
                TargetYear: $scope.currentYear.annualSales
            }
        })
       .then(function (resp) {
           CommonService.addGrowl({
               timeout: 5,
               type: 'success', // (optional - info, danger, success, warning)
               content: 'The annual target was successfully saved !'
           });
       });
    };

    //Week
    $scope.calculateMonthlyAndAnnualSales = function () {
        $scope.currentYear.monthlySales = ($scope.currentYear.weeklySales * 4.3).toFixed(2);
        $scope.currentYear.annualSales = ($scope.currentYear.weeklySales * 52).toFixed(2);
    };

    //Month
    $scope.calculateWeeklyAndAnnualSales = function () {
        $scope.currentYear.weeklySales = ($scope.currentYear.monthlySales / 4.3).toFixed(2);
        $scope.currentYear.annualSales = ($scope.currentYear.monthlySales * 12).toFixed(2);
    };

    //Year
    $scope.calculateWeeklyAndMonthlySales = function () {
        $scope.currentYear.weeklySales = ($scope.currentYear.annualSales / 52).toFixed(2);
        $scope.currentYear.monthlySales = ($scope.currentYear.annualSales / 12).toFixed(2);
    };

    $scope.getWeeklyTargets = function (annualTarget) {
        return (annualTarget / 52).toFixed(2);
    };

    $scope.getMonthlyTargets = function (annualTarget) {
        return (annualTarget / 12).toFixed(2);
    };

    $scope.getAnnualTargets = function (annualTarget) {
        return annualTarget.toFixed(2);
    };
};

csrTargetsController.$inject = ['$scope', '$http', 'UsersService', 'CommonService'];

module.exports = csrTargetsController;