'use strict';

function messagingService(sandbox){
    return {
        publish: sandbox.stub(),
        subscribe: sandbox.stub(),
        unsubscribe: sandbox.stub()
    };
}

module.exports = messagingService;