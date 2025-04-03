'use strict';

var dependInjects = ['PosDataService', 'CommonService', 'PaymentViewConst'];

var _payment = null,
    _completedPayments = null,
    _user    = null,
    _results = [],
    SalesEnums = require('../model/SalesEnums');

var paymentService = function (PosDataService, CommonService, PaymentViewConst) {
    return {
        getExchangeRate: getExchangeRate,
        getPaymentMethods: getPaymentMethods,
        getCardTypes: getCardTypes,
        getActiveBanks: getActiveBanks,
        getCurrencyCodes: getCurrencyCodes,
        checkStoreCardBalance: checkStoreCardBalance,
        checkGiftVoucherDetails: checkGiftVoucherDetails,
        validate: validatePayment,
        isRequiredFieldsValid: isRequiredFieldsValid,
        getStoreCardCustomerId: getStoreCardCustomerId,
        canEditCustomerCardNo:canEditCustomerCardNo
    };

    function getExchangeRate(code) {
        var params = {
            currencyCode: code
        };

        return PosDataService.getExchangeRateData(params);
    }

    function getPaymentMethods() {
        return PosDataService.getPaymentMethodsData();
    }

    function getCurrencyCodes() {
        return PosDataService.getCurrencyCodesData();
    }

    function checkStoreCardBalance(params) {
        return PosDataService.checkStoreCardBalance(params);
    }

    function getStoreCardCustomerId(params) {
        return PosDataService.getStoreCardCustomerId(params);
    }

    function checkGiftVoucherDetails(params) {
        return PosDataService.checkGiftVoucherDetails(params);
    }

    function canEditCustomerCardNo(){
        return PosDataService.hasPermission(SalesEnums.Permissions.EditStoreCardNo);
    }

    //region Validation

    function validatePayment(payment, user ,completedPayments) {
        var SalesEnums = require('../model/SalesEnums');
        var promises = [];
        var deferred = CommonService.$q.defer();
        if (!payment) {
            deferred.reject('Cannot load Payment object');

            return deferred.promise;
        }

        if (!user) {
            deferred.reject('Cannot load User object');

            return deferred.promise;
        }

        _payment = payment;
        _user = user;
        _completedPayments = completedPayments;

        var methodId = parseInt(_payment.paymentMethodId) || 0;

        switch (methodId) {
            case SalesEnums.PaymentMethods.StoreCard:
                promises.push(validateStoreCardDetails());
                break;
            case SalesEnums.PaymentMethods.GiftVoucher:
                promises.push(validateGiftVoucherDetails());
                break;
        }

        CommonService.$q.all(promises)
            .then(
            function () {
                deferred.resolve(true)
            },
            function () {
                deferred.reject(_results);
            });

        return deferred.promise;
    }

    function validateRequired(name, title) {
        var _title = title || S(name).capitalize().s;
        var deferred = CommonService.$q.defer();

        if (!_payment[name]) {
            deferred.reject();
            _results.push('');
        }

        deferred.resolve();
        return deferred.promise;
    }

    function isRequiredFieldsValid(payment) {
        if (!payment) {
            return false;
        }
        _payment = payment;

        var ignored = ['change', 'storeCardNo'];
        var ret = true;

        var methodId = parseInt(_payment.paymentMethodId) || 0;
        var viewItems = PaymentViewConst[methodId];

        if (viewItems && viewItems.length > 0) {
            _.each(viewItems, function (name) {
                if (!_.contains(ignored, name) && !_payment[name]) {
                    if ((name === 'tendered' && !_payment.amount)) {
                        ret = true;
                    }
                    else {
                        ret = false;
                    }
                }
            });

        }

        return ret;

    }

    function validateStoreCardDetails() {
        var deferred = CommonService.$q.defer(),
            params = {
                storeCardNo: _payment.storeCardNo
            };

        if (!_payment.storeCardNo || _payment.storeCardNo.length !== 16) {
            var msg = "A 16 digits 'Store Card Number' is required";

            deferred.reject();
            _results.push(msg);
            return deferred.promise;
        }

        checkStoreCardBalance(params).then(function (data) {
            if (data) {
                if (data.errorMessage) {
                    deferred.reject();
                    _results.push(data.errorMessage);
                }
                else{
                    var alreadyPaidStoreCardAmount = _.chain(_completedPayments)
                        .where({
                            paymentMethodId: SalesEnums.PaymentMethods.StoreCard.toString(),
                            storeCardNo: _payment.storeCardNo
                        })
                        .reduce(function (memo, storeCardPayment) {
                            return memo + parseInt(storeCardPayment.amount || 0);
                        }, 0)
                        .value();

                    if (data.availableBalance < alreadyPaidStoreCardAmount + _payment.tendered) {

                        var msg = "Tendered amount has been changed as per available balance in your Store Card.";

                        _payment.tendered = data.availableBalance - alreadyPaidStoreCardAmount;

                        deferred.reject();
                        _results.push(msg);
                    }
                    else {

                        if (data.customerId) {
                            CommonService.$broadcast("pos:forceSelectCustomer", data.customerId);
                        }
                    }
                }

            } else {
                msg = 'Cannot get data from the server.';

                deferred.reject();
                _results.push(msg);
            }

            deferred.resolve();

        }, function (reason) {
            deferred.reject();
            _results.push(reason);

        });

        return deferred.promise;
    }

    function validateGiftVoucherDetails() {
        var deferred = CommonService.$q.defer(),
            params = {
                giftVoucherIssuer: _payment.voucherIssuer,
                otherCompanyNo: _payment.voucherIssuerCode || '',
                giftVoucherNo: _payment.voucherNo
            };

        var giftVoucherUsed = false;
        if (_payment.voucherIssuerCode) {

            giftVoucherUsed = _.where(_completedPayments,
                {
                    paymentMethodId: SalesEnums.PaymentMethods.GiftVoucher.toString(),
                    voucherIssuer: _payment.voucherIssuer,
                    voucherIssuerCode: _payment.voucherIssuerCode,
                    voucherNo: _payment.voucherNo
                });
        }
        else {
            giftVoucherUsed = _.where(_completedPayments,
                {
                    paymentMethodId: SalesEnums.PaymentMethods.GiftVoucher.toString(),
                    voucherIssuer: _payment.voucherIssuer,
                    voucherNo: _payment.voucherNo
                });
        }
        if (giftVoucherUsed && giftVoucherUsed.length > 0) {
            var msg = "This Gift Voucher has been used in current sale. So can not be used again";
            deferred.reject();
            _results.push(msg);
            deferred.resolve();
        }
        else {
            checkGiftVoucherDetails(params).then(function (data) {
                if (data) {
                    if (data.errorMessage) {
                        deferred.reject();
                        _results.push(data.errorMessage);
                    }
                    else if (data.value < _payment.tendered) {
                        var msg = "Tendered amount has been changed as per value of your Gift Voucher.";

                        _payment.tendered = data.value;
                        deferred.reject();
                        _results.push(msg);
                    }
                }
                else {
                    msg = 'Cannot get data from the server.';

                    deferred.reject();
                    _results.push(msg);
                }
                deferred.resolve();

            }, function (reason) {
                deferred.reject();
                _results.push(reason);
            });
        }
        return deferred.promise;
    }

    function validateReturnedReason() {
        var deferred = CommonService.$q.defer();

        checkGiftVoucherDetails(params).then(function (data) {
            if (data) {
                if (data.errorMessage) {
                    deferred.reject();
                    _results.push(data.errorMessage);
                }
                else if (data.value < _payment.tendered) {
                    var msg = "Tendered amount has been changed as per value of your Gift Voucher.";

                    _payment.tendered = data.value;
                    deferred.reject();
                    _results.push(msg);

                }
            } else {
                msg = 'Cannot get data from the server.';

                deferred.reject();
                _results.push(msg);
            }

            deferred.resolve();

        }, function (reason) {
            deferred.reject();
            _results.push(reason);
        });

        return deferred.promise;
    }

    function getActiveBanks() {
        return PosDataService.getActiveBanks();
    }

    function getCardTypes() {
        return PosDataService.getCardTypesData();
    }

};

paymentService.$inject = dependInjects;

module.exports = paymentService;
