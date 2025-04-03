'use strict';

require('angular-mocks');
require('angular');
require('../../js/index.js');
var sinon = require('../../node_modules/sinon/lib/sinon.js');

describe('Services :: ProductsService :: ', function () {
    var productsService,
            mockSalesDataService,
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
    });

    beforeEach(angular.mock.module('Sales', 'Sales.common', 'mockModule'));

    // create the service under test
    beforeEach(inject(function (SalesDataService, ProductsService, $q, $rootScope) {
        q = $q;
        scope = $rootScope.$new();
        productsService = ProductsService;

    }));

    //endregion

    //region Tests

    describe('when initialized', function () {

        it('should provide the service', function () {
            expect(productsService).toBeDefined();
        });

    });

    //endregion

    //region Init

    function initMocks() {

        loadMocks();

    }

    function loadMocks() {
        mockSalesDataService = require('../mocks/mockSalesDataService.js')(sandbox);
    }

    //endregion

});