'use strict';

var cls = CurrencyRateData.prototype;

function CurrencyRateData() {
    this.currencyName = '';
    this.currencyCode = '';
    this.rate = 0;
    this.rateChanged = 0;
    this.dateFrom = null;
    this.dateFromChanged = null;
    this.createdBy = '';
    this.showEditFields = false;
    this.operationAllowed = false;

}

module.exports = CurrencyRateData;