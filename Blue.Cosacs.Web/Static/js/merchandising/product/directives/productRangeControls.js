define([
    'jquery',
    'angular',
    'underscore',
    'url',
    'notification',
    'moment',
    'datepicker',
    'merchandising/product/services/productRangeProvider'

],
    function ($, angular, _, url) {
        'use strict';
        
        return function (user, pageHelper, productRangeProvider, $rootScope) {
            return {
                restrict: 'E',
                scope: {
                    sku:'=',
                    minstock: '=',
                    maxstock: '=',
                    productId: '=',
                    isSpecial: '=',
                    isAshley:'='
                },
                templateUrl: url.resolve('/Static/js/merchandising/product/templates/productRange.html'),
                link: function (scope) {
                    scope.canView = user.hasPermission('StockRangeValueView');
                    scope.canEdit = user.hasPermission('StockRangeValueEdit');

                    productRangeProvider.getAshleyEnabled().then(function (response) {
                        if (response == "True")
                            scope.GetAshleyEnable = true;
                        else
                            scope.GetAshleyEnable = false;
                        //debugger;
                        console.log('Response ' + response);
                    }, function (response) {
                        pageHelper.notification.show(response.message);
                    });

                    console.log('Permission Attr ' + scope.canView);
                    scope.productRangeObj = { 'ProductId': scope.productId, 'SKU': scope.sku, 'MinVal': scope.minstock, 'MaxVal': scope.maxstock};
                    scope.add = function (productRange, productRangeForm) {
                        scope.productRangeObj = { 'ProductId': scope.productId, 'SKU': scope.sku, 'MinVal': scope.minstock, 'MaxVal': scope.maxstock };
                        //if (!scope.isAshley) {
                        //    pageHelper.notification.show("Min-Max rule can only set for ashley produts!!");
                        //    return;
                        //}
                        if (productRangeForm.$invalid) {
                            pageHelper.notification.show("Please fill minimum and maximum range for product properly!!");
                            return;
                        }
                        if (parseInt(scope.productRangeObj.MaxVal) <= 0 || parseInt(scope.productRangeObj.MinVal) <= 0) {
                            pageHelper.notification.show("Minimum and Maximum value must be greater than zero!!");
                            return;
                        }
                        if (parseInt(scope.productRangeObj.MaxVal) < parseInt(scope.productRangeObj.MinVal)) {
                            pageHelper.notification.show("Minimum value could not be more than maximum value!!");
                            return;
                        }
                        

                        return productRangeProvider
                            .save(scope.productRangeObj)
                            .then(function (response) {
                                scope.editing = false;
                                //scope.productRange = { id: 0, productId: scope.productId, minStock: scope.productRange.minVal, maxStock: scope.productRange.maxVal };

                                //scope.productRange.push(response);

                                //taxHelper.refreshTaxRate();
                                pageHelper.notification.show('Product range saved successfully');

                            }, function (response) {
                                pageHelper.notification.show(response.message);
                            });

                        return null;
                    };
                }
            };
        };
    });