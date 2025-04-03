'use strict';

var dependInjects = ['$scope', 'CommonService', 'LocalisationService', 'UsersService',
                     'BasketService', 'PrinterService', 'LookupDataService', '$window', 'PosDataService', '$location',
                     '$routeParams', '$route'];
var SalesEnums = require('../model/SalesEnums');

function salesController($scope, CommonService, LocalisationService, UsersService,
                         BasketService, PrinterService, LookupDataService, $window, PosDataService, $location,
                         $routeParams, $route) {

    $scope.saleType= 1;

    var modalInstance;
    var isMaximised = true;
    $scope.isReprintAllowed = false;
    $scope.isCashierBalanceOutstanding = false;
    var pickLists = [
        {
            name: 'CCT',
            v: 'cardType'
        }
    ];

    $scope.toggleMaximise = toggleMaximise;
    $scope.showTaxInclusivePrice = showTaxInclusivePrice;
    $scope.setView = setView;
    $scope.cancelSale = cancelSale;
    $scope.finaliseSale = finaliseSale;
    $scope.refundOrExchange = refundOrExchange;
    $scope.enableFinaliseSale = enableFinaliseSale;
    $scope.manualReturn = manualReturn;
    $scope.changeSaleType = changeSaleType;
    $scope.showMessage = showMessage;
    $scope.startNewSale = startNewSale;
    $scope.printReceipt = printReceipt;
    $scope.rePrintItemContracts = rePrintItemContracts;
    $scope.go = go;
    $scope.isCollapsed = false;
    activate();

    $scope.showPrintBtn = false;

    //region Public Methods

    function go(path) {
        $location.path(path);
    }

    function toggleMaximise() {
        isMaximised = !isMaximised;
        angular.element('body').toggleClass('full-screen');
        $scope.togglePopover = isMaximised ? 'Minimize' : 'Maximise';
        if (isMaximised) {
            angular.element('.container').toggleClass('saleContainer').removeClass('container');
        } else {
            angular.element('.saleContainer').toggleClass('container').removeClass('saleContainer');
        }
    }

    function showTaxInclusivePrice() {
        return $scope.cart.taxType === 'E';
    }

    function cancelSale() {
        var title = 'Confirm Sale Cancellation',
            msg = 'You have chosen to cancel this sale and clear all products and ' +
                  'customer data. Are you sure you want to do this?';

        var cancelConfirmation = CommonService.$dialog.messageBox(title, msg, [
            {
                label: 'Yes, Cancel The Sale',
                result: 'yes',
                cssClass: 'btn btn-primary'
            },
            {
                label: 'No, Go Back',
                result: 'no'
            }
        ]);

        cancelConfirmation.open().then(function (choice) {
            if (choice === 'yes') {
                startNewSale();
                $scope.isResetSearch = true;
                $scope.$parent.isResetSearch = true;
            }
        });
    }

    function finaliseSale() {
        if (!$scope.cart.hasPayments()) {
            CommonService.error("You need to add a payment for this order.");
            return;
        }

        BasketService.completeSales(undefined, $scope.MasterData.settings.thermalPrinterName);
    }

    function refundOrExchange() {

        if ($scope.cart.items.length > 0) {
            var title = 'Confirm Sale Cancellation',
                msg = 'In order to load a receipt the current sale will need to be cancelled. ' +
                      'This will clear all products and customer data. Are you sure you want to do this?';

            var cancelConfirmation = CommonService.$dialog.messageBox(title, msg, [
                {
                    label: 'Yes, Cancel The Sale',
                    result: 'yes',
                    cssClass: 'btn btn-primary'
                },
                {
                    label: 'No, Go Back',
                    result: 'no'
                }
            ]);

            cancelConfirmation.open().then(function (choice) {
                if (choice === 'yes') {
                    startNewSale();
                    showReceiptEntryDialog();
                }
            });
        }
        else {
            showReceiptEntryDialog();
        }
    }

    function enableFinaliseSale() {
        return false;
        //return (!$scope.cart.saleComplete && $scope.cart.authorisedBy !== undefined
        //        && $scope.cart.authorisedBy !== null
        //        && !$scope.cart.isCustomerDataMissing && $scope.cart.balance <= 0) || false;
    }

    function manualReturn() {
        CommonService.$broadcast('pos:return:manual');
    }

    //    function tillOpenRequested() {
    //        console.log('Till opening...');
    //    }

    //endregion

    //region Private Methods

    function printReceipt(invoiceNo) {
        //var copyCount = 1;
        //
        //if ($scope.cart && $scope.cart.originalOrder && $scope.cart.originalOrder.order) {
        //    copyCount = 2;
        //}
        BasketService.printReceipt(invoiceNo, 1, 'Reprint');
    }

    function rePrintItemContracts() {
        if (!$scope.cart) {
            return;
        }

        BasketService.printWarrantyContracts($scope.cart.InvoiceNo || $scope.cart.originalOrderId, $scope.cart);
    }

    function startNewSale(pageLoad) {
        BasketService.startNewSale();

        setView('productSearch');

        _.each($scope.MasterData.ContactTypes, function (key, value) {
            $scope.cart.Customer.contacts.push({
                type: value,
                value: ''
            });
        });

        $scope.cart = BasketService.cart;
        $scope.payment = $scope.cart.payment;

        $scope.basketChangesAllowed = true;
        if ($scope.user) {
            $scope.cart.BranchNo = $scope.user.BranchNumber;
            $scope.cart.CreatedBy = $scope.user.userId;
        }

        CommonService.$broadcast('pos:return', false);
        CommonService.$broadcast('pos:resetCustomerSearch');

        if (!pageLoad) {
            //$scope.cart.taxType = $scope.MasterData.settings.taxType;
            //$scope.cart.taxRate = $scope.MasterData.settings.taxRate;

            $location.url($location.path());
            $route.reload();
        }

    }

    function showReceiptEntryDialog() {

        var options = {
            templateUrl: '/Sales/views/templates/receiptCodeEntry.html',
            controller: 'ReceiptEntryController'
        };

        var modalInstance = CommonService.$dialog.dialog(options);

        modalInstance.open().then(function (result) {
            if (!result) {
                return;
            }

            CommonService.$broadcast('pos:receipt:load', result);
        });

    }

    function setView(viewName) {
        _.forOwn($scope.views, function (prop, key) {
            $scope.views[key].visible = (key === viewName);
        });
    }

    function saleCompleted(event, order) {
        if (!order.hasPayments()) {
            CommonService.error("You need to add a payment for this order.");
            return;
        }
        _.each($scope.cart.items, function (item) {

            var addedExtendedWarranties = _.filter(item.warranties, function (w) {
                return !w.isWarrantyFree;
            });

            var availableExtendedWarranties = _.filter(item.availableWarranties, function (w) {
                return !w.isWarrantyFree;
            });

            if (item.quantity > addedExtendedWarranties.length && availableExtendedWarranties.length > 0) {
                var potentialWarranty = _.max(availableExtendedWarranties, function (ew) {
                    return ew.price;
                });

                potentialWarranty.quantity = item.quantity - addedExtendedWarranties.length;
                item.potentialWarranties.push(potentialWarranty);
            }
        });

        BasketService.completeSales(order,$scope.MasterData.settings.thermalPrinterName);
    }

    function setScreenHeight() {
        var winHeight = $window.innerHeight,
            newHeight = winHeight - 160;

        angular.element('.facet-data-container').height(newHeight + 'px');

        angular.element('.item-list').height(newHeight + 'px');
    }

    function showMessage(msg, type) {
        CommonService.addGrowl({
            timeout: 3,
            top: '0px',
            type: type || 'danger',
            content: msg
        });
    }

    // function changeSaleType() {
    //     _.each($scope.cart.items, function (item) {
    //         item.isDutyFree = $scope.cart.isDutyFreeSale;
    //         item.isTaxFree = $scope.cart.isTaxFreeSale;
    //         _.each(item.warranties, function (w) {
    //             //w.isDutyFree = $scope.cart.isDutyFreeSale;
    //             w.isTaxFree = $scope.cart.isTaxFreeSale;
    //         });
    //         _.each(item.installations, function (i) {
    //             //i.isDutyFree = $scope.cart.isDutyFreeSale;
    //             i.isTaxFree = $scope.cart.isTaxFreeSale;
    //         });
    //         _.each(item.kitItems, function (k) {
    //             k.isDutyFree = $scope.cart.isDutyFreeSale;
    //             k.isTaxFree = $scope.cart.isTaxFreeSale;
    //         });
    //         item.updateKitDiscount();
    //     });
    //
    //     $scope.cart.updateTotalAmount();
    // }

    function changeSaleType(flag) {

        if (flag === 'duty') {
            $scope.cart.dutyFreeSaleAuthorisationPending = $scope.cart.isDutyFreeSale;
            $scope.cart.authorisationPending = $scope.cart.isDutyFreeSale || $scope.cart.authorisationPending;

            $scope.cart.setDutyFreeSales();

        } else {
            $scope.cart.setTaxFreeSales();
        }
    }

    function getSettings() {
          PosDataService.getSettingsData().then(function (data) {
            if (data) {
                $scope.MasterData.settings = data;

                $scope.MasterData.refundExchangeReasons = [];

                if (data.returnReason) {
                    _.each(data.returnReason, function (reason) {
                        if (reason) {
                            $scope.MasterData.refundExchangeReasons.push(S(reason).trim().s);
                        }
                    });
                }
                //CommonService.settingCache.put('sales/common', data);
            }

            getTax();
        });
        //}
    }

    function getTax() {
        PosDataService.getTax().then(function (response) {
            if (response && response.status === 'success' && response.data) {
                $scope.cart.taxRate = response.data.currentTaxRate * 100;
                $scope.MasterData.taxRate = response.data.currentTaxRate * 100;
                $scope.cart.taxType = response.data.isTaxInclusive ? 'I' : 'E';
            }
        });
    }

    function getOrderForReceipt(event, receiptNumber) {
        if ($scope.cart.items.length > 0) {
            CommonService.error('Cannot load receipt as sale in progress. Please complete the existing sale to load a receipt.',
                                'Receipt load failed');
        }
        else {
            BasketService.getOrderForReceipt(receiptNumber);
            $scope.setView('itemList');
            $scope.receiptOp = true;
            CommonService.$broadcast('pos:resetCustomerSearch');
        }
    }

    $scope.$watch(function (scope) {
                      return scope.views.payment.visible
                  },
                  function (newValue, oldValue) {
                      if (newValue) {
                          CommonService.$broadcast('pos:return', $scope.cart.itemBeingReturned);
                      }
                  }
    );

    function getDiscountAuthorisationLimit() {
        var params = {
            branchNumber: $scope.user.BranchNumber
        };
        PosDataService.getDiscountLimit(params).then(function (data) {
            if (data) {
                $scope.MasterData.discountLimit = data;
            }
        });
    }

    function setCardTypes() {
        PosDataService.getCardTypesData().then(function (data) {
            $scope.MasterData.cardType = [];

            if (data && data.CardTypes) {
                $scope.MasterData.cardType = data.CardTypes;
            }

        });
    }

    function isCashierBalanceOutstanding() {
        var params = {
            userId: $scope.user.userId
        };
        PosDataService.isCashierBalanceOutstanding(params).then(function (data) {
            $scope.isCashierBalanceOutstanding = data || false;
            if (data) {
                CommonService.error('Can not take any payment. Outstanding income must be deposited before completing a sale.',
                                    'Information');
            }
        });
    }

    function activate() {
        $scope.checkUser();

        $scope.views = {
            productSearch: {
                visible: true
            },
            itemList: {
                visible: false
            },
            customer: {
                visible: false
            },
            payment: {
                visible: false
            },
            mockNativeDevices: {
                visible: false
            }
        };
        $scope.user = UsersService.getCurrentUser();
        if (!$scope.MasterData) {
            $scope.MasterData = {};
        }
        $scope.togglePopover = 'Minimize';
        getSettings();
        getDiscountAuthorisationLimit();

        isCashierBalanceOutstanding();
        LocalisationService.getSettings().then(function (data) {
            $scope.culture = data;
        });

        startNewSale(true);

        setCardTypes();

        // $scope.$on('shell:till:open', tillOpenRequested);
        $scope.$on('pos:sale:completed', saleCompleted);
        $scope.$on('facet:search:completed', setScreenHeight);
        $scope.$on('pos:receipt:load', getOrderForReceipt);

        CommonService.$timeout(function () {
            angular.element('body').addClass('full-screen');
        }, 0);

        angular.element($window).bind('resize', setScreenHeight);

        $window.handleCustomerCardSwept = function (data) {

            if (data && data.cardNumber) {

                $scope.$safeApply($scope, function () {
                    CommonService.$broadcast('shell:magstripe:swipe', data.cardNumber);
                });
            }

        };
        if ($location.search().inv) {
            var invoiceNo = $location.search().inv;
            CommonService.$broadcast('pos:receipt:load', invoiceNo);
            $location.search({});
        }

        PosDataService.hasPermission(SalesEnums.Permissions.ReprintOrderReceipts).then(function (data) {
            $scope.isReprintAllowed = data || false;
        });

        angular.element('.container').toggleClass('saleContainer').removeClass('container');

    }

    function char(code) {
        return String.fromCharCode(code)
    }

    function repeatStr(str, no) {
        str = str || '*';
        no = no || 1;

        return S('').padLeft(no, str).s;
    }

    //endregion

}

salesController.$inject = dependInjects;

module.exports = salesController;