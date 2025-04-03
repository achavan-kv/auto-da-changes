/*global _, moment, module */
var productLinkController = function ($scope, $http, $dialog, $filter, $q,
                                      $anchorScroll, $location, hierarchyService, CommonService) {
    'use strict';

    $scope.hasEditLinkPermission = true;
    $scope.hasViewLinkPermission = true;
    $scope.formData = {
        searchName: "",
        searchDateFrom: "",
        searchDateTo: "",
        linkLevelNames: ['Level_1', 'Level_2', 'Level_3', 'Level_4', 'Level_5'],
        allLinks: [],
        links: [],
        allProducts: [],
        allProductsScope: null,
        newProductLink: { },
        newNonStock: { }
    };

    var templates = function () {
        var linkTemplate = {
            Id: 0,
            Name: null,
            EffectiveDate: null,
            linkProducts: [],
            linkNonStocks: []
        };
        var productLinkTemplate = {
            Id: null,
            LinkId: null,
            Level_1: null,
            Level_2: null,
            Level_3: null,
            Level_4: null,
            Level_5: null,
            Order: null
        };
        var nonStockTemplate = {
            Id: null,
            LinkId: null,
            NonStockId: null,
            Order: null,
            NonStockObj: {}
        };
        var searchFilter = {
            Name: '',
            dateFromObj: {},
            DateFrom: '',
            dateToObj: {},
            DateTo: '',
            PageSize: 5,
            PageCount: -1,
            PageIndex: 1
        };

        return {
            getLinkTemplate: function () {
                // assign clones the object (just like using extend)
                return _.assign({}, linkTemplate);
            },
            getProductLink: function () {
                // assign clones the object (just like using extend)
                return _.assign({}, productLinkTemplate);
            },
            getNonStock: function () {
                // assign clones the object (just like using extend)
                return _.assign({}, nonStockTemplate);
            },
            getSearchFilter: function () {
                // assign clones the object (just like using extend)
                return _.assign({}, searchFilter);
            },
            getSearchFilterPath: function (optionalSearchObj) {
                if (!optionalSearchObj) {
                    optionalSearchObj = this.getSearchFilter();
                }
                return 'Name=' + encodeURIComponent(optionalSearchObj.Name) +
                    '&DateFrom=' + encodeURIComponent(optionalSearchObj.DateFrom) +
                    '&DateTo=' + encodeURIComponent(optionalSearchObj.DateTo) +
                    '&PageSize=' + encodeURIComponent(optionalSearchObj.PageSize) +
                    '&PageIndex=' + encodeURIComponent(optionalSearchObj.PageIndex);
            },
            initLinkTemplate: function (linkTemplate) {
                if (linkTemplate) {
                    linkTemplate.Id = 0;
                    linkTemplate.Name = '';
                    linkTemplate.EffectiveDate = (moment(new Date()).add(1, 'days')).toISOString();
                    linkTemplate.effectiveDate = parseDate(linkTemplate.EffectiveDate);
                    linkTemplate.linkProducts = [];
                    linkTemplate.linkNonStocks = [];
                }
                return linkTemplate;
            }
        };
    }();

    var growlTimeout = 3;

    $scope.filter = $location.search();
    if (parseInt($scope.filter.PageIndex, 10) !== 1) {
        // fixes: cannot load specific page indexes
        $location.search('PageIndex', 1);
    }
    if (_.isUndefined($scope.filter.Name) || _.isUndefined($scope.filter.DateFrom) ||
        _.isUndefined($scope.filter.DateTo)) { // The filter wasn't initialized yet
        $location.search(templates.getSearchFilterPath());
    } else {
        if (!_.isUndefined($scope.filter.DateTo) && $scope.filter.DateTo.length > 10) {
            $scope.filter.dateToObj = moment($scope.filter.DateTo).toDate();
        }
        if (!_.isUndefined($scope.filter.DateFrom) && $scope.filter.DateFrom.length > 10) {
            $scope.filter.dateFromObj = moment($scope.filter.DateFrom).toDate();
        }
    }

    var findDateSeparator = function (dateString) {
        var separators = ['-', '/'],
            foundSeparator = '-',
            i;
        for (i = 0; i < separators.length; i++) {
            if (_.indexOf(separators[i], dateString) > 0) {
                foundSeparator = separators[i];
            }
        }
        return foundSeparator;
    };

    // Removes the time component from the object (just leaves the date)
    var parseDate = function (dateString) {
        if (dateString && typeof dateString.getMonth === 'function' &&
            dateString instanceof Date) {
            return parseDate(dateString.toISOString());
        }

        if ('undefined' === typeof dateString || '' === dateString) {
            return null;
        }

        if (dateString.length > 10) {
            var indexOfT = dateString.indexOf('T');
            if (indexOfT >= 10) {
                dateString = dateString.split('T')[0];
            }
        }

        var separator = findDateSeparator(dateString);
        var parts = dateString.split(separator);
        if (parts.length !== 3) {
            return null;
        }

        var year = parseInt(parts[0], 10),
            month = parseInt(parts[1], 10),
            day = parseInt(parts[2], 10);

        if (month < 1 || year < 1 || day < 1) {
            return null;
        }

        return new Date(year, (month - 1), day);
    };

    $scope.isEffectiveDateInvalid = function (linkElement) {
        if (!linkElement.effectiveDate) {
            return false; // An undefined or null date is not invalid
        }
        var guiCurrentEffectiveDate = parseDate(linkElement.effectiveDate);
        if (guiCurrentEffectiveDate && typeof guiCurrentEffectiveDate.getMonth === 'function' &&
            guiCurrentEffectiveDate instanceof Date) {
            if (moment(linkElement.EffectiveDate).format() !== moment(guiCurrentEffectiveDate).format()) {
                return isLinkEffectiveDateInThePast(linkElement, 1);
            } else {
                return false;
            }
        }
        return true;
    };

    var isLinkEffectiveDateInThePast = function (linkElement, dayOffset) {
        var oneDayInMillisecondEpoch = 86400000,
            todayDateValueEpoch = +(parseDate(new Date())),
            dateObj = parseDate(linkElement.effectiveDate),
            dateToTestEpoch = +(dateObj);

        return (todayDateValueEpoch + (dayOffset * oneDayInMillisecondEpoch)) > dateToTestEpoch;
    };

    var detectLevelsNumber = function (scope) {
        var levelsNumber = 3,
            i;
        if (scope.formData.allLevels && scope.formData.allLevels.length > 0) {
            levelsNumber = scope.formData.allLevels.length;
        }
        for (i = 5; i > levelsNumber; i--) {
            if (scope.formData.linkLevelNames.length >= i) {
                scope.formData.linkLevelNames.splice(i - 1, 1);
            }
        }
    };

    // dynamically deleted number of levels
    $scope.$watch(function (scope) {
            return scope.formData.allLevels;
        },
        function () {
            if ($scope.formData && typeof $scope.formData.ErrorLoadingHierarchy === 'boolean' &&
                !$scope.formData.ErrorLoadingHierarchy) {
                detectLevelsNumber($scope);
            }
        });

    var sortSubLinksAfterLoad = function (linkElement) {
        var filterFunc = $filter('orderBy');

        if (linkElement && linkElement.linkProducts &&
            linkElement.linkProducts.length > 0) {
            linkElement.linkProducts = filterFunc(linkElement.linkProducts, 'Order');
        }

        if (linkElement && linkElement.linkNonStocks &&
            linkElement.linkNonStocks.length > 0) {
            linkElement.linkNonStocks = filterFunc(linkElement.linkNonStocks, 'Order');
        }

        return linkElement;
    };

    var sortLinksDescendingAfterLoad = function (loadedLinks) {
        var filterFunc = $filter('orderBy');

        if (loadedLinks && loadedLinks.length > 0) {
            return filterFunc(loadedLinks, 'Id', true);
        }

        return [];
    };

    var readDateScopeObject = function (dateObj) {
        return dateObj && typeof dateObj.getMonth === 'function' ?
            parseDate(dateObj).toISOString() : '';
    };

    $scope.search = function () {
        var newFilter = templates.getSearchFilter();
        newFilter.Name = $scope.filter.Name || '';
        newFilter.DateFrom = readDateScopeObject($scope.filter.dateFromObj);
        newFilter.DateTo = readDateScopeObject($scope.filter.dateToObj);
        if (!_.isUndefined($scope.filter.PageIndex)) {
            newFilter.PageIndex = $scope.filter.PageIndex;
        }

        var newSearch = templates.getSearchFilterPath(newFilter);
        $http({
            method: "GET",
            url: '/NonStocks/Api/ProductLink/LoadAllLinks?' + newSearch,
            data: JSON.stringify(newFilter)
        })
            .then(function (response) {
                if (response && response.data && response.data.Links) {
                    response.data.Links =
                        _.map(response.data.Links, function (link) {
                            link.effectiveDate = parseDate(link.EffectiveDate);
                            if (link.linkProducts || link.linkNonStocks) {
                                // enforcing the link order on the angular gui
                                sortSubLinksAfterLoad(link);
                            }
                            return link;
                        });

                    _.extend($scope.filter, {
                        PageCount: parseInt(response.data.PageCount, 10),
                        PageIndex: parseInt(response.data.PageIndex, 10),
                        RecordCount: parseInt(response.data.RecordCount, 10)
                    });

                    $scope.formData.links = sortLinksDescendingAfterLoad(response.data.Links);
                }
            })
            .then(function () {
                // check first for errors or if levels aren't loaded
                if ($scope.formData && (
                        $scope.formData.ErrorLoadingHierarchy || // error
                        !$scope.formData.allLevels // levels not loaded
                    )) {
                    hierarchyService.getHierarchy($scope);
                }
            })
            .then(function () {
                ngHelpers.getProdLinkCol();
                ngHelpers.getProdLinkRest();
            })
            .then(function () {
                // check first if non-stocks (allProductsScope) are already loaded
                if ($scope.formData && !$scope.formData.allProductsScope) {
                    $http({ // Load all products
                        url: '/NonStocks/Api/NonStock/LoadAll',
                        method: "GET"
                    })
                        .then(function (response) {
                            if (response && response.data && response.data.NonStocks) {
                                $scope.formData.allProducts = _.filter(response.data.NonStocks, function(nonStock){
                                    return nonStock.Type !== "rassist";
                                });
                                var tmpAllProdScope = {};
                                _.map($scope.formData.allProducts, function (nonStock) {
                                    tmpAllProdScope[nonStock.Id] =
                                        nonStock.SKU + ' - ' +
                                        nonStock.ShortDescription + ' ' +
                                        nonStock.LongDescription;
                                });
                                $scope.formData.allProductsScope = tmpAllProdScope;
                            }
                        });
                }
            });
    };

    $scope.search();

    var resetFormDataLevelsGuiVals = function () {
        if (_.isArray($scope.formData.levels)) {
            var i;
            for (i = 0; i < $scope.formData.levels.length; i++) {
                delete $scope.formData.levels[i].val;
            }
        }
    };

    var clearProductLinkEditing = function () {
        resetFormDataLevelsGuiVals();
        $scope.formData.newProductLink = templates.getProductLink();
    };

    var clearNonStockEditing = function () {
        $scope.formData.newNonStock = templates.getNonStock();
    };

    clearProductLinkEditing();
    clearNonStockEditing();

    var ngHelpers = function () {
        var totalColumnsAvailable = 11;
        $scope.helpers = {};

        var execProdLinkCol = function () {
            var deferredPromise = $q.defer();
            setInterval(function () {
                if (_.isArray($scope.formData.levels)) {
                    var numOfLevels = $scope.formData.levels.length;
                    var colSpaceAvailable = Math.floor(totalColumnsAvailable / numOfLevels);
                    var colRest = (totalColumnsAvailable - (colSpaceAvailable * numOfLevels));
                    if (colRest === 0) {
                        deferredPromise.resolve(colSpaceAvailable - 1);
                    } else {
                        deferredPromise.resolve(colSpaceAvailable);
                    }
                } else {
                    deferredPromise.reject('Levels not loaded yet.');
                }
            }, 500);

            return deferredPromise.promise;
        };
        var execProdLinkRest = function () {
            var deferredPromise = $q.defer();
            setInterval(function () {
                if (_.isArray($scope.formData.levels)) {
                    var numOfLevels = $scope.formData.levels.length;
                    var colSpaceAvailable = Math.floor(totalColumnsAvailable / numOfLevels);
                    var colRest = (totalColumnsAvailable - (colSpaceAvailable * numOfLevels));
                    if (colRest === 0) {
                        deferredPromise.resolve(numOfLevels);
                    } else {
                        deferredPromise.resolve(colRest);
                    }
                } else {
                    deferredPromise.reject('Levels not loaded yet.');
                }
            }, 500);

            return deferredPromise.promise;
        };

        return {
            getProdLinkCol: function () {
                return execProdLinkCol()
                    .then(function (result) {
                        $scope.helpers.prodLinkCol = result;
                    },
                    function (error) {
                        $scope.helpers.error = error;
                        $scope.helpers.prodLinkCol = 3;
                    });
            },
            getProdLinkRest: function () {
                return execProdLinkRest()
                    .then(function (result) {
                        $scope.helpers.prodLinkRest = result;
                    },
                    function (error) {
                        $scope.helpers.error = error;
                        $scope.helpers.prodLinkRest = 3;
                    });
            }
        };
    }();
    $scope.ngHelpers = ngHelpers;
    $scope.helpers.prodLinkCol = 3;
    $scope.helpers.prodLinkRest = 1;

    var getSelectedProductLinks = function () {
        var retSelectedLinks = [],
            selectedLink = $scope.formData.levels;

        if (selectedLink) {
            var linksLength = selectedLink.length,
                i;
            if (linksLength > 0) {
                for (i = 0; i < linksLength ; i++) {
                    var tmpLink = selectedLink[i].val;
                    retSelectedLinks[i] = null; // init link placeholder
                    if (tmpLink) {
                        retSelectedLinks[i] = tmpLink;
                    }
                }
            }
        }
        return retSelectedLinks;
    };

    $scope.isNewNonStockValid = function () {
        var newNonStock = $scope.formData.newNonStock;
        return newNonStock.NonStockId > 0;
    };

    $scope.getInvalidLinkText = function (productItem, levelName) {
        if (productItem && productItem[levelName]) {
            var invalidLink = productItem[levelName] || '';
            invalidLink = invalidLink.trim();
            if (invalidLink.length >= 1) {
                return invalidLink;
            }
        }
        return "No Value";
    };

    var getNonStockObj = function (nonStockId) {
        var retNonStock = null;
        if (nonStockId > 0 && $scope.formData.allProducts.length > 0) {
            retNonStock =
                _.find($scope.formData.allProducts, function (prod) {
                    return prod.Id === Number.parseInt(nonStockId);
                });
        }
        return retNonStock;
    };

    $scope.addingNewNonStockProductLink = false;
    $scope.addNewNonStockProductLink = function () {
        if ($scope.formData && _.isArray($scope.formData.links)) {
            if (!$scope.addingNewNonStockProductLink) {
                var newProductLink = templates.getLinkTemplate();
                newProductLink = templates.initLinkTemplate(newProductLink);

                $scope.startEditDetails(newProductLink);
                $scope.formData.links.splice(0, 0, newProductLink); // append to top of array

                $scope.addingNewNonStockProductLink = true; // start adding link workflow
            }
        }
    };

    var getNonStockLevel = function (id, linkElement) {
        $http.get('/NonStocks/Api/ProductLink/' + id)
            .success(function (response) {
                if (response && response.Link) {
                    //var newNonStockLevel = mapNonStockLevels([response])[0];
                    var indexNS = _.indexOf($scope.formData.links, linkElement);
                    if (indexNS !== -1) {
                        //populateNonStockProductData(newNonStockLevel);
                        var newLink = sortSubLinksAfterLoad(response.Link);
                        newLink.effectiveDate = parseDate(newLink.EffectiveDate);
                        $scope.formData.links.splice(indexNS, 1, newLink);
                    }
                }
            });
    };

    $scope.stopEditingLink = function (index, linkElement) {
        if (linkElement && linkElement.Id > 0) {
            getNonStockLevel(linkElement.Id, linkElement);
        } else {
            $scope.discardNonStockProductLink();
        }
    };

    $scope.discardNonStockProductLink = function () {
        if ($scope.formData && _.isArray($scope.formData.links)) {
            if ($scope.addingNewNonStockProductLink) {
                // will always discard the first element on the links array
                $scope.formData.links.splice(0, 1);
                $scope.addingNewNonStockProductLink = false; // finish adding link workflow
            }
        }
    };

    $scope.removeLinkElement = function (index, linkElement) {
        if (linkElement.Id && !linkElement.edit) {
            var confirmDel = $dialog.messageBox('Confirm Delete',
                'You have chosen to delete the non-stock link "' + linkElement.Name +
                '". Are you sure you want to proceed?', [{
                    cssClass: 'btn-primary',
                    label: 'Delete',
                    result: 'yes'
                }, {
                    label: 'Cancel',
                    result: 'no'
                }
                ]);
            confirmDel.open().then(function (result) {
                if (result === 'yes') {
                    $http({
                        method: 'DELETE',
                        url: '/NonStocks/Api/ProductLink/' + linkElement.Id,
                        data: linkElement.Id
                    })
                        .success(function (response) {
                            if (response && response.Result === 'Done') {
                                var tmpLinkElement = _.find($scope.formData.links, function (link) {
                                    return link.Id === response.Id;
                                });
                                var indexNS = _.indexOf($scope.formData.links, tmpLinkElement);
                                if (indexNS !== -1) {
                                    $scope.formData.links.splice(indexNS, 1);
                                }
                                $scope.search();

                                CommonService.addGrowl({
                                    timeout: growlTimeout,
                                    type: 'info', // (optional - info, danger, success, warning)
                                    content: 'NonStock Product link deleted'
                                });
                            }
                        });
                }
            });
        }
    };

    $scope.startEditDetails = function (linkElement) {
        var today = new Date();
        var tomorrow = new Date(today);
        tomorrow.setDate(today.getDate()+1);

        linkElement.productLinkEdit = true;
        linkElement.nonStocksEdit = true;
        linkElement.effectiveDate = tomorrow;
    };

    $scope.toggleProductLinkEdit = function (linkElement, toggleToState) {
        if (typeof toggleToState !== 'boolean') {
            linkElement.addingProductLink = !linkElement.addingProductLink;
        } else {
            linkElement.addingProductLink = toggleToState;
        }

        clearProductLinkEditing();
    };

    $scope.toggleNonStocksEdit = function (linkElement, toggleToState) {
        if (typeof toggleToState !== 'boolean') {
            linkElement.addingNonStocks = !linkElement.addingNonStocks;
        } else {
            linkElement.addingNonStocks = toggleToState;
        }

        clearNonStockEditing();
    };

    $scope.saveLink = function (linkElement) {
        // check if the product link and the non-stocks edit is saved

        if ($scope.isProductLinkValid(linkElement) &&
            !linkElement.addingProductLink && !linkElement.addingNonStocks) {

            $http({
                method: 'POST',
                url: '/NonStocks/Api/ProductLink',
                data: JSON.stringify(linkElement)
            })
                .success(function (response) {
                    if (response && response.SavedLink) {
                        var newLink = sortSubLinksAfterLoad(response.SavedLink);
                        fillLinkNonStockObjects(newLink);
                        newLink.effectiveDate = parseDate(newLink.EffectiveDate);

                        $scope.formData.links.splice(0, 1, newLink);
                        $scope.addingNewNonStockProductLink = false; // finish adding link workflow

                        CommonService.addGrowl({
                            timeout: growlTimeout,
                            type: 'info', // (optional - info, danger, success, warning)
                            content: 'Non-stock product link saved'
                        });

                    }
                });
        } else {
            var errorMessage = '';

            if ($scope.isEffectiveDateInvalid(linkElement)) {
                errorMessage = "The link's EffectiveDate can only be created/saved with a value " +
                    "bigger that today's date (please choose the dates tomorrow or after).";
                return emitGrowlMessage(errorMessage, 9);
            }

            if (linkElement && linkElement.linkProducts && linkElement.linkProducts.length === 0) {
                errorMessage = 'Please create at least one valid product link.';
                return emitGrowlMessage(errorMessage);
            }

            if (linkElement && linkElement.linkNonStocks && linkElement.linkNonStocks.length === 0) {
                errorMessage = 'Please select at least one valid non-stock item.';
                return emitGrowlMessage(errorMessage);
            }

            if (linkElement && linkElement.Name && linkElement.Name.length <= 0) {
                errorMessage = 'Please select a valid name.';
                return emitGrowlMessage(errorMessage);
            }

            if (linkElement.EffectiveDate) {
                errorMessage = 'Please select a valid date.';
                return emitGrowlMessage(errorMessage);
            }
        }
    };

    var fillLinkNonStockObjects = function (newLink) {
        newLink.linkNonStocks =
            _.map(newLink.linkNonStocks, function (prodLink) {
                var changedProdLink = prodLink;
                changedProdLink.NonStockObj =
                    getNonStockObj(prodLink.NonStockId);
                return changedProdLink;
            });
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

    $scope.saveNewProductLink = function (linkElement) {
        if (!linkElement.linkProducts) {
            linkElement.linkProducts = [];
        }

        var tmpLinkId = linkElement.Id,
            tmpLevels = getSelectedProductLinks(),
            newProdLinkClone = templates.getProductLink();

        newProdLinkClone.Id = null;
        newProdLinkClone.LinkId = tmpLinkId;
        newProdLinkClone.Level_1 = tmpLevels[0] || null;
        newProdLinkClone.Level_2 = tmpLevels[1] || null;
        newProdLinkClone.Level_3 = tmpLevels[2] || null;
        newProdLinkClone.Level_4 = tmpLevels[3] || null;
        newProdLinkClone.Level_5 = tmpLevels[4] || null;
        newProdLinkClone.Order = null;

        if (!checkIfLinkProductsContainsElement(linkElement.linkProducts, newProdLinkClone)) {
            linkElement.linkProducts.push(newProdLinkClone);
        } else {
            warnAboutDuplicateSelections();
        }
        $scope.toggleProductLinkEdit(linkElement);

    };
    $scope.saveNewNonStock = function (linkElement) {
        if (!linkElement.linkNonStocks) {
            linkElement.linkNonStocks = [];
        }
        var tmpLinkId = linkElement.Id,
            tmpNonStockId = $scope.formData.newNonStock.NonStockId,
            tmpNonStockObj = getNonStockObj(tmpNonStockId);

        if ($scope.isNewNonStockValid()) {
            var newNonStockClone = templates.getNonStock();
            newNonStockClone.Id = 0; // defined only after DB save
            newNonStockClone.LinkId = tmpLinkId;
            newNonStockClone.NonStockId = tmpNonStockId;
            newNonStockClone.Order = null;
            newNonStockClone.NonStockObj = tmpNonStockObj;

            if (!checkIfLinkNonStocksContainsElement(linkElement.linkNonStocks, newNonStockClone)) {
                linkElement.linkNonStocks.push(newNonStockClone);
            } else {
                warnAboutDuplicateSelections();
            }

            $scope.toggleNonStocksEdit(linkElement);
        }
    };

    var warnAboutDuplicateSelections = function () {
        var resultOk = function (result) {
            if (result === 'Ok') {
                // all done
                return null;
            }
            return null;
        };

        var warnDuplicates = $dialog.messageBox('Duplicate selection',
            'Because the element already exists, it will not be added', [{
                cssClass: 'btn-primary',
                label: 'Ok',
                result: 'Ok'
            }]);
        warnDuplicates.open().then(resultOk);
    };

    var checkIfLinkNonStocksContainsElement = function (linkNonStocks, element) {
        var testElem = _.filter(linkNonStocks, function (link) {
            return parseInt(link.LinkId, 10) === parseInt(element.LinkId, 10) &&
                parseInt(link.NonStockId, 10) === parseInt(element.NonStockId, 10);
        });
        return testElem.length > 0;
    };

    var checkIfLinkProductsContainsElement = function (linkProducts, element) {
        var testElem = _.filter(linkProducts, function (link) {
            return parseInt(link.LinkId, 10) === parseInt(element.LinkId, 10) &&
                link.Level_1 === element.Level_1 &&
                link.Level_2 === element.Level_2 &&
                link.Level_3 === element.Level_3 &&
                link.Level_4 === element.Level_4 &&
                link.Level_5 === element.Level_5;
        });
        return testElem.length > 0;
    };

    $scope.removeLinkProduct = function (index, productItem, linkElement) {
        linkElement.linkProducts.splice(index, 1);
    };

    $scope.removeNonStock = function (index, nonStocksItem, linkElement) {
        linkElement.linkNonStocks.splice(index, 1);
    };

    $scope.isProductLinkValid = function (linkElement) {
        return !$scope.isEffectiveDateInvalid(linkElement) &&
            linkElement.linkProducts.length > 0 &&
            linkElement.linkNonStocks.length > 0 &&
            linkElement.Name.length > 0 &&
            linkElement.EffectiveDate;
    };

    $scope.clearSearchForm = function () {
        //$scope.filter = _.omit($scope.filter, 'Name', 'DateFrom', 'DateTo');
        $scope.filter.Name = '';
        $scope.filter.DateFrom = '';
        $scope.filter.dateFromObj = '';
        $scope.filter.DateTo = '';
        $scope.filter.dateToObj = '';

        // fixes: cannot load specific page indexes
        $scope.filter.PageIndex = 1;

        // resets the search string
        $location.search(templates.getSearchFilterPath($scope.filter));
        $scope.search();
    };

    $scope.selectPage = function (page) {
        $scope.filter.PageIndex = page;
        $scope.search();
    };
};

productLinkController.$inject = ['$scope', '$http', '$dialog', '$filter', '$q',
    '$anchorScroll', '$location', 'HierarchyService', 'CommonService'];
module.exports = productLinkController;
