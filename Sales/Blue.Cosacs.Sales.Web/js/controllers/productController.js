'use strict';

var dependInjects = ['$scope', 'ProductsService', 'CommonService', 'BasketService', '$window'];

function productController($scope, ProductsService, CommonService, BasketService, $window) {

    $scope.fieldLabels = {"Level_1": "Department", "Level_2": "Category", "Level_3": "Class"};

    activate();

    //region Private Methods

    function barcodeScanned(event, barcode, kitItem, exchangeItem, isUpc) {

        ProductsService.getProductByBarcode(barcode, isUpc).then(function (data) {
            if (!data || !data.response.numFound) {
                var msg = "Cannot find a product with no: " + barcode;
                CommonService.error(msg, 'Information');
                return;
            }

            var doc = data.response.docs[0];

            $scope.cart.searchItem = $scope.cart.createNewProduct(doc);

            if (kitItem && kitItem.ParentId) {
                $scope.cart.searchItem.parentId = parseInt(kitItem.ParentId);
                $scope.cart.searchItem.quantity = kitItem.Quantity || 1;
            } else if (exchangeItem && exchangeItem.id) {
                var exchangeItemId = exchangeItem.id;

                $scope.cart.searchItem.originalId = parseInt(exchangeItemId);
                console.log('%c exchangeItem = %s', 'color:white; background:blue;', JSON.stringify(exchangeItem));

                $scope.cart.searchItem.quantity =(exchangeItem.availableQuantity || exchangeItem.quantity) || 0;

                var unClaimedWarranty = _.first(exchangeItem.unClaimedInstantReplacementWarranties);

                if (unClaimedWarranty) {
                    $scope.cart.searchItem.isReplacement = true;
                    var isFreeWarrantyExpired = BasketService.isFreeWarrantyExpired(exchangeItem);

                    if (isFreeWarrantyExpired) {
                        // Exchange using the IR warranty
                        exchangeItem.isIrExchange = true;

                        unClaimedWarranty.isClaimed = true;
                        //unClaimedWarranty.returned = false;
                    }
                    else {
                        // Normal exchange
                        var freeWarranty = exchangeItem.getItemFreeWarranty();

                        if (freeWarranty) {
                            freeWarranty.isClaimed = true;
                        }

                        unClaimedWarranty.returned = true;
                        unClaimedWarranty.isClaimed = false;
                    }
                }

            }

            CommonService.$broadcast('pos:searchitem:add');
        });

    }

    function activate() {
        $scope.$on('shell:barcode:scan', barcodeScanned);

        $window.handleBarcodeScanned = function (data) {

            if (data && data.barcode) {
                var isUpc = data.Symbology && data.Symbology === "Upca";

                var isInvoice = S(data.barcode).startsWith("IN$"),
                    barcode = isInvoice ? S(data.barcode).strip('IN$').s : data.barcode,
                    eventName = isInvoice ? 'pos:receipt:load' : 'shell:barcode:scan';

                $scope.$safeApply($scope, function () {
                    CommonService.$broadcast(eventName, barcode, null, null, isUpc);
                });
            }

        };
    }

    //endregion

}

productController.$inject = dependInjects;

module.exports = productController;