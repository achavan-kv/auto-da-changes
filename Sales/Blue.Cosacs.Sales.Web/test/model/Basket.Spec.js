'use strict';
var Basket     = require('../../js/model/Basket.js'),
    CartItem   = require('../../js/model/CartItem.js'),
    SalesEnums = require('../../js/model/SalesEnums');
var basket;

describe('Basket Model', function () {

    beforeEach(function () {
        basket = new Basket();
    });

    afterEach(function () {
        basket = undefined;
    });

    describe("Basket's Customer", function () {

        it('should not require customer details by default', function () {
            var required = basket.isCustomerDataRequired;

            expect(required).toBe(false);
        });

        it('should require customer details when warranty is being purchased', function () {
            basket.installationBeingPurchased = true;
            basket.WarrantyBeingPurchased = false;

            var required = basket.isCustomerDataRequired;

            expect(required).toBe(true);
        });

        it('should require customer details when an installation is being purchased', function () {
            basket.installationBeingPurchased = true;

            var required = basket.isCustomerDataRequired;

            expect(required).toBe(true);
        });

        it('should require customer details when an item is being returned/exchanged', function () {
            basket.itemBeingReturned = true;

            var required = basket.isCustomerDataRequired;

            expect(required).toBe(true);
        });

        it('should not flag customer details as missing when no warranty being purchased', function () {
            basket.WarrantyBeingPurchased = false;

            var missing = basket.isCustomerDataRequired;

            expect(missing).toBe(false);
        });

    });

    describe('Function: hasReturnedItems', function () {

        it('should return true if it has return items', function () {
            var options = {
                taxRate: 0,
                taxType: SalesEnums.TaxTypes.Exclusive
            };

            var cartItem = new CartItem(options);

            basket.items.push(cartItem);

            var result = basket.hasReturnedItems();
            expect(result).toBeFalsy();

            var warranty = {returned: true};
            cartItem.warranties.push(warranty);

            result = basket.hasReturnedItems();
            expect(result).toBeTruthy();
        });

    });

    describe('Function: isReturnItemsMissingReason', function () {
        it('should return true if it has return items that is missing Return reason', function () {
            var cartItem = {
                returned: true, returnReason: undefined
            };

            basket.items.push(cartItem);

            var result = basket.isReturnItemsMissingReason();
            expect(result).toBeTruthy();

            cartItem.returnReason = 'anything';

            var result = basket.isReturnItemsMissingReason();
            expect(result).toBeFalsy();

        });

    });

});
