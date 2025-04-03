define([
    'angular',
    'underscore',
    'url',
    'moment',
    'jquery'],

    function (angular, _, url, moment, $) {
        'use strict';

        return function ($scope, location, pageHelper, stockCountRepo, user) {
            var userCanStartPerpetualQuarterly = user.hasPermission('StockCountStartPerpetualQuarterly'),
                userCanStartPerpetual = user.hasPermission('StockCountStartPerpetual'),
                userCanVerify = user.hasPermission('StockCountEdit'),
                userCanClosePerpetualQuarterly = user.hasPermission('StockCountClosePerpetualQuarterly'),
                userCanClosePerpetual = user.hasPermission('StockCountClosePerpetual'),
                statuses = {
                    pending: 'Scheduled',
                    inProgress: 'In Progress',
                    closed: 'Closed',
                    cancelled: 'Cancelled'
                },

                types = {
                    Perpetual: 'Perpetual',
                    Quarterly: 'Quarterly'
                };

            function status() {
                if ($scope.stockCount.cancelledById) {
                    return statuses.cancelled;
                }
                if ($scope.stockCount.closedById) {
                    return statuses.closed;
                }
                if ($scope.stockCount.startedById) {
                    return statuses.inProgress;
                }
                return statuses.pending;
            }

            function canStartPerpetualQuarterly() {
                return userCanStartPerpetualQuarterly &&
                    moment().isAfter($scope.stockCount.countDate) &&
                    $scope.status === statuses.pending;
            }

            function canStartPerpetual() {
                return userCanStartPerpetual  &&
                    moment().isAfter($scope.stockCount.countDate) &&
                    $scope.status === statuses.pending &&
                    $scope.stockCount.type === types.Perpetual;
            }

            function canVerify() {
                return userCanVerify &&
                    $scope.status === statuses.inProgress;
            }

            function canClosePerpetualQuarterly() {
                return !$scope.saving &&
                    userCanClosePerpetualQuarterly &&
                    $scope.status === statuses.inProgress;
            }

            function canClosePerpetual() {
                return !$scope.saving &&
                    userCanClosePerpetual &&
                    $scope.status === statuses.inProgress &&
                    $scope.stockCount.type === types.Perpetual;
            }

            function canCancelPerpetualQuarterly() {
                return !$scope.saving &&
                    userCanClosePerpetualQuarterly &&
                    $scope.status !== statuses.cancelled &&
                    $scope.status !== statuses.closed;
            }

            function canCancelPerpetual() {
                return !$scope.saving &&
                    userCanClosePerpetual &&
                    $scope.stockCount.type === types.Perpetual &&
                    $scope.status !== statuses.cancelled &&
                    $scope.status !== statuses.closed;
            }

            function canSave(form) {
                return !$scope.saving &&
                    $scope.status === statuses.inProgress &&
                    form &&
                    form.$valid &&
                    form.$dirty;
            }

            function canPrint() {
                return !$scope.saving &&
                    $scope.status === statuses.inProgress;
            }

            function canPrintVariances() {
                return !$scope.saving &&
                    $scope.status === statuses.inProgress;
            }

            function showClose() {
                return (userCanClosePerpetualQuarterly || (userCanClosePerpetual && $scope.stockCount.type === types.Perpetual)) &&
                    $scope.status === statuses.inProgress;
            }

            function showCancel() {
                return (userCanClosePerpetualQuarterly || (userCanClosePerpetual && $scope.stockCount.type === types.Perpetual)) &&
                    $scope.status !== statuses.cancelled &&
                    $scope.status !== statuses.closed;
            }

            function showSave(form) {
                return $scope.status === statuses.inProgress &&
                    form &&
                    form.$valid &&
                    form.$dirty;
            }

            function showPrint() {
                return $scope.status === statuses.inProgress;
            }

            function showPrintVariances() {
                return $scope.status === statuses.inProgress;
            }

            function canSearch() {
                return $scope.status !== statuses.pending;
            }

            function start() {
                if (canStartPerpetualQuarterly()) {
                    window.location = url.resolve('/Merchandising/StockCountStart/Start/' + $scope.stockCount.id);
                }
                else if (canStartPerpetual()) {
                    window.location = url.resolve('/Merchandising/StockCountStart/StartPerpetual/' + $scope.stockCount.id);
                }
                else {
                    return;
                }
            }
            
            function close(isForcefullyClose) {
                if (!canClosePerpetualQuarterly()) {
                    return;
                }
                save();

                $scope.saving = true;
                stockCountRepo.close($scope.stockCount.id, isForcefullyClose)
                    .then(function (stockCount) {
                        $scope.saving = false;
                        if (angular.isString(stockCount) === false) {
                            $scope.stockCount = stockCount;
                            $scope.status = status();
                        } else {
                            $("#p-msg-text").text(stockCount);
                            $("#mi-modal").modal({ backdrop: 'static', keyboard: false });
                        }
                    }, function (err) {
                        $scope.saving = false;
                        if (err && err.message) {
                            pageHelper.notification.showPersistent(err.message);
                        }
                    });
            }

            $("#modal-btn-yes").on("click", function () {
                close(true);
                $("#mi-modal").modal('hide');
            });

            $("#modal-btn-no").on("click", function () {
                $("#mi-modal").modal('hide');
            });

            function closePerpetual() {
                if (!canClosePerpetual()) {
                    return;
                }
                save();

                $scope.saving = true;

                stockCountRepo.closePerpetual($scope.stockCount.id)
                    .then(function (stockCount) {
                        $scope.stockCount = stockCount;
                        $scope.status = status();
                        $scope.saving = false;
                    }, function (err) {
                        $scope.saving = false;
                        if (err && err.message) {
                            pageHelper.notification.showPersistent(err.message);
                        }
                    });
            }

            function performClose() {
                if (canClosePerpetual()) {
                    closePerpetual();
                }
                else {
                    close(false);
                }
            }

            function cancel() {
                if (!canCancelPerpetualQuarterly()) {
                    return;
                }
                save();

                $scope.saving = true;

                stockCountRepo.cancel($scope.stockCount.id)
                    .then(function (stockCount) {
                        $scope.stockCount = stockCount;
                        $scope.status = status();
                        $scope.saving = false;
                    }, function (err) {
                        $scope.saving = false;
                        if (err && err.message) {
                            pageHelper.notification.showPersistent(err.message);
                        }
                    });
            }

            function cancelPerpetual() {
                if (!canCancelPerpetual()) {
                    return;
                }
                save();

                $scope.saving = true;

                stockCountRepo.cancelPerpetual($scope.stockCount.id)
                    .then(function (stockCount) {
                        $scope.stockCount = stockCount;
                        $scope.status = status();
                        $scope.saving = false;
                    }, function (err) {
                        $scope.saving = false;
                        if (err && err.message) {
                            pageHelper.notification.showPersistent(err.message);
                        }
                    });
            }

            function performCancel() {
                if (canCancelPerpetual()) {
                    cancelPerpetual();
                }
                else {
                    cancel();
                }
            }

            function save(form) {
                if (!canSave(form)) {
                    return;
                }

                $scope.saving = true;

                stockCountRepo.save($scope.paginator.page)
                    .then(function () {
                        $scope.saving = false;
                        $scope.$broadcast('saved');
                    }, function (err) {
                        $scope.saving = false;
                        if (err && err.message) {
                            pageHelper.notification.showPersistent(err.message);
                        }
                    });
            }

            function exportProducts() {
                return url.open('Merchandising/StockCount/Export/' + $scope.stockCount.id);
            }

            function print() {
                if (!canPrint()) {
                    return;
                }
                url.open('/Merchandising/StockCount/Print/' + $scope.stockCount.id);
            }

            function printVariances() {
                if (!canPrintVariances()) {
                    return;
                }

                url.open('/Merchandising/StockCount/PrintVariance/' + $scope.stockCount.id);
            }

            function productSearchSetup() {
                return {
                    placeholder: "Enter a SKU or product description",
                    width: '100%',
                    minimumInputLength: 2,
                    ajax: {
                        url: url.resolve('Merchandising/Products/SearchStockProducts'),
                        dataType: 'json',
                        data: function (term) {
                            return {
                                q: term,
                                rows: 10,
                                locationId: $scope.stockCount.locationId
                            };
                        },
                        results: function (result) {
                            return {
                                results: _.map(result.data, function (doc) {
                                    return {
                                        sku: doc.sku,
                                        description: doc.longDescription,
                                        id: doc.productId,
                                        averageWeightedCost: doc.averageWeightedCost
                                    };
                                })
                            };
                        }
                    },
                    formatResult: function (data) {
                        return "<table class='WarrantyResults'><tr><td><b> " + data.sku + " </b></td><td> : </td><td> " + data.description + "</td></tr></table>";
                    },
                    formatSelection: function (data) {
                        return data.sku;
                    },
                    dropdownCssClass: "productResults",
                    escapeMarkup: function (m) {
                        return m;
                    }
                };
            }

            function processSearchResult(searchResult) {
                if (searchResult) {
                    $scope.productId = searchResult.id;
                    $scope.params.productId = searchResult.id;
                    $scope.searchResult = searchResult;
                    $scope.paginator.get();
                }
            }

            function defaultParams() {
                return {
                    id: $scope.stockCount.id
                };
            }

            function clear() {
                $scope.searchResult = {};
                $scope.paginator.reset();
                $scope.paginator.get();
            }

            $scope.$watch('stockCount', function () {
                $scope.status = status();
            });

            $scope.saving = false;
            $scope.clear = clear;
            $scope.params = {};
            $scope.productSearch = stockCountRepo.productSearch;
            $scope.resolve = url.resolve;
            $scope.canStartPerpetualQuarterly = canStartPerpetualQuarterly;
            $scope.canStartPerpetual = canStartPerpetual;
            $scope.canVerify = canVerify;
            $scope.canClosePerpetualQuarterly = canClosePerpetualQuarterly;
            $scope.canClosePerpetual = canClosePerpetual;
            $scope.canCancelPerpetualQuarterly = canCancelPerpetualQuarterly;
            $scope.canCancelPerpetual = canCancelPerpetual;
            $scope.canSave = canSave;
            $scope.canSearch = canSearch;
            $scope.canPrint = canPrint;
            $scope.canPrintVariances = canPrintVariances;
            $scope.showClose = showClose;
            $scope.showCancel = showCancel;
            $scope.showSave = showSave;
            $scope.showPrint = showPrint;
            $scope.showPrintVariances = showPrintVariances;
            $scope.start = start;
            $scope.close = close;
            $scope.cancel = cancel;
            $scope.save = save;
            $scope.print = print;
            $scope.performClose = performClose;
            $scope.performCancel = performCancel;
            $scope.printVariances = printVariances;
            $scope.productSearchSetup = productSearchSetup;
            $scope.processSearchResult = processSearchResult;
            $scope.defaultParams = defaultParams;
            $scope.exportProducts = exportProducts;
        };
    });