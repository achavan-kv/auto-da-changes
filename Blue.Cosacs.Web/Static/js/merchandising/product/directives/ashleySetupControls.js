

define([
    'jquery',
    'angular',
    'underscore',
    'url',
    'notification',
    'moment',
    'datepicker',
    'merchandising/product/services/ashleySetupProvider'
],
    function ($, angular, _, url) {
        'use strict';

        return function (user, pageHelper, ashleySetupProvider, $rootScope) {
            return {
                restrict: 'E',
                scope: {
                    isAshley: '=',
                    isSpecial: '=',
                    isAutoPo: '=',
                    isOnline: '=',
                    productId: '=',
                    sku: '='
                },
                templateUrl: url.resolve('/Static/js/merchandising/product/templates/AshleySetup.html'),
                link: function (scope) {
                    scope.canView = user.hasPermission('ProductAttributesView'); //Need to replace Test
                    scope.canEdit = user.hasPermission('ProductAttributesEdit');
                    //scope.ashleySetupObj = { 'ProductId': productId,'minstock': 0, 'maxstock': 0, 'SKU': sku, 'isAshleyProduct': isAshley, 'IsSpecialProduct': isSpecial, 'IsAutoPO': IsAutoPO, 'IsOnlineAvailable': isOnline };
                    scope.ashleySetupObj = { 'ProductId': scope.productId, 'SKU': scope.sku, 'isAshleyProduct': scope.isAshley, 'IsSpecialProduct': scope.isSpecial, 'IsAutoPO': scope.isAutoPo, 'IsOnlineAvailable': scope.isOnline };

                    function replaceNullValues(v, d) {
                        if (v == null && d == 'bool') {
                            return false;
                        }
                        if (v == null && d == 'int') {
                            return 0;
                        }
                        if (v == null && d == 'string') {
                            return '';
                        }
                        return v;
                    }

                    scope.CheckAttributesIsAshley = function (value) {
                        if (!value) {
                            scope.isAutoPo = value;
                            scope.isSpecial = value;
                            //alert('CheckAttributesIsAshley');
                        }
                    }

                    scope.CheckAttributesIsSpecial = function (value) {
                        if (value) {
                            scope.isAshley = value;
                            scope.isAutoPo = true;
                            scope.isAutoPo = value;
                            //alert('CheckAttributesIsSpecial');
                        }
                    }

                    scope.CheckAttributesIsAutoPo = function (value) {
                        if (value) {
                            scope.isAshley = value;
                            //alert('CheckAttributesIsAutoPo');
                        }
                    }

                    scope.isAshley = replaceNullValues(scope.isAshley, 'bool');
                    scope.isSpecial = replaceNullValues(scope.isSpecial, 'bool');
                    scope.isAutoPo = replaceNullValues(scope.isAutoPo, 'bool');

                    ashleySetupProvider.getAshleyEnabled().then(function (response) {
                        if (response == "True")
                            scope.GetAshleyEnable = true;
                        else
                            scope.GetAshleyEnable = false;
                        console.log('Response ' + response);
                    }, function (response) {
                        pageHelper.notification.show(response.message);
                    });

                    scope.add = function (ashleySetupAttr, ashleySetupForm) {
                        scope.ashleySetupObj = {
                            'ProductId': replaceNullValues(scope.productId, 'int'),
                            'SKU': replaceNullValues(scope.sku, 'string'),
                            'isAshleyProduct': replaceNullValues(scope.isAshley, 'bool'),
                            'IsSpecialProduct': replaceNullValues(scope.isSpecial, 'bool'),
                            'IsAutoPO': replaceNullValues(scope.isAutoPo, 'bool'),

                            'IsOnlineAvailable': replaceNullValues(scope.isOnline, 'bool')
                        };
                        if (ashleySetupForm.$invalid) {
                            pageHelper.notification.show("Please fill data properly!!");
                            return;
                        }
                        //if (!scope.ashleySetupObj.isAshleyProduct) {
                        //    pageHelper.notification.show("Please tick Ashley product, to save product attribute!!");
                        //    return;
                        //}
                        //if ((!scope.ashleySetupObj.isAshleyProduct && !scope.ashleySetupObj.IsSpecialProduct && !scope.ashleySetupObj.IsAutoPO && !scope.ashleySetupObj.IsOnlineAvailable)) {
                        //    pageHelper.notification.show("Please check atleast one attribute to save!!");
                        //    return;
                        //}
                        return ashleySetupProvider
                            .save(scope.ashleySetupObj)
                            .then(function (response) {
                                scope.editing = false;
                                pageHelper.notification.show('Product attribute saved successfully');

                            }, function (response) {
                                pageHelper.notification.show(response.message);
                            });

                        return null;
                    };
                }
            };
        };
    });