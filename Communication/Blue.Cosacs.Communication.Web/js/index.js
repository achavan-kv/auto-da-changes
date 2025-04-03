'use strict';

require('./controllers');

var deps = [
    'Communication.controllers'
];
var communication = angular.module('Communication', deps);

module.exports = communication;