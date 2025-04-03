'use strict';

angular.module('Sales.widgets', [])
    .directive('bbPayment', require('./paymentDirective'))
    .directive('bbCustomer', require('./customerDirective'));
