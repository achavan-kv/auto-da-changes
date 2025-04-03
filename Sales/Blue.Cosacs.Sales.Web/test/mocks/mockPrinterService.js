'use strict';

function mockPrinterService(sandbox){
    return {
        print: sandbox.stub()
    };
}

module.exports = mockPrinterService;