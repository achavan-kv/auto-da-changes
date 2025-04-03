'use strict';

var deps = ['ngRoute', 'glaucous.portal', 'Payments.controllers', 'Payments.services', 'Payments.filters', 'Payments.directives'];

// Create your app
var Payments = angular.module('Payments', deps);

require('./services');
require('./filters');
require('./controllers');
require('./directives');

module.exports = Payments;
