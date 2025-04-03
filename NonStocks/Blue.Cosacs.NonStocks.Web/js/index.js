'use strict';

var deps = ['ngRoute', 'glaucous.portal', 'NonStocks.controllers', 'NonStocks.services', 'NonStocks.filters', 'NonStocks.directives'];

// Create your app
var nonStocks = angular.module('NonStocks', deps);

require('./services');
require('./filters');
require('./controllers');
require('./directives');

module.exports = nonStocks;
