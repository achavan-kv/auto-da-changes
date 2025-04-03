'use strict';

var matrix = {
    1: ['tendered', 'change'],  //Cash
    2: ['tendered', 'change', 'currency'], //Foreign Cash
    3: ['storeCardNo', 'tendered'], //Store Card
    4: ['tendered', 'voucherNo', 'voucherIssuer'],  //Gift Voucher
    5: ['tendered', 'bank', 'chequeNo', 'bankAccountNo'],  //Cheque
    6: ['tendered', 'bank', 'cardType', 'cardNo'],  //Debit Card
    7: ['tendered', 'bank', 'bankAccountNo'],  //Standing Order
    8: ['tendered', 'chequeNo', 'currency'],  //Travellers Cheque
    9:[],//Direct Debit
    10: ['tendered', 'bank', 'cardType', 'cardNo'],  //Credit Card
    11: ['tendered', 'change']  //Optical Postings
};

module.exports = matrix;