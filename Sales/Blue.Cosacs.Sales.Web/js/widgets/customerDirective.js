'use strict';

function customerDirective() {

    return {
        restrict: 'EA',
        controller: 'CustomerController',
        transclude: false,
        replace: true,
        scope: {
            customer: '=', 
            isCustomerDetailsLocked: '=',
            basketChangesAllowed: "=",
            isCustomerDataRequired:"="
        },
        templateUrl: 'Sales/views/templates/customer.html'
    };
}

module.exports = customerDirective;