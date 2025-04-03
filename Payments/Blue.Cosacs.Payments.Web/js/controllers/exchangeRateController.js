/*global _, angular, console, moment, require, module */

var CurrencyRateData = require('../model/CurrencyRateData.js');
var exchangeRateController = function ($scope, promiseDataService, CommonService) {
    'use strict';

    $scope.selectedSearchCurrency = null;
    $scope.selectedSearchDateFrom = null;
    $scope.selectedNewCurrency = null;
    $scope.newRate = undefined;
    $scope.newDateFrom = null;
    $scope.currencyData = [];
    $scope.currencyList = [];
    $scope.showNewRow = false;
    $scope.searchExistingData = searchExistingData;
    $scope.isValidRate = isValidRate;
    $scope.isValidDate = isValidDate;

    $scope.editCurrencyRate = editCurrencyRate;
    $scope.deleteCurrency = deleteCurrency;
    $scope.saveChanges = saveChanges;
    $scope.cancelChanges = cancelChanges;
    $scope.addNew = addNew;
    $scope.saveNew = saveNew;
    $scope.cancelNewRecord = cancelNewRecord;

    activate();
    //////////////////

    function activate() {
        angular.element('body').removeClass('full-screen');
        getCurrencyCode();
        getExchangeRateData($scope.selectedSearchCurrency, $scope.selectedSearchDateFrom);
    }

    function getCurrencyCode() {
        promiseDataService.getAllExchangeRates().then(function (data) {
            if (typeof data === 'object') {
                $scope.currencyList = data;
            }
            else {
                console.log(data);
            }
        });

    }

    function getExchangeRateData(searchCurrency, searchDate) {
        var params = {
            currencyCode: searchCurrency ? searchCurrency : "All",
            dateFrom: searchDate ? moment(searchDate).format() : "All"
        };
        promiseDataService.getAllExchangeRatesByCode(params).then(function (data) {
            if (typeof data === 'object') {
                var result = [];
                _.each(data.ExchangeRateDetails, function (c) {
                    var currency = new CurrencyRateData();
                    _.merge(currency, c);
                    currency.operationAllowed = new Date(currency.DateFrom) > new Date();
                    result.push(currency);
                });
                $scope.currencyData = result;
            }
            else {
                CommonService.alert('Exception while fetching existing Exchange Rate data...', 'Error');
                console.log(data);
            }
        });
    }

    function searchExistingData() {
        var currencyCode = $scope.selectedSearchCurrency ? $scope.selectedSearchCurrency.split("-")[0].trim() : null;
        getExchangeRateData(currencyCode, $scope.selectedSearchDateFrom);
    }

    function isValidRate(inputRate) {
        if (!inputRate || isNaN(inputRate)) {
            return false;
        }
        else {
            return inputRate >= 0;
        }
    }

    function isValidDate(inputDate) {
        return inputDate && inputDate > new Date();
    }

    //Update
    function saveChanges(currency) {
        var errorMessage;
        if (!(currency.rateChanged && currency.dateFromChanged)) {
            errorMessage = 'Data Required';
        }
        else if (!isValidRate(currency.rateChanged)) {
            errorMessage = 'Rate must be a non negative number.';
        }
        else if (!isValidDate(currency.dateFromChanged)) {
            errorMessage = ' DateFrom must be greater than today.';
        }
        else {

            currency.createdBy = $scope.currentUser.userId;

            promiseDataService.updateExchangeRate(currency).then(function (data) {
                if (promiseDataService.checkReturnedValue(data)) {
                    currency.Rate = currency.rateChanged;
                    currency.DateFrom = currency.dateFromChanged;
                    currency.rateChanged = 0;
                    currency.dateFromChanged = null;
                    currency.operationAllowed = new Date(currency.DateFrom) > new Date();

                    CommonService.addGrowl({
                        timeout: 3,
                        type: 'success',
                        content: currency.CurrencyCode + " currency saved successfully"
                    });
                    currency.showEditFields = false;
                }
                else {
                    errorMessage = data.CustomError || data.errors;
                    CommonService.addGrowl({
                        timeout: 3,
                        type: 'danger',
                        content: errorMessage
                    });
                }
            });
        }
        if (errorMessage) {
            CommonService.addGrowl({
                timeout: 3,
                type: 'danger',
                content: errorMessage
            });
        }
    }

    //Delete
    function deleteCurrency(selectedCurrency) {

        var title = 'Delete Exchange Rate Entry',
            msg = 'Are you sure you want to delete this Exchange Rate Entry?';

        var deleteConfirmation = CommonService.$dialog.messageBox(title, msg, [
            {
                label: 'Delete',
                result: 'delete',
                cssClass: 'btn btn-primary'
            },
            {
                label: 'Cancel',
                result: 'cancel'
            }
        ]);
        deleteConfirmation.open().then(function (choice) {
            if (choice === 'delete') {
                var params = {
                    currencyCode: selectedCurrency.CurrencyCode,
                    dateFrom: selectedCurrency.DateFrom
                };
                promiseDataService.deleteExchangeRate(params).then(function (data) {
                    if (promiseDataService.checkReturnedValue(data)) {
                        $scope.currencyData = _.reject($scope.currencyData, function (currency) {
                            return (currency.CurrencyCode === selectedCurrency.CurrencyCode && currency.DateFrom === selectedCurrency.DateFrom);
                        });
                        CommonService.addGrowl({
                            timeout: 3,
                            type: 'success',
                            content: selectedCurrency.CurrencyCode + " currency deleted successfully"
                        });
                    }
                    else {
                        CommonService.addGrowl({
                            timeout: 3,
                            type: 'danger',
                            content: data.errors
                        });
                    }
                });
            }
        });
    }

    //Insert - New
    function saveNew() {
        var errorMessage;
        if (!($scope.selectedNewCurrency && $scope.newRate && $scope.newDateFrom)) {
            errorMessage = 'Data Required';
        }
        else if (!isValidRate($scope.newRate)) {
            errorMessage = 'Rate must be a non negative number.';
        }
        else if (!isValidDate($scope.newDateFrom)) {
            errorMessage = ' DateFrom must be greater than today.';
        }
        else {
            var currencySplit = $scope.selectedNewCurrency.split("-");

            var newCurrencyData = new CurrencyRateData();
            newCurrencyData.CurrencyCode = currencySplit[0].trim();
            newCurrencyData.CurrencyName = currencySplit[1].trim();
            newCurrencyData.Rate = $scope.newRate;
            newCurrencyData.DateFrom = $scope.newDateFrom;
            newCurrencyData.createdBy = $scope.currentUser.userId;
            newCurrencyData.operationAllowed = new Date($scope.newDateFrom) > new Date();

            promiseDataService.insertExchangeRate(newCurrencyData).then(function (data) {
                if (promiseDataService.checkReturnedValue(data)) {
                    $scope.currencyData.splice(0, 0, newCurrencyData);
                    $scope.showNewRow = false;
                    $scope.selectedNewCurrency = '';
                    $scope.newRate = '';
                    $scope.newDateFrom = null;

                    CommonService.addGrowl({
                        timeout: 3,
                        type: 'success',
                        content: newCurrencyData.currencyCode + " currency added successfully"
                    });
                }
                else {
                    errorMessage = data.CustomError ? data.CustomError : data.errors;
                    CommonService.addGrowl({
                        timeout: 3,
                        type: 'danger',
                        content: errorMessage
                    });
                }
            });
        }
        if (errorMessage) {
            CommonService.addGrowl({
                timeout: 3,
                type: 'danger',
                content: errorMessage
            });
        }
    }

    //Cancel Edit Changes
    function cancelChanges(currency) {
        currency.dateFromChanged = null;
        currency.rateChanged = 0;
        currency.showEditFields = false;
    }

    //Cancel New Record Changes
    function cancelNewRecord() {
        $scope.selectedNewCurrency = '';
        $scope.newRate = '';
        $scope.newDateFrom = null;
        $scope.showNewRow = false;
    }

    //Show Input Form
    function addNew() {
        $scope.showNewRow = true;
    }

    //Show Form to Edit
    function editCurrencyRate(currency) {
        currency.rateChanged = currency.Rate;
        currency.dateFromChanged = new Date(currency.DateFrom);
        currency.showEditFields = true;
    }

};

exchangeRateController.$inject = ['$scope', 'promiseDataService', 'CommonService'];;

module.exports = exchangeRateController;