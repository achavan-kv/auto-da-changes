'use strict';
var humanize = require('../core/humanize');

var humanizeNumber = function () {
    return function (input) {
        return humanize.compactInteger(input, 2);
    };
};

//humanizeNumber.$inject = ['humanize']

module.exports = humanizeNumber;