'use strict';

var cls = Price.prototype;

function Price() {
    this.regularPrice = 0;
    this.cashPrice = 0;
    this.dutyFreePrice = 0;
    this.taxRate = 0;
    this.regularPriceTaxInclusive = 0;
    this.cashPriceTaxInclusive = 0;
    this.dutyFreePriceTaxInclusive = 0;

}

module.exports = Price;