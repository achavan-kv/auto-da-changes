'use strict';

var Payment = require('../../js/model/Payment.js');
var payment;

describe('Payment Model', function () {

    beforeEach(function () {
        payment = new Payment();
    });

    afterEach(function () {
        payment = undefined;
    });

    describe('Changes', function () {

        it('Should Return correct change when applying rate', function () {
            var amount = parseFloat(faker.finance.amount()),
                tendered = parseFloat(faker.finance.amount()),
                rate = parseFloat(faker.finance.amount()),
                expected = 0;

            payment.amount = amount;
            payment.tendered = tendered;
            payment.currencyRate = rate;

            var tempTendered = rate ? rate * tendered : tendered;
            expected = (amount - tempTendered) * -1;

            var actual = payment.getChange();

            expect(actual).toEqual(expected);

        });

        it('Should Return correct change when not applying rate', function () {
            var amount = parseFloat(faker.finance.amount()),
                tendered= amount * 2,  // ensure tendered > amount always
                rate = 0,
                expected = 0;

            payment.amount = amount;
            payment.tendered = tendered;

            expected = (amount - tendered) * -1;

            var actual = payment.getChange();

            expect(actual).toEqual(expected);

        });

        it('Should Return 0 Change when payment amount is greater or equal to the tendered amount after applying rate',
           function () {
               var amount = parseFloat(faker.finance.amount()),
                   rate = parseFloat(faker.finance.amount()),
                   expected = 0;

               // Apply rate
               var amountRated = rate ? rate * amount : amount;
               payment.amount = amountRated
               payment.tendered = amount;
               payment.currencyRate = rate;

               var actual = payment.getChange();

               expect(actual).toEqual(expected);

               payment.tendered = amount - 10;

               actual = payment.getChange();

               expect(actual).toEqual(expected);
           });

    });

    it('Should Restrict the Tendered Amount to the Payment Amount', function () {
        payment.amount = 10;
        payment.tendered = 20;

        payment.restrictTenderedAmount();

        expect(payment.tendered).toEqual(payment.amount);
    });

    it('should reset payment details when cleared', function () {
        payment.amount = faker.finance.amount();
        payment.bankAccountNo = 'bankAccountNo';
        payment.bank = 'bank';
        payment.chequeNo = 'chequeNo';
        payment.cardType = 'cardType';
        payment.cardNo = 'cardNo';

        payment.clear();

        expect(payment.bankAccountNo).toBeFalsy();
        expect(payment.bank).toBeFalsy()
        expect(payment.chequeNo).toBeFalsy();
        expect(payment.cardType).toBeFalsy();
        expect(payment.cardNo).toBeFalsy();

    });

});