/*global module*/
var promiseDataService = function ($http, $q) {
    'use strict';

    var getData = function (url) {
        return $q(function (resolve, error) {
            $http.get(url).
                success(function (data) {
                    resolve(data);
                }).
                error(function () {
                    if (error) {
                        error.apply(arguments);
                    }
                });
        });
    };

    var getDataWithParams = function (url, params) {
        return $q(function (resolve, error) {
            $http({
                method: 'GET',
                url: url,
                params: params
            }).
                success(function (data) {
                    resolve(data);
                }).
                error(function () {
                    if (error) {
                        error.apply(arguments);
                    }
                });
        });
    };

    var postData = function (url, params) {
        return $q(function (resolve, error) {
            $http.post(url, params).
                success(function (data) {
                    resolve(data);
                }).
                error(function () {
                    if (error) {
                        error.apply(arguments);
                    }

                });
        });
    };

    var putData = function (url, params) {
        return $q(function (resolve, error) {
            $http.put(url, params).
                success(function (data) {
                    resolve(data);
                }).
                error(function () {
                    if (error) {
                        error.apply(arguments);
                    }

                });
        });
    };

    var deleteData = function (url, params) {
        return $q(function (resolve, error) {
            $http({
                method: 'DELETE',
                url: url,
                params: params
            }).
                success(function (data) {
                resolve(data);
            }).
                error(function () {
                    if (error) {
                        error.apply(arguments);
                    }

                });
        });
    };

    var getCustomerAgreementsById = function (customerId) {
        return $q(function (resolve, error) {
            $http.get('/Payments/DataMocks/Credit/CustomerAgreement/CustomerAgreementsSearchByIdMock_1.json').
                success(function (data) {
                    resolve(data);
                }).
                error(function (data, status) {
                    error(data, status);
                });
        });
    };

    var getCustomerTransactionsById = function (customerId) {
        return $q(function (resolve, error) {
            $http.get('/Payments/DataMocks/Credit/Transactions/CustomerTransactions.json').
                success(function (data) {
                    resolve(data);
                }).
                error(function (data, status) {
                    error(data, status);
                });
        });
    };

    var getVoucherValue = function (voucherCode) {
        return $q(function (resolve, error) {
            $http.get('/Payments/DataMocks/GiftVouchers/VoucherCodeIdMock.json').
                success(function (data) {
                    resolve(data);
                }).
                error(function (data, status) {
                    error(data, status);
                });
        });
    };

    var getAllPaymentMethodsData = function () {
        return getData('payments/api/PaymentSetup/GetAllPaymentMethods');
    };

    var getAllPaymentMapData = function () {
        return getData('payments/api/PaymentMethodMap');
    };

    var getActivePaymentMethodsData = function () {
        return getData('payments/api/PaymentSetup/GetActivePaymentMethods');
    };

    var getBanksData = function () {
        return getData('Payments/Api/BankMaintenance/GetBanks');
    };

    var getActiveBanksData = function () {
        return getData('Payments/Api/BankMaintenance/GetActiveBanks');
    };

    var updatePaymentMethodStatus = function (params) {
        return postData("payments/api/PaymentSetup", params);
    };

    var getAllExchangeRates = function () {
        return getData('payments/api/ExchangeRate/GetRates');
    };

    var getActiveExchangeRates = function () {
        return getData('payments/api/ExchangeRate/GetActiveRates');
    };

    var getAllExchangeRatesByCode = function (params) {
        return getDataWithParams('payments/api/ExchangeRate/GetByCode', params);
    };

    var updateExchangeRate = function (ccyDetails) {
        return putData("/payments/api/ExchangeRate/GetByCode", ccyDetails);
    };

    var deleteExchangeRate = function (params) {
        return deleteData("/payments/api/ExchangeRate/Delete", params);
    };

    var insertExchangeRate = function (ccyDetails) {
        return postData("/payments/api/ExchangeRate/Insert", ccyDetails);
    };

    var getExchangeRateData = function (params) {
        return getData('payments/api/foreignCurrency', params);
    };

    var getSettingsData = function () {
        return getData('payments/api/settings');
    };

    var checkReturnedValue = function (data) {
        if (data) {
            if (!data.Valid) {
                return false;
            }
            else {
                return true;
            }
        }
    };

    return {
        getAllPaymentMapData: getAllPaymentMapData,
        getAllPaymentMethodsData: getAllPaymentMethodsData,
        getActivePaymentMethodsData: getActivePaymentMethodsData,
        updatePaymentMethodStatus: updatePaymentMethodStatus,
        getCustomerAgreementsById: getCustomerAgreementsById,
        getCustomerTransactionsById: getCustomerTransactionsById,
        getBanksData: getBanksData,
        getActiveBanksData: getActiveBanksData,
        getVoucherValue: getVoucherValue,
        getAllExchangeRatesByCode: getAllExchangeRatesByCode,
        getAllExchangeRates: getAllExchangeRates,
        getActiveExchangeRates: getActiveExchangeRates,
        updateExchangeRate: updateExchangeRate,
        checkReturnedValue: checkReturnedValue,
        deleteExchangeRate: deleteExchangeRate,
        insertExchangeRate: insertExchangeRate,
        getExchangeRateData: getExchangeRateData,
        getSettingsData: getSettingsData
    };

};

promiseDataService.$inject = ['$http', '$q'];
module.exports = promiseDataService;
