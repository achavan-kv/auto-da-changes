'use strict';

//require('./filters');
require('./directives');
require('./controllers');
require('./services');
require('ngCropper');

var deps = [
    'ngCropper',
    'Credit.services',
    //'Credit.filters',
    'Credit.directives',
    'Credit.controllers'
];

var credit = angular.module('Credit', deps);
module.exports = credit;
