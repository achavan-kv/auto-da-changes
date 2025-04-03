'use strict';

var CartItem = require('../../js/model/CartItem');
var SalesEnums = require('../../js/model/SalesEnums');
var warranty;

describe('Warranty Model', function () {

    beforeEach(function () {
        var options = {
            itemType: SalesEnums.CartItemTypes.Warranty,
            taxRate: parseFloat(faker.finance.amount()),
            taxExclusivePrice: parseFloat(faker.finance.amount())
        };

        warranty = new CartItem(options);
    });

    afterEach(function () {
        warranty = undefined;
    });

    it('Should return the correct warranty type based on its Type Code', function () {
        warranty.warrantyTypeCode = 'F';

        expect(warranty.isWarrantyFree).toBeTruthy();

        warranty.warrantyTypeCode = 'E';
        expect(warranty.isWarrantyFree).toBeFalsy();

        warranty.warrantyTypeCode = 'I';
        expect(warranty.isWarrantyFree).toBeFalsy();
    });

});