'use strict';

var cls = Payment.prototype;

function Payment(amount) {
    this.amount = amount || 0;
    this.tendered = 0;
    this.currencyRate = 0;
    this.paymentMethodId = 0;
    this.payType = '';
    this.method = '';
    this.currency = null;
    this.bankAccountNo = null;
    this.storeCardNo = null;
    this.voucherNo = null;
    this.bank = null;
    this.chequeNo = null;
    this.cardType = null;
    this.cardNo = null;
    this.currencyCode = null;
    this.tempPaymentId = 0;
    this.voucherIssuer = 'C';
    this.voucherIssuerCode = null;
    this.subType = '';
    this.isChange = false;
    this.currencyAmount=0;
}

cls.getChange = function () {
    var self = this,
        ret = 0;

    if (self.tendered) {
        var tendered = self.currencyRate ? self.currencyRate * self.tendered : self.tendered;
        var change = (self.amount - tendered) * -1;

        ret = change < 0 ? 0 : change;
    }

    return ret;
};

cls.restrictTenderedAmount = function () {
    var self = this;

    if ((self.amount < 0 && self.tendered > 0 )|| (self.amount > 0 && self.tendered < 0 ) || Math.abs(self.tendered) > Math.abs(self.amount)) {
        self.tendered = self.amount;
    }
};

cls.clear = function () {

    // this.amount = 0;
    //this.paymentMethodId = undefined;
    this.currencyRate = undefined;
    this.currencyCode = '';
    this.currency = undefined;
    this.bankAccountNo = undefined;
    this.bank = undefined;
    this.chequeNo = undefined;
    this.cardType = undefined;
    this.cardNo = undefined;

    this.storeCardNo = undefined;
    this.voucherNo = undefined;
    this.method = '';
    this.tempPaymentId = undefined;
    this.voucherIssuer = 'C';
    this.voucherIssuerCode = undefined;
    this.subType = '';
    this.currencyAmount = undefined;
};


module.exports = Payment;