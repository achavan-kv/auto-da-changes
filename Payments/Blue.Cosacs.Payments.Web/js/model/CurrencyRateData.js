'use strict';

var cls = CurrencyRateData.prototype;

function CurrencyRateData() {
    this.CurrencyName = '';
    this.CurrencyCode = '';
    this.Rate = 0;
    this.RateChanged = 0;
    this.DateFrom = null;
    this.DateFromChanged = null;
    this.CreatedBy = '';
    this.ShowEditFields = false;
    this.OperationAllowed = false;

}

module.exports = CurrencyRateData;