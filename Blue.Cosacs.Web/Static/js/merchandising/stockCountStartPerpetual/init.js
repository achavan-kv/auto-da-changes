define([
    'angular',
    'jquery',
    'angularShared/app',
    'merchandising/app',
    'merchandising/stockCountStartPerpetual/controllers/stockCountStartPerpetual',
    'merchandising/stockCountStartPerpetual/services/stockCountStartPerpetualResourceProvider'
],
    function (angular, $, cosacs, merchandising, stockCountCtrl, stockCountRepo) {
        'use strict';

        return {
            init: function ($el) {
                cosacs();

                angular.module('merchandising')
                    .service('stockCountStartPerpetualResourceProvider', ['$q', '$resource', 'apiResourceHelper', stockCountRepo])
                    .controller('stockCountStartPerpetualCtrl', ['$scope', 'pageHelper', 'stockCountStartPerpetualResourceProvider', '$http', stockCountCtrl]);
                return angular.bootstrap($el, ['myApp', 'merchandising']);
            }
        };
    });