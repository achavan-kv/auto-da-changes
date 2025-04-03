'use strict';

var dependInjects = ['$scope', 'CommonService', 'BasketService', 'PosDataService', 'ProductsService'];

var Utilities = require('../model/Utilities'),
    SalesEnums = require('../model/SalesEnums');

var basketController = function ($scope, CommonService, BasketService, PosDataService, ProductsService) {

    $scope.cart.basketChangesAllowed = true;

    $scope.manualPriceEdit = manualPriceEdit;
    $scope.addSearchItem = addSearchItem;
    $scope.decreaseQuantity = decreaseQuantity;
    $scope.addWarranty = addWarranty;
    $scope.removeWarranty = removeWarranty;
    $scope.restrictWarrantyMaximumDuration = restrictWarrantyMaximumDuration;
    $scope.addInstallation = addInstallation;
    $scope.removeInstallation = removeInstallation;
    $scope.increaseReturnQuantity = increaseReturnQuantity;
    $scope.decreaseReturnQuantity = decreaseReturnQuantity;
    $scope.requestReturnAuthorisation = requestReturnAuthorisation;
    $scope.requestManualReturnAuthorisation = requestManualReturnAuthorisation;
    $scope.showAvailableDiscount = showAvailableDiscount;
    $scope.addAssociatedProductAsNewItem = addAssociatedProductAsNewItem;
    $scope.discountChanged = discountChanged;
    $scope.removeSelectedDiscount = removeSelectedDiscount;
    $scope.addSelectedDiscount = addSelectedDiscount;
    $scope.refundExchangeReasons = $scope.MasterData.refundExchangeReasons;
    $scope.exchangeItem = exchangeItem;
    $scope.returnItem = returnItem;
    $scope.returnWarranty = returnWarranty;
    $scope.reasonChangedForItem = reasonChangedForItem;
    $scope.reasonChangedForWarranty = reasonChangedForWarranty;
    $scope.installationReturnReasonChanged = installationReturnReasonChanged;
    $scope.increaseInstallationQty = increaseInstallationQty;
    $scope.decreaseInstallationQty = decreaseInstallationQty;
    $scope.increaseInstallationRetQty = increaseInstallationRetQty;
    $scope.decreaseInstallationRetQty = decreaseInstallationRetQty;
    $scope.canDoExchange = canDoExchange;
    $scope.canDoIrExchange = canDoIrExchange;


    activate();

    var isAdding = false;

    //region Scope functions

    var SalesEnums = require('../model/SalesEnums');
    var CartItem = require('../model/CartItem');

    function manualPriceEdit() {
        // TODO: refactor this
        //this.item.price = parseFloat(this.item.manualPrice);
        $scope.cart.updateTotalAmount();
    }

    function addSearchItem() {
        addItemToCart($scope.cart.searchItem);
    }

    function decreaseQuantity(item) {
        var result = $scope.cart.decreaseItemQuantity(item);

        if (result && !result.success) {
            CommonService.alert(result.msg, result.title);
        } else {
            $scope.cart.updateTotalAmount();
        }
    }

    function addWarranty(warranty, item) {
        $scope.cart.addWarranty(warranty, item);

        $scope.cart.setIsCustomerDataRequired();
        $scope.cart.updateTotalAmount();
    }

    function removeWarranty(warrantyIndex, item) {
        $scope.cart.removeWarranty(warrantyIndex, item);

        $scope.cart.updateTotalAmount();
    }

    function restrictWarrantyMaximumDuration(warranty) {
        $scope.cart.restrictWarrantyMaximumDuration(warranty);
    }

    function addInstallation(installationToAdd, item) {
        if (!item.canAddInstallation) {
            return;
        }

        var existingData = _.find(item.installations, function (inst) {
            return inst.itemNo === installationToAdd.itemNo;
        });
        if (existingData) {
            existingData.quantity += 1;
        }
        else {
            var installation = $scope.cart.convertToCartItem(installationToAdd, false);
            item.installations = item.installations || [];
            item.installations.push(installation);
        }

        if (item.totalInstallations === item.quantity) {
            item.canAddInstallation = false;
        }
        else {
            item.canAddInstallation = true;
        }

        $scope.cart.installationBeingPurchased = true;

        $scope.cart.updateTotalAmount();
    }

    function removeInstallation(installationIndex, item) {
        var removedInstallation = item.installations.splice(installationIndex, 1)[0];

        if (item.installations.length === 0) {
            $scope.cart.installationBeingPurchased = false;
        }

        item.canAddInstallation = true;
        $scope.cart.updateTotalAmount();

        var found = _.find(item.availableInstallations, function (installation) {
            return installation.Id === removedInstallation.Id;
        });
        if (found) {
            return;
        }

        item.availableInstallations.push(removedInstallation);
    }

    function increaseReturnQuantity(item) {
        if (item.returnQuantity === item.availableQuantity) {
            return;
        }

        item.returnQuantity += 1;

        var relatedCartItem = _.find($scope.cart.items, function (cartItem) {
            return cartItem.itemNo === item.itemNo;
        });

        relatedCartItem.returnQuantity += 1;

        var notReturnedFreeWarrantyList = _.filter(item.warranties, function (w) {
            return w.isWarrantyFree && !w.returned && w.operationsRequired;
        });

        if (notReturnedFreeWarrantyList && notReturnedFreeWarrantyList.length > 0) {
            notReturnedFreeWarrantyList[0].returned = true;
            notReturnedFreeWarrantyList[0].returnQuantity = 1;
            relatedCartItem.warranties.push(notReturnedFreeWarrantyList[0]);
        }


        var notReturnedNonFreeWarrantyList = _.filter(item.warranties, function (w) {
            return !w.isWarrantyFree && !w.returned && w.operationsRequired;
        });

        if (notReturnedNonFreeWarrantyList && notReturnedNonFreeWarrantyList.length > 0
            && (relatedCartItem.availableQuantity
                - relatedCartItem.returnQuantity
                < notReturnedNonFreeWarrantyList.length )) {
            notReturnedNonFreeWarrantyList[0].returned = true;
            notReturnedNonFreeWarrantyList[0].returnQuantity = 1;
            relatedCartItem.warranties.push(notReturnedNonFreeWarrantyList[0]);
        }

        var beingKept = item.freeWarrantyBeingKept,
            beingKeptLength = beingKept ? beingKept.length : 0;

        if (beingKeptLength > 0) {
            //beingKept[0].returned = true;
            //beingKept[0].returnQuantity = 1;
            beingKeptLength + 1;
        }

        if ((item.quantity - item.returnQuantity) === beingKeptLength) {
            item.canWarrantyBeKept = false;
        }

        var notReturnedInstallationsList = _.filter(item.installations, function (i) {
            return !i.returned && i.operationsRequired;
        });

        var returnedInstallationsList = _.find(item.installations, function (i) {
            return i.returned && i.availableQuantity > i.returnQuantity;
        });

        var totalNotReturned = _.reduce(item.installations, function (memo, inst) {
            return memo + inst.availableQuantity - inst.returnQuantity;
        }, 0);

        if (item.availableQuantity - item.returnQuantity < totalNotReturned) {
            if (returnedInstallationsList != null) {
                returnedInstallationsList.returnQuantity += 1;
            }
            else {
                notReturnedInstallationsList[0].returned = true;
                notReturnedInstallationsList[0].returnQuantity += 1;
                relatedCartItem.installations.push(notReturnedInstallationsList[0]);
            }
        }

        if (item.discounts) {
            _.each(item.discounts, function (d) {
                // d.amount = Math.abs(d.unitDiscount * item.returnQuantity);
                // d.taxAmount = Math.abs(d.unitTaxAmount * item.returnQuantity);
                // d.percentage = Math.abs(d.unitPercentage * item.returnQuantity);

                d.returnQuantity = item.returnQuantity;
                d.quantity = item.quantity;
            });

            relatedCartItem.updateDiscount();
        }

        var kitItems = _.filter($scope.cart.originalOrder.order.items, function (i) {
            return i.parentId === item.productItemId;
        });

        if (kitItems && kitItems.length > 0) {
            _.each(kitItems, function (kit) {
                kit.returnQuantity += 1;
            });
        }

        var relatedKitItems = _.filter($scope.cart.items, function (i) {
            return i.parentId === item.productItemId;
        });

        if (relatedKitItems && relatedKitItems.length > 0) {
            _.each(relatedKitItems, function (kit) {
                kit.returnQuantity += 1;
            });
        }

        item.updateDiscount();
        $scope.cart.updateTotalAmount();
    }

    function decreaseReturnQuantity(item) {
        if (item.returnQuantity === 1) {
            return;
        }

        item.returnQuantity -= 1;

        var relatedCartItem = _.find($scope.cart.items, function (cartItem) {
            return cartItem.itemNo === item.itemNo;
        });

        relatedCartItem.returnQuantity -= 1;

        var returnedFreeWarrantyList = _.filter(item.warranties, function (w) {
            return w.isWarrantyFree && w.returned && w.operationsRequired;
        });

        if (returnedFreeWarrantyList && returnedFreeWarrantyList.length > 0) {
            returnedFreeWarrantyList[returnedFreeWarrantyList.length - 1].returned = false;
            returnedFreeWarrantyList[returnedFreeWarrantyList.length - 1].returnQuantity = 0;
            relatedCartItem.warranties = _.reject(relatedCartItem.warranties, function (freeWarranty) {
                return freeWarranty.isWarrantyFree && freeWarranty.id
                                                      === returnedFreeWarrantyList[returnedFreeWarrantyList.length
                                                                                   - 1].id;
            });
        }

        var returnedNonFreeWarrantyList = _.filter(item.warranties, function (w) {
            return !w.isWarrantyFree && w.returned && w.operationsRequired;
        });

        if (returnedNonFreeWarrantyList && returnedNonFreeWarrantyList.length > 0) {
            returnedNonFreeWarrantyList[returnedNonFreeWarrantyList.length - 1].returned = false;
            returnedNonFreeWarrantyList[returnedNonFreeWarrantyList.length - 1].returnQuantity = 0;
            relatedCartItem.warranties = _.reject(relatedCartItem.warranties, function (warranty) {
                return !warranty.isWarrantyFree && warranty.id
                                                   === returnedNonFreeWarrantyList[returnedNonFreeWarrantyList.length
                                                                                   - 1].id;
            });

        }

        var beingKept = item.freeWarrantyBeingKept;

        if ((item.quantity - item.returnQuantity) > (beingKept.length)) {
            item.canWarrantyBeKept = true;
        }

        var returnedInstallationsList = _.filter(item.installations, function (i) {
            return i.returned && i.operationsRequired;
        });

        if (returnedInstallationsList && returnedInstallationsList.length > 0) {
            var inst = returnedInstallationsList[returnedInstallationsList.length - 1];
            if (inst.availableQuantity > 1 && inst.returnQuantity > 1) {
                inst.returnQuantity -= 1;
                //var cartInstallation = _.find(relatedCartItem.installations, function (installation) {
                //    return installation.id === inst.id;
                //});
                //cartInstallation.returnQuantity -=1;
            }
            else {
                inst.returnQuantity = 0;
                inst.returned = false;
                relatedCartItem.installations = _.reject(relatedCartItem.installations, function (installation) {
                    return installation.id === inst.id;
                });
            }
        }

        if (item.discounts) {

            _.each(item.discounts, function (d) {
                // d.amount = Math.abs(d.unitDiscount * item.returnQuantity);
                // d.taxAmount = Math.abs(d.unitTaxAmount * item.returnQuantity);
                // d.percentage = Math.abs(d.unitPercentage * item.returnQuantity);

                d.returnQuantity = item.returnQuantity;
                d.quantity = item.quantity;
            });

            relatedCartItem.updateDiscount();
        }

        var kitItems = _.filter($scope.cart.originalOrder.order.items, function (i) {
            return i.parentId === item.productItemId;
        });

        if (kitItems && kitItems.length > 0) {
            _.each(kitItems, function (kit) {
                kit.returnQuantity -= 1;
            });
        }

        var relatedKitItems = _.filter($scope.cart.items, function (i) {
            return i.parentId === item.productItemId;
        });

        if (relatedKitItems && relatedKitItems.length > 0) {
            _.each(relatedKitItems, function (kit) {
                kit.returnQuantity -= 1;
            });
        }
        item.updateDiscount();
        $scope.cart.updateTotalAmount();
    }

    function increaseInstallationRetQty(item, installation) {
        if (installation.returnQuantity === installation.availableQuantity) {
            return;
        }

        installation.returnQuantity += 1;
        $scope.cart.updateTotalAmount();
    }

    function decreaseInstallationRetQty(item, installation) {
        if (installation.returnQuantity === 1 || (item.quantity - item.returnQuantity == installation.availableQuantity
                                                                                         - installation.returnQuantity)) {
            return;
        }

        installation.returnQuantity -= 1;
        $scope.cart.updateTotalAmount();
    }

    function requestReturnAuthorisation() {
        var message = 'The customer is either exchanging or requesting a refund for an item that ' +
                      'they have purchased. This change requires authorisation.';

        CommonService.$broadcast('requestAuthorisation', {
            text: message,
            title: 'Refund/Exchange',
            requiredPermission: SalesEnums.Permissions.AuthoriseRefundExchange
        });
    }

    function requestManualReturnAuthorisation() {

        var message = 'The customer is either exchanging or requesting a refund for an item that ' +
                      'they have purchased but the sale cannot be found. The order items have been ' +
                      'added manually. This change requires authorisation.';

        CommonService.$broadcast('requestAuthorisation', {
            text: message,
            title: 'Manual Refund/Exchange',
            requiredPermission: SalesEnums.Permissions.AuthoriseManualRefundExchange
        });
    }

    function showAvailableDiscount(item) {
        if (item.availableDiscounts.length > 0) {
            item.showAvailableDiscount = !item.showAvailableDiscount;
        }
        else {
            var msg = 'No discount is available for selected item.';
            // CommonService.alert('No discount is available for selected item.', 'Discount not available');
            CommonService.addGrowl({
                                       timeout: 5,
                                       type: 'warning',
                                       content: msg
                                   });
        }
    }

    function addAssociatedProductAsNewItem(newItem) {
        addItemToCart(newItem);
    }

    function discountChanged(key, value, item) {
        item.setSelectedDiscount(key, value);
    }

    function removeSelectedDiscount(item, code) {
        item.removeDiscount(code);

        $scope.cart.updateTotalAmount();
    }

    function addSelectedDiscount(item) {
        var totalDiscounts = Math.abs(item.manualDiscountPercentage + item.selectedDiscount.percentage);
        var authorisationNeeded = false;

        if (item.selectedDiscount) {
            item.selectedDiscount.isTaxFree = $scope.cart.isTaxFreeSale;
            item.selectedDiscount.isDutyFree = $scope.cart.isDutyFreeSale;
        }

        if (totalDiscounts > $scope.MasterData.discountLimit) {
            authorisationNeeded = true;
            $scope.cart.pendingDiscAuthorisationItemNo = item.itemNo;
            requestDiscountLimitAuthorisation(totalDiscounts, item);

        }
        if (!authorisationNeeded || !$scope.cart.pendingDiscAuthorisationItemNo
            || $scope.cart.pendingDiscAuthorisationItemNo != item.itemNo) {
            if ($scope.cart.pendingDiscAuthorisationItemNo == item.itemNo) {
                $scope.cart.pendingDiscAuthorisationItemNo = null;
            }
            addDiscount(item);
        }
    }

    function addDiscount(item) {
        item.addSelectedDiscount($scope.cart.taxRate);
        $scope.cart.updateTotalAmount();
    }

    function reasonChangedForItem(originalItem) {
        var cartItem = _.find($scope.cart.items, function (item) {
            return item.itemNo === originalItem.itemNo;
        });
        if (cartItem) {
            cartItem.returnReason = originalItem.returnReason;
        }
    }

    function reasonChangedForWarranty(warranty) {
        var cartItem = _.find($scope.cart.returnedItems, function (item) {
            return item.id === warranty.id;
        });
        if (cartItem) {
            cartItem.returnReason = warranty.returnReason;
        }
    }

    function installationReturnReasonChanged(installation) {
        var cartItem = _.find($scope.cart.returnedItems, function (item) {
            return item.id === installation.id;
        });
        if (cartItem) {
            cartItem.returnReason = installation.returnReason;
        }
    }

    function increaseInstallationQty(item, installation) {
        if (item.quantity === installation.quantity) {
            return;
        }

        installation.quantity += 1;

        if (item.totalInstallations === item.quantity) {
            item.canAddInstallation = false;
        }
        else {
            item.canAddInstallation = true;
        }
        $scope.cart.updateTotalAmount();
    }

    function decreaseInstallationQty(item, installation) {
        if (installation.quantity == 1) {
            return;
        }

        installation.quantity -= 1;
        item.canAddInstallation = true;
        $scope.cart.updateTotalAmount();
    }

    //endregion

    //region Private functions

    function addItemToCart(itemToAdd, doneCallback) {

        if ($scope.cart.saleComplete) {
            CommonService.alert(
                'The previous sale has been completed. You need to start a new sale before you can add items.',
                'Sale completed');
            isAdding = false;
            return;
        }

        if ($scope.cart.originalOrder.order) {
            var originalItem = _.find($scope.cart.originalOrder.order.items, function (item) {
                return item.itemNo === itemToAdd.ProductItemNo && item.replacementInProgress;
            });
            if (originalItem) {
                originalItem.replacementInProgress = false;
                CommonService.$broadcast('shell:barcode:scan', originalItem.itemNo, null, originalItem);
                return;
            }
        }

        addItem(itemToAdd).then(function () {
            var itemNo = itemToAdd.ProductItemNo || itemToAdd.itemNo || itemToAdd.productItemNo; // TODO: Fix this mess

            var addedItem = _.find($scope.cart.items, function (item) {
                return !item.returned && item.itemNo === itemNo.trim();
            });
            if (addedItem.quantity > 1) {
                $scope.views.itemList.message = 'Quantity increased for ' + itemNo;
            }
            else {
                $scope.views.itemList.message = 'Item ' + itemNo + ' added';
            }
            $('.basket-info').show().fadeOut(2000, function () {
                $scope.views.itemList.message = '';
            });

            if (_.isFunction(doneCallback)) {
                doneCallback();
            }

            isAdding = false;
        }, function () {
            isAdding = false;
        });


    }

    function addItem(item) {
        var itemToAdd;

        if (item instanceof CartItem) {
            itemToAdd = item;
        }
        else {
            itemToAdd = $scope.cart.createNewProduct(item);
        }
        var deferred = CommonService.$q.defer(),
            itemNo = (itemToAdd.itemNo || itemToAdd.ProductItemNo || itemToAdd.ItemNoWarrantyLink),
            cartItem = $scope.cart.getItem(itemNo);

        itemNo = S(itemNo).trim().s;
        itemToAdd.itemNo = itemNo;

        if (cartItem) {
            $scope.cart.addItemToCart(itemToAdd);
            deferred.resolve();
        } else {
            var promises = [];

            promises.push(getWarrantiesForItem(itemToAdd));
            promises.push(getInstallationsForItem(itemToAdd));
            promises.push(getDiscountsForItem(itemToAdd));
            promises.push(getAssociatedProductsForItem(itemToAdd));
            if (itemToAdd.isKitParent) {
                promises.push(addKitItems(itemToAdd));
            }

            //TODO: Process Item's Kit
            CommonService.$q.all(promises).then(function () {

                $scope.cart.addItemToCart(itemToAdd);
                deferred.resolve();
            });

        }

        return deferred.promise;
    }

    function manualReturnCartContents() {
        $scope.cart.returnType = SalesEnums.ReturnType.Manual;
        $scope.cart.manualReturn = true;
        $scope.cart.originalOrder.receiptNumber = 'UNKNOWN';
        $scope.cart.originalOrder.order = BasketService.getNewBasket();

        var itemTotal = 0;

        _.each($scope.cart.items, function (item) {
            itemTotal += (item.price + (item.taxAmount || 0)) * item.quantity;
            var newItem = new CartItem({
                                           itemType: item.itemTypeId,
                                           taxRate: item.taxRate
                                       });
            item.warranties = [];
            item.installations = [];
            item.availableQuantity = item.quantity;
            _.extend(newItem, item);

            newItem.isTaxFree = item.isTaxFree;
            newItem.isDutyFree = item.isDutyFree;

            $scope.cart.originalOrder.order.items.push(newItem);
        });

        itemTotal = itemTotal * 100 / 100;

        $scope.cart.authorisationPending = false;
        //$scope.cart.itemBeingReturned = true;

        //$scope.cart.credit = itemTotal;
        $scope.cart.items = [];
        $scope.cart.updateTotalAmount();

        $scope.setView('itemList');

        CommonService.$broadcast('pos:return', true);
    }

    function returnAuthorisationSuccess(authorisedBy) {
        $scope.cart.authorisedBy = authorisedBy;
    }

    function returnAuthorisationFailure() {
        if ($scope.cart.itemBeingReturned) {
            $scope.cart.authorisationPending = true;
        }
        $scope.cart.authorisedBy = undefined;
    }

    function getWarrantiesForItem(item) {
        var self = this,
            deferred = CommonService.$q.defer();

        var params = {
            Product: item.itemNo,
            PriceVATEx: item.price,
            Location: $scope.user.BranchNumber || 0
        };

        PosDataService.getItemWarranties(params).then(function (data) {
            if (data && data.Items) {
                var isIrwCarried = false;

                // Get item free warranties
                var freeWarranties = _.filter(data.Items, function (warranty) {
                    return warranty.warrantyLink.TypeCode === SalesEnums.WarrantyTypes.Free;
                });

                // Get item other available optional warranties
                var optionalWarranties = _.filter(data.Items, function (warranty) {
                    return warranty.warrantyLink.TypeCode !== SalesEnums.WarrantyTypes.Free;
                });

                if (item.isReplacement) {
                    var originalId = item.originalId,
                        purchasedDate = $scope.cart.originalOrder.order.createdOn,
                        originalItem = _.find($scope.cart.originalOrder.order.items, {id: originalId});

                    if (originalItem) {
                        var orgIrW = originalItem.getItemIrWarranty(),
                            orgFreeWarranty = originalItem.getItemFreeWarranty();

                        if (orgIrW && orgFreeWarranty) {
                            var minFreeMonthsPercentage = $scope.MasterData.settings.minFreeMonthIRW || 50,
                                warrantyLength = orgFreeWarranty.warrantyLengthMonths || 0,
                                minFreeMonths = warrantyLength * (minFreeMonthsPercentage * 0.01),
                                remainingCoverage = 0;

                            if (BasketService.isFreeWarrantyExpired(originalItem)) {
                                remainingCoverage = BasketService.getWarrantyRemainingCoverage(orgIrW);
                            }
                            else {
                                remainingCoverage = BasketService.getWarrantyRemainingCoverage(orgFreeWarranty);
                            }

                            isIrwCarried = !orgIrW.isClaimed;

                            _.forEach(freeWarranties, function (w) {

                                if (remainingCoverage > minFreeMonths) {
                                    w.warrantyLink.Length = Math.ceil(remainingCoverage);
                                } else {
                                    w.warrantyLink.Length = Math.ceil(minFreeMonths);
                                }

                            });
                        }

                    }

                }

                item.addFreeWarranties(freeWarranties || []);
                item.addAvailableWarranties(optionalWarranties || [], isIrwCarried);

                $scope.cart.setIsCustomerDataRequired();
            }

            deferred.resolve();
        });

        return deferred.promise;
    }

    function getDiscountsForItem(item) {
        var deferred = CommonService.$q.defer();
        var params = {
            branch: $scope.user.BranchNumber || 0,
            itemNo: item.itemNo,
            department: item.department
        };

        // Get item's discounts
        PosDataService.getItemDiscounts(params).then(function (data) {

            item.addAvailableDiscounts(data);
            deferred.resolve();
        });

        return deferred.promise;
    }

    function addItemKitDiscount(item) {
        // ToRemove
        // Check 'StockRepository.SetUpKitDiscount' on 'Courts.Net'
        if (!item.kitItems || item.kitItems.length === 0) {
            return;
        }

        var category = S(item.category).between('', ' - ').s;

        if (S(category).isNumeric()) {
            var params = {
                branch: $scope.user.BranchNumber || 0,
                category: category
            };

            // Get item's discounts
            PosDataService.getItemKitDiscount(params).then(function (data) {
                if (data) {
                    item.addDiscount({code: data.k, description: data.v, isKitDiscount: true});
                }


            });
        }
    }

    function getAssociatedProductsForItem(item) {
        var deferred = CommonService.$q.defer();
        var branchNo = $scope.branchNo || $scope.user.BranchNumber || 0;
        var branchFascia = $scope.branchFascia || $scope.currentUser.BranchFacia;

        var params = {
            productId: item.productItemId,
            branch: branchNo
        };

        // Get item free warranties
        PosDataService.getAssociatedProducts(params).then(function (data) {
            if (data.response && data.response.docs) {
                item.addAssociatedProducts(data.response.docs || []);
            }

            deferred.resolve();
        });

        return deferred.promise;
    }

    function getInstallationsForItem(item) {
        var deferred = CommonService.$q.defer();

        // TODO: complete this
        var params = {
            branch: $scope.user.BranchNumber || 0,
            itemNo: item.itemNo,
            taxType: $scope.cart.taxType,
            taxRate: $scope.cart.taxRate
        };

        PosDataService.getInternalInstallations(params).then(function (data) {
            if (typeof data === 'object' && data) {
                item.addAvailableInstallations(data || []);
            }

            deferred.resolve();
        });

        return deferred.promise;
    }

    function addKitItems(cartItem) {
        if (!cartItem) {
            return;
        }

        var deferred = CommonService.$q.defer(),
            promises = [],
            params = {
                branch: $scope.user.BranchNumber || 0,
                itemNo: cartItem.itemNo
            };

        PosDataService.getItemKitProducts(params).then(function (kItems) {

            if (kItems && kItems.length > 0) {

                kItems.forEach(function (item) {
                    promises.push(getKitProduct(item));
                });

                cartItem.kitDiscountItemId = kItems[0] ? kItems[0].DiscountItem.Id : 0;
                var disItem = kItems[0] ? kItems[0].DiscountItem : null;

                CommonService.$q.all(promises).then(function (kitItems) {

                    kitItems.forEach(function (kitItem) {
                        cartItem.addKitItem(kitItem);
                    });

                    if (cartItem.isKitParent) {
                        var kitQty = cartItem.quantity || 0,
                            salePrice = cartItem.salePrice || 0,
                            compTotal = cartItem.totalKitAmount || 0;

                        cartItem.kitDiscount = compTotal - salePrice;

                        cartItem.itemKitDiscountPercentage =
                            Utilities.roundFloat((cartItem.kitDiscount / (compTotal / kitQty)) * 100);


                        cartItem.updateKitDiscount(disItem);

                        cartItem.price = 0;
                        cartItem.taxRate = 0;
                        cartItem.taxAmount = 0;
                        cartItem.showAvailableDiscount = true;
                    }
                    deferred.resolve();
                }, function () {
                    CommonService.error(
                        'Can not sell this Kit Item. One or more items from this Kit Item is not available',
                        'Information');
                    deferred.reject();
                });
            }
            else {
                deferred.resolve();
            }
        });

        return deferred.promise;
    }

    function getKitProduct(item) {
        var deferred = CommonService.$q.defer(),
            itemNo = S(item.ItemNo).trim().s,
            discount = item.DiscountItem;

        ProductsService.getProductByBarcode(itemNo).then(function (data) {
            if (!data || !data.response.numFound) {
                deferred.reject();
            }

            var kitItem = data.response.docs[0];
            if (kitItem) {
                kitItem.parentId = parseInt(item.ParentId);
                kitItem.quantity = item.Quantity || 1;
                kitItem.discountItem = discount;
            }
            deferred.resolve(kitItem);
        });

        return deferred.promise;
    }

    function addKitItem(cartItem) {
        _.forEach(cartItem.kit.Items, function (item) {
            CommonService.$broadcast('shell:barcode:scan', S(item.ItemNo).trim().s, item);
        });

        if (cartItem) {
            addItemKitDiscount(cartItem);
        }
    }

    function exchangeItem(item) {
        var itemNo = S(item.itemNo).trim().s;

        returnItem(item);

        // if (item.hasInstantReplacementWarranty) {
        PosDataService.GetAvailableQuantity(item.itemNo, $scope.user.BranchNumber).then(function (data) {
            if (data && data > 0) {
                CommonService.$broadcast('shell:barcode:scan', itemNo, null, item);
            }
            else {
                var msgLabel = "exchange";

                if (item.hasInstantReplacementWarranty) {
                    item.replacementInProgress = true;

                    msgLabel = "perform Instant Replacement for";
                }

                var msg = 'System can not ' + msgLabel + ' the item : ' + item.itemNo +
                          ' as it is not available in stock.' + 'Please select any item from Products tab.';

                CommonService.error(msg, 'Information');

                $scope.setView('productSearch');
            }
        });
        // }
        // else {
        //     $scope.setView('productSearch');
        // }
    }


    function canDoExchange(item) {
        var ret = true;

        if (item.hasInstantReplacementWarranty && item.id) {
            var wWarranties = item.unClaimedInstantReplacementWarranties,
                warQty = wWarranties ? wWarranties.length : 0;

            if (warQty > 0) {
                ret = true;

                _.each(wWarranties, function (w) {
                    if (BasketService.isWarrantyExpired(w)) {
                        ret = false;

                        return false; //exit loop
                    }
                });
            } else {
                ret = false;
            }
        }

        return ret;
    }

    function returnWarranty(warrantyIndex, item) {
        var daysSincePurchase = moment(new Date())
        .diff(moment($scope.cart.originalOrder.order.createdOn).format('YYYY-MM-DD'), 'day');

        if ($scope.MasterData.settings.warrantyCancelDays < daysSincePurchase) {
            if (item.returned) {
                var returnedNonFreeWarranties = _.filter(item.warranties, function (warranty) {
                    return !warranty.isWarrantyFree && warranty.returned;
                });
                if (returnedNonFreeWarranties && returnedNonFreeWarranties.length >= item.returnQuantity) {
                    CommonService.addGrowl({
                                               timeout: 3,
                                               type: 'danger',
                                               content: 'Can not return warranty on its own as Warranty Cancel Day limit ('
                                                        + $scope.MasterData.settings.warrantyCancelDays
                                                        + ') has been passed'
                                           });
                    return;
                }
            }
            else {
                CommonService.addGrowl({
                                           timeout: 3,
                                           type: 'danger',
                                           content: 'Can not return warranty on its own as Warranty Cancel Day limit ('
                                                    + $scope.MasterData.settings.warrantyCancelDays
                                                    + ') has been passed'
                                       });
                return;
            }
        }

        var returnedWarranty = item.warranties[warrantyIndex];
        var promises = [];
        promises.push(nonFreeWarrantyUtility(item, returnedWarranty));
        CommonService.$q.all(promises).then(function () {
            $scope.cart.returnWarranty(warrantyIndex, item);
        });
    }

    function returnItem(item) {
        if (!item.returnedItems) {
            item.returnedItems = [];
        }

        returnNonFreeWarranties(item).then(function () {
            $scope.cart.returnItem(item);
            returnItemKit(item.productItemId);
            //returnItemInstallations(item);
        });

    }

    function returnNonFreeWarranties(item) {
        var deferred = CommonService.$q.defer();

        var nonFreeWarranties = _.filter(item.warranties, function (warranty) {
            return !warranty.isWarrantyFree && !warranty.returned;
        });

        if (nonFreeWarranties.length > 0) {
            var promises = [];
            _.each(nonFreeWarranties, function (w) {
                promises.push(nonFreeWarrantyUtility(item, w));
            });
            CommonService.$q.all(promises).then(function () {
                deferred.resolve();
            });
        }
        else {
            deferred.resolve();
        }

        return deferred.promise;
    }

    function returnItemKit(parentId) {
        var kitItems = _.filter($scope.cart.originalOrder.order.items, function (item) {
            return item.parentId === parentId;
        });

        if (kitItems && kitItems.length > 0) {
            _.each(kitItems, function (kit) {
                returnItem(kit);
            });
        }
    }

    //function returnItemInstallations(item) {
    //
    //    if (item.installations && item.installations.length > 0) {
    //        _.each(item.installations, function (installation) {
    //            installation.returned = true;
    //            installation.returnQuantity = 1;
    //
    //            item.returnedItems.push(installation);
    //        });
    //    }
    //}

    function calculateElapsedMonths(warranty) {
        var currentDate = new Date();
        var purchasedDate = new Date($scope.cart.originalOrder.order.createdOn);
        var totalMonthsElapsed = (currentDate.getFullYear() - purchasedDate.getFullYear()) * 12
                                 + (currentDate.getMonth() - purchasedDate.getMonth());
        if (totalMonthsElapsed > warranty.warrantyLengthMonths) {
            return -1;
        }
        else if (totalMonthsElapsed < warranty.warrantyLengthMonths) {
            return totalMonthsElapsed;
        }
        else {
            return 0;
        }
    }

    function nonFreeWarrantyUtility(item, nonFreeWarranty) {
        var deferred = CommonService.$q.defer();
        var monthsElapsed = calculateElapsedMonths(nonFreeWarranty);

        if (monthsElapsed > 0) {
            var params = {
                warrantyNumber: nonFreeWarranty.itemNo,
                branch: $scope.user.BranchNumber || 0,
                elapsedMonths: monthsElapsed
            };
            PosDataService.getWarrantyReturnPercentage(params).then(function (data) {
                if (data) {
                    nonFreeWarranty.warrantyReturnPercentage = data.PercentageReturn;
                    deferred.resolve();
                }
            });
        }
        else if (monthsElapsed < 0) {
            nonFreeWarranty.warrantyReturnPercentage = 0;
            deferred.resolve();
        }
        else {
            deferred.resolve();
        }
        return deferred.promise;
    }

    function searchResultClicked(event, data) {
        //if (isAdding) {
        //    return;
        //}

        if (!$scope.cart.saleComplete) {
            CommonService.$timeout(function () {
                data.element.addClass('added').find('.cart_in').addClass('added');
            }, 0);
            CommonService.$timeout(function () {
                data.element.removeClass('added').find('.cart_in').removeClass('added');
            }, 1000);
        }
        isAdding = true;
        addItemToCart(data.resultItem);
        //CommonService.$broadcast('pos:item:add', data.resultItem);
    }

    function requestDiscountLimitAuthorisation(totalDiscount, item) {
        var message = 'The total discount applied for the current item is ' + Utilities.roundFloat(totalDiscount)
                      + '% which is greater than the threshold ' + $scope.MasterData.discountLimit +
                      '%. This requires authorisation.';

        CommonService.$broadcast('requestAuthorisation', {
            text: message,
            title: 'Discount Limit Authorisation',
            requiredPermission: SalesEnums.Permissions.AuthoriseDiscountLimit
        });
    }

    function canDoIrExchange(item) {
        return BasketService.canDoIrExchange(item);
    }

    function activate() {

        $scope.$on('authorisationSuccess', function (event, data) {
            if ($scope.cart.pendingDiscAuthorisationItemNo) {
                var item = _.find($scope.cart.items, function (i) {
                    return i.itemNo === $scope.cart.pendingDiscAuthorisationItemNo && !i.returned;
                });
                addDiscount(item);
                $scope.cart.pendingDiscAuthorisationItemNo = null;
            }
            if ($scope.cart.itemBeingReturned) {
                $scope.cart.authorisationPending = false;
            }
            returnAuthorisationSuccess(data.authorisedBy);
        });

        $scope.$on('authorisationFailure', function (event, data) {
            returnAuthorisationFailure(data);
        });

        $scope.$on('pos:searchitem:add', function () {
            $scope.addSearchItem();
        });

        $scope.$on('facetsearch:result:click', searchResultClicked);

        $scope.$on('pos:item:add', function (event, item) {
            addItemToCart(item);
        });

        $scope.$on('pos:return:manual', function () {
            manualReturnCartContents();
        });

    }

    //endregion

};

basketController.$inject = dependInjects;

module.exports = basketController;