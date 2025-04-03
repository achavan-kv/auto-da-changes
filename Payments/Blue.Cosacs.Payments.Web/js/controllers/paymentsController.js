/*global _, S, moment, module */
var paymentsController = function ($scope, $http, $location, customer, stringUtils, promiseDataService) {
    'use strict';

    $scope.MasterData = {};
    $scope.mainTitle = "Payments";
    $scope.moment = moment;
    $scope.math = Math;
    $scope.dateFormatShort = 'DD-MMM-YYYY';
    $scope.dateFormat = 'DD-MMM-YYYY HH:mm:ss';

    $scope.agreements = [];
    $scope.order = null;
    $scope.AgreementTotalDue = 0;
    $scope.AgreementTotalDueNextPayment = null;
    $scope.bailiffTotalFeeDue = 0;

    $scope.paymentMethodsScope = {};
    $scope.paymentMethod = undefined;
    $scope.foreignCashScope = {};
    $scope.foreignCash = undefined;
    $scope.foreignCurrentyLocalValue = 0;

    $scope.transactions = null;

    $scope.customerId = '';
    $scope.customerName = '';
    $scope.customerAddress = '';
    $scope.customerGender = '';
    $scope.paymentAmount = 0;
    $scope.tenderedAmount = 0;
    $scope.voucherCode = '';
    $scope.voucherValue = 0;
    $scope.change = 0;

    $scope.LoadedData = {
        AgreementNumber: null,
        Customer: null
    };

    var searchValues = {
        CustomerId: null,
        SelectedAgreement: null
    };

    $scope.searchValues = searchValues = $location.search();

    var getPaymentMethodsScopeObj = function (methodsArray) {
        var retValue = {};
        _.map(methodsArray, function (mt) {
            if (mt.Description) {
                retValue[mt.Description] = mt.Description;
            }
        });
        return retValue;
    };

    var getBanksScopeObj = function (banksArray) {
        var retValue = {};
        _.map(banksArray, function (bk) {
            if (bk.BankName) {
                retValue[bk.BankName] = bk.BankName;
            }
        });
        return retValue;
    };

    var getForeignCashScopeObj = function (methodsArray) {
        var retValue = {};
        _.map(methodsArray, function (mt) {
            if (mt) {
                retValue[mt] = mt;
            }
        });
        return retValue;
    };

    var refreshForeignCashSelected = function (scope) {
        if (!stringUtils.isNullOrWhitespace($scope.foreignCash)) {
            var p = {
                currencyCode: getForeignCashCode($scope.foreignCash),
                dateFrom: moment(new Date()).format()
            };

            promiseDataService.getAllExchangeRatesByCode(p)
                .then(function (cashData) {
                    var rateObj = null;
                    if (cashData && cashData.ExchangeRateDetails && cashData.ExchangeRateDetails.length > 0) {
                        rateObj = cashData.ExchangeRateDetails[0];
                    }

                    if (rateObj && rateObj.Rate) {
                        // this value should be how much 1 unit of foreign currency is worth in local money
                        $scope.foreignCurrentyLocalValue = rateObj.Rate;
                        calculateChange($scope);
                    }
                });
        } else {
            clearForeignCashValues(scope);
        }
    };

    var getForeignCashCode = function (foreignCashString) {
        var retString = '';
        if (foreignCashString.indexOf('-') > 0) {
            retString = foreignCashString.split('-')[0];
        }
        if (!stringUtils.isNullOrWhitespace(foreignCashString)) {
            retString = retString.trim();
        }
        return retString;
    };

    var clearForeignCashValues = function (scope) {
        scope.foreignCash = undefined;
        scope.foreignCurrentyLocalValue = 0;
        calculateChange(scope);
    };

    $scope.$watchGroup(['paymentMethod', 'foreignCash'],
        function (newValues, oldValues, scope) {
            if (scope.paymentMethod === 'Foreign Cash') {
                refreshForeignCashSelected(scope);
            } else {
                clearForeignCashValues(scope);
            }
        });

    $scope.loadPaymentsScreen = function () {

        promiseDataService.getActivePaymentMethodsData().then(function (paymentMethods) {
            if (paymentMethods) {
                $scope.paymentMethodsScope = getPaymentMethodsScopeObj(paymentMethods.PaymentMethods);
            }
        });

        promiseDataService.getActiveExchangeRates().then(function (rates) {
            if (rates) {
                $scope.foreignCashScope = getForeignCashScopeObj(rates);
            }
        });

        promiseDataService.getActiveBanksData().then(function (banks) {
            if (banks && banks.Banks) {
                $scope.banksScope =
                    getBanksScopeObj(banks.Banks);
            }
        });

        if (searchValues.CustomerId) {
            $http.get('/Payments/DataMocks/Credit/Customer/CustomerSearchById_1.json').
                success(function (data) {
                    if (data) {
                        if (data.length > 0) { // if its an array, extract the customer
                            data = data[0];
                        }
                        $scope.customerId = data.Id;

                        executeLoadCustomerAgreements($scope.customerId);
                        executeLoadCustomerTransactions($scope.customerId);

                        $scope.customerName =
                            data.Title + ' ' + data.FirstName + ' ' + data.LastName;
                        $scope.addressType = data.AddressType;
                        $scope.customerAddress =
                            (data.Address1 ? data.Address1 + '\n' : '') +
                            (data.Address2 ? data.Address2 + '\n' : '') +
                            (data.Address3 ? data.Address3 + '\n' : '') +
                            (data.PostCode ? data.PostCode : '');
                        $scope.agreements = [data];
                    }
                }).
                error(function (data, status) {
                    window.console.log("Func:loadPaymentsScreen, CustomerId, Error: " + status + ";\r\n" + data);
                });
        } else if (searchValues.OrderNumber) {
            searchValues.OrderNumber = 1;
            $http.get('/Payments/DataMocks/Sales/Order/SearchControllerMock_' + searchValues.OrderNumber + '.json').
                success(function (data) {
                    if (data) {
                        var tmpCust = data.customer;
                        if (tmpCust) {
                            $scope.customerName =
                                tmpCust.title + ' ' + tmpCust.firstName + ' ' + tmpCust.lastName;
                            $scope.addressType = tmpCust.AddressType;
                            $scope.customerAddress =
                                (tmpCust.addressLine1 ? tmpCust.addressLine1 + '\n' : '') +
                                (tmpCust.addressLine2 ? tmpCust.addressLine2 + '\n' : '') +
                                (tmpCust.postCode ? tmpCust.postCode : '');
                        } else {
                            $scope.customerName = 'N.A.';
                        }

                        $scope.order = data;
                    }
                }).
                error(function (data, status) {
                    window.console.log("Func:loadPaymentsScreen, OrderNumber, Error: " + status + ";\r\n" + data);
                });
        }
    };

    $scope.loadPaymentsScreen();

    var executeLoadCustomerTransactions = function (customerId) {
        promiseDataService.getCustomerTransactionsById(customerId)
            .then(function (data) {
                if (data && data.Transactions) {
                    $scope.transactions = data.Transactions;
                }
            });
    };

    var executeLoadCustomerAgreements = function (customerId) {
        promiseDataService.getCustomerAgreementsById(customerId)
            .then(function (data) {
                if (data) {
                    if (data.Agreements && data.Agreements.length > 0) {
                        _.map(data.Agreements, function (ag) {
                            ag.agreementVisible = false;
                            ag.agreementSelected =
                                ag.AgreementNumber === parseInt(searchValues.SelectedAgreement, 10);

                            ag.agreementPaymentAmount = ag.NextPaymentValue;
                        });

                        fillCalcPaymentDueValues(data.Agreements);
                        fillCallBailiffFeeValues(data.Agreements);

                        $scope.agreements = data.Agreements;
                    }
                }
            }, function (data, status) {
                window.console.log("Func:getCustomerAgreementsById, agreement, Error: " + status + ";\r\n" + data);
            });
    };

    $scope.updateAgreementCalculatedValues = function () {
        var allAgreements = $scope.agreements;
        fillCalcPaymentDueValues(allAgreements);
        fillCallBailiffFeeValues(allAgreements);
    };

    var fillCalcPaymentDueValues = function (agreements) {
        var tmpAgreementTotalDue = 0;
        var tmpAgreementTotalDueNextPayment = null;

        _.map(agreements, function (ag) {
            tmpAgreementTotalDue += ag.NextPaymentValue || 0;

            var tmpDate = ag.DateNextPaymentDue ? +(moment(ag.DateNextPaymentDue)) : null;
            if (isNumeric(tmpDate)) {
                if (tmpAgreementTotalDueNextPayment === null || tmpAgreementTotalDueNextPayment > tmpDate) {
                    tmpAgreementTotalDueNextPayment = tmpDate; // find nearest 'DateNextPaymentDue'
                }
            }
        });
        $scope.AgreementTotalDue = tmpAgreementTotalDue;
        $scope.AgreementTotalDueNextPayment = moment(tmpAgreementTotalDueNextPayment);
        if ($scope.AgreementTotalDue > 0) {
            $scope.paymentAmount = $scope.AgreementTotalDue;
        }
    };

    var fillCallBailiffFeeValues = function (agreements) {
        var tmpTotalBailiffFee = 0;
        _.map(agreements, function (ag) {
            ag.BailiffFee = 0;
            if (ag.Bailiff && isNumeric(ag.BailiffPercentFee) && ag.BailiffPercentFee > 0) {
                var tmpNextPaymentValue = ag.agreementPaymentAmount || 0;
                var tmpNum = tmpNextPaymentValue * ag.BailiffPercentFee / 100;
                if (isNumeric(tmpNum)) {
                    ag.BailiffFee = tmpNum;
                    tmpTotalBailiffFee += tmpNum;
                }
            }
        });
        $scope.bailiffTotalFeeDue = tmpTotalBailiffFee;
    };

    var isNumeric = function (val) {
        return !isNaN(val) && isFinite(val);
    };

    var executeLoadVoucherValue = function (voucherCode) {
        promiseDataService.getVoucherValue(voucherCode)
            .then(function (data) {
                if (stringUtils.isNullOrWhitespace(voucherCode)) {
                    $scope.voucherValue = 0;
                } else if (data) {
                    $scope.voucherValue = data.Value;
                }
            }, function (data, status) {
                window.console.log("Func:getVoucherValue, voucher, Error: " + status + ";\r\n" + data);
            });
    };

    $scope.$watchGroup(['tenderedAmount', 'bailiffTotalFeeDue', 'voucherValue', 'paymentAmount'],
        function (newValues, oldValues, scope) {
            if (!isNumeric(scope.bailiffTotalFeeDue)) {
                scope.bailiffTotalFeeDue = 0;
            }
            calculateChange(scope);
        });

    $scope.applyVoucher = function () {
        executeLoadVoucherValue($scope.voucherCode, function () {
            calculateChange($scope);
        });
    };

    var calculateChange = function (scope) {
        var tenderedValue = scope.tenderedAmount;
        if (scope.foreignCurrentyLocalValue > 0) {
            tenderedValue = tenderedValue * scope.foreignCurrentyLocalValue;
        }

        scope.change = tenderedValue - scope.bailiffTotalFeeDue -
            Math.abs(scope.voucherValue) - scope.paymentAmount;

        scope.change = Math.round(scope.change * 100) / 100;
    };

    $scope.loadCustomerAgreements = function (customerId) {
        if (customerId) {
            executeLoadCustomerAgreements(customerId);
        }
    };

    $scope.agreementPanelBodyToggle = function (elementAgreement) {
        if (elementAgreement) {
            if (typeof elementAgreement.agreementVisible !== 'boolean') {
                elementAgreement.agreementVisible = false;
            }
            elementAgreement.agreementVisible = !elementAgreement.agreementVisible;
        }
    };

    $scope.isPaymentButtonEnabled = function () {
        return !($scope.paymentAmount !== 0 && $scope.tenderedAmount !== 0 && $scope.change >= 0);
    };

    $scope.isTenderedAmountInputDisabled = function () {
        if ($scope.paymentMethod === 'Gift Voucher') {
            $scope.tenderedAmount = 0;
            return true;
        } else {
            return false;
        }
    };

    function getSettings() {
        promiseDataService.getSettingsData().then(function (data) {
            if (data) {
                $scope.MasterData.settings = data;

                $scope.MasterData.paymentCardType = [];

                if (data.PaymentCardType) {
                    _.each(data.PaymentCardType, function (cardType) {
                        if (cardType) {
                            $scope.MasterData.paymentCardType.push(S(cardType).trim().s);
                        }
                    });
                }
            }
        });
    }

    getSettings();

};

paymentsController.$inject = ['$scope', '$http', '$location', 'customer', 'stringUtils', 'promiseDataService'];
module.exports = paymentsController;
