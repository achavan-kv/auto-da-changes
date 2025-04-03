'use strict';

function mockPaymentService(sandbox){
    return {
        getExchangeRate: sandbox.stub(),
        getPaymentMethods:sandbox.stub(),
        getCurrencyCodes:sandbox.stub()
    };
}

module.exports = mockPaymentService;