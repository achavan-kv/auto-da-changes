'use strict';

function mockLocalisationService(sandbox){
    return {
        fetchLocalisationSettings: sandbox.stub(),
        getSettings:sandbox.stub()
    };
}

module.exports = mockLocalisationService;