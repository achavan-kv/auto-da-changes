'use strict';

var cls = CartItem.prototype;

var SalesEnums = require('./SalesEnums'),
    Utilities = require('./Utilities'),
    Discount = require('./Discount'),
    Price = require('./Price');

function CartItem(options, newObj) {
    var CartItemFactory = require('./CartItemFactory');
    var _manualTaxAmount = 0,
        _quantity = 1,
        _manualPrice = 0,
        _manualDiscount = 0,
        _manualDiscountPercentage = 0,
        _taxExclusivePrice = 0,
        _taxInclusivePrice = 0,
        _dutyFreePrice = 0,
        _promoPriceExclusive = 0,
        _promoPriceInclusive = 0,
        _promoPrice = 0,
        _taxAmount = 0,
        _isTaxFree = options.isTaxFree || false,
        _isDutyFree = options.isDutyFree || false;

    this.id = 0;
    this.discountChange = "0";
    this.discountPercentageChange = "0";
    //this.itemTypeId = options.itemType || SalesEnums.CartItemTypes.Product;
    this.taxType = options.taxType || SalesEnums.TaxTypes.Exclusive;
    this.defaultTaxRate = options.taxRate || 0;

    this.priceData = new Price();
    this.taxRate = options.taxRate || 0;
    this.parentId = 0;
    this.orderId = 0;
    this.originalId = 0;
    this.itemNo = '';
    this.description = '';
    this.posDescription = '';
    this.warrantyLengthMonths = null;
    this.warrantyEffectiveDate = null;
    this.warrantyContractNo = null;
    this.warrantyTypeCode = null;
    this.productItemId = 0;
    this.warrantyLinkId = 0;
    this.itemUPC = null;
    this.itemSupplier = null;
    this.costPrice = null;
    this.retailPrice = null;
    this.department = '';   //Level_1's code
    this.category = '';     //Level_2
    this.class = '';        //Level_3

    this.canAddWarranty = false;
    this.canAddInstallation = false;
    this.showAvailableDiscount = false;

    this.installations = [];     // installations
    this.warranties = [];       // warranties
    this.selectedDiscount = null;       // selected discount
    this.discounts = [];        //discounts
    this.availableWarranties = [];       // available warranties
    this.availableDiscounts = [];       // available discounts
    this.availableInstallations = [];       // available installations
    this.availableAssociatedProducts = [];
    this.kitItems = [];

    this.returned = false;
    this.returnQuantity = 0;
    this.canWarrantyBeKept = false;
    this.returnReason = null;
    this.warrantyReturnPercentage = 100;
    this.operationsRequired = true;
    this.availableQuantity;
    this.parentItemNo = 0;
    this.totalKitAmount = 0;
    this.itemKitDiscountPercentage = 0;
    this.potentialWarranties = [];
    this.isKitParent = false;
    this.kitDiscount = 0;

    this.isClaimed = false;
    this.saleTaxAmount = 0;
    this.isIrExchange = false;
    this.isReplacement = false;
    this.replacementInProgress = false;

    this.dutyFreePriceExclusive = 0;
    this.dutyFreePriceInclusive = 0;

    this._factory = new CartItemFactory(options);

    //region Properties methods

    Object.defineProperty(this, 'quantity', {
        get: function () {
            return _quantity || 1;
        },
        set: function (value) {
            _quantity = value || 1;
        },
        enumerable: true
    });

    Object.defineProperty(this, 'dutyFreePrice', {
        get: function () {
            return _dutyFreePrice;
        },
        set: function (value) {
            _dutyFreePrice = value;
            this.dutyFreePriceExclusive = value || 0;

            // if (this.taxType === SalesEnums.TaxTypes.Exclusive) {
            //     this.dutyFreePriceExclusive = value || 0;
            //     this.dutyFreePriceInclusive = Utilities.roundFloat(value + value * this.taxRate * 0.01) || 0;
            // }
            // else {
            //     this.dutyFreePriceExclusive = Utilities.excludeTax(value, this.taxRate);
            //     this.dutyFreePriceInclusive = value || 0;
            // }
        },
        enumerable: true
    });

    Object.defineProperty(this, 'promoPrice', {
        get: function () {
            return _promoPrice;
        },
        set: function (value) {
            _promoPrice = value;

            if (this.taxType === SalesEnums.TaxTypes.Exclusive) {
                this._promoPriceExclusive = value || 0;
                this._promoPriceInclusive = value + value * this.taxRate * 0.01 || 0;
            }
            else {
                this._promoPriceExclusive = value, this.taxRate;
                this._promoPriceInclusive = value || 0;
            }
        },
        enumerable: true
    });

    Object.defineProperty(this, 'taxExclusivePrice', {
        get: function () {
            return _taxExclusivePrice;
        },
        set: function (value) {
            _taxExclusivePrice = value || 0;
        },
        enumerable: true
    });

    Object.defineProperty(this, 'taxInclusivePrice', {
        get: function () {
            return _taxInclusivePrice;
        },
        set: function (value) {
            _taxInclusivePrice = value || 0;
        },
        enumerable: true
    });

    Object.defineProperty(this, 'price', {
        get: function () {
            if (this.itemTypeId === SalesEnums.CartItemTypes.Warranty && this.isWarrantyFree) {
                return 0;
            }

            var price = this.taxExclusivePrice || 0;

            if (this._promoPriceExclusive && !this.returned) {
                price = this._promoPriceExclusive;
            }

            if (this.isZeroPrice) {
                price = this.manualPrice || 0;
            }
            else if (this.isDutyFree && !this.returned) {
                price = (this.dutyFreePriceExclusive || 0);
            }

            var negFactor = this.returned ? (price < 0 ? 1 : -1) : 1;

            return price * negFactor;
        },
        set: function (value) {
            if (this.isDutyFree) {
                this.dutyFreePriceExclusive = Math.abs(value || 0);
            }
            else {
                this.taxExclusivePrice = Math.abs(value || 0);
            }
        },
        enumerable: true
    });

    Object.defineProperty(this, 'taxAmount', {
        get: function () {
            var self = this;
            var negFactor = self.returned ? ((self.manualTaxAmount && self.manualTaxAmount < 0) ? 1 : -1) : 1;

            if (self.isTaxFree && !self.returned) {
                return 0;
            }

            if (self.manualTaxAmount) {
                return Math.abs(this.manualTaxAmount) * negFactor;
            }

            if (self.isDutyFree && !self.returned) {
                return (self.dutyFreePriceInclusive - self.dutyFreePriceExclusive);
            }

            // if (this._promoPriceExclusive && !this.returned) {
            //     return this._promoPriceInclusive - this._promoPriceExclusive;
            // }

            var tax = Math.abs(_taxAmount) * negFactor;

            return tax;
        },
        set: function (value) {
            _taxAmount = value || 0;
        },
        enumerable: true
    });

    Object.defineProperty(this, 'manualTaxAmount', {
        get: function () {
            return _manualTaxAmount;
        },
        set: function (value) {
            _manualTaxAmount = value;
        },
        enumerable: true
    });

    Object.defineProperty(this, 'manualPrice', {
        get: function () {
            return _manualPrice;
        },
        set: function (value) {
            _manualPrice = value;
        },
        enumerable: true
    });

    //region Discounts

    Object.defineProperty(this, 'manualDiscount', {
        get: function () {
            if (!_manualDiscount) {
                _manualDiscount = 0;
            }

            return parseFloat(_manualDiscount);
        },
        set: function (value) {
            _manualDiscount = value || 0;
        },
        enumerable: true
    });

    Object.defineProperty(this, 'manualDiscountPercentage', {
        get: function () {
            if (!_manualDiscountPercentage) {
                _manualDiscountPercentage = 0;
            }

            return parseFloat(_manualDiscountPercentage);
        },
        set: function (value) {
            _manualDiscountPercentage = value || 0;
        },
        enumerable: true
    });

    Object.defineProperty(this, 'discountPercentageRemaining', {
        get: function () {
            if (this.discounts) {

                var code = this.selectedDiscount ? this.selectedDiscount.code : '';

                var percentageTotal = _.reduce(this.discounts, function (sum, item) {
                    var percentage = (!item.percentage || isNaN(item.percentage) || item.code
                                                                                    === code) ? 0 : parseFloat(
                        item.percentage);
                    return sum + percentage;
                }, 0);
            }
            return parseFloat(100 - Math.abs(parseFloat(percentageTotal)));
        },
        enumerable: true
    });

    Object.defineProperty(this, 'discountAmountRemaining', {
        get: function () {
            var price = (this.totalKitAmount || this.ItemTotal) || 0;

            if (this.discounts) {
                var code = this.selectedDiscount ? this.selectedDiscount.code : '';

                var amountTotal = _.reduce(this.discounts, function (sum, item) {
                    var amount = (!item.amount || isNaN(item.amount) || item.code === code) ? 0 : parseFloat(
                        item.amount);
                    return sum + amount;
                }, 0);
            }

            return parseFloat(price - Math.abs(parseFloat(amountTotal)));
        },
        enumerable: true
    });

    //endregion

    Object.defineProperty(this, 'ItemTotal', {
        get: function () {
            var ret = (this.price + this.taxAmount) || 0;

            return ret;
        },
        enumerable: true
    });

    Object.defineProperty(this, 'salePrice', {
        get: function () {
            var self = this;

            if (self.isInstantReplacementWarranty && (self.isClaimed)) {
                return 0;
            }

            if (self.itemTypeId === SalesEnums.CartItemTypes.Warranty && self.returned) {
                return Utilities.roundFloat((self.price * self.warrantyReturnPercentage) / 100);
            }

            var discount = self.manualDiscount || 0,
                unitDiscount = 0;

            if (discount) {
                unitDiscount = discount * (self.returned ? self.returnQuantity : self.quantity);
            }

            return (self.price + unitDiscount );
        },
        enumerable: true
    });

    Object.defineProperty(this, 'saleTaxAmount', {
        get: function () {
            var self = this;

            if (self.isTaxFree && !self.returned) {
                return 0;
            }

            if (self.isInstantReplacementWarranty && (self.isClaimed)) {
                return 0;
            }

            if (self.itemTypeId === SalesEnums.CartItemTypes.Warranty && self.returned) {
                return self.salePrice * self.taxRate * 0.01;
            }

            if (self.itemTypeId === SalesEnums.CartItemTypes.Discount && self.returned) {
                return self.taxAmount || 0;
            }

            var discount = self.manualDiscount || 0,
                unitDiscountTax = 0;

            if (discount) {
                var taxTotal = self.getDiscountsTax();
                unitDiscountTax = taxTotal;// * (self.returned ? self.returnQuantity : self.quantity);
            }

            // if (!self.returned) {
            //     unitDiscountTax = Math.abs(unitDiscountTax);
            // }

            return (self.taxAmount + unitDiscountTax);
        },
        enumerable: true
    });

    Object.defineProperty(this, 'ItemNetTotal', {
        get: function () {
            var self = this;

            var total = self.returned ? self.returnQuantity * (self.salePrice) :
                        self.quantity * (self.salePrice);

            return total;
        },
        enumerable: true
    });

    Object.defineProperty(this, 'ItemTaxTotal', {
        get: function () {
            var self = this;

            if (self.isTaxFree && !self.returned) {
                return 0;
            }

            var quantity = self.returned ? self.returnQuantity : self.quantity,
                total = quantity * self.saleTaxAmount;

            return total;
        },
        enumerable: true
    });

    Object.defineProperty(this, 'GrandTotal', {
        get: function () {
            var warrantyNetTotal = this.getSubItemNetTotal(this.warranties),
                installationNetTotal = this.getSubItemNetTotal(this.installations),
                kitItems = this.getSubItemNetTotal(this.kitItems),
                itemTotal = (kitItems || 0 ) + this.ItemNetTotal;

            if (this.isKitParent && this.returned) {
                itemTotal = this.salePrice;
            }

            var ret = (itemTotal || 0) + (warrantyNetTotal || 0) + (installationNetTotal || 0);

            return ret;
        },
        enumerable: true
    });

    Object.defineProperty(this, 'GrandTaxTotal', {
        get: function () {
            var warrantyTaxTotal = this.getSubItemTaxTotal(this.warranties),
                installationTaxTotal = this.getSubItemTaxTotal(this.installations),
                kitItemsTax = this.getSubItemTaxTotal(this.kitItems),
                // discountsTax = this.isKitParent ? 0 : this.getDiscountsTax(),
                itemTaxTotal = this.isKitParent ? kitItemsTax : this.ItemTaxTotal;

            var ret = itemTaxTotal + warrantyTaxTotal + installationTaxTotal;// + discountsTax;

            if (this.isKitParent && this.returned) {
                ret = this.saleTaxAmount * -1;
            }


            return ret;
        },
        enumerable: true
    });

    Object.defineProperty(this, 'isZeroPrice', {
        get: function () {
            return this.taxExclusivePrice === 0;
        },
        enumerable: true
    });

    Object.defineProperty(this, "isWarrantyFree", {
        get: function () {
            return this.warrantyTypeCode === SalesEnums.WarrantyTypes.Free;
        },
        enumerable: true
    });

    Object.defineProperty(this, "isInstantReplacementWarranty", {
        get: function () {
            return this.warrantyTypeCode === SalesEnums.WarrantyTypes.InstantReplacement;
        },
        enumerable: true
    });

    Object.defineProperty(this, "warrantyType", {
        get: function () {
            switch (this.warrantyTypeCode) {
                case SalesEnums.WarrantyTypes.Extended:
                    return "Extended";
                case SalesEnums.WarrantyTypes.InstantReplacement:
                    return "Instant Replacement";
                default:
                    return "Free";
            }
        },
        enumerable: true
    });

    Object.defineProperty(this, "hasInstantReplacementWarranty", {
        get: function () {
            var self = this;

            var irWarranties = _.filter(self.warranties, function (w) {
                return w.isInstantReplacementWarranty === true;
            });

            return irWarranties && (irWarranties.length > 0);

        },
        enumerable: true
    });

    Object.defineProperty(this, "totalWarrantyCoverage", {
        get: function () {
            var self = this;

            return _.reduce(self.warranties, function (memo, w) {
                return memo + w.warrantyLengthMonths;
            }, 0);

        },
        enumerable: true
    });

    Object.defineProperty(this, "unClaimedInstantReplacementWarranties", {
        get: function () {
            var self = this;

            var irWarranties = _.filter(self.warranties, function (w) {
                return (w.isInstantReplacementWarranty === true && !(w.isClaimed));
            });

            return irWarranties;

        },
        enumerable: true
    });

    Object.defineProperty(this, "claimedInstantReplacementWarranties", {
        get: function () {
            var self = this;

            var irWarranties = _.filter(self.warranties, function (w) {
                return (w.isInstantReplacementWarranty === true && (w.isClaimed));
            });

            return irWarranties;

        },
        enumerable: true
    });

    Object.defineProperty(this, "freeWarrantyBeingKept", {
        get: function () {
            var self = this;

            var beingKept = _.filter(self.warranties, function (w) {
                return w.isWarrantyFree && !w.returned;
            });

            return beingKept;
        },
        enumerable: true
    });

    //Object.defineProperty(this, 'isKitParent', {
    //    get: function () {
    //        return this.isKitParent;
    //    },
    //    set: function (value) {
    //        this.isKitParent = value || 0;
    //    },
    //    enumerable: false
    //});

    Object.defineProperty(this, 'totalInstallations', {
        get: function () {
            return _.reduce(this.installations, function (memo, inst) {
                return memo + inst.quantity;
            }, 0);
        },
        enumerable: false
    });


    Object.defineProperty(this, 'isDutyFree', {
        get: function () {
            return _isDutyFree || false;
        },
        set: function (value) {
            var self = this;
            _isDutyFree = value;

            self.changeSubsPropertyValue('isDutyFree', _isDutyFree);

        },
        enumerable: true
    });

    Object.defineProperty(this, 'isTaxFree', {
        get: function () {
            return _isTaxFree || false;
        },
        set: function (value) {
            var self = this;
            _isTaxFree = value;

            self.changeSubsPropertyValue('isTaxFree', _isTaxFree);

        },
        enumerable: true
    });

    //endregion

}

//region Warranties

cls.addFreeWarranties = function (warranties) {
    var self = this,
        itemType = SalesEnums.CartItemTypes.Warranty;

    _.each(warranties, function (w) {
        var warranty = self._factory.createCartItem(itemType, w);
        warranty.parentItemNo = self.itemNo;
        var canWarrantyBeAdded = self.canAddWarrantyToItem(warranty);

        if (canWarrantyBeAdded) {
            self.warranties.push(warranty);
        }
        else if (!self.isReplacement) {
            self.availableWarranties.push(warranty);
        }

    });
};

cls.addAvailableWarranties = function (warranties, isIrwCarried) {
    var self = this,
        itemType = SalesEnums.CartItemTypes.Warranty;

    _.each(warranties, function (w) {
        var warranty = self._factory.createCartItem(itemType, w);
        warranty.parentItemNo = self.itemNo;

        if (isIrwCarried && warranty.isInstantReplacementWarranty) {
            self.warranties.push(warranty);
        }
        else {
            self.availableWarranties.push(warranty);
        }

    });

    self.canAddWarranty = self.availableWarranties.length > 0;
};

cls.removeFreeWarrantyForRemovedItem = function () {
    var self = this;

    var freeWarranty = self.getItemFreeWarranty();

    if (!freeWarranty) {
        return;
    }

    self.warranties = _.reject(self.warranties, function (w) {
        return w.isWarrantyFree === true;
    });

    for (var i = 0; i < self.quantity; i++) {
        self.warranties.splice(0, 0, _.cloneDeep(freeWarranty));
    }
};

cls.removeExtendedWarrantyForRemovedItem = function () {
    var self = this;

    var extendedWarranties = _.filter(self.warranties, function (w) {
        return w.isWarrantyFree === false;
    });

    if (self.quantity === 1 || extendedWarranties.length <= (self.quantity - 1)) {
        return true;
    }

    var uniqueWarranties = _.uniq(extendedWarranties, false, function (w) {
        return w.warrantyLinkId;
    });

    if (uniqueWarranties.length > 1) {
        return false;
    }

    self.warranties.splice(self.warranties.indexOf(uniqueWarranties[0]), 1);

    return true;

};

cls.removeExtendedWarrantyFromAvailableList = function () {
    var self = this;

    var extendedWarranties = _.filter(self.warranties, function (w) {
        return w.isWarrantyFree === false;
    });

    if (extendedWarranties.length < (self.quantity - 1)) {
        return;
    }

    _.each(extendedWarranties, function (warranty) {
        self.availableWarranties = _.filter(self.availableWarranties, function (w) {
            return w.price.WarrantyId !== warranty.price.WarrantyId;
        });
    });
};

cls.addFreeWarrantyForItem = function () {
    var self = this;

    var freeWarranty = _.find(self.warranties, function (w) {
        return w.isWarrantyFree === true;
    });
    if (freeWarranty) {
        var options = {
                taxRate: freeWarranty.TaxRate,
                taxType: freeWarranty.taxType,
                isTaxFree: freeWarranty.isTaxFree || self.isTaxFree,
                isDutyFree: freeWarranty.isDutyFree || self.isDutyFree
            },
            convertedCartItem = new CartItem(options);

        _.extend(convertedCartItem, freeWarranty);

        convertedCartItem.isTaxFree = self.isTaxFree;
        convertedCartItem.isDutyFree = self.isDutyFree;

        self.warranties.splice(0, 0, convertedCartItem);
    }
};

cls.populateAvailableWarrantiesWithAlreadyAddedWarranty = function () {
    var self = this;

    var extendedWarranty = _.find(self.warranties, function (w) {
        return w.isWarrantyFree === false;
    });

    if (!extendedWarranty) {
        return;
    }

    self.availableWarranties = self.availableWarranties || [];

    var found = _.find(self.availableWarranties, function (w) {
        return w.warrantyLinkId === extendedWarranty.warrantyLinkId;
    });

    if (found) {
        return;
    }

    self.availableWarranties.push(extendedWarranty);
};

cls.getItemFreeWarranty = function () {
    var self = this;

    var freeWarranty = _.find(self.warranties, function (w) {
        return w.isWarrantyFree === true;
    });

    return freeWarranty;
};

cls.getItemFreeWarranties = function () {
    var self = this;

    var freeWarranties = _.filter(self.warranties, function (w) {
        return w.isWarrantyFree === true;
    });

    return freeWarranties;
};

cls.getItemIrWarranty = function () {
    var self = this;

    var freeWarranty = _.find(self.warranties, function (w) {
        return w.isInstantReplacementWarranty === true;
    });

    return freeWarranty;
};

/**
 * only add one Free Warranty
 * @param warranty
 * @returns {*}
 *  TODO: Need to check if any rules need to be applied in this case
 */
cls.canAddWarrantyToItem = function (warranty) {
    var self = this;

    if (!warranty.isWarrantyFree) {
        return true;
    }

    var existingWarranties = self.getItemFreeWarranties() || [];

    return (self.quantity > existingWarranties.length);

};

//endregion

//region Installations

cls.addAvailableInstallations = function (internalInstallations) {
    var self = this,
        itemType = SalesEnums.CartItemTypes.Installation;

    _.each(internalInstallations, function (installation) {
        var internalInstallation = self._factory.createCartItem(itemType, installation);
        internalInstallation.parentItemNo = self.itemNo;
        self.availableInstallations.push(internalInstallation);
    });

    self.canAddInstallation = self.availableInstallations.length > 0;
};

cls.removeInstallationForRemovedItem = function () {
    var self = this;

    self.installations = self.installations || [];

    if (self.quantity === 1 || self.totalInstallations <= (self.quantity - 1)) {
        return true;
    }

    var uniqueInstallations = _.uniq(self.installations, false, function (installation) {
        return installation.itemNo;
    });

    if (uniqueInstallations.length > 1) {
        return false;
    }

    uniqueInstallations[0].quantity -= 1;
    //self.installations.splice(self.installations.indexOf(uniqueInstallations[0]), 1);

    return true;
};

cls.removeInstallationFromAvailableList = function () {
    var self = this;

    if (!self.installations) {
        return;
    }

    if (self.installations.length < (self.quantity - 1)) {
        return;
    }

    _.each(self.installations, function (installation) {
        self.availableInstallations = _.filter(self.availableInstallations, function (i) {
            return i.itemNo !== installation.itemNo;
        });
    });

};

cls.populateAvailableInstallationsWithAlreadyAddedInstallation = function () {
    var self = this;

    _.each(self.installations, function (installation) {

        var found = _.find(self.availableInstallations, function (i) {
            return i.itemNo === installation.itemNo;
        });

        if (found) {
            return;
        }

        self.availableInstallations.push(installation);
    });
};


//endregion

//region Item Refunds

cls.setAsReturned = function (isManual) {
    var self = this;

    self.returned = true;
    self.returnQuantity = self.availableQuantity;
    self.canWarrantyBeKept = false;

    var warranties = [];

    _.each(self.warranties, function (w) {
        if (w.operationsRequired) {
            w.originalId = w.id;
            w.parentId = 0;
            w.orderId = 0;
            w.returned = true;
            w.returnQuantity = 1;
        }
        //TO DO - Check whether it is required or not
        if (!isManual) {
            var newWarranty = new CartItem({
                                               taxRate: w.taxRate,
                                               taxType: self.taxType,
                                               isTaxFree: w.isTaxFree || self.isTaxFree,
                                               isDutyFree: w.isDutyFree || self.isDutyFree
                                           });

            newWarranty.itemTypeId = SalesEnums.CartItemTypes.Warranty;
            _.extend(newWarranty, w);

            newWarranty.isTaxFree = w.isTaxFree;
            newWarranty.isDutyFree = w.isDutyFree;

            warranties.push(newWarranty);
        }
    });

    if (!isManual) {
        self.warranties = warranties;
    }

    _.each(self.installations, function (i) {
        if (i.operationsRequired) {
            i.originalId = i.id;
            i.parentId = 0;
            i.orderId = 0;
            i.returned = true;
            i.returnQuantity = i.availableQuantity;
        }
    });

    _.each(self.discounts, function (d) {
        d.returned = true;
        d.returnQuantity = self.returnQuantity;
        d.quantity = self.quantity;
        d.percentage = -1 * d.percentage;
        d.amount = -1 * d.amount;// d.unitDiscount * (self.returnQuantity);
        d.taxAmount = -1 * d.taxAmount;// d.unitTaxAmount * (self.returnQuantity);

    });
    //self.setFreeReturnWarranties();
};

cls.cancelAsReturned = function () {
    var self = this;

    self.returned = false;
    self.returnReason = null;
    self.returnQuantity = 0;

    //self.availableQuantity = self.quantity - self.returnQuantity;
    self.canWarrantyBeKept = true;

    _.each(self.warranties, function (w) {
        if (w.operationsRequired) {
            w.returned = false;
            w.returnReason = null;
            w.returnQuantity = 0;
            w.isClaimed = false;
        }
    });

    _.each(self.installations, function (i) {
        if (i.operationsRequired) {
            i.returned = false;
            i.returnReason = null;
            i.returnQuantity = 0;
        }
    });

    _.each(self.discounts, function (d) {
        d.returned = false;
        d.returnQuantity = 0;
        d.amount = Utilities.roundFloat(d.unitDiscount * self.availableQuantity);
        d.percentage = Utilities.roundFloat(d.unitPercentage * self.availableQuantity);
    });

    self.itemReturnTotalAmount = 0;
};

cls.setFreeReturnWarranties = function () {
    var self = this,
        keptFreeWarranties = self.freeWarrantyBeingKept,
        returnQuantity = self.returnQuantity || 0;

    if (!keptFreeWarranties || keptFreeWarranties.length === 0) {
        return;
    }

    _.each(keptFreeWarranties, function (w) {
        if (returnQuantity > 0) {
            w.returned = true;
            w.returnQuantity = 1;
        }

        returnQuantity -= 1;
    });
};

//endregion

//region Item Discounts

cls.addAvailableDiscounts = function (discounts) {
    this.availableDiscounts = discounts;
};

cls.addSelectedDiscount = function (globalTaxRate) {
    if (!this.selectedDiscount) {
        return;
    }

    this.addDiscount(this.selectedDiscount, globalTaxRate);

    this.selectedDiscount = null;

    this.discountPercentageChange = 0;
    this.discountChange = 0;
    this.selectedDiscountKey = "";
};

cls.addDiscount = function (d, globalTaxRate) {

    if (!d) {
        return;
    }

    var self = this,
        taxRate = d.taxRate || this.taxRate || globalTaxRate,
        discount = new Discount(self.taxType, taxRate);

    if (d instanceof Discount) {
        discount = d;
    } else {
        _.extend(discount, d);
    }

    var oldItem = _.find(self.discounts, {'code': discount.code});

    if (oldItem) {
        oldItem.amount = discount.amount;
        oldItem.percentage = discount.percentage;
        oldItem.isTaxFree = discount.isTaxFree;
        oldItem.isDutyFree = discount.isDutyFree;
    } else {
        discount.parentItemNo = self.itemNo;
        discount.isTaxFree = self.isTaxFree;
        discount.isDutyFree = self.isDutyFree;

        self.discounts.push(discount);
    }

    self.updateDiscount();
};

cls.removeDiscount = function (code) {
    if (this.discounts) {
        this.discounts = _.reject(this.discounts, {'code': code});
        this.selectedDiscount = null;
        this.selectedDiscountKey = "";
        this.updateDiscount();
    }
};

cls.setSelectedDiscount = function (key, value) {
    var self = this,
        oldItem = _.find(self.discounts, {'code': key});

    if (oldItem) {
        self.selectedDiscount = oldItem;
    } else {
        var dis = _.find(self.availableDiscounts, {'k': key});

        if (dis) {
            var taxRate = dis.taxRate || self.taxRate;

            self.selectedDiscount = new Discount(self.taxType, taxRate, key, value);

            var price = dis.Price || 0,
                dutyFreePrice = dis.dutyFreePrice || 0;

            self.selectedDiscount.isFixedDiscount = (price !== 0);
            self.selectedDiscount.amount = price;
            self.selectedDiscount.dutyFreePrice = dutyFreePrice;
            self.discountChange = price;
            self.discountAmountChanged(dis);
        }
    }

};

cls.discountAmountChanged = function (dis) {
    var self = this;

    var amount = self.selectedDiscount.amount || 0,
        maxValue = self.discountAmountRemaining,
        taxRate = dis.taxRate || self.taxRate,
        isInclusive = (self.taxType === SalesEnums.TaxTypes.Inclusive),
        price = (self.totalKitAmount || isInclusive ? self.ItemTotal : self.price) || 0;

    amount = isInclusive ? Utilities.excludeTax(amount, taxRate) : amount;

    if (Math.abs(amount) > maxValue) {
        amount = maxValue;
        self.selectedDiscount.amount = Utilities.roundFloat(maxValue, 4);
        self.discountChange = self.selectedDiscount.amount;
    }

    self.selectedDiscount.percentage = 0;

    if (amount && price) {
        var ret = (amount / price) * 100;
        self.selectedDiscount.percentage = Utilities.roundFloat(parseFloat(ret.toFixed(4)), 4);
    }


    self.discountPercentageChange = self.selectedDiscount.percentage;
};

cls.discountPercentageChanged = function () {
    if (!this.selectedDiscount) { // || !this.ItemTotal) {
        this.selectedDiscount = {
            amount: 0,
            percentage: 0
        };

        return;
    }

    var percentage = this.selectedDiscount.percentage || 0,
        maxValue = this.discountPercentageRemaining,
        isInclusive = (this.taxType === SalesEnums.TaxTypes.Inclusive),
        price = (this.totalKitAmount || isInclusive ? this.ItemTotal : this.price) || 0;

    if (Math.abs(percentage) > maxValue) {
        percentage = maxValue;
        this.selectedDiscount.percentage = Utilities.roundFloat(maxValue, 4);
        this.discountPercentageChange = this.selectedDiscount.percentage;
    }

    this.selectedDiscount.amount = 0;

    if (percentage) {
        var ret = (percentage * 0.01 * price);

        this.selectedDiscount.amount = Utilities.roundFloat(parseFloat(ret), 4);
    }

    this.discountChange = this.selectedDiscount.amount;
};

//endregion

//region Kit's Items

cls.addKitItem = function (kitItem) {
    var self = this,
        itemType = SalesEnums.CartItemTypes.Product,
        discount = kitItem.discountItem;

    var newKitItem = self._factory.createCartItem(itemType, kitItem);

    self.totalKitAmount += newKitItem.salePrice * (newKitItem.quantity || 1);
    // self.totalKitAmount += newKitItem.salePrice * (newKitItem.quantity || 1);
    self.kitItems.push(newKitItem);
};

cls.updateKitDiscount = function (disItem) {
    var self = this,
        taxRate = self.taxRate,
        discount = new Discount(self.taxType, taxRate);

    if (!disItem) {
        disItem = {
            "ItemNo": "DS",
            "Description": "DISCOUNT KIT CODES"
        }
    }
    else {
        taxRate = disItem.taxRate || taxRate;
    }
    //kitQty = self.quantity || 0;
    //kitDiscount = (self.totalKitAmount * kitQty) * (self.itemKitDiscountPercentage * 0.01);

    if (!self.totalKitAmount || !self.kitDiscount) {
        return;
    }

    discount.code = self.kitDiscountItemId;
    discount.itemNo = disItem.ItemNo || "DS";
    discount.description = disItem.Description || "KIT CODE DISCOUNT";
    discount.percentage = self.itemKitDiscountPercentage;
    discount.isKitDiscount = true;
    discount.amount = self.kitDiscount;
    discount.parentItemNo = self.itemNo;
    discount.taxRate = disItem.taxRate;
    discount.taxType = self.taxType;

    self.addDiscount(discount);

    self.updateDiscount();
};

cls.getKitDiscount = function () {
    var self = this,
        ret;

    if (self.discounts && self.discounts.length > 0) {
        ret = _.find(self.discounts, {'isKitDiscount': true});
    }

    return ret;
};

//endregion

cls.addAssociatedProducts = function (associatedProducts) {
    var self = this,
        itemType = SalesEnums.CartItemTypes.Product;

    _.each(associatedProducts, function (product) {
        var associatedProduct = self._factory.createCartItem(itemType, product);

        self.availableAssociatedProducts.push(associatedProduct);
    });
};

cls.increaseQuantity = function () {
    var self = this,
        oldQuantity = self.quantity || 1;

    self.quantity += 1;

    self.addFreeWarrantyForItem();

    if (self.isKitParent) {
        self.kitItems.forEach(function (kitItem) {
            var quantity = (kitItem.quantity || 1),
                baseQuantity = quantity / oldQuantity;

            kitItem.quantity += baseQuantity;
        });

        self.updateKitDiscount();
    }

    self.canAddWarranty = true;
    self.canAddInstallation = true;
    self.updateDiscount();

};

cls.getSubItemNetTotal = function (subItems) {
    var subItemTotal = 0;

    if (subItems) {
        subItemTotal = _.reduce(subItems, function (memo, item) {
            return memo + item.ItemNetTotal;

        }, 0);
    }

    return subItemTotal;
};

cls.getSubItemTaxTotal = function (subItems) {
    var subItemTaxTotal = 0;

    if (subItems) {
        subItemTaxTotal = _.reduce(subItems, function (memo, item) {
            return memo + item.ItemTaxTotal;

        }, 0);
    }

    return subItemTaxTotal;
};

cls.getDiscountsTax = function () {
    var self = this,
        totalTax = 0;

    if (self.discounts) {
        totalTax = _.reduce(self.discounts, function (memo, item) {
            return memo + item.taxAmount;

        }, 0);
    }

    return totalTax || 0;
};

cls.isReturnMissingReason = function () {
    var self = this;

    if (self.returned && !self.returnReason) {
        return true;
    }

    //var ret = false,
    //    subItems = self.getReturnedSubItems();
    //
    //if (subItems && subItems.length > 0) {
    //    _.each(subItems, function (i) {
    //        if (!i.returnReason) {
    //            ret = true;
    //            return false; //break loop
    //        }
    //    });
    //}
    //
    //return ret;
};

cls.getReturnedSubItems = function () {
    var self = this,
        returnedWarranties = [],
        returnInstallations = [];

    if (self.warranties && self.warranties.length > 0) {
        returnedWarranties = _.filter(self.warranties, function (i) {
            return i.returned;
        });
    }

    if (self.installations && self.installations.length > 0) {
        returnInstallations = _.filter(self.installations, function (i) {
            return i.returned;
        });
    }

    return _.union(returnedWarranties, returnInstallations);
};

cls.updateDiscount = function () {
    var self = this, discountTotal = 0, percentageTotal = 0;

    if (self.discounts) {
        discountTotal = _.reduce(self.discounts, function (sum, item) {
            var discount = (!item.amount || isNaN(item.amount)) ? 0 :
                           parseFloat(item.amount);
            return sum + (discount);

        }, 0);

        percentageTotal = _.reduce(this.discounts, function (sum, item) {
            var percentage = (!item.percentage || isNaN(item.percentage)) ? 0 :
                             parseFloat(item.percentage);

            return sum + percentage;

        }, 0);

    }

    var quantity = self.returned ? self.returnQuantity : self.quantity;

    if (quantity === 0) {
        quantity = 1;
    }

    self.manualDiscount = parseFloat(discountTotal || 0) / quantity;
    self.manualDiscountPercentage = parseFloat(percentageTotal || 0);
};
cls.changeSubsPropertyValue = function (property, value) {
    var self = this;

    if (!_.has(self, property)) {
        return;
    }

    _.each(self.warranties, function (w) {
        w[property] = value;
    });
    _.each(self.installations, function (i) {
        i[property] = value;
    });
    _.each(self.kitItems, function (k) {
        k[property] = value;
    });

    _.each(self.discounts, function (i) {
        i[property] = value;
    });

    self.updateKitDiscount();
};

module.exports = CartItem;