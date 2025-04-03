'use strict';

var cls = Discount.prototype;

var SalesEnums = require('./SalesEnums'),
    Utilities = require('./Utilities');

function Discount(taxType, taxRate, code, description) {
    var _amount = 0,
        _percentage = 0,
        _taxAmount = 0,
        _description = '';

    this.code = code || '';
    this.taxType = taxType || SalesEnums.TaxTypes.Exclusive;
    this.taxRate = taxRate || 0;
    this.itemNo = '';
    this.description = '';
    this.isTaxFree = false;
    this.isDutyFree = false;
    this.isFixedDiscount = false;
    this.id = null;
    this.isKitDiscount = false;

    this.originalId = 0;
    this.returned = false;
    this.returnQuantity = 0;
    this.quantity = 0;
    this.parentItemNo;
    this.unitDiscount = 0;
    this.unitTaxAmount = 0;
    this.unitPercentage = 0;
    this.isNew = true;

    this.dutyFreePrice = 0;

    if (description) {
        _description = description ? description.split('-') : '';
        this.itemNo = _description[0].trim();
        _description.splice(0, 1);
        this.description = _description.join('-').trim()
    }


    //region Properties

    Object.defineProperty(this, 'amount', {
        get: function () {
            return _amount || 0;
        },
        set: function (amount) {
            var self = this;

            _amount = amount || 0;

            if (self.isNew) {
                if (self.taxType === SalesEnums.TaxTypes.Inclusive && !self.isKitDiscount) {
                    _amount = (_amount * 100) / (100 + self.taxRate);
                }

                _amount = Math.abs(_amount || 0);

                var incDiscount = Utilities.includeTax(_amount, self.taxRate);

                _taxAmount = Math.abs(incDiscount - _amount);
                _taxAmount = _taxAmount * -1;
                _amount = _amount * -1;
            }

        },
        enumerable: true
    });

    Object.defineProperty(this, 'percentage', {
        get: function () {
            return _percentage || 0;
        },
        set: function (percentage) {
            var val = percentage || 0,
                self = this;

            _percentage = self.returned ? val : Math.abs(val) * -1;
        },
        enumerable: true
    });

    Object.defineProperty(this, 'taxAmount', {
        get: function () {

            return this.isTaxFree ? 0 : _taxAmount;
        },
        set:function (val) {
            _taxAmount= val;
        },
        enumerable: true
    });

    //endregion

}

module.exports = Discount;