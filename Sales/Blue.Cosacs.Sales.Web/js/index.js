'use strict';

require('Scope-onReady');

require('./filters');
require('./widgets');
require('./services');
require('./controllers');

var dependencies = ['Scope.onReady', 'Sales.filters', 'Sales.services', 'Sales.controllers', 'Sales.widgets'];

// Create your app
var sales = angular.module('Sales', dependencies);

module.exports = sales;