'use strict';

function mockProductsService(sandbox) {
    return {
        getAllProducts: sandbox.stub(),
        getProductByBarcode: sandbox.stub()
    };
}

module.exports = mockProductsService;