/*global _, moment, module */
var searchController = function ($scope, $http, $compile, LookupDataService, CommonService,
                                 $dialog, SettingUtils) {
    'use strict';

    $scope.MasterData = {};
    $scope.CountryTaxRate = null;

    var pickLists = [{
        name: 'BRANCH',
        v: 'branches'
    },
        {
            name: 'fascia',
            v: 'fascia'
        }
    ];

    var growlTimeout = 3;

    function populatePickLists (list) {
        // TODO: Fix
        _.each(list, function (item) {
            LookupDataService.populate(item.name, function (data) {
                if (_.isFunction(item.filter)) {
                    $scope.MasterData[item.v] = item.filter(data);
                } else {
                    $scope.MasterData[item.v] = data;
                }
            });
        });
    }

    $scope.GetCountryTaxRate = function () {
        SettingUtils.getCountryTaxRate(function (data) {
            $scope.CountryTaxRate = data.currentTaxRate;
        });
    };

    $scope.GetCountryTaxRate();

    populatePickLists(pickLists);

    $scope.nonStockEditPrice = function (doc) {
        if (_.isUndefined(doc.editPrices) || _.isNull(doc.editPrices) || !doc.editPrices) {
            doc.editPrices = true;
        } else {
            doc.editPrices = false;
        }
        getPriceInfo(doc);
    };

    $scope.addNewPrice = function (doc) {

        var today = new Date();
        var tomorrow = new Date(today);
        tomorrow.setDate(today.getDate()+1);

        doc.newLocationPrice = {CostPrice: 0, RetailPrice: 0, TaxInclusivePrice: 0, DiscountValue: 0, effectiveDate: tomorrow, endDate: tomorrow};
        doc.addNewPrice = true;
        doc.addPricing = true;
    };

    $scope.newRetailPriceChanged = function (doc) {
        if (!_.isUndefined(doc.newLocationPrice.RetailPrice) && !_.isUndefined(doc.TaxRate)) {
            var retailPrice = doc.newLocationPrice.RetailPrice * 1; // Convert to numeric value
            doc.newLocationPrice.TaxInclusivePrice = parseFloat(
                (retailPrice + (retailPrice * doc.TaxRate / 100)).toFixed(2)
            );
        }
    };

    $scope.newTaxInclusivePriceChanged = function (doc) {
        if (!_.isUndefined(doc.newLocationPrice.TaxInclusivePrice) && !_.isUndefined(doc.TaxRate)) {
            var taxInclusivePrice = doc.newLocationPrice.TaxInclusivePrice * 1;
            doc.newLocationPrice.RetailPrice = parseFloat(
                (taxInclusivePrice / ((100 + doc.TaxRate) / 100)).toFixed(2)
            );
        }
    };

    var isNewPriceStringValueValid = function (price) {
        if (_.isUndefined(price)) {
            return false;
        } else {
            if (parseFloat(price) >= 0.00) {
                return true;
            } else {
                return false;
            }
        }
    };

    $scope.isCostPriceStringValueValid = function (CostPrice) {
        return isNewPriceStringValueValid(CostPrice);
    };

    $scope.isRetailPriceStringValueValid = function (RetailPrice) {
        return isNewPriceStringValueValid(RetailPrice);
    };

    $scope.isTaxInclusivePriceStringValueValid = function (TaxInclusivePrice) {
        return isNewPriceStringValueValid(TaxInclusivePrice);
    };

    $scope.isDiscountValueStringValueValid = function (DiscountValue) {
        return isNewPriceStringValueValid(DiscountValue);
    };

    var getPriceInfo = function (data) {
        $http({
            method: 'GET',
            url: 'NonStocks/Api/Price/' + data.Id
        }).then(function (priceData) {
            var taxRate = data.TaxRate;

            if (_.isUndefined(taxRate)) {
                data.TaxRate = taxRate = $scope.CountryTaxRate;
            }
            _.each(priceData.data.NonStockPrice, function (locationPrice) {
                getDisplayLocationPrice(locationPrice, taxRate);
            });

            data.priceData = priceData.data.NonStockPrice;
        });
    };

    $scope.DisplayTaxRate = function(taxRate){
        if (taxRate >= 0) {
            return taxRate;
        } else {
            return $scope.CountryTaxRate;
        }
    };

    $scope.isEffectiveDateInTheFuture = function (effectiveDate){
        return (+(new Date())) > (+effectiveDate);
    };

    var emitGrowlMessage = function (errorMessage, timeout) {
        var defaultTimeout = growlTimeout;
        if (!isNaN(timeout) &&
            parseInt(timeout, 10) > defaultTimeout) {
            defaultTimeout = parseInt(timeout, 10);
        }
        if (errorMessage.length > 0) {
            CommonService.addGrowl({
                timeout: defaultTimeout,
                type: 'danger', // (optional - info, danger, success, warning)
                content: 'Error: ' + errorMessage
            });
        }
    };

    var getDisplayLocationPrice = function (locationPrice, taxRate) {
        if (!_.isUndefined(locationPrice.Fascia) && !_.isNull(locationPrice.Fascia)) {
            locationPrice.Fascia = $scope.MasterData.fascia[locationPrice.Fascia];
        }

        if (!_.isUndefined(locationPrice.BranchNumber) && !_.isNull(locationPrice.BranchNumber)) {
            locationPrice.BranchName = $scope.MasterData.branches[locationPrice.BranchNumber];
        }

        locationPrice.CostPrice = ((locationPrice.CostPrice * 100) / 100).toFixed(2);
        locationPrice.RetailPrice = ((locationPrice.RetailPrice * 100) / 100).toFixed(2);
        locationPrice.TaxInclusivePrice = locationPrice.TaxInclusivePrice.toFixed(2);


        locationPrice.DiscountValue = _.isNull(locationPrice.DiscountValue) ? 0 : locationPrice.DiscountValue.toFixed(2);
        locationPrice.effectiveDate = moment(locationPrice.EffectiveDate)._d;
        locationPrice.endDate = _.isNull(locationPrice.EndDate) ? null : moment(locationPrice.EndDate)._d;

        // convert to numbers
        locationPrice.CostPrice = parseFloat(locationPrice.CostPrice);
        locationPrice.RetailPrice =  parseFloat(locationPrice.RetailPrice);
        locationPrice.TaxInclusivePrice = parseFloat(locationPrice.TaxInclusivePrice);
        locationPrice.DiscountValue = parseFloat(locationPrice.DiscountValue);

        locationPrice.OriginalCostPrice = locationPrice.CostPrice;
        locationPrice.OriginalRetailPrice = locationPrice.RetailPrice;
        locationPrice.OriginalTaxInclusivePrice = locationPrice.TaxInclusivePrice;
        locationPrice.OriginalDiscountValue = locationPrice.DiscountValue;
        locationPrice.OriginalEffectiveDate = locationPrice.effectiveDate;

        var priceItemDate = moment(locationPrice.effectiveDate)._d;
        var priceEndDate = locationPrice.endDate || null;
        priceEndDate = _.isNull(priceEndDate) ? null : moment(priceEndDate)._d;

        locationPrice.EffectiveDateInThePast = false;
        locationPrice.EndDateInThePast = false;

        locationPrice.EffectiveDateInThePast = $scope.isEffectiveDateInTheFuture(priceItemDate);
        locationPrice.EndDateInThePast = _.isNull(locationPrice.EndDate) ? false : $scope.isEffectiveDateInTheFuture(priceEndDate);

    };

    $scope.createNewLocationPrice = function (doc) {

        var branchNumber = null;
        var fascia = null;
        var nonStockType = doc.NonStockType;
        var costPrice = doc.newLocationPrice.CostPrice * 1;
        var retailPrice = doc.newLocationPrice.RetailPrice * 1;
        var discountValue = doc.newLocationPrice.DiscountValue * 1;
        var taxInclusivePrice = doc.newLocationPrice.TaxInclusivePrice;
        var taxRate = doc.TaxRate;
        var effectiveDate = _.isNull(doc.newLocationPrice.effectiveDate) ? null : moment(doc.newLocationPrice.effectiveDate).format('YYYY-MM-DD');
        var endDate = doc.NonStockType !== 'Discount' ? null : (
            _.isNull(doc.newLocationPrice.endDate) ? null : moment(doc.newLocationPrice.endDate).format('YYYY-MM-DD'));
        var errorMessage = '';

        if ($scope.isEffectiveDateInTheFuture(doc.newLocationPrice.effectiveDate)) {
            errorMessage = "The Effective Date can only be created/saved with a value " +
            "bigger than today's date (please choose the dates tomorrow or after).";
            return emitGrowlMessage(errorMessage, 9);
        }

        if (!_.isUndefined(endDate) && !_.isNull(endDate) && $scope.isEffectiveDateInTheFuture(doc.newLocationPrice.endDate)) {
            errorMessage = "The End Date can only be created/saved with a value " +
            "bigger than today's date (please choose the dates tomorrow or after).";
            return emitGrowlMessage(errorMessage, 9);
        }

        if (!_.isUndefined(endDate) && !_.isNull(endDate) && (endDate < effectiveDate)) {
            errorMessage =  "The End Date cannot be before the Effective Date.";
            return emitGrowlMessage(errorMessage, 9);
        }

        if (!_.isUndefined(doc.newLocationPrice.BranchNumber)) {
            branchNumber = doc.newLocationPrice.BranchNumber;
        }

        if (!_.isUndefined(doc.newLocationPrice.Fascia)) {
            fascia = doc.newLocationPrice.Fascia;
        }

        $http({
            method: 'PUT',
            url: 'NonStocks/Api/Price/',
            data: {
                Id: 0,
                NonStockId: doc.Id,
                NonStockType: nonStockType,
                Fascia: fascia,
                BranchNumber: branchNumber,
                CostPrice: costPrice,
                RetailPrice: retailPrice,
                TaxInclusivePrice: taxInclusivePrice,
                DiscountValue: discountValue,
                EffectiveDate: effectiveDate,
                EndDate: endDate
            }
        }).then(function (locationPrice) {
            if (locationPrice.hasError) {
                CommonService.addGrowl({
                    type: 'warning', // (optional - info, danger, success, warning)
                    content: 'Error: ' + locationPrice.message
                });
            } else {
                getDisplayLocationPrice(locationPrice.data.NonStockPrice, taxRate);
                doc.priceData.unshift(locationPrice.data.NonStockPrice);
                doc.WarrantyHasPrices = "Yes";
                doc.addNewPrice = false;

                CommonService.addGrowl({
                    type: 'info', // (optional - info, danger, success, warning)
                    content: 'Non Stock Price added successfully',
                    timeout: growlTimeout
                });
            }
        }, function(response){
            CommonService.addGrowl({
                type: 'danger', // (optional - info, danger, success, warning)
                content: response.data,
                timeout: growlTimeout
            });
        });
    };

    $scope.saveLocationPriceChange = function (nonStockTaxRate, locationPrice) {
        var errorMessage = "";
        var costPrice = locationPrice.CostPrice * 1;
        var retailPrice = locationPrice.RetailPrice * 1;
        var taxInclusivePrice = locationPrice.TaxInclusivePrice;
        var discountValue = locationPrice.DiscountValue;
        var taxRate = nonStockTaxRate;
        var effectiveDate = _.isNull(locationPrice.effectiveDate) ? null : moment(locationPrice.effectiveDate).format('YYYY-MM-DD');
        var endDate = locationPrice.NonStockType !== 'discount' ? null : (
            _.isNull(locationPrice.endDate) ? null : moment(locationPrice.endDate).format('YYYY-MM-DD'));

        if ($scope.isEffectiveDateInTheFuture(locationPrice.effectiveDate))
        {
            errorMessage = "The Effective Date can only be created/saved with a value " +
            "bigger than today's date (please choose the dates tomorrow or after).";
            return emitGrowlMessage(errorMessage, 9);
        }

        if (!_.isUndefined(endDate) && !_.isNull(endDate) && $scope.isEffectiveDateInTheFuture(locationPrice.endDate))
        {
            errorMessage = "The End Date can only be created/saved with a value " +
            "bigger than today's date (please choose the dates tomorrow or after).";
            return emitGrowlMessage(errorMessage, 9);
        }

        if (!_.isUndefined(endDate) && !_.isNull(endDate) && (endDate < effectiveDate)) {
            errorMessage =  "The End Date cannot be before the Effective Date.";
            return emitGrowlMessage(errorMessage, 9);
        }

        if (locationPrice.NonStockType !== 'discount') {
            if (((!retailPrice || retailPrice < 0)) || !costPrice || costPrice < 0) {
                return null;
            }
        } else {
            if (!discountValue || discountValue < 0) {
                return null;
            }
        }

        $http({
            url: 'NonStocks/Api/Price/',
            method: 'PUT',
            data: {
                Id: locationPrice.Id,
                NonStockId: 0,
                CostPrice: costPrice,
                RetailPrice: retailPrice,
                TaxInclusivePrice: taxInclusivePrice,
                DiscountValue: discountValue,
                EffectiveDate: effectiveDate,
                EndDate: endDate
            }
        }).success(function (savedLocationPrice) {
            locationPrice.OriginalCostPrice = savedLocationPrice.NonStockPrice.CostPrice;
            locationPrice.OriginalRetailPrice = savedLocationPrice.NonStockPrice.RetailPrice;
            locationPrice.OriginalTaxInclusivePrice = (savedLocationPrice.NonStockPrice.RetailPrice +
            (savedLocationPrice.NonStockPrice.RetailPrice * taxRate / 100)).toFixed(2);
            locationPrice.OriginalDiscountValue = savedLocationPrice.NonStockPrice.DiscountValue;

            CommonService.addGrowl({
                type: 'info', // (optional - info, danger, success, warning)
                content: 'Non Stock Price updated successfully',
                timeout: growlTimeout
            });
        });
    };

    $scope.deleteLocationPrice = function (doc, locationPrice) {
        var id = locationPrice.Id;
        var deleteConfirmation = $dialog.messageBox('Confirm Price Delete',
            'You have chosen to delete the Price information for Store Type "' +
            (locationPrice.Fascia || 'All') + '", Store Location "' +
            (locationPrice.BranchName || 'All') + '". Are you sure you want to do this?', [{
                label: 'Delete',
                result: 'yes',
                cssClass: 'btn-primary'
            }, {
                label: 'Cancel',
                result: 'no'
            }]);

        deleteConfirmation.open().then(function (choice) {
            if (choice === 'yes') {
                $http({
                    method: 'DELETE',
                    url: 'NonStocks/Api/Price/' + id
                }).success(function (data) {
                    if (data.Result === "Done") {
                        doc.priceData = _.reject(doc.priceData, function (locationPrice) {
                            return locationPrice.Id === id;
                        });

                        CommonService.addGrowl({
                            type: 'info', // (optional - info, danger, success, warning)
                            content: 'Non Stock Price deleted successfully',
                            timeout: growlTimeout
                        });

                        if (doc.priceData) {
                            if (doc.priceData.length === 0) {
                                doc.NonStockHasPrices = "No";
                            } else {
                                doc.NonStockHasPrices = "Yes";
                            }
                        } else {
                            doc.NonStockHasPrices = "No";
                        }
                    }
                });
            }
        });
    };

    $scope.cancelNewLocationPrice = function (doc) {
        doc.newLocationPrice = {};
        doc.addNewPrice = false;
    };

    $scope.cancelLocationPriceChange = function (locationPrice) {
        locationPrice.CostPrice = parseFloat(locationPrice.OriginalCostPrice);
        locationPrice.RetailPrice = parseFloat(locationPrice.OriginalRetailPrice);
        locationPrice.TaxInclusivePrice = parseFloat(locationPrice.OriginalTaxInclusivePrice);
        locationPrice.DiscountValue = parseFloat(locationPrice.OriginalDiscountValue);
    };

    $scope.retailPriceChanged = function (nonStockTaxRate, locationPrice) {
        if (!_.isUndefined(locationPrice) && !_.isUndefined(nonStockTaxRate)) {
            var retailPrice = locationPrice.RetailPrice * 1; // Convert to numeric value
            var taxRate = nonStockTaxRate;
            locationPrice.TaxInclusivePrice = parseFloat(
                (retailPrice + (retailPrice * taxRate / 100)).toFixed(2)
            );
        }
    };

    $scope.taxInclusivePriceChanged = function (nonStockTaxRate, locationPrice) {
        if (!_.isUndefined(locationPrice) && !_.isUndefined(nonStockTaxRate)){
            var taxInclusivePrice = locationPrice.TaxInclusivePrice * 1;
            var taxRate = nonStockTaxRate;
            locationPrice.RetailPrice = parseFloat(
                (taxInclusivePrice / ((100 + taxRate) / 100)).toFixed(2)
            );
        }
    };
};

searchController.$inject = ['$scope', '$http', '$compile', 'LookupDataService', 'CommonService',
    '$dialog', 'SettingUtils'];
module.exports = searchController;
