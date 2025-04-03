'use strict';

var Basket = require('../../js/model/Basket.js');

function mockBasketService(sandbox){
    return {
        cart: new Basket(),
        startNewSale: sandbox.stub(),
        completeSales: sandbox.stub(),
        getWarrantyContract: sandbox.stub(),
        getOrderForReceipt: sandbox.stub(),
        getRefundExchangeReasons:sandbox.stub()
    };
}

module.exports = mockBasketService;