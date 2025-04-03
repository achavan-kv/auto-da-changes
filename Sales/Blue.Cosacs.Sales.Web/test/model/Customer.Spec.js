'use strict';

var Customer = require('../../js/model/Customer.js');
var customer;

describe('Model: Customer:', function () {

    beforeEach(function () {
        customer = new Customer();
    });

    afterEach (function() { customer = undefined;    });

    describe("Customer's Data", function () {

        xit('should require customer title', function () {
            customer.isCustomerDataRequired = true;

            customer.FirstName = 'Ziggy';
            customer.LastName = 'Stardust';
            customer.AddressLine1 = 'Red dust lane';
            customer.TownOrCity = 'Mars';
            customer.PostCode = '656536';

            var missing = customer.IsCustomerDataMissing;

            expect(missing).toBeTruthy();
        });

        xit('should require customer first name', function () {
            customer.isCustomerDataRequired = true;

            customer.Title = 'Mr';
            customer.LastName = 'Stardust';
            customer.AddressLine1 = 'Red dust lane';
            customer.TownOrCity = 'Mars';
            customer.PostCode = '656536';

            var missing = customer.IsCustomerDataMissing;

            expect(missing).toBeTruthy();
        });

        xit('should require customer last name', function () {
            customer.isCustomerDataRequired = true;

            customer.Title = 'Mr';
            customer.FirstName = 'Ziggy';
            customer.AddressLine1 = 'Red dust lane';
            customer.TownOrCity = 'Mars';
            customer.PostCode = '656536';

            var missing = customer.IsCustomerDataMissing;

            expect(missing).toBeTruthy();
        });

        xit('should require customer address line 1', function () {
            customer.isCustomerDataRequired = true;

            customer.Title = 'Mr';
            customer.FirstName = 'Ziggy';
            customer.LastName = 'Stardust';
            customer.TownOrCity = 'Mars';
            customer.PostCode = '656536';

            var missing = customer.IsCustomerDataMissing;

            expect(missing).toBeTruthy();
        });

        xit('should require customer town/city', function () {
            customer.isCustomerDataRequired = true;

            customer.Title = 'Mr';
            customer.FirstName = 'Ziggy';
            customer.LastName = 'Stardust';
            customer.AddressLine1 = 'Red dust lane';
            customer.PostCode = '656536';

            var missing = customer.IsCustomerDataMissing;

            expect(missing).toBeTruthy();
        });

        xit('should require customer postcode', function () {
            customer.isCustomerDataRequired = true;

            customer.Title = 'Mr';
            customer.FirstName = 'Ziggy';
            customer.LastName = 'Stardust';
            customer.AddressLine1 = 'Red dust lane';
            customer.TownOrCity = 'Mars';

            var missing = customer.IsCustomerDataMissing;

            expect(missing).toBeTruthy();
        });

        xit('should not require customer data if customer data is not required', function () {
            customer.isCustomerDataRequired = false;

            var missing = customer.IsCustomerDataMissing;

            expect(missing).toBeFalsy();
        });

        xit('should check that all required customer data has been entered', function () {
            customer.isCustomerDataRequired = true;

            customer.Title = 'Mr';
            customer.FirstName = 'Ziggy';
            customer.LastName = 'Stardust';
            customer.AddressLine1 = 'Red dust lane';
            customer.TownOrCity = 'Mars';
            customer.PostCode = '656536';

            var missing = customer.IsCustomerDataMissing;

            expect(missing).toBe(false);
        });

    });

});