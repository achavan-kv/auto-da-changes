'use strict';

function paymentDirective() {

    return {
        restrict: 'EA',
        controller: 'PaymentController',
        transclude: false,
        replace: true,
        scope: {
            cart: '=',
            payment: '=',
            user: '=',
            payMethods: '=',
            cardTypes: '=',
            currencyCodes: '=',
            paymentVisible: '='
        },
        templateUrl: 'Sales/views/templates/payment.html'
    };
}

module.exports = paymentDirective;