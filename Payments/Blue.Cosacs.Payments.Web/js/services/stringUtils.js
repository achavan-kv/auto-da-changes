/*global _, module */
var stringUtils = function () {
    'use strict';

    var isNullOrWhitespace = function (obj) {
        return (_.isUndefined(obj) || _.isNull(obj) || (typeof obj === 'string' && obj.trim() === ''));
    };

    return {
        isNullOrWhitespace: isNullOrWhitespace
    };
};

stringUtils.$inject = [];
module.exports = stringUtils;
