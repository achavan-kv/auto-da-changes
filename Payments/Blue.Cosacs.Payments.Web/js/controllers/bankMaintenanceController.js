/*global _, moment, module */
var bankMaintenanceController = function ($scope, $http, $location, jsonUrlUtils,
                                          CommonService, promiseDataService) {
    'use strict';

    $scope.mainTitle = "Bank Maintenance";

    // init values
    var growlTimeout = 3;
    $scope.creatingNewBank = false;
    $scope.newBank = {bankName: "", bankCode: "", active: true};

    $scope.showNewRow = function ($event) {
        $scope.creatingNewBank = true;
        $event.preventDefault();
    };

    $scope.loadBanks = function () {
        promiseDataService.getBanksData().then(function (data) {
            if (data && data.Banks) {
                $scope.Banks = data.Banks;
            }
        });
    };

    $scope.loadBanks();

    $scope.createNewBank = function () {

        var bank = $scope.newBank;

        $http({
            method: 'PUT',
            url: 'Payments/Api/BankMaintenance',
            data: bank
        }).then(function (data) {
            if (data.data.Result === "Success") {

                CommonService.addGrowl({
                    type: 'info', // (optional - info, danger, success, warning)
                    content: 'Bank added successfully',
                    timeout: growlTimeout
                });

                $scope.newBank = {};
                $scope.populateExistingBanks([data.data.Bank][0]);
                $scope.creatingNewBank = false;

            } else {
                CommonService.addGrowl({
                    type: 'danger', // (optional - info, danger, success, warning)
                    content: data.data.Message,
                    timeout: growlTimeout
                });
            }
        });
    };

    $scope.cancelNewBank = function () {
        $scope.newBank = {};
        $scope.creatingNewBank = false;
    };

    $scope.editBank = function (bank) {
        bank.edit = true;
    };

    $scope.cancelBankEdit = function (bank) {
        bank.edit = false;
    };

    $scope.saveBankEdit = function (bank) {
        $http({
            method: 'PUT',
            url: 'Payments/Api/BankMaintenance',
            data: bank
        }).then(function (data) {
            if (data.data.Result === "Success") {

                CommonService.addGrowl({
                    type: 'info', // (optional - info, danger, success, warning)
                    content: 'Bank updated successfully',
                    timeout: growlTimeout
                });

                bank.edit = false;

            } else {
                CommonService.addGrowl({
                    type: 'danger', // (optional - info, danger, success, warning)
                    content: data.data.Message,
                    timeout: growlTimeout
                });
            }
        });

    };

    $scope.populateExistingBanks = function (bank) {
        $scope.Banks.push({
            BankName: bank.BankName,
            Active: bank.Active
        });
    };

};

bankMaintenanceController.$inject = ['$scope', '$http', '$location', 'jsonUrlUtils',
    'CommonService', 'promiseDataService'];
module.exports = bankMaintenanceController;


