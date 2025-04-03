/*global _, module */
var settingUtils = function ($http) {
    'use strict';

    var getCountryTaxRate = function (callback) {
        $http({
            method: 'GET',
            url: '/Cosacs/Merchandising/Tax/GetTaxSettings'
        }).then(function (ret) {
            if (ret && ret.data && ret.data.data) {
                callback(ret.data.data);
            } else {
                throw "Error getting the Country TaxRate from Merchandising.";
            }
        });
    };

    return {
        getCountryTaxRate: getCountryTaxRate
    };
};

settingUtils.$inject = ['$http'];
module.exports = settingUtils;
