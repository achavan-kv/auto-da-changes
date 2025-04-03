define(
    [
        'angular',
        'url',
        'underscore',
        'merchandising/product/services/additionalCostProvider'
    ],
    function (angular, url, _) {
        'use strict';
        return function (user, pageHelper, costsResourceProvider, productResourceProvider, additionalCostProvider, $rootScope) {
            return {
                restrict: 'E',
                scope: {
                    costPrice: '=',
                    currencies: '=',
                    product: '=',
                    productType: '=',
                    PreviousProductType: '=',
                    AddtionalCostPrices: '=',
                    vendors: "="
                },
                templateUrl: url.resolve('/Static/js/merchandising/product/templates/costControls.html'),
                link: function (scope, element, attrs) {

                    scope.canView = user.hasPermission("CostPriceView");
                    scope.canEdit = user.hasPermission("CostPriceEdit") && scope.productType !== "RepossessedStock";
                    scope.localCurrency = pageHelper.localisation.localCurrency;

                    //Code to Add Controls For Multiple Cost costPrice

                    scope.addtionalCPs = scope.product.addtionalCPs;
                    console.log(scope.addtionalCPs);
                    scope.Vendors = [];
                    scope.AddNewCPCount = 0;
                    scope.AddNewCostPrice = function ()  {
                        var additionalCp = {
                            "ID": 0,
                            "vendorId": '',
                            "ProductId": scope.product.id,
                            "VendorName": '',
                            "supplierCost": '',
                            "supplierCurrency": '',
                            "lastLandedCost": '',
                            "averageWeightedCost": ''
                        };
                        additionalCp.ID = scope.AddNewCPCount + 1;
                        scope.AddNewCPCount = additionalCp.ID;
                        if (scope.addtionalCPs != null) {
                            scope.addtionalCPs.push(additionalCp);
                            console.log(scope.addtionalCPs);
                            if (scope.addtionalCPs.length > 0) {
                                scope.canSaveAdditionalCp = false;
                            }
                            else {
                                scope.canSaveAdditionalCp = true;
                            }
                        }
                    }

                    scope.removeCP = function (item)  {
                        var index = scope.addtionalCPs.indexOf(item);
                        scope.addtionalCPs.splice(index, 1);
                        updateIDs(item);
                        scope.AddNewCPCount = parseInt(scope.AddNewCPCount) - 1;
                        if (scope.addtionalCPs.length > 0) {
                            scope.canSaveAdditionalCp = false;
                        }
                        else {
                            scope.canSaveAdditionalCp = true;
                        }
                    }
                    scope.AdditionalCostOK = function (from) {
                        from.averageWeightedCost = from.lastLandedCost;
                    }
                    function updateIDs(item) {
                        if (scope.addtionalCPs != null) {
                            for (var i = 0; i < scope.addtionalCPs.length; i++) {
                                if (parseInt(scope.addtionalCPs[i].ID) > parseInt(item.ID)) {
                                    scope.addtionalCPs[i].ID = parseInt(scope.addtionalCPs[i].ID) - 1;

                                }
                            }
                        }
                    }
                    ///End for the code of Ashley


                    scope.canSave = function (form) {

                        var IsValidVendor = true;
                        if (scope.addtionalCPs != null) {
                            if (scope.addtionalCPs.length > 0) {
                                for (var i = 0; i < scope.addtionalCPs.length; i++) {
                                    if (scope.addtionalCPs[i].vendorId == '' || scope.addtionalCPs[i].vendorId == "" || scope.addtionalCPs[i].vendorId == null) {
                                        IsValidVendor = false;
                                        return scope.canEdit && form.$valid && IsValidVendor;
                                    }
                                }
                            }
                        }
                        return scope.canEdit && form.$valid;
                    }

                    function checkForDuplicateVendorInAdditionalCP(originalVendor) {
                        if (scope.addtionalCPs[0].vendorId == originalVendor) {
                            console.log(scope.vendors[originalVendor]);
                            pageHelper.notification.show("You are adding duplicate additional cost price for default vendor : " + scope.vendors[originalVendor]);
                            return false;
                        }
                        if (scope.addtionalCPs != null) {
                            for (var i = 0; i < scope.addtionalCPs.length; i++) {
                                for (var j = 0; j < scope.addtionalCPs.length; j++) {
                                    if (i == j) {
                                        continue;
                                    }
                                    if (scope.addtionalCPs[i].vendorId == originalVendor) {
                                        console.log(scope.vendors[originalVendor]);
                                        pageHelper.notification.show("You are adding duplicate additional cost price for default vendor : " + scope.vendors[originalVendor]);
                                        return false;
                                    }
                                    if (scope.addtionalCPs[i].vendorId == scope.addtionalCPs[j].vendorId) {
                                        pageHelper.notification.show("You are adding duplicate additional cost price for duplicate vendor : " + scope.vendors[scope.addtionalCPs[i].vendorId]);
                                        console.log(scope.vendors[scope.addtionalCPs[i].vendorId]);
                                        return false;
                                    }
                                }
                            }
                        }
                        return true;
                    }


                    scope.ok = function (form) {

                        if (scope.product.previousProductType == 'RegularStock') {
                            scope.costPrice.averageWeightedCost = form;

                        }

                    }

                    costsResourceProvider.getAshleyEnabled().then(function (response) {
                        if (response == "True")
                            scope.GetAshleyEnable = true;
                        else
                            scope.GetAshleyEnable = false;

                        if (scope.addtionalCPs == null) {
                            scope.canSaveAdditionalCp = false;
                        }
                        else{
                            scope.canSaveAdditionalCp = true;
                        }

                        console.log('Response ' + response);
                    }, function (response) {
                        pageHelper.notification.show(response.message);
                    });

                    scope.canSaveAdditionalCp = true;//scope.product.productAttributes != null && (scope.product.productAttributes.isAshleyProduct != null || scope.product.productAttributes.isAshleyProduct != undefined || scope.product.productAttributes.isAshleyProduct != false);

                    scope.save = function () {
                        scope.saving = true;
                        scope.costPrice.productId = scope.product.id;
                        
                        if (scope.product.previousProductType == 'RegularStock') {
                            scope.costPrice.averageWeightedCost = scope.costPrice.lastLandedCost;

                        }
                        if (scope.addtionalCPs != null) {
                            if (scope.GetAshleyEnable && scope.addtionalCPs.length > 0) {
                                if (!checkForDuplicateVendorInAdditionalCP(scope.product.primaryVendorId)) {
                                    scope.saving = false;
                                    return;
                                }
                            }
                        }
                                                
                        var costService = costsResourceProvider
                            .save(scope.costPrice)
                            .then(function (response) {
                                scope.costPrice.id = response.id;
                                pageHelper.notification.show('Cost price saved successfully');
                                productResourceProvider.getProductStatusUpdate(scope.product.id).then(function (result) {
                                    scope.product.status = result.status;
                                });

                                $rootScope.$broadcast("productUpdated");
                                scope.saving = false;
                            }, function (response) {
                                pageHelper.notification.show(response.message);
                            });                        
                        //Executes for Ashley Product Only
                        if (scope.addtionalCPs != null) {
                            if (scope.GetAshleyEnable && scope.addtionalCPs.length > 0) {

                                if (true) {
                                    var additionalCostService = additionalCostProvider
                                        .save(scope.addtionalCPs)
                                        .then(function (response) {
                                            scope.costPrice.id = response.id;
                                            pageHelper.notification.show('Additional Cost price saved successfully');
                                            productResourceProvider.getProductStatusUpdate(scope.product.id).then(function (result) {
                                                scope.product.status = result.status;
                                            });

                                            $rootScope.$broadcast("productUpdated");
                                            scope.saving = false;
                                        }, function (response) {
                                            pageHelper.notification.show(response.message);
                                        });
                                }
                                else {
                                    pageHelper.notification.show('The Addtional Cost could only be set for Ashley Product');
                                }
                            }
                        }
                    };
                }
            };
        };
    });