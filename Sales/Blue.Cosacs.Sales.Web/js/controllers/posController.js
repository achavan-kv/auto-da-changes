"use strict";

var dependInjects = ['$scope', 'CommonService', 'LocalisationService', 'UsersService',
                     'BasketService', 'PrinterService', 'LookupDataService', '$window', 'PosDataService', '$location',
                     '$routeParams', '$route'];
var SalesEnums = require('../model/SalesEnums'),
    Utilities = require('../model/Utilities');

function posController($scope, CommonService, LocalisationService, UsersService,
                       BasketService, PrinterService, LookupDataService, $window, PosDataService, $location,
                       $routeParams, $route) {

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
    $scope.itemDiscountChanged = itemDiscountChanged;
    $scope.go = go;
    $scope.isCollapsed = false;

    activate();

    PosDataService.getCSRList().then(function (data) {
        var results = {};

        _.forEach(data, function (current) {
            results[current.k] = current.k + ' - ' + current.v;
        });
        $scope.csrList = results;
    });

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

    function itemDiscountChanged(item, isAmount) {
        if (!item) {
            return;
        }

        if (!item.selectedDiscount) { // || !this.price) {
            item.selectedDiscount = {
                amount: 0,
                percentage: 0
            };
        }

        if (isAmount) {
            item.selectedDiscount.amount = Utilities.roundFloat(parseFloat(item.discountChange), 4);
            item.discountAmountChanged();
        } else {
            item.selectedDiscount.percentage = Utilities.roundFloat(parseFloat(item.discountPercentageChange), 4);
            item.discountPercentageChanged();
        }

        $scope.cart.updateTotalAmount();
    }

    function printReceipt(invoiceNo) {
        //var copyCount = 1;
        //
        //if ($scope.cart && $scope.cart.originalOrder && $scope.cart.originalOrder.order) {
        //    copyCount = 2;
        //}

        var printer = $scope.MasterData.settings.thermalPrinterName || "EPSON TM-T70 Receipt";
        BasketService.printReceipt(invoiceNo, 1, 'Reprint', printer);
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
                var potentialWarranty = _.max(_.filter(item.availableWarranties, function (w) {
                    return !w.isWarrantyFree
                }), function (ew) {
                    return ew.price;
                });

                potentialWarranty.quantity = item.quantity - addedExtendedWarranties.length;
                item.potentialWarranties.push(potentialWarranty);
            }
        });

        BasketService.completeSales(order, $scope.MasterData.settings.thermalPrinterName);
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

    function changeSaleType(flag) {

        if (flag === 'duty') {
            $scope.cart.dutyFreeSaleAuthorisationPending = $scope.cart.isDutyFreeSale;
            $scope.cart.authorisationPending = $scope.cart.isDutyFreeSale || $scope.cart.authorisationPending;

            // if (!$scope.cart.isDutyFreeSale) {
            $scope.cart.setDutyFreeSales();
            // }

        } else {
            $scope.cart.taxFreeSaleAuthorisationPending = $scope.cart.isTaxFreeSale;
            $scope.cart.authorisationPending = $scope.cart.isTaxFreeSale || $scope.cart.authorisationPending;

            // if (!$scope.cart.isTaxFreeSale) {
            $scope.cart.setTaxFreeSales();
            // }
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

                if (data.decimalPlaces) {
                    var tmp = S(data.decimalPlaces || '0').strip('C').s,
                        places = parseInt(tmp, 10);

                    $scope.culture = {
                        "CurrencySymbol": data.currencySymbolForPrint || "$",
                        "DecimalPlaces": places
                    };

                    $scope.cart.decimalPlaces = places;
                }

            }

            getTax();
        });
        //}
    }

    function setCardTypes() {
        PosDataService.getCardTypesData().then(function (data) {
            $scope.MasterData.cardType = [];

            if (data && data.CardTypes) {
                $scope.MasterData.cardType = data.CardTypes;
            }

        });
    }

    function getOrderForReceipt(event, receiptNumber) {
        if ($scope.cart.items.length > 0) {
            CommonService.error(
                'Cannot load receipt as sale in progress. Please complete the existing sale to load a receipt.',
                'Receipt load failed');
        }
        else {
            BasketService.getOrderForReceipt(receiptNumber);
            $scope.setView('itemList');
            $scope.receiptOp = true;
            // CommonService.$broadcast('pos:resetCustomerSearch');
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

    function getTax() {
        PosDataService.getTax().then(function (response) {

            if (response && response.status === 'success' && response.data) {
                $scope.cart.taxRate = response.data.currentTaxRate * 100;
                $scope.MasterData.taxRate = response.data.currentTaxRate * 100;
                $scope.cart.taxType = response.data.isTaxInclusive ? 'I' : 'E';
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
                CommonService.error(
                    'Can not take any payment. Outstanding income must be deposited before completing a sale.',
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

        startNewSale(true);

        isCashierBalanceOutstanding();


        // LocalisationService.getSettings().then(function (data) {
        //     console.log('%c data = %s', 'color:white; background:blue;', JSON.stringify(data));
        //     $scope.culture = data;
        // });

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

posController.$inject = dependInjects;

module.exports = posController;
