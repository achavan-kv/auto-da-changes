'use strict';

function mockCommonService(sandbox) {

    return {
        $q: sandbox.stub(),
        $timeout: sandbox.stub(),
        $log: sandbox.stub(),
        $dialog: {
            prompt: sandbox.stub(),
            alert: sandbox.stub(),
            error: sandbox.stub(),
            dialog: sandbox.stub(),
            messageBox: sandbox.stub()
        },
        $broadcast: sandbox.stub(),
        alert: sandbox.stub(),
        error: sandbox.stub(),
        addGrowl: sandbox.stub(),
        handleAjaxError: sandbox.stub(),
        handleAjaxSuccess: sandbox.stub(),
        messaging: {
            publish: sandbox.stub(),
            subscribe: sandbox.stub(),
            unsubscribe: sandbox.stub()
        }
    };

}

module.exports = mockCommonService;