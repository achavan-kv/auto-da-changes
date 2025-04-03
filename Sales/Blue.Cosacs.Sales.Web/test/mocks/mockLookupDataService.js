'use strict';

function mockLookupDataService(sandbox) {
    return {
        k2v: sandbox.stub(),
        populate: sandbox.stub()
    };
}

module.exports = mockLookupDataService;