'use strict';


var cls = DiscountLimitData.prototype;

function DiscountLimitData() {
    this.storeType;
    this.branchNumber;
    this.branchName;
    this.limitPrice;
    this.limitPriceChanged;
    this.createdBy;
    this.showEditFields = false;
}

module.exports = DiscountLimitData;