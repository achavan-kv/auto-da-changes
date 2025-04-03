'use strict';

var SalesEnums = require('./SalesEnums');

var Customer = require('./Customer'),
    Payment = require('./Payment'),
    CartItem = require('./CartItem'),
    CartItemFactory = require('./CartItemFactory'),
    Utilities = require('./Utilities'),
    Discount = require('./Discount');

var cls = Basket.prototype;

function Basket() {
    var _warrantyBeingPurchased = false,
        _installationBeingPurchased = false,
        _itemBeingReturned = false,
        _isCustomerDataRequired = false,
        _isTaxFreeSale = false,
        _isDutyFreeSale = false;

    this.totalAmount = 0;
    this.InvoiceNo = 0;
    this.itemsTotal = 0;
    this.totalTaxAmount = 0;
    this.paymentsTotal = 0;
    this.changeTotal = 0;
    this.balance = 0;
    //this.credit = 0;
    this.taxType = SalesEnums.TaxTypes.Exclusive;
    this.taxRate = 0;
    this.originalOrderId = null;

    this.saleComplete = false;
    this.Customer = new Customer();     // Customer
    this.searchItem = null;             // CartItem
    this.items = [];                    // CartItem

    // this for returned warranties, installations, etc.. that doesn't have parent item
    this.returnedItems = [];            // CartItem

    this.payments = [];                 // Payment
    this.payment = new Payment();       // Payment
    this.originalOrder = {
        receiptNumber: 'UNKNOWN',
        order: null
    };

    this.isAuthoried = false;
    this.Comments = null;
    this.returnType = SalesEnums.ReturnType.None;
    // Temp
    this.CreatedBy = 0;
    this.BranchNo = 0;
    this.createdOn = null;
    this.pendingDiscAuthorisationItemNo;
    this.isCustomerDetailsLocked = false;

    this.decimalPlaces = 2;
    //region Properties

    Object.defineProperty(this, 'isTaxFreeSale', {
        get: function () {
            return _isTaxFreeSale;
        },
        set: function (value) {
            _isTaxFreeSale = value;

            this.changeSaleType();
        },
        enumerable: true
    });

    Object.defineProperty(this, 'isDutyFreeSale', {
        get: function () {
            return _isDutyFreeSale;
        },
        set: function (value) {
            _isDutyFreeSale = value;

            this.changeSaleType();
        },
        enumerable: true
    });

    Object.defineProperty(this, 'WarrantyBeingPurchased', {
        get: function () {
            return _warrantyBeingPurchased;
        },
        set: function (value) {
            _warrantyBeingPurchased = value;

            this.setIsCustomerDataRequired();
        },
        enumerable: true
    });

    Object.defineProperty(this, 'installationBeingPurchased', {
        get: function () {
            return _installationBeingPurchased;
        },
        set: function (value) {
            _installationBeingPurchased = value;
            this.setIsCustomerDataRequired();
        },
        enumerable: true
    });

    Object.defineProperty(this, 'itemBeingReturned', {
        get: function () {
            return _itemBeingReturned;
        },
        set: function (value) {
            _itemBeingReturned = value;

            this.setIsCustomerDataRequired();
        },
        enumerable: true
    });

    Object.defineProperty(this, 'TotalDiscount', {
        get: function () {
            var discountTotal = 0;

            if (this.items) {
                discountTotal = _.reduce(this.items, function (sum, item) {
                    var discount = (!item.manualDiscount || isNaN(item.manualDiscount)) ? 0 :
                                   parseFloat(item.manualDiscount);

                    return sum + discount;

                }, 0);
            }

            return parseFloat(discountTotal);
        },
        enumerable: true
    });

    Object.defineProperty(this, 'isCustomerDataRequired', {
        get: function () {
            return _isCustomerDataRequired;
        },
        set: function (value) {
            _isCustomerDataRequired = value;

        },
        enumerable: true
    });

    Object.defineProperty(this, 'isCustomerDataMissing', {
        get: function () {
            if (!_isCustomerDataRequired) {
                return false;
            }

            if (!this.Customer) {
                return true;
            }

            var isFieldsMissing = !this.Customer.Title || !this.Customer.FirstName || !this.Customer.LastName
                                  || !this.Customer.AddressLine1 || !this.Customer.TownOrCity;
            return (!this.Customer.isCardLinked && isFieldsMissing);

        },
        enumerable: true
    });

    Object.defineProperty(this, 'isReturn', {
        get: function () {
            return (this.originalOrder && this.originalOrder.order);
        },
        enumerable: true
    });

    Object.defineProperty(this, 'totalQuantity', {
        get: function () {
            var self = this,
                totalQuantity = 0;

            if (self.items) {
                totalQuantity = _.reduce(self.items, function (memo, item) {
                    return memo + (item.returned ? item.returnQuantity : item.quantity);
                }, 0);
            }

            return totalQuantity;
        },
        enumerable: true
    });

    //endregion
}


cls.getItem = function (itemNo) {
    var cartItem = _.find(this.items, function (item) {
        return (item.itemNo === itemNo && !item.returned);
    });

    return cartItem;
};

cls.addItemToCart = function (itemToAdd) {
    var self = this;
    var isNew = false;
    var itemNo = (itemToAdd.ItemNoWarrantyLink || itemToAdd.itemNo);
    var existingItem = self.getItem(itemNo);

    if (existingItem) {
        existingItem.increaseQuantity();
    }
    else {
        var item = {};

        if (itemToAdd instanceof CartItem) {
            item = itemToAdd;
        }
        else {
            item = this.createNewProduct(itemToAdd);
        }

        self.items.push(item);
        isNew = true;


    }

    this.updateTotalAmount();
    this.setIsCustomerDataRequired();
    return isNew;
};

cls.createNewProduct = function (itemToAdd) {
    var self = this,
        options = {
            taxRate: itemToAdd.priceData && itemToAdd.priceData.taxRate ? itemToAdd.priceData.taxRate
                                                                          * 100 : self.taxRate,
            taxType: self.taxType,
            isTaxFree: self.isTaxFreeSale,
            isDutyFree: self.isDutyFreeSale
        },

        factory = new CartItemFactory(options);

    return factory.createCartItem(SalesEnums.CartItemTypes.Product, itemToAdd);
};

cls.addSearchedItem = function () {
    var self = this;

    if (!self.searchItem) {
        return;
    }

    self.addItemToCart(self.searchItem, function () {
        self.searchItem = null;
    });

};

cls.removeItem = function (itemToRemove) {
    var self = this,
        parentId = itemToRemove.parentId || 0;

    var remainingItems = _.reject(self.items, function (item) {
        return (item.itemNo === itemToRemove.itemNo || item.parentId === itemToRemove.productItemId);
    });

    var remainingIndividualItems = _.reject(self.returnedItems, function (item) {
        return (item.parentItemNo === itemToRemove.itemNo);
    });

    self.returnedItems = remainingIndividualItems;
    self.items = remainingItems;
    self.updateTotalAmount();

    self.setIsCustomerDataRequired();

    if (itemToRemove.isReplacement && self.originalOrder.order && self.originalOrder.order.items) {
        var exchangeItem = _.find(self.originalOrder.order.items, {id: parentId});

        if (exchangeItem && exchangeItem.claimedInstantReplacementWarranties
            && exchangeItem.claimedInstantReplacementWarranties.length > 0) {
            exchangeItem.isIrExchange = false;

            self.cancelReturnOrExchange(exchangeItem);

            var claimedWarranty = exchangeItem.claimedInstantReplacementWarranties[0];

            if (claimedWarranty) {
                claimedWarranty.isClaimed = false;
                claimedWarranty.returned = false;
            }

        }

    }
};

cls.hasPayments = function () {
    return (this.payments && this.payments.length > 0);
};

cls.addWarranty = function (warranty, item) {
    var self = this;

    item.warranties = item.warranties || [];

    if (warranty.warrantyTypeCode === SalesEnums.WarrantyTypes.Free) {
        var removedFreeWarranty = _.find(item.warranties, function (w) {
            return w.isWarrantyFree;
        });
        item.availableWarranties = _.filter(item.availableWarranties, function (w) {
            return w.itemNo !== warranty.itemNo;
        });

        item.warranties = _.filter(item.warranties, function (w) {
            return w.itemNo !== removedFreeWarranty.itemNo;
        });

        for (var i = 0; i < item.quantity; i++) {
            //item.warranties.push(warranty);
            item.warranties.splice(0, 0, self.convertToCartItem(warranty, true));
        }
        item.availableWarranties.splice(0, 0, self.convertToCartItem(removedFreeWarranty, true));
    }
    else {
        var extendedWarranties = _.filter(item.warranties, function (w) {
            return !w.isWarrantyFree;
        });

        if (extendedWarranties.length === item.quantity) {
            return;
        }

        var newWarranty = self.convertToCartItem(warranty, true);

        item.warranties.push(newWarranty);

        self.updateTotalAmount();

        if ((extendedWarranties.length + 1) < item.quantity) {
            item.canAddWarranty = true;
            return;
        }

        //item.availableWarranties = _.filter(item.availableWarranties, function (w) {
        //    return w.itemNo !== warranty.itemNo;
        //});

        item.canAddWarranty = false;

        if (!warranty.isWarrantyFree) {
            self.WarrantyBeingPurchased = true;
        }

        self.setIsCustomerDataRequired();
    }
};

cls.removeWarranty = function (warrantyIndex, item) {
    var self = this;
    var removedWarranty = item.warranties.splice(warrantyIndex, 1)[0];

    self.updateTotalAmount();

    item.availableWarranties = item.availableWarranties || [];
    item.canAddWarranty = true;

    var extendedWarranty = _.find(item.warranties, function (w) {
        return w.isWarrantyFree === false;
    });

    if (!extendedWarranty) {
        self.WarrantyBeingPurchased = false;
    }

    var found = _.find(item.availableWarranties, function (w) {
        return w.warrantyLinkId === removedWarranty.warrantyLinkId;
    });

    if (found) {
        return;
    }

    item.availableWarranties.push(removedWarranty);

    self.setIsCustomerDataRequired();

};

cls.restrictWarrantyMaximumDuration = function (warranty) {
    if (warranty.warrantyLengthMonths > warranty.warrantyMaxLength) {
        warranty.warrantyLengthMonths = warranty.warrantyMaxLength;
    }
};

cls.updateTotalAmount = function () {
    this.updateDueDetails();

    this.totalAmount = (this.itemsTotal || 0) + (this.totalTaxAmount || 0);

    //var total = _.reduce(this.items, function (sum, item) {
    //    return sum + item.GrandTotal + item.GrandTaxTotal;
    //}, 0);

    this.balance = this.totalAmount;// = total;

    this.updateBalance();

};

cls.updateBalance = function () {
    var payments = _.reduce(this.payments, function (sum, payment) {
        return sum + (payment.amount || 0);
    }, 0);

    this.balance = (this.totalAmount - payments); // - (this.changeTotal || 0);
    this.payment.amount = this.balance = Utilities.roundFloat(this.balance, this.decimalPlaces);
    this.payment.tendered = Utilities.roundFloat(this.balance, this.decimalPlaces);
    this.balance = Utilities.roundFloat(this.balance, this.decimalPlaces);
    //this.credit = Math.round(this.credit * 100) / 100;
};

// TODO: Refactor this
cls.updateDueDetails = function () {
    var self = this,
        total = 0,
        taxes = 0;

    _.each(self.items, function (item) {
        total += item.GrandTotal;
        taxes += item.GrandTaxTotal;
    }, 0);

    self.itemsTotal = total;

    self.totalTaxAmount = taxes;

    self.addOrphanReturnsTotal();
};

cls.addOrphanReturnsTotal = function () {
    var self = this,
        total = 0,
        taxes = 0;

    _.each(self.returnedItems, function (item) {
        total += (item.GrandTotal || 0);
        taxes += (item.GrandTaxTotal || 0);
    }, 0);

    self.itemsTotal += total;
    self.totalTaxAmount += taxes;
};

cls.addPayment = function () {
    var self = this,
        paymentMethodId = parseInt(self.payment.paymentMethodId),
        isForeign = _.contains([SalesEnums.PaymentMethods.ForeignCash,
                                SalesEnums.PaymentMethods.TravellersCheque], paymentMethodId),
        tendered = isForeign ?
                   self.payment.currencyRate * parseFloat(self.payment.tendered) :
                   parseFloat(self.payment.tendered),
        amount = self.payment.amount;

    self.payment.amount = tendered;
    self.payment.tempPaymentId = self.payments.length;
    self.payment.currencyAmount = isForeign ? parseFloat(self.payment.tendered) : 0;
    self.payment.voucherIssuer = paymentMethodId === SalesEnums.PaymentMethods.GiftVoucher ?
                                 self.payment.voucherIssuer :
                                 null;

    if (paymentMethodId === SalesEnums.PaymentMethods.GiftVoucher) {
        self.payment.subType = self.payment.voucherIssuer === 'N' ? 'Non-Courts' : 'Courts';
    }

    if (Math.abs(tendered) > Math.abs(amount)) {
        var change = (tendered - amount) * -1;
        self.payment.amount = amount;
        //self.addChangePayment(change);
        self.changeTotal += change;
    }
    self.payments.push(_.cloneDeep(self.payment));

    //self.changeTotal += self.payment.getChange();
    self.paymentsTotal += tendered;
    self.payment.tendered = null;
    self.updateBalance();
    self.basketChangesAllowed = false;
};

cls.addChangePayment = function (amount) {
    var self = this,
        changePayment = new Payment();

    changePayment.amount = amount;
    changePayment.tendered = amount;
    changePayment.paymentMethodId = parseInt(SalesEnums.PaymentMethods.Cash);
    changePayment.isChange = true;
    changePayment.voucherIssuer = null;

    self.payments.push(changePayment);
};

cls.selectItem = function (itemNo) {
    this.searchItem = this.getItem(itemNo);
};

cls.increaseItemQuantity = function (item) {

    item.increaseQuantity();

    this.setIsCustomerDataRequired();
    this.updateTotalAmount();

};

//TODO: Move this to CartItem
cls.decreaseItemQuantity = function (item) {
    var msg,
        result = {
            title: '',
            msg: '',
            success: true
        },
        oldQuantity = item.quantity;

    if (item.quantity === 0) {
        return result;
    }

    var extendedWarrantyRemoved = item.removeExtendedWarrantyForRemovedItem();

    if (!extendedWarrantyRemoved) {
        msg = 'More than one extended warranty has been added to the cart for this item.' +
              ' You need to remove the unwanted warranty before you can reduce the item quantity.';

        return {
            title: 'Extended warranties',
            msg: msg,
            success: true
        };
    }

    // item.removeExtendedWarrantyFromAvailableList();

    var installationRemoved = item.removeInstallationForRemovedItem();

    if (!installationRemoved) {
        msg = 'More than one installation has been added to the cart for this item. You need to remove' +
              ' the unwanted installation before you can reduce the item quantity.';

        return {
            title: 'Installations',
            msg: msg,
            success: true
        };
    }

    //item.removeInstallationFromAvailableList();

    item.quantity -= 1;

    if (item.isKitParent) {
        if (item.quantity === 0) {
            item.kitItems = [];
        } else if (oldQuantity > item.quantity) {
            item.kitItems.forEach(function (kitItem) {
                var quantity = (kitItem.quantity || 1),
                    baseQuantity = quantity / oldQuantity;

                kitItem.quantity -= baseQuantity;
            });

            item.updateKitDiscount();
        }
    }

    item.removeFreeWarrantyForRemovedItem();

    var extendedWarranties = _.filter(item.warranties, function (w) {
        return w.isWarrantyFree === false;
    });

    if (extendedWarranties.length === item.quantity) {
        item.canAddWarranty = false;
    }

    if (item.installations.length === item.quantity) {
        item.canAddInstallation = false;
    }

    item.updateDiscount();
    this.updateTotalAmount();
    this.setIsCustomerDataRequired();

    return result;
};

cls.getCartWarranties = function () {
    var warranties = _.flatten(this.items, 'warranties');

    return warranties;
};

cls.getExtendedCartWarranties = function () {
    var warranties = this.getCartWarranties();

    var extendedWarranties = _.filter(warranties, function (w) {
        return w.isWarrantyFree === false;
    });

    return extendedWarranties;
};

cls.setIsCustomerDataRequired = function () {

    var hasExtendedWarranty = false;

    _.each(this.items, function (i) {
        if (_.find(i.warranties, function (w) {
                return w.warrantyTypeCode === SalesEnums.WarrantyTypes.Extended ||
                       w.warrantyTypeCode === SalesEnums.WarrantyTypes.InstantReplacement;
            })) {
            hasExtendedWarranty = true;
        }
    });

    this.isCustomerDataRequired = (hasExtendedWarranty) ||
                                  (this.returnedItems && this.returnedItems.length > 0) ||
                                  this.installationBeingPurchased || this.itemBeingReturned;
};

//region Refunds

cls.setOriginalOrder = function (data) {
    if (!data) {
        return;
    }

    var self = this,
        items = [];

    var order = new Basket();
    _.merge(order, data);
    order.items = [];
    self.soldBy = data.soldBy.toString();
    if (data.customer) {
        self.Customer = setOriginalOrderCustomer(data.customer);
        self.isCustomerDetailsLocked = true;
    }

    if (data.items) {

        _.each(data.items, function (i) {
            var item = new CartItem({
                itemType: i.itemTypeId,
                taxRate: i.taxRate,
                taxType: self.taxType,
                isTaxFree: i.isTaxFree || false,
                isDutyFree: i.isDutyFree || false
            });

            _.merge(item, i);
            item.availableQuantity = item.quantity - item.returnQuantity;
            item.quantity = item.quantity;
            item.returnQuantity = item.returnQuantity || 0;
            item.kitItems = i.kitItems || [];
            item.potentialWarranties = i.potentialWarranties || [];
            item.isKitParent = (i.itemTypeId == SalesEnums.CartItemTypes.Kit);

            setOriginalOrderWarranties(item);
            setOriginalOrderKitItems(item);
            setOriginalOrderInstallation(item);
            setOriginalOrderDiscounts(item);
            if (i.returned) {
                item.operationsRequired = false;
            }

            order.items.push(item);
        });

    }

    self.originalOrderId = data.id;
    //BOC CR 2018-13
    var agreementInvoiceNumber = data.agreementInvoiceNumber;
    var id = data.id;
    var inv_num = "";
    if (agreementInvoiceNumber == null || agreementInvoiceNumber == "") {
        inv_num = id
    }
    else {
        var a = agreementInvoiceNumber;
        var b = "-";
        var position = 3;
        inv_num = [a.slice(0, position), b, a.slice(position)].join('');
    }

    self.originalOrder.receiptNumber = inv_num || 'UNKNOWN';//CR 2018-13
    //EOC CR 2018-13
    self.originalOrder.order = order;
};

function setOriginalOrderCustomer(originalCustomer) {
    var customer = new Customer();
    customer.CustomerId = originalCustomer.customerId;
    customer.Title = originalCustomer.title;
    customer.FirstName = originalCustomer.firstName;
    customer.LastName = originalCustomer.lastName;
    customer.HomePhoneNumber = originalCustomer.homePhoneNumber;
    customer.MobileNumber = originalCustomer.mobileNumber;
    customer.Email = originalCustomer.email;
    customer.AddressLine1 = originalCustomer.addressLine1;
    customer.AddressLine2 = originalCustomer.addressLine2;
    customer.TownOrCity = originalCustomer.townOrCity;
    customer.PostCode = originalCustomer.postCode;
    customer.isSalesCustomer = originalCustomer.isSalesCustomer;

    return customer;
}

function setOriginalOrderKitItems(item) {
    if (!item.kitItems || item.kitItems.length === 0) {
        return;
    }
    var kitItems = item.kitItems;
    item.kitItems = [];

    _.each(kitItems, function (w) {

        var kitItems = new CartItem({
            itemType: w.itemTypeId,
            taxRate: w.taxRate
        });

        _.merge(kitItems, w);

        kitItems.originalId = w.id;

        if (w.returned) {
            kitItems.operationsRequired = false;
        }

        item.kitItems.push(kitItems);
    });
}

function setOriginalOrderWarranties(item) {

    if (!item.warranties || item.warranties.length === 0) {
        return;
    }
    var warranties = item.warranties;
    item.warranties = [];

    _.each(warranties, function (w) {

        var warranty = new CartItem({
            itemType: w.itemTypeId,
            taxRate: w.taxRate,
            isTaxFree: w.isTaxFree || false,
            isDutyFree: w.isDutyFree || false
        });

        _.merge(warranty, w);

        warranty.originalId = w.id;

        if (w.returned) {
            warranty.operationsRequired = false;
        }

        item.warranties.push(warranty);
    });
}

function setOriginalOrderInstallation(item) {
    if (!item.installations || item.installations.length === 0) {
        return;
    }
    var installations = item.installations;
    item.installations = [];

    _.each(installations, function (i) {

        var installation = new CartItem({
            itemType: i.itemTypeId,
            taxRate: i.taxRate,
            isTaxFree: i.isTaxFree || false,
            isDutyFree: i.isDutyFree || false
        });

        _.merge(installation, i);
        installation.originalId = i.id;
        installation.availableQuantity = i.quantity - i.returnQuantity;
        installation.returnQuantity = installation.returnQuantity || 0;

        if (i.returned) {
            installation.operationsRequired = false;
        }

        item.installations.push(installation);
    });
}

function setOriginalOrderDiscounts(item) {
    if (!item.discounts || item.discounts.length === 0) {
        return;
    }

    var discounts = item.discounts;
    item.discounts = [];

    _.each(discounts, function (d) {
        var taxType = item.taxType || this.taxType,
            taxRate = item.taxRate || this.taxRate,
            discount = new Discount(taxType, taxRate);

        d.isNew = false;
        _.extend(discount, d);

        discount.amount = d.amount;

        discount.taxAmount = d.taxAmount;
        discount.percentage = d.percentage;
        discount.unitDiscount = d.amount;// / item.quantity;
        discount.unitTaxAmount = d.taxAmount;// / item.quantity;
        discount.unitPercentage = discount.percentage;// / item.quantity;
        discount.originalId = discount.id;
        discount.isTaxFree = item.isTaxFree;
        discount.isDutyFree = item.isDutyFree;

        if (d.returned) {
            discount.operationsRequired = false;
        }

        item.discounts.push(discount);
    });
}

cls.returnItem = function (item) {
    var self = this,
        id = item.id;

    if (!item) {
        return;
    }

    // item = self.convertToCartItem(item, true);

    item.isIrExchange = false;

    self.itemBeingReturned = true;

    item.setAsReturned(true);

    item.originalId = id;

    item.updateDiscount();
    self.items.push(self.convertToCartItem(item, true));

    item.updateDiscount();
    //self.addItemToCart(item);

    var idList = _.pluck(item.warranties, 'id');
    self.returnedItems = _.reject(self.returnedItems, function (r) {
        return _.contains(idList, r.id);
    });
    idList = _.pluck(item.installations, 'id');
    self.returnedItems = _.reject(self.returnedItems, function (r) {
        return _.contains(idList, r.id);
    });

    self.authorisationPending = true;

    self.updateTotalAmount();
};

cls.cancelReturnOrExchange = function (item) {
    var self = this;

    item.cancelAsReturned();

    self.removeItem(item);
    self.cancelKitsReturnOrExchange(item.productItemId);
    self.updateTotalAmount();

    var beingReturned = getBeingReturned(self);

    if (!beingReturned) {
        self.authorisationPending = false;
        self.itemBeingReturned = false;
        self.returnType = SalesEnums.ReturnType.None;
    }
};

cls.cancelKitsReturnOrExchange = function (parentId) {
    var self = this;
    var kitItems = _.filter(self.originalOrder.order.items, function (item) {
        return item.parentId === parentId;
    });

    if (kitItems && kitItems.length > 0) {
        _.each(kitItems, function (kit) {
            self.cancelReturnOrExchange(kit);
        });
    }
};

cls.returnWarranty = function (warrantyIndex, item) {
    //TODO: Get rid of the index
    var self = this,
        retWarranty = item.warranties[warrantyIndex];

    retWarranty.parentItemNo = item.itemNo;
    retWarranty.returned = true;
    retWarranty.returnQuantity = 1;
    retWarranty.parentId = 0;


    item.canWarrantyBeKept = true;

    //TODO
    //self.credit += -((retWarranty.price || 0) + (retWarranty.taxAmount || 0) || 0);

    if (!retWarranty.isWarrantyFree) {
        self.returnedItems.push(retWarranty);
    }

    self.authorisationPending = true;
    self.itemBeingReturned = true;
    self.updateTotalAmount();
    self.setIsCustomerDataRequired();
};

cls.returnInstallation = function (index, item) {
    var self = this,
        retInstallation = item.installations[index];

    retInstallation.parentItemNo = item.itemNo;
    retInstallation.returned = true;
    retInstallation.returnQuantity = retInstallation.availableQuantity;
    retInstallation.parentId = 0;

    self.returnedItems.push(retInstallation);
    self.authorisationPending = true;
    self.itemBeingReturned = true;
    self.updateTotalAmount();
    self.setIsCustomerDataRequired();
};

cls.cancelReturnInstallation = function (index, item) {
    if (item.returned) {
        return;
    }

    var self = this,
        retInstallation = item.installations[index];

    retInstallation.returned = false;
    retInstallation.returnQuantity = 0;

    self.returnedItems = _.without(self.returnedItems, retInstallation);

    var beingReturned = getBeingReturned(self);

    if (!beingReturned) {
        self.itemBeingReturned = false;
        self.authorisationPending = false;
    }

    self.updateTotalAmount();
    self.setIsCustomerDataRequired();
};

cls.cancelReturnWarranty = function (warrantyIndex, item) {
    var self = this,
        retWarranty = item.warranties[warrantyIndex];

    var beingKept = _.filter(item.warranties, function (w) {
        return !w.isWarrantyFree && !w.returned;
    });
    if ((item.availableQuantity - item.returnQuantity) === beingKept.length) {
        return;
    }

    retWarranty.returned = false;
    retWarranty.returnQuantity = 0;

    //TODO
    //self.credit -= ((retWarranty.price || 0) + (retWarranty.taxAmount || 0) || 0);
    self.returnedItems = _.without(self.returnedItems, retWarranty);

    var beingReturned = getBeingReturned(self);

    if (!beingReturned) {
        self.authorisationPending = false;
        self.itemBeingReturned = false;
    }

    self.updateTotalAmount();
    self.setIsCustomerDataRequired();
};

cls.hasReturnedItems = function () {
    var self = this;
    var ret = false;

    if (self.returnedItems && self.returnedItems.length > 0) {
        return true;
    }

    _.each(self.items, function (i) {
        if (i.returned) {
            ret = true;
            return false; //exit loop
        }

        var results = i.getReturnedSubItems();
        if (!results || results.length > 0) {
            ret = true;
            return false; //exit loop
        }
    });

    return ret;
};

cls.isReturnItemsMissingReason = function () {
    var self = this;
    var ret = false;

    _.each(self.returnedItems, function (i) {
        if (!i.parentId && i.returned && !i.returnReason) {
            ret = true;
            return false; //exit loop
        }
    });

    _.each(self.items, function (i) {
        if (!i.parentId && (i.returned) && !i.returnReason) {
            ret = true;
            return false; //exit loop
        }


    });

    return ret;
};

function getBeingReturned(self) {
    var isAnyItemReturned = _.some(self.originalOrder.order.items, function (i) {
        return i.returned === true && i.operationsRequired;
    });

    var isAnyIndividualReturned = self.returnedItems && self.returnedItems.length > 0;

    return isAnyItemReturned || isAnyIndividualReturned;
}

cls.convertToCartItem = function (target, newCopyNeeded) {
    if (target instanceof CartItem && !newCopyNeeded) {
        return target;
    }
    var self = this,
        options = {
            taxRate: target.TaxRate,
            taxType: target.taxType
        },
        convertedCartItem = new CartItem(options);

    _.extend(convertedCartItem, target);

    convertedCartItem.isTaxFree = target.isTaxFree || self.isTaxFreeSale;
    convertedCartItem.isDutyFree = target.isDutyFree || self.isDutyFreeSale;

    return convertedCartItem;
};

cls.changeSaleType = function () {
    var self = this;

    _.each(self.items, function (item) {
        item.isDutyFree = self.isDutyFreeSale;
        item.isTaxFree = self.isTaxFreeSale;
    });

    self.updateTotalAmount();
};

cls.setDutyFreeSales = function () {
    var self = this;

    _.each(self.items, function (item) {
        item.isDutyFree = self.isDutyFreeSale;
    });

    self.updateTotalAmount();
};

cls.setTaxFreeSales = function () {
    var self = this;

    _.each(self.items, function (item) {
        item.isTaxFree = self.isTaxFreeSale;
    });

    self.updateTotalAmount();
};

//endregion

module.exports = Basket;