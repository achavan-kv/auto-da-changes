'use strict';

require('./filters');
require('./directives');
require('./controllers');
require('./services');
require('./filters');

var deps = [
    'SalesManagement.services',
    'SalesManagement.filters',
    'SalesManagement.directives',
    'SalesManagement.controllers'
];
var salesManagement = angular.module('SalesManagement', deps);

module.exports = salesManagement;
