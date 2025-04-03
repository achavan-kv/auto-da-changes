'use strict';


var warrantyTypeEnum = Object.freeze({Extended: 'E', Free: 'F', InstantReplacement: 'I'});
exports.WarrantyTypes = warrantyTypeEnum;

var taxTypeEnum = Object.freeze({Exclusive: 'E', Inclusive: 'I'});
exports.TaxTypes = taxTypeEnum;

var paymentMethodTypeEnum = Object.freeze({
                                              Cash: 1,
                                              ForeignCash: 2,
                                              StoreCard: 3,
                                              GiftVoucher: 4,
                                              Cheque: 5,
                                              DebitCard: 6,
                                              StandingOrder: 7,
                                              TravellersCheque: 8,
                                              DirectDebit: 9,
                                              CreditCard: 10,
                                              OpticalPostings: 11
                                          });
exports.PaymentMethods = paymentMethodTypeEnum;

var cartItemTypeEnum = Object.freeze({
                                         Product: 1,
                                         Warranty: 2,
                                         Installation: 3,
                                         NonStock: 4,
                                         Discount: 5,
                                         Kit: 8
                                     });
exports.CartItemTypes = cartItemTypeEnum;

var returnTypeEnum = Object.freeze({
                                       Remote: 1,
                                       Manual: 2,
                                       Exchange: 3,
                                       None: 4
                                   });
exports.ReturnType = returnTypeEnum;

var permissionEnum = Object.freeze({
                                       SalesMenu: 8800,
                                       PosMenu: 8801,
                                       SearchOrders: 8802,
                                       ChangeDiscountLimitSetup: 8805,
                                       ContractsSetup: 8806,
                                       AuthoriseRefundExchange: 8807,
                                       AuthoriseManualRefundExchange: 8808,
                                       EditStoreCardNo: 8809,
                                       ReprintOrderReceipts: 8810,
                                       AuthoriseDiscountLimit: 8811
                                   });
exports.Permissions = permissionEnum;