/*global _, moment, module */
var jsonUrlUtils = function () {
    'use strict';

    var getEndpointUrl = function () {
        return null;
    };

    return {
        getEndpointUrl: getEndpointUrl
    };
};

jsonUrlUtils.$inject = ['stringUtils'];
module.exports = jsonUrlUtils;
