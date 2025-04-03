/*global _, moment, module */
var promotionsController = function ($scope, $http, $compile, LookupDataService, CommonService, $dialog, $location,
                                     $anchorScroll, $attrs, $timeout) {
    'use strict';

    $scope.MasterData = {};
    $scope.startDateDefault = (moment(new Date()).add(1, 'days')).toISOString();
    $scope.newPromotion = {startDate : "", endDate: ""};
    var growlTimeout = 3;

    var pickLists = [
        {
            name: 'BRANCH',
            v: 'branches'
        },
        {
            name: 'fascia',
            v: 'fascia'
        }
    ];

    $scope.MasterData.PromotionActions = {
        '%': 'Percentage Discount',
        '=': 'Set Price'
    };

    $scope.creatingNewPromotion = false;

    function populatePickLists(list) {
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

    populatePickLists(pickLists);

    function setDefaultValueForActiveFrom(value) {
        if (value) {
            $scope.filterPromotions.ActiveFrom = new Date(moment($scope.filterPromotions.ActiveFrom).format());
        }
    }

    $scope.clearFilter = function () {
        $scope.filterPromotions = _.omit($scope.filterPromotions, 'NonStock', 'SKU', 'ActiveTo', 'ActiveFrom',
            'BranchNumber', 'Fascia');
        setDefaultValueForActiveFrom();
        $scope.performSearch();
    };

    $scope.filterPromotions = $location.search();

    if (!$scope.filterPromotions.PageSize) {
        $scope.filterPromotions.PageSize = 10;
    }

    if (!$scope.filterPromotions.SKU) {
        $scope.filterPromotions.SKU = "";
    }

    if (!$scope.filterPromotions.ActiveFrom) {
        delete $scope.filterPromotions.ActiveFrom;
    } else {
        $scope.filterPromotions.ActiveFrom =
            _.isNull($scope.filterPromotions.ActiveFrom) ? '' : $scope.filterPromotions.ActiveFrom;
    }

    if (!$scope.filterPromotions.ActiveTo) {
        delete $scope.filterPromotions.ActiveTo;
    } else {
        $scope.filterPromotions.ActiveTo =
            _.isNull($scope.filterPromotions.ActiveTo) ? '' : $scope.filterPromotions.ActiveTo;
    }

    $scope.populateExistingPromotions = function (allPromotions) {
        $location.hash('search');
        $anchorScroll();

        _.each(allPromotions, function (promo) {
            promo.Filters = [];

            if (promo.PercentageDiscount > 0) {
                promo.IsPercentage = true;
                promo.PromotionAmount = promo.PercentageDiscount;
            } else if (promo.RetailPrice > 0) {
                promo.IsPercentage = false;
                promo.PromotionAmount = promo.RetailPrice;
            }

            promo.startDate = moment(promo.StartDate).toDate();
            promo.endDate = moment(promo.EndDate).toDate();
            promo.StartDate = promo.startDate.toISOString();
            promo.EndDate = promo.endDate.toISOString();

            promo.Filters.push({
                name: 'Fascia',
                value: promo.Fascia || 'All'
            });

            var branchNumber = _.find($scope.MasterData.branches, function (value, key) {
                return key === promo.BranchNumber;
            });

            promo.Filters.push({
                name: 'Branch',
                value: branchNumber || ' All'
            });

            promo.Filters.push({
                name: 'NonStock',
                value: promo.NonStockNumber + ' - ' +
                promo.ShortDescription + ' ' +
                promo.LongDescription,
                nonStockId: promo.NonStockId,
                nonStockUrl: '/#/NonStocks/Details/' + promo.NonStockId
            });
        });

        return allPromotions;
    };

    var getFormattedDate = function (dateObj, filter) {
        var refFormattedStr = '';

        if (_.isNull(dateObj)) {
            return '';
        }

        var filterParam = '&' + filter + '=';
        if (_.isDate(dateObj)) {
            refFormattedStr = filterParam + dateObj.toJSON();
        }

        return refFormattedStr;
    };

    $scope.performSearch = function () {
        var optionalSearchObj = $scope.filterPromotions;
        var tmpActiveFrom = getFormattedDate(optionalSearchObj.ActiveFrom, 'ActiveFrom');
        var tmpActiveTo = getFormattedDate(optionalSearchObj.ActiveTo, 'ActiveTo');

        var queryString = 'NonStock=' + encodeURIComponent(optionalSearchObj.NonStock || '') +
            '&SKU=' + encodeURIComponent(optionalSearchObj.SKU || '') +
            tmpActiveFrom +
            tmpActiveTo +
            '&BranchNumber=' + encodeURIComponent(optionalSearchObj.BranchNumber || '') +
            '&Fascia=' + encodeURIComponent(optionalSearchObj.Fascia || '') +
            '&PageSize=' + encodeURIComponent(optionalSearchObj.PageSize) +
            '&PageIndex=' + encodeURIComponent(optionalSearchObj.PageIndex);

        $http({
            method: 'GET',
            url: 'NonStocks/Api/Promotions/?' + queryString
        }).success(function (data) {
            if (data) {
                _.extend($scope.filterPromotions, {
                    PageCount: data.PageCount,
                    PageIndex: data.PageIndex,
                    RecordCount: data.RecordCount
                });
                $scope.promotions = $scope.populateExistingPromotions(data.Page);
            }
        });
    };

    $scope.performSearch();

    $scope.showNewRow = function ($event) {
        $scope.creatingNewPromotion = true;
        $event.preventDefault();
        var today = new Date();
        var tomorrow = new Date(today);
        tomorrow.setDate(today.getDate()+1);
        $scope.newPromotion.startDate = tomorrow;
        $scope.newPromotion.endDate = tomorrow;
    };

    $scope.canSave = function (promotion) {
        if (!promotion.PromotionAmount || !promotion.startDate || !promotion.endDate || !promotion.Action || !promotion.NonStock) {
            return false;
        }
        return true;
    };

    $scope.nonStockSearchSetup = function(){
        $http({ // Load all non stocks
            url: '/NonStocks/Api/NonStock/LoadAll',
            method: 'GET'
        }).then(function (response) {
            if (response && response.data && response.data.NonStocks) {
                var tmpAllProdScope = {};
                $scope.allProducts = _.filter(response.data.NonStocks, function(nonStock){
                    return nonStock.Type !== "discount";
                });
                _.map($scope.allProducts, function (nonStock) {
                    tmpAllProdScope[nonStock.Id] = nonStock.SKU + ' - ' +
                    nonStock.ShortDescription + ' ' +
                    nonStock.LongDescription;
                });
                $scope.allProductsScope = tmpAllProdScope;
            }
        });
    };

    $scope.nonStockSearchSetup();

    $scope.isEffectiveDateInValid = function (effectiveDate) {
        if ((+(new Date())) > (+effectiveDate)) { // Check if this date is in the past
            return true;
        } else {
            return false;
        }
    };

    var emitGrowlMessage = function (errorMessage, timeout) {
        var defaultTimeout = growlTimeout;
        if (!isNaN(timeout) && parseInt(timeout, 10) > defaultTimeout) {
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

    $scope.createNewPromotion = function () {
        var errorMessage = "";

        if (!$scope.canSave) {
            return;
        }

        var promotion = $scope.newPromotion;

        if ($scope.isEffectiveDateInValid(promotion.startDate) || $scope.isEffectiveDateInValid(promotion.endDate)) {
            errorMessage = "The Start Date / End Date can only be created/saved with a value " +
                "bigger than today's date (please choose the dates tomorrow or after).";
            return emitGrowlMessage(errorMessage, 9);
        } else {
            if ((promotion.endDate) < promotion.startDate) {
                errorMessage = "The End Date cannot be before the Start Date";
                return emitGrowlMessage(errorMessage, 9);
            }

            promotion.StartDate = _.isNull(promotion.startDate) ? '' : promotion.startDate;
            promotion.EndDate = _.isNull(promotion.endDate) ? '' : promotion.endDate;
            if (promotion.Action === '%') {
                promotion.PercentageDiscount = promotion.PromotionAmount;
                promotion.RetailPrice = null;
            } else if (promotion.Action === '=') {
                promotion.RetailPrice = promotion.PromotionAmount;
                promotion.PercentageDiscount = null;
            }
            promotion.NonStockId = promotion.NonStock.Id;

            $http({
                method: 'PUT',
                url: 'NonStocks/Api/Promotions/',
                data: promotion
            }).then(function (data) {
                if (data.data.Result === "Success") {

                    CommonService.addGrowl({
                        type: 'info', // (optional - info, danger, success, warning)
                        content: 'Non Stock Promotion added successfully',
                        timeout: growlTimeout
                    });

                    $scope.newPromotion = {};

                    if (data.data.NonStockPromotion.Fascia) {
                        data.data.NonStockPromotion.Fascia = _.find($scope.MasterData.fascia, function (value, key) {
                            return key === data.data.NonStockPromotion.Fascia;
                        });
                    }

                    $scope.promotions.push($scope.populateExistingPromotions([data.data.NonStockPromotion])[0]);
                    $scope.creatingNewPromotion = false;
                } else {
                    CommonService.addGrowl({
                        type: 'danger', // (optional - info, danger, success, warning)
                        content: data.data.Message,
                        timeout: growlTimeout
                    });
                }
            });
        }
    };

    $scope.deletePromotion = function (promotionIn) {
        var promotion = promotionIn;
        var id = promotion.Id;

        if (!id) {
            return;
        }

        var deleteConfirmation = $dialog.messageBox('Confirm Promotion Delete',
            'You have chosen to delete this Promotion. Are you sure you want to do this?', [{ label: 'Delete', result: 'yes' }, {
                label: 'Cancel',
                result: 'no'
            }]);

        deleteConfirmation.open().then(function (choice) {
            if (choice === 'yes') {
                $http({
                    method: 'DELETE',
                    url:'NonStocks/Api/Promotions/' + id
                }).then(function (data) {
                    if (data.data.Result === "Success") {

                        CommonService.addGrowl({
                            type: 'info', // (optional - info, danger, success, warning)
                            content: 'Non Stock Promotion deleted successfully',
                            timeout: growlTimeout
                        });

                        $scope.promotions = _.reject($scope.promotions, function (promotion) {
                            return promotion.Id === id;
                        });
                    }
                });
            }
        });
    };

    $scope.cancelNewPromotion = function () {
        $scope.newPromotion = {};

        $scope.creatingNewPromotion = false;
    };

    $scope.selectPage = function (page) {
        $scope.filterPromotions.PageIndex = page;
        $scope.performSearch();
    };

};

promotionsController.$inject = ['$scope', '$http', '$compile', 'LookupDataService', 'CommonService', '$dialog', '$location',
    '$anchorScroll', '$attrs', '$timeout'];
module.exports = promotionsController;
