'use strict';

require('angular-mocks');
require('angular');
require('../../js/index.js');
var sinon = require('../../node_modules/sinon/lib/sinon.js');

describe('Services :: PaymentService :: ', function () {
    var paymentService,
            mockSalesDataService,
            mockCommonService,
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

        var mockModule = angular.module('mockModule', []);
        mockModule.value('SalesDataService', mockSalesDataService);
        mockModule.value("CommonService", mockCommonService);
    });

    beforeEach(angular.mock.module('Sales', 'Sales.common', 'mockModule'));

    // create the service under test
    beforeEach(inject(function (CommonService, SalesDataService, PaymentService, $q, $rootScope) {
        q = $q;
        scope = $rootScope.$new();
        paymentService = PaymentService;

    }));

    //endregion

    //region Tests

    describe('when initialized', function () {

        it('should provide the service', function () {
            expect(paymentService).toBeDefined();
        });

    });

    //endregion

    //region Init

    function initMocks() {

        loadMocks();

    }

    function loadMocks() {
        mockSalesDataService = require('../mocks/mockSalesDataService.js')(sandbox);
        mockCommonService = require('../mocks/mockCommonService.js')(sandbox);
    }

    //endregion

});