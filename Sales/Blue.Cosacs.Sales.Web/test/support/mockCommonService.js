'use strict';

require('angular-mocks');

var q,
    timeout,
    log;


inject(function ( $q,$timeout,$log) {
    q = $q;
    timeout=$timeout;
    log = $log;

});

function mockCommonService(){
    return {
        // common angular dependencies
        $q: q,
        $timeout: timeout,
        $log: log,
        $dialog:{}, // $dialog,
        $broadcast: {}, //$broadcast,
        // generic
        alert:function(){},
        error:function(){},
        nullDate: new Date(1900, 0, 1),
        handleAjaxError: function(){},
        handleAjaxSuccess:  function(){}
    };
}

module.exports = mockCommonService;