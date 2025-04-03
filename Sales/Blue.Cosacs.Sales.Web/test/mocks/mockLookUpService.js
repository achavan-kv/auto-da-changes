'use strict';

function mockLookUpService(sandbox){
    return {
        k2v: sandbox.stub(),
        populate: sandbox.stub(),
        wrapList: sandbox.stub(),
        getValue: sandbox.stub(),
        getMonths: sandbox.stub()
    };
}

module.exports = mockLookUpService;