'use strict';

require('angular-mocks');
require('angular');
require('../../js/index.js');
var sinon = require('../../node_modules/sinon/lib/sinon.js');

//require('../../node_modules/jasmine-sinon/lib/jasmine-sinon.js');

describe('Services :: SalesDataService :: ', function () {
    var salesDataService,
            mockCommonService,
            httpBackend,
            q,
            scope,
            sandbox;

    //region Setup

    beforeEach(function () {
        sandbox = sinon.sandbox.create();
    });

    afterEach(function () {
        sandbox.restore();
    });

    // mock dependencies
    beforeEach(function () {
        initMocks();

       var mockModule = angular.module('mockModule',[]);
        mockModule.value('CommonService', mockCommonService);
        mockModule.value('xhr', httpBackend);
    });

    beforeEach(angular.mock.module('Sales', 'Sales.common','mockModule'));

    // create the service under test
    beforeEach(inject(function (CommonService, SalesDataService, $q, $rootScope, $httpBackend) {
        q = $q;
        scope = $rootScope.$new();
        httpBackend = $httpBackend;
        salesDataService = SalesDataService;

    }));

    //endregion

    //region Tests

    describe('when initialized', function () {

        it('should provide the service', function () {
            expect(salesDataService).toBeDefined();
        });

    });

    //endregion

    //region Init

    function initMocks() {

        loadMocks();

    }

    function loadMocks() {
        mockCommonService = require('../mocks/mockCommonService.js')(sandbox);
         }

    //endregion

});