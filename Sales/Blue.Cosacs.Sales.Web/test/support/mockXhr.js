'use strict';

require('angular-mocks');

var http;


inject(function ( $httpBackend) {
    http = $httpBackend;

});

function mockXhr(){

    return http;
}

module.exports = mockXhr;