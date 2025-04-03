'use strict';

var CartItem   = require('../../js/model/CartItem'),
    SalesEnums = require('../../js/model/SalesEnums'),
    Utilities  = require('../../js/model/Utilities');
var cartItem, taxExclusivePrice, taxRate, discount, taxInclusivePrice, quantity, dutyFreePrice;

describe('Model: CartItem ', function () {

    beforeEach(function () {
        taxExclusivePrice = chance.floating({fixed: 2, min: 100, max: 10000}),
            dutyFreePrice = chance.floating({fixed: 2, min: 100, max: 10000}),
            taxRate = chance.integer({min: 0, max: 50}),
            discount = chance.floating({fixed: 2, min: 0, max: taxExclusivePrice}),
            quantity = chance.integer({min: 1, max: 50});

        taxExclusivePrice = Utilities.roundFloat(taxExclusivePrice);
        dutyFreePrice = Utilities.roundFloat(dutyFreePrice);
        taxRate = Utilities.roundFloat(taxRate);

        var options = {
            taxRate: 0,
            taxType: SalesEnums.TaxTypes.Exclusive
        };

        cartItem = new CartItem(options);

        cartItem.taxExclusivePrice = taxExclusivePrice;
        cartItem.quantity = quantity;
        cartItem.taxRate = taxRate;
        cartItem.dutyFreePrice = dutyFreePrice;

        taxInclusivePrice = Utilities.includeTax(taxExclusivePrice, taxRate);
    });

    afterEach(function () {
        cartItem = undefined;
    });

    describe('Price Module:', function () {

        it('should set Item prices', function () {
            cartItem.taxType = SalesEnums.TaxTypes.Exclusive;

            expect(cartItem.taxExclusivePrice).toEqual(taxExclusivePrice);
            expect(cartItem.price).toEqual(taxExclusivePrice);

            var expected = Utilities.includeTax(taxExclusivePrice, taxRate);
            expect(cartItem.taxInclusivePrice).toEqual(expected);

            taxInclusivePrice = chance.floating({fixed: 2, min: 100, max: 10000});
            taxInclusivePrice = Utilities.roundFloat(taxInclusivePrice);

            cartItem.taxType = SalesEnums.TaxTypes.Inclusive;
            cartItem.price = taxInclusivePrice;

            expect(cartItem.taxInclusivePrice).toEqual(taxInclusivePrice);

            expected = Utilities.excludeTax(taxInclusivePrice, taxRate);
            expect(cartItem.taxExclusivePrice).toEqual(expected);
            expect(cartItem.price).toEqual(expected);
        });

        it('should set Duty Free prices', function () {
            cartItem.isDutyFree = true;
            cartItem.taxType = SalesEnums.TaxTypes.Exclusive;

            expect(cartItem.dutyFreePriceExclusive).toEqual(dutyFreePrice);
            expect(cartItem.price).toEqual(dutyFreePrice);

            var expected = Utilities.includeTax(dutyFreePrice, taxRate);
            expect(cartItem.dutyFreePriceInclusive).toEqual(expected);

            dutyFreePrice = chance.floating({fixed: 2, min: 100, max: 10000});
            dutyFreePrice = Utilities.roundFloat(dutyFreePrice);

            cartItem.taxType = SalesEnums.TaxTypes.Inclusive;
            cartItem.dutyFreePrice = dutyFreePrice;

            expect(cartItem.dutyFreePriceInclusive).toEqual(dutyFreePrice);

            expected = Utilities.excludeTax(dutyFreePrice, taxRate);
            expect(cartItem.dutyFreePriceExclusive).toEqual(expected);
            expect(cartItem.price).toEqual(expected);
        });

    });

    describe('Payment Totals', function () {


        it('Should Calculate Net Total Item', function () {
            var tax = chance.floating({fixed: 2, min: 0, max: taxExclusivePrice}),
                expected = taxExclusivePrice;

            cartItem.quantity = 1;
            expect(cartItem.ItemNetTotal).toEqual(expected);

            // Factor Item quantity
            cartItem.quantity = quantity;
            expected = Utilities.roundFloat(quantity * taxExclusivePrice);

            expect(cartItem.ItemNetTotal).toEqual(expected);

            cartItem.manualTaxAmount = Utilities.roundFloat(tax);

            // Factor Manual Discount
            cartItem.manualDiscount = discount;
            expected = Utilities.roundFloat((quantity * (taxExclusivePrice + discount)));

            expect(cartItem.ItemNetTotal).toEqual(expected);

        });

        it('Should return Item\'s net tax', function () {
            var tax = (taxRate * 0.01) * (taxExclusivePrice),
                expected = (quantity * tax );

            expect(cartItem.itemTax).toEqual(Utilities.roundFloat(tax));

        });

        it('Should return Item\'s total net tax', function () {
            var tax = Utilities.roundFloat((taxRate * 0.01) * (taxExclusivePrice)),
                expected = Utilities.roundFloat((quantity * tax ));

            cartItem.manualDiscount = 0;

            expect(cartItem.ItemTaxTotal).toEqual(expected);
        });

        it('Should return Item\'s net tax as zero when it is a tax free sale', function () {
            cartItem.isTaxFree = true;

            expect(cartItem.itemTax).toEqual(0);

        });

        it('Should return Item\'s manual tax amount', function () {
            var manualTax = chance.floating({fixed: 2, min: 0, max: taxExclusivePrice});

            cartItem.manualTaxAmount = manualTax;

            expect(cartItem.itemTax).toEqual(manualTax);
            expect(cartItem.taxAmount).toEqual(manualTax);

        });

        it('Should Calculate Item\'s Total Tax When Tax type is Exclusive', function () {
            var tax = Utilities.roundFloat((taxRate * 0.01) * (taxExclusivePrice + discount)),
                expected = (quantity * tax );

            cartItem.manualDiscount = discount;
            expected = Utilities.roundFloat(expected);

            expect(cartItem.ItemTaxTotal).toEqual(expected);

        });

        it('Should Calculate Item\'s Total Tax When Tax type is Inclusive', function () {
            var tax = (taxInclusivePrice - taxExclusivePrice),
                expected = (quantity * tax );

            cartItem.manualDiscount = 0;

            cartItem.taxType = SalesEnums.TaxTypes.Inclusive;

            expect(Utilities.roundFloat(cartItem.ItemTaxTotal)).toEqual(Utilities.roundFloat(expected));

        });

        it('Should Calculate Grand Total Item', function () {
            var tax = chance.floating({fixed: 2, min: 0, max: taxExclusivePrice}),
                expected = taxExclusivePrice;

            cartItem.manualDiscount = 0;
            cartItem.quantity = 1;
            expect(cartItem.GrandTotal.toFixed(2)).toEqual(expected.toFixed(2));

            // Factor Item quantity
            cartItem.quantity = quantity;
            expected = quantity * taxExclusivePrice;

            expect(cartItem.GrandTotal.toFixed(2)).toEqual(expected.toFixed(2));

            cartItem.taxAmount = tax;

            // Factor Manual Discount
            cartItem.manualDiscount = discount;
            expected = (quantity * (taxExclusivePrice + discount));

            expect(cartItem.GrandTotal.toFixed(2)).toEqual(expected.toFixed(2));

        });

        it('should return Tax Total of zero when it is a tax free sale', function () {

            cartItem.isTaxFree = true;

            var expected = quantity * (taxExclusivePrice);

            expect(cartItem.ItemTaxTotal).toEqual(0);
        });

    });

    it('Should Change Tax Inclusive Price When Manual Tax Amount Changes', function () {
        var tax = chance.floating({fixed: 2, min: 0, max: taxExclusivePrice}),
            expected = taxExclusivePrice + tax;

        cartItem.manualTaxAmount = Utilities.roundFloat(tax);

        expect(cartItem.taxInclusivePrice).toEqual(Utilities.roundFloat(expected));
    });

    //region Method Tests

    describe('function: isReturnMissingReason', function () {
        it('should returns true if the Item is returned and the return reason was not set otherwise false',
           function () {

               expect(cartItem.isReturnMissingReason()).toBeFalsy();

               cartItem.returned = true;

               expect(cartItem.isReturnMissingReason()).toBeTruthy();

           });

    });

    describe('function: getReturnedSubItems', function () {
        it('should return a list of returned warranties if any', function () {
            var warranty = {returned: true};

            cartItem.warranties.push(warranty);

            var result = cartItem.getReturnedSubItems();

            expect(result.length).toBeGreaterThan(0);
        });

        it('should return a list of returned installations if any', function () {
            var installation = {returned: true};

            cartItem.installations.push(installation);

            var result = cartItem.getReturnedSubItems();

            expect(result.length).toBeGreaterThan(0);
        });

    });

    //endregion

});