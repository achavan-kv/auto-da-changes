/*global _, alert, moment, module */
var nonStockController = function ($scope, $http, $location, $routeParams, $interval,
                                   CommonService, hierarchyService, LookupDataService, SettingUtils) {
    'use strict';
    $scope.MasterData = {};
    $scope.CountryTaxRate = null;

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

    var growlTimeout = 3;

    $scope.NonStockTypes = {
        'inst': 'Installation',
        'rassist': 'Ready Assist',
        'assembly': 'Assembly Cost',
        'generic': 'Generic Service',
        'discount': 'Discount',
        'annual': 'Annual Service Contract'
    };

    $scope.RefundTypes = {
        'full': 'Full',
        'prorata': 'Pro Rata'
    };

    function populatePickLists(list) {
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

    $scope.GetCountryTaxRate = function () {
        SettingUtils.getCountryTaxRate(function (data) {
            $scope.CountryTaxRate = data.currentTaxRate;
        });
    };

    $scope.GetCountryTaxRate();

    $scope.hasEditLinkPermission = true;
    $scope.formData = {
        type: "",
        Id: 0,
        canEditSKU: true,
        canEditVendorUPC: true,
        SKU: "",
        VendorUPC: "",
        shortDescription: "",
        longDescription: "",
        discountRecurringPeriod: 0,
        isStaffDiscount: false,
        isActive: true,
        taxRate: null,
        hierarchy: null,
        allLevels: [],
        levels: [],
        isFullRefund: false,
        refundType: "",
        coverageValue: null,
        loadData: null
    };

    var setRefundType = function (nonStockType, isFullRefund) {
        if (nonStockType === 'annual' && isFullRefund === true) {
            return 'full';
        } else {
            return 'prorata';
        }
    };

    $http({
        url: '/NonStocks/Api/NonStock/Load?id=' + $routeParams.Id,
        method: "GET"
    })
        .then(function (response) {
            if (response && response.data && response.data.NonStock) {
                var nonStockObj = response.data.NonStock;
                $scope.formData.type = nonStockObj.Type;
                $scope.formData.Id = nonStockObj.Id;
                $scope.formData.SKU = nonStockObj.SKU;
                $scope.formData.VendorUPC = nonStockObj.VendorUPC;
                $scope.formData.shortDescription = nonStockObj.ShortDescription;
                $scope.formData.longDescription = nonStockObj.LongDescription;
                $scope.formData.length = nonStockObj.Length;
                $scope.formData.discountRecurringPeriod = nonStockObj.DiscountRecurringPeriod;
                $scope.formData.isStaffDiscount = nonStockObj.IsStaffDiscount;
                $scope.formData.isActive = nonStockObj.Active;
                $scope.formData.taxRate = nonStockObj.TaxRate;
                $scope.formData.hierarchy = nonStockObj.Hierarchy;
                $scope.formData.hasProductLink = nonStockObj.HasProductLink;
                $scope.formData.isFullRefund = nonStockObj.IsFullRefund;
                $scope.formData.refundType = setRefundType(nonStockObj.Type, nonStockObj.IsFullRefund);
                $scope.formData.coverageValue = nonStockObj.CoverageValue;

                if ($scope.formData.SKU && $scope.formData.SKU.length > 0) {
                    $scope.formData.canEditSKU = false;
                }
                if ($scope.formData.VendorUPC && $scope.formData.VendorUPC.length >= 0) {
                    $scope.formData.canEditVendorUPC = false;
                }
            }
        })
        .then(function () {
            hierarchyService.getHierarchy($scope);
        });


    $scope.locked = function () {
        return false;
    };

    var getNameForSelected = function (levelIndex, selectedKey) {
        var retSelectedName = '';
        if ($scope.formData && $scope.formData.levels && $scope.formData.levels.length > 0) {
            var selectedLevel = $scope.formData.levels[levelIndex];
            if (selectedLevel && selectedLevel.dataScope) {
                retSelectedName = selectedLevel.dataScope[selectedKey];
            }
        }
        return retSelectedName;
    };

    var refundType = function (nonStockType) {
        if (nonStockType === 'annual') {
            return $scope.formData.refundType === 'full';
        } else {
            return false;
        }
    };
    //---------------Product Import Validation----------------
    function validateComma(str) {
        if (str.indexOf(',') > -1) {
            return false; //if the str contains the comma then reutn false
        }
        return true;//otherwise return true
    }
    // show alert box to display message
    function alert(text, title) {
        CommonService.addGrowl({
            timeout: 30,
            type: 'danger', // (optional - info, danger, success, warning)
            content: text
        }
        );
    }
    $('#confirm').find('button.ok').off('click').on('click', function () {
        return $('#confirm').hide();
    });
    //---------------Product Import Validation End ----------------
    $scope.saveDetails = function (isSaveAndContinue) {
        var hierarchy = [],
            i;
        //Validate Comma from POS description, Long Description, Vendor UPC and Corporate UPC
        var failedValidationCount = 0;
        var failedValidationMessage = "<b> Please correct below details - </b><br />";
        if (!validateComma($scope.formData.shortDescription)) {
            failedValidationCount++;
            failedValidationMessage += "<br />" + failedValidationCount.toString() + ". Remove comma from field <b>\"Short Description\" </b>";
        }
        if (!validateComma($scope.formData.longDescription)) {
            failedValidationCount++;
            failedValidationMessage += "<br />" + failedValidationCount.toString() + ". Remove comma from field <b>\"Long Description\"</b>";
        }

        if (failedValidationCount > 0) {
            return alert(failedValidationMessage, "Field Validation");
            return;
        }
        for (i = 0; i < $scope.formData.levels.length; i++) {
            var curVal = $scope.formData.levels[i];
            hierarchy[i] = {
                LevelName: curVal.name,
                SelectedKey: curVal.val,
                SelectedValue: getNameForSelected(i, curVal.val)
            };
        }

        $scope.formData.Id = $routeParams.Id;

        var nonStockItem = {
            Id: $scope.formData.Id || 0,
            Type: $scope.formData.type,
            SKU: $scope.formData.SKU,
            VendorUPC: $scope.formData.VendorUPC,
            ShortDescription: $scope.formData.shortDescription,
            LongDescription: $scope.formData.longDescription,
            Length: $scope.formData.length,
            DiscountRecurringPeriod: $scope.formData.discountRecurringPeriod,
            IsStaffDiscount: $scope.formData.isStaffDiscount,
            Active: $scope.formData.isActive,
            TaxRate: $scope.formData.taxRate,
            Hierarchy: hierarchy,
            IsFullRefund: refundType($scope.formData.type),
            CoverageValue: $scope.formData.coverageValue
        };

        var endsWith = function (s, el) {
            return s.indexOf(el, s.length - el.length) !== -1;
        };

        var applyNewSavedIdToUrl = function (id) {
            var savedId = '/' + id,
                path = $location.path(),
                newPath = path.replace('/0', savedId);

            $scope.formData.Id = id;
            if (endsWith(newPath, savedId)) {
                $location.path(newPath);
            }
        };

        $scope.tryToSaveAndContinueCount = 0;
        $scope.tryToSaveAndContinue = function (isSaveAndContinue) {
            if ($scope.tryToSaveAndContinueCount <= 3) {
                var currentId = '' + $routeParams.Id;
                if (currentId === '0') {
                    $interval(function () {
                        // try to save and continue again
                        $scope.tryToSaveAndContinueCount += 1;
                        $scope.tryToSaveAndContinue(isSaveAndContinue);
                    }, 500);
                } else {
                    // where -> currentId !== '0'
                    $interval(function () {
                        var fullUrl = $location.absUrl();
                        if (endsWith(fullUrl, '/' + $routeParams.Id) && isSaveAndContinue === true) {
                            $location.path('/NonStocks/Details/0').replace();
                        }
                    }, 2000);
                }
            }
        };

        $http({
            url: '/NonStocks/Api/NonStock/Save',
            data: JSON.stringify(nonStockItem),
            method: "POST"
        })
            .then(function (response) {
                if (response.statusText === "Created" || response.statusText === "Accepted" && response.data.Result === "Done") {
                    // success
                    var id = response.data.Id;

                    applyNewSavedIdToUrl(id);
                    CommonService.addGrowl({
                        timeout: 5,
                        type: 'success', // (optional - info, danger, success, warning)
                        content: 'Non-Stock ' + id + ' Saved'
                    });

                    if (isSaveAndContinue === true) {
                        $scope.tryToSaveAndContinueCount = 0;
                        $scope.tryToSaveAndContinue(isSaveAndContinue);
                    }
                }
            },
            function (response) {
                CommonService.addGrowl({
                    type: 'danger', // (optional - info, danger, success, warning)
                    content: response.data,
                    timeout: growlTimeout
                });
            });
    };

    var getPriceInfo = function (id) {
        $http({
            url: 'NonStocks/Api/Price/' + id,
            method: 'GET'
        }).then(function (priceData) {
            $scope.prices = [];
            $scope.prices = priceData.data.NonStockPrice;

            _.each(priceData.data.NonStock, function (locationPrice) {
                getDisplayLocationPrice(locationPrice);
            });
        });
    };

    var getDisplayLocationPrice = function (locationPrice) {
        if (!_.isUndefined(locationPrice.Fascia) && !_.isNull(locationPrice.Fascia)) {
            locationPrice.Fascia = $scope.MasterData.fascia[locationPrice.Fascia];
        }

        if (!_.isUndefined(locationPrice.BranchNumber) && !_.isNull(locationPrice.BranchNumber)) {
            locationPrice.BranchName = $scope.MasterData.branches[locationPrice.BranchNumber];
        }
    };

    $scope.populateExistingPromotions = function (allPromotions) {
        _.each(allPromotions, function (singlePromotion) {
            singlePromotion.Filters = [];

            if (singlePromotion.PercentageDiscount > 0) {
                singlePromotion.IsPercentage = true;
                singlePromotion.PromotionAmount = singlePromotion.PercentageDiscount;
            } else if (singlePromotion.RetailPrice > 0) {
                singlePromotion.IsPercentage = false;
                singlePromotion.PromotionAmount = singlePromotion.RetailPrice;
            }

            singlePromotion.startDate = moment(singlePromotion.StartDate).toDate();
            singlePromotion.endDate = moment(singlePromotion.EndDate).toDate();
            singlePromotion.StartDate = singlePromotion.startDate.toISOString();
            singlePromotion.EndDate = singlePromotion.endDate.toISOString();

            singlePromotion.Filters.push({
                name: 'Fascia',
                value: singlePromotion.Fascia || 'All'
            });

            var branchNumber = _.find($scope.MasterData.branches, function (value, key) {
                return key === singlePromotion.BranchNumber;
            });

            singlePromotion.Filters.push({
                name: 'Branch',
                value: branchNumber || ' All'
            });

            singlePromotion.Filters.push({
                name: 'NonStock',
                value: singlePromotion.NonStockNumber,
                nonStockId: singlePromotion.NonStockId,
                nonStockUrl: '/#/NonStocks/Details/' + singlePromotion.NonStockId
            });
        });

        return allPromotions;
    };

    var getPromotions = function (id) {
        $http({
            method: 'GET',
            url: 'NonStocks/Api/Promotions/' + id
        }).success(function (data) {
            if (data) {
                $scope.promotions = $scope.populateExistingPromotions(data.NonStockPromotions);
            }
        });
    };

    if ($routeParams.Id !== 0) {
        getPriceInfo($routeParams.Id);
        getPromotions($routeParams.Id);
    }

    $scope.getNonStockPriceLink = function () {
        if ($scope.formData && $scope.formData.Id) {
            var query = {
                query: $scope.formData.SKU
            };

            var queryStr = JSON.stringify(query);
            return "/#/NonStocks/Search?q=" + queryStr;
        } else {
            return "";
        }
    };

    $scope.hideLength = function (type) {
        if (type !== 'rassist' && type !== 'annual') {
            return true;
        } else {
            return false;
        }
    };
};

nonStockController.$inject = ['$scope', '$http', '$location', '$routeParams',
    '$interval', 'CommonService', 'HierarchyService', 'LookupDataService', 'SettingUtils'];
module.exports = nonStockController;
