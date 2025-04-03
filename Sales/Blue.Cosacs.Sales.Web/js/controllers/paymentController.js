'use strict';

var dependInjects = ['$scope', 'CommonService', 'PaymentService', 'PaymentViewConst'];

var Utilities = require('../model/Utilities');

function paymentController($scope, CommonService, PaymentService, PaymentViewConst) {

    $scope.sections = {
        chequeNo: {
            visible: false
        },
        bank: {
            visible: false
        },
        cardType: {
            visible: false
        },
        cardNo: {
            visible: false
        },
        bankAccountNo: {
            visible: false
        },
        storeCardNo: {
            visible: false
        },
        voucherNo: {
            visible: false
        },
        tendered: {
            visible: false
        },
        change: {
            visible: false
        },
        currency: {
            visible: false
        },
        buttonPay: {
            visible: true
        }
    };
    $scope.calChange = calChange;
    $scope.choosePaymentMethod = choosePaymentMethod;
    $scope.savePayment = savePayment;
    $scope.restrictTenderedAmount = restrictTenderedAmount;
    $scope.checkPayment = checkPayment;
    $scope.onCurrencyChange = onCurrencyChange;
    $scope.isValidStoreCardInput = isValidStoreCardInput;
    $scope.checkReturnPayment = checkReturnPayment;
    $scope.authoriseStoreCardEdit = authoriseStoreCardEdit;
    $scope.canEditCustCard = false;
    $scope.canEditCustCard = false;
    $scope.checkingEditAuthorisation = false;
    $scope.getCustomerDetails = getCustomerDetails;
    activate();

    $scope.listClass = 'form-control';
    var SalesEnums = require('../model/SalesEnums');

    //region Public Methods

    function calChange() {
        return $scope.payment.getChange();
    }

    function choosePaymentMethod(value, type) {
        $scope.payment.paymentMethodId = value + '';
        $scope.payment.payType = type + '';
        setView();
    }

    function savePayment() {

        var isIniValid = $scope.payment &&
                         $scope.user &&
                         PaymentService.isRequiredFieldsValid($scope.payment);

        if (!isIniValid) {
            return;
        }

        var methodId = parseInt($scope.payment.paymentMethodId) || 0;

        if ($scope.cart.originalOrder.order) {

            var isReturnPaymentValid;
            switch (methodId) {
                case SalesEnums.PaymentMethods.StoreCard:
                    isReturnPaymentValid = validateStoreCardPayment(methodId);
                    break;
                // case SalesEnums.PaymentMethods.CreditCard:
                //     isReturnPaymentValid = validateCardPayment(methodId);
                //     break;
                case SalesEnums.PaymentMethods.Cheque:
                    //isReturnPaymentValid = validateChequePayment(methodId);
                    break;
            }

            if (isReturnPaymentValid) {
                showError(isReturnPaymentValid);
                return;
            }
        }

        if (methodId === SalesEnums.PaymentMethods.GiftVoucher &&
            $scope.payment.voucherIssuer === 'N' && !$scope.payment.voucherIssuerCode) {
            return;
        }

        PaymentService.validate($scope.payment, $scope.user, $scope.cart.payments).then(function (results) {

            if ($scope.cart.isReturnItemsMissingReason()) {
                var msg = 'Missing return reason for the returned items';
                showError(msg);
                return;
            }

            processPayment();

        }, function (results) {

            if (results instanceof Array) {
                _.each(results, function (msg) {
                    if (msg) {
                        showError(msg);
                    }
                });
            }
            else {
                showError(results);
            }

            return;

        });


    }

    function showError(msg) {
        CommonService.addGrowl({
                                   timeout: 3,
                                   type: 'danger',
                                   content: msg
                               });
    }

    function processPayment() {
        $scope.cart.addPayment();

        if ($scope.sections.change.visible) {
            $scope.$emit('shell:till:open');
        }

        if ($scope.cart.balance === 0) {
            $scope.$emit('pos:sale:completed', $scope.cart);
        }

        $scope.payment.clear();
        $scope.payment.paymentMethodId = 0;
        resetSectionsVisibility();
    }

    function restrictTenderedAmount() {
        if ($scope.sections.change.visible && $scope.payment.amount > 0) {
            return;
        }
        $scope.payment.restrictTenderedAmount();
    }

    function checkPayment() {
        if ($scope.paymentVisible && $scope.cart.Customer && !($scope.cart.Customer.CustomerId || $scope.cart.Customer.isSalesCustomer)) {
            $scope.cart.Customer = null;
        }

        return (!$scope.payment.amount && $scope.cart.balance) || !$scope.payment.paymentMethodId
               || (!$scope.payment.tendered && $scope.cart.balance) || !$scope.cart.soldBy
               || checkReturnPayment();
    }

    function authoriseStoreCardEdit() {
        $scope.checkingEditAuthorisation = true;
        var message = 'Current user do not have permission to enter Store Card number manually. ' +
                      'Authorised person should grant permission current user to input ';

        CommonService.$broadcast('requestAuthorisation', {
            text: message,
            title: 'Allow Permission',
            requiredPermission: SalesEnums.Permissions.EditStoreCardNo,
            permissionArea: 'Sales'
        });
    }

    function getCustomerDetails() {
        customerCardSwept(false, $scope.payment.storeCardNo);
    }

    //endregion

    //region Private Methods

    function checkReturnPayment() {
        return ($scope.cart.itemBeingReturned || $scope.cart.returnedItems.length > 0) &&
               ($scope.cart.authorisationPending || !$scope.cart.authorisedBy);
    }

    function loadExchangeRate(code) {

        if ($scope.payment.paymentMethodId === '2' || $scope.payment.paymentMethodId === '8') {

            PaymentService.getExchangeRate(code).then(function (data) {
                $scope.payment.currencyRate = parseFloat(data);

                $scope.payment.tendered = Utilities.roundFloat($scope.cart.balance / $scope.payment.currencyRate);
            }, function () {
                CommonService.error('Could not load exchange rate.', 'Load Failed');
            });

        }
        else {
            $scope.payment.currencyRate = null;
        }
    }

    function resetSectionsVisibility() {
        _.forOwn($scope.sections, function (prop, key) {
            $scope.sections[key].visible = false;
        });
    }

    function setView() {
        var methodId = parseInt($scope.payment.paymentMethodId) || 0;
        var viewItems = PaymentViewConst[methodId];
        $scope.payment.tendered = Utilities.roundFloat($scope.cart.balance);


        if (viewItems && viewItems.length > 0) {
            _.forOwn($scope.sections, function (prop, key) {
                $scope.sections[key].visible = _.contains(viewItems, key);
            });

            $scope.sections.buttonPay.visible = true;
        }

    }

    function onCurrencyChange(key, value) {
        if (key) {
            $scope.payment.currencyCode = key;
            loadExchangeRate(key);
        }
        else {
            $scope.payment.currencyRate = 0;
            $scope.payment.tendered =
                $scope.cart.balance ? (Utilities.roundFloat($scope.cart.balance * 100, 4) / 100) : 0;
        }

    }

    function setPayMethods(isReturn) {
        if (isReturn) {
            if ($scope.cart.totalAmount < 0) {

                $scope.paymentMethods = [];

                var isRestrict = isRestrictedReturnPayment();

                if ($scope.cart.manualReturn) {
                    addDefaultPaymentMethods();
                }
                else if ($scope.cart.originalOrder.order && $scope.cart.originalOrder.order.payments) {
                    if (isRestrict) {
                        addDefaultPaymentMethods();
                    }
                    setReturnPayMethods();
                }
            }
            else {
                $scope.paymentMethods = $scope.payMethods;
            }
        }
        else {
            $scope.paymentMethods = $scope.payMethods;
        }
    }

    function addDefaultPaymentMethods() {
        var payments = _.filter($scope.payMethods, function (payment) {
            return parseInt(payment.value) === parseInt(SalesEnums.PaymentMethods.Cash) ||
                   parseInt(payment.value) === parseInt(SalesEnums.PaymentMethods.Cheque);
        });

        if (payments && payments.length > 0) {
            _.each(payments, function (payment) {
                $scope.paymentMethods.push(payment);
            });
        }
    }

    function setReturnPayMethods() {
        _.each($scope.cart.originalOrder.order.payments, function (p) {

            var id = p.paymentMethodId,
                existing = _.some($scope.paymentMethods, {value: id}),
                payment = _.find($scope.payMethods, {value: id});

            var canAddPayMethod = _.some($scope.payMethods, function (pm) {
                return pm.value === parseInt(p.paymentMethodId) && pm.isReturnAllowed;
            });

            if (canAddPayMethod && !existing && payment) {
                $scope.paymentMethods.push(payment);
            }
            else if (!payment) {
                var isCashMethodAdded = _.some($scope.paymentMethods, {value: SalesEnums.PaymentMethods.Cash});
                if (!isCashMethodAdded) {
                    $scope.paymentMethods.push(_.find($scope.payMethods,
                                                      {value: SalesEnums.PaymentMethods.Cash}));
                }
            }
        });

        var hasCash = _.findIndex($scope.paymentMethods, {"value": SalesEnums.PaymentMethods.Cash});

        if (hasCash < 0) {
            var hasAllowedCash = _.findIndex($scope.paymentMethods, {"isCashReturnAllowed": true});

            if (hasAllowedCash >= 0) {
                $scope.paymentMethods.push(_.find($scope.payMethods,
                                                  {value: SalesEnums.PaymentMethods.Cash}));
            }
        }
    }

    function isRestrictedReturnPayment() {
        if ($scope.cart.originalOrder.order) {
            var retList = _.some($scope.cart.originalOrder.order.payments, function (p) {
                return _.find($scope.payMethods, function (pm) {
                    return pm.value === parseInt(p.paymentMethodId) && !pm.isReturnAllowed;
                });
            });
            return retList;
        }
    }

    function customerCardSwept(event, cardNumber) {
        if (!cardNumber) {
            return;
        }

        if ($scope.sections.storeCardNo.visible) {
            $scope.payment.storeCardNo = cardNumber;
        }
        if ($scope.views['customer'].visible || !$scope.cart.Customer.FirstName) {
            var params = {
                storeCardNo: cardNumber
            };
            PaymentService.getStoreCardCustomerId(params).then(function (customerId) {
                if (customerId) {
                    CommonService.$broadcast("pos:forceSelectCustomer", customerId, true);
                }
                else {
                    CommonService.addGrowl({
                                               timeout: 3,
                                               type: 'danger',
                                               content: "Customer data not found"
                                           });
                }
            });
        }
    }

    $scope.$on('authorisationSuccess', function (event, data) {
        if ($scope.checkingEditAuthorisation) {
            $scope.editCustCardGranted = true;
            $scope.checkingEditAuthorisation = false;
        }
    });

    $scope.$on('authorisationFailure', function (event, data) {
        $scope.checkingEditAuthorisation = false;
    });

    function activate() {
        PaymentService.getPaymentMethods().then(function (data) {
            $scope.payMethods = [];

            if (data) {
                _.each(data.PaymentMethods, function (pmethod) {
                    if (pmethod.Active) {

                        $scope.payMethods.push({
                                                   value: pmethod.Id,
                                                   type: pmethod.Description,
                                                   isReturnAllowed: pmethod.IsReturnAllowed,
                                                   isCashReturnAllowed: pmethod.IsCashReturnAllowed
                                               });
                    }
                });

                $scope.paymentMethods = $scope.payMethods;
            }
        });

        PaymentService.getActiveBanks().then(function (data) {
            if (data && data.Banks) {
                var tmpBankScope = {};
                // convert from array to scope object
                _.map(data.Banks, function (bk) {
                    if (bk.BankCode) {
                        tmpBankScope[bk.BankCode] = bk.BankName;
                    }
                });
                // set scope variable
                $scope.banksScope = tmpBankScope;
            }
        });

        PaymentService.canEditCustomerCardNo().then(function (data) {
            $scope.canEditCustCard = data || false;
            $scope.editCustCardGranted = data || false;
        });

        PaymentService.getCurrencyCodes().then(function (data) {
            $scope.currencyCodes = [];

            if (data) {
                _.each(data, function (currencyCode) {
                    $scope.currencyCodes.push({
                                                  k: currencyCode.Code,
                                                  v: currencyCode.Name,
                                                  rate: currencyCode.Rate
                                              });
                });
            }

        });

        $scope.$on('pos:return', function (event, data) {
            setPayMethods(data);
        });
        $scope.$on('shell:magstripe:swipe', customerCardSwept);

    }

    //region Validation

    function isValidStoreCardInput(input) {
        if (!$scope.sections.storeCardNo.visible) {
            return true;
        }

        if (!input || _.isNaN(input)) {
            return false;
        }
        else if (input.length != 16) {
            return false;
        }

        return true;
    }

    function validateStoreCardPayment(methodId) {
        var storeCardNo = parseInt($scope.payment.storeCardNo),
            isPaidByCard = _.some($scope.cart.originalOrder.order.payments, {'paymentMethodId': methodId});

        if (!isPaidByCard) {
            return "Cannot use a Store Card for refund as there was not any used for original purchase";
        }

        var specificStoreCardList = _.where($scope.cart.originalOrder.order.payments, {
            paymentMethodId: methodId,
            storeCardNo: storeCardNo
        });

        if (!specificStoreCardList.length) {
            return "Store Card used for refund must be one of the cards used for original purchase";
        }

        var sum = _.reduce(specificStoreCardList, function (memo, specificStoreCardPayment) {
            return memo + specificStoreCardPayment.amount;
        }, 0);

        var alreadyPaidStoreCardAmount = _.chain($scope.cart.payments)
        .where({
                   paymentMethodId: $scope.payment.paymentMethodId,
                   storeCardNo: $scope.payment.storeCardNo
               })
        .reduce(function (memo, storeCardPayment) {
            return memo + parseInt(storeCardPayment.amount || 0);
        }, 0)
        .value();

        if (Math.abs(alreadyPaidStoreCardAmount + $scope.payment.tendered) > sum) {
            $scope.payment.tendered = -1 * (sum - Math.abs(alreadyPaidStoreCardAmount));
            return "Tendered amount for refund should not exceed the original paid amount for that Store Card: " + sum
                   + ". So tendered amount has been updated to match that.";
        }

    }

    function validateCardPayment(methodId) {
        var cardNo = parseInt($scope.payment.cardNo),
            isPaidByCard = _.some($scope.cart.originalOrder.order.payments, {'paymentMethodId': methodId});

        if (!isPaidByCard) {
            return "Cannot use a Card for refund as there was not any used for original purchase";
        }

        var specificCardList = _.where($scope.cart.originalOrder.order.payments, {
            paymentMethodId: methodId,
            bank: $scope.payment.bank,
            cardType: $scope.payment.cardType,
            cardNo: cardNo
        });

        if (!specificCardList.length) {
            return "Card used for refund must be one of the cards used for original purchase";
        }
        var sum = _.reduce(specificCardList, function (memo, specificCardPayment) {
            return memo + specificCardPayment.amount;
        }, 0);

        var alreadyPaidCardAmount = _.chain($scope.cart.payments)
        .where({
                   paymentMethodId: $scope.payment.paymentMethodId,
                   bank: $scope.payment.bank,
                   cardType: $scope.payment.cardType,
                   cardNo: $scope.payment.cardNo
               })
        .reduce(function (memo, cardPayment) {
            return memo + parseInt(cardPayment.amount || 0);
        }, 0)
        .value();

        if (Math.abs(alreadyPaidCardAmount + $scope.payment.tendered) > sum) {
            $scope.payment.tendered = -1 * (sum - Math.abs(alreadyPaidCardAmount));
            return "Tendered amount for refund should not exceed the original paid amount for that Card : " + sum
                   + ". So tendered amount has been updated to match that.";
        }

    }

    //endregion

    //endregion

}

paymentController.$inject = dependInjects;

module.exports = paymentController;