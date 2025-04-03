'use strict';

var SalesEnums = require('./SalesEnums'),
    Utilities = require('./Utilities'),
    CartItem = require('./CartItem'),
    Price = require('./Price'),
    cls = CartItemFactory.prototype;

var _options = {
    taxType: SalesEnums.TaxTypes.Exclusive,
    taxRate: 0,
    isTaxFree: false,
    isDutyFree: false,
    branchNo: 0
};

function CartItemFactory(options) {
    _options.taxType = options.taxType || SalesEnums.TaxTypes.Exclusive;
    _options.taxRate = options.taxRate || 0;
    _options.isTaxFree = options.isTaxFree || false;
    _options.isDutyFree = options.isDutyFree || false;
    _options.branchNo = options.branchNo || 0;
}

cls.createCartItem = function (itemType, newItem) {
    var item = new CartItem(_options);

    switch (itemType) {
        case SalesEnums.CartItemTypes.Warranty:
            return mapWarranty(item, newItem);
            break;
        case SalesEnums.CartItemTypes.Installation:
            return mapInstallation(item, newItem);
            break;
        case SalesEnums.CartItemTypes.Discount:
            return mapDiscount(item, newItem);
            break;
        default:
            return mapProductItem(item, newItem);
        //defaults to CartItemTypes.Product

    }
};

//region Private Methods

function mapWarranty(warranty, w) {
    warranty.itemTypeId = SalesEnums.CartItemTypes.Warranty;
    warranty.taxRate = w.warrantyLink.TaxRate || _options.taxRate;

    warranty.warrantyLengthMonths = w.warrantyLink.Length;
    warranty.warrantyMaxLength = w.warrantyLink.Length;
    warranty.warrantyTypeCode = w.warrantyLink.TypeCode;
    warranty.warrantyLinkId = w.warrantyLink.Id;
    warranty.description = trimDescription(w.warrantyLink.Description);
    warranty.posDescription = trimDescription(w.warrantyLink.posDescription) || warranty.description;
    warranty.warrantyEffectiveDate = moment().toDate();
    warranty.itemNo = w.warrantyLink.Number;
    warranty.productItemId = w.price ? w.price.WarrantyId : null;

    var newPriceData = new Price,
        taxAmount = 0,
        retailPrice = 0,
        priceTaxInclusive = 0,
        costPrice = 0;

    if (w.promotion) {
        var p = w.promotion.Price || 0;

        retailPrice =
            _options.taxType == SalesEnums.TaxTypes.Exclusive ? p : Utilities.excludeTax(p, warranty.taxRate) || 0;
    }
    else {
        retailPrice = w.price.RetailPrice || 0;
        costPrice = w.price.CostPrice || 0;

        // taxAmount = w.price.TaxAmount || 0;
        // priceTaxInclusive = w.price.TaxInclusivePriceChange || (retailPrice + taxAmount);
    }

    priceTaxInclusive = Utilities.includeTax(retailPrice, warranty.taxRate) || 0;
    taxAmount = Math.abs((priceTaxInclusive - retailPrice) || 0);

    newPriceData.regularPrice = retailPrice;
    newPriceData.cashPrice = retailPrice;
    newPriceData.dutyFreePrice = retailPrice;

    newPriceData.regularPriceTaxInclusive = priceTaxInclusive;
    newPriceData.cashPriceTaxInclusive = priceTaxInclusive;
    newPriceData.dutyFreePriceTaxInclusive = priceTaxInclusive;

    newPriceData.taxRate = warranty.taxRate;

    warranty.priceData = newPriceData;

    // warranty.price = Utilities.roundFloat(w.price.RetailPrice);
    warranty.taxAmount = taxAmount;
    warranty.costPrice = costPrice;
    warranty.retailPrice = retailPrice;


    warranty.dutyFreePrice = retailPrice;
    warranty.dutyFreePriceInclusive = priceTaxInclusive || 0;
    warranty.taxExclusivePrice = retailPrice;
    warranty.taxInclusivePrice = priceTaxInclusive;

    if (w.warrantyLink && w.warrantyLink.WarrantyTags && w.warrantyLink.WarrantyTags.length > 0) {
        var tag = _.find(w.warrantyLink.WarrantyTags, {LevelId: 3});

        if (tag && tag.TagName) {

            warranty.department = tag.TagName;
        }
    }

    return warranty;
}

function mapProductItem(item, p) {
    var newPriceData = new Price;

    if (p.priceData) {
        _.extend(newPriceData, p.priceData);
        newPriceData.taxRate = newPriceData.taxRate * 100; //As on POS, warranty and Non Stock we store tax rate  in shown format 12.5 not 0.125 like merchandising
    }

    if (p.promoData) {
        var newProPrice = new Price;
        _.extend(newProPrice, p.promoData);

        newProPrice.cashPrice = newProPrice.cashPrice || newPriceData.cashPrice;
        newProPrice.regularPrice = newProPrice.regularPrice || newPriceData.regularPrice;
        newProPrice.dutyFreePrice = newProPrice.dutyFreePrice || newPriceData.dutyFreePrice;
        newProPrice.regularPriceTaxInclusive =
            newProPrice.regularPriceTaxInclusive || newPriceData.regularPriceTaxInclusive;
        newProPrice.cashPriceTaxInclusive =
            newProPrice.cashPriceTaxInclusive || newPriceData.cashPriceTaxInclusive;
        newProPrice.dutyFreePriceTaxInclusive =
            newProPrice.dutyFreePriceTaxInclusive || newPriceData.dutyFreePriceTaxInclusive;
        newProPrice.taxRate = newProPrice.taxRate * 100; //As on POS, warranty and Non Stock we store tax rate  in shown format 12.5 not 0.125 like merchandising

        newPriceData = newProPrice;
        item.promoPrice = newProPrice.cashPrice || 0;
        item.hasPromotionPrice = true;
    }

    item.itemTypeId = p.isSet ? SalesEnums.CartItemTypes.Kit : SalesEnums.CartItemTypes.Product;
    item.priceData = newPriceData;
    item.dutyFreePrice = newPriceData.dutyFreePrice || 0;
    item.dutyFreePriceInclusive = newPriceData.dutyFreePriceTaxInclusive || 0;
    item.taxExclusivePrice = newPriceData.cashPrice || 0;
    item.taxInclusivePrice = newPriceData.cashPriceTaxInclusive || 0;

    item.taxRate = newPriceData.taxRate || _options.taxRate;
    item.itemNo = p.productItemNo;
    item.productItemId = p.productItemId;
    item.warrantyLinkId = p.productItemNo; //TODO: Remove this
    item.manualTaxAmount = 0;// p.ManualTaxAmount; TODO: Check this
    item.description = p.description;
    item.posDescription = p.posDescription || item.description;
    item.itemSupplier = 'NA';
    var taxAmount = p.taxAmount || (item.taxInclusivePrice - item.taxExclusivePrice) || 0;
    item.taxAmount = taxAmount;

    //item.itemSupplier = p.ItemSupplier ? p.ItemSupplier.trim() : p.ItemSupplier;

    var hierarchyTags = p.hierarchyTags;
    if (hierarchyTags && hierarchyTags.length > 0) {
        var departmentTag = _.find(hierarchyTags, {"levelId": 1}),
            categoryTag = _.find(hierarchyTags, {"levelId": 2}),
            classTag = _.find(hierarchyTags, {"levelId": 3});

        item.department = departmentTag ? departmentTag.tagName : '';
        item.category = categoryTag ? categoryTag.tagId : 0;
        item.class = classTag ? classTag.tagName : '';
    }

    // for kit items
    item.parentId = p.parentId;
    item.quantity = p.quantity || 1;
    item.isKitParent = p.IsKitParent || p.isSet;
    item.kitDiscount = p.KitDiscount;

    return item;
}

function mapInstallation(installation, i) {
    installation.itemTypeId = SalesEnums.CartItemTypes.Installation;
    installation.taxRate = i.TaxRate || _options.taxRate;
    installation.taxExclusivePrice = i.TaxExclusivePrice || 0;
    installation.taxIclusivePrice = i.TaxInclusivePrice || 0;
    installation.taxAmount = i.TaxAmount;
    installation.dutyFreePrice = i.DutyFreePrice || 0;

    installation.description = trimDescription(i.ItemDescription1, i.ItemDescription2);
    installation.posDescription = trimDescription(i.posDescription) || installation.description;
    installation.itemNo = i.ItemNo;
    installation.costPrice = i.CostPrice;
    //installation.taxAmount = i.TaxAmount;
    installation.productItemId = i.ItemId;

    var dutyPriceTaxInclusive = Utilities.includeTax(installation.dutyFreePrice, installation.taxRate) || 0;
    installation.dutyFreePriceInclusive = dutyPriceTaxInclusive;

    return installation;
}

function mapDiscount(item, d) {
    item.itemTypeId = SalesEnums.CartItemTypes.Discount;
    item.itemNo = d.code;
    item.description = d.item.description;
    item.posDescription = d.item.posDescription || item.description;
}

function trimDescription(d1, d2) {
    if (!d1 && !d2) {
        return '';
    }

    var description1 = d1 || '',
        description2 = d2 || '';

    return description1.trim() + ' ' + description2.trim();
}

//endregion

module.exports = CartItemFactory;