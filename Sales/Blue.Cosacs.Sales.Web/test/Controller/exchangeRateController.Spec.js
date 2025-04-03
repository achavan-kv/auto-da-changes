'use strict';

require('angular-mocks');
require('../../js/index.js');

var sinon = require('../../node_modules/sinon/lib/sinon.js');

// TODO: fake this with data
var currencyData = [];
var exchangeRateDetails = [];

describe('Controller :: ExchangeRateController ::', function () {
    var mockSalesDataService,
        mockCommonService,
        exchangeRateController,
        q,
        scope,
        sandbox;

    //region Setup

    beforeEach(function () {
        sandbox = sinon.sandbox.create();
    });

    beforeEach(angular.mock.module('Sales', 'Sales.common'));

    beforeEach(inject(function ($rootScope, $controller, $q) {
        scope = $rootScope.$new();
        q = $q;

        initMocks();

        var dependInjects = {
            $scope: scope,
            SalesDataService: mockSalesDataService,
            CommonService: mockCommonService
        };

        exchangeRateController = $controller('ExchangeRateController', dependInjects);

    }));

    afterEach(function () {
        sandbox.restore();
    });

    //endregion

    //region Tests

    describe('when initialized', function () {

        it('should provide the controller', function () {
            expect(exchangeRateController).toBeDefined();
        });

    });

    //endregion

    //region Init

    function initMocks() {

        loadMocks();

        var defer = q.defer();
        defer.resolve(currencyData);

        mockSalesDataService.getCurrencyData.returns(defer.promise);

        defer = q.defer();
        defer.resolve(exchangeRateDetails);

        mockSalesDataService.getExchangeRateDetails.returns(defer.promise);
    }

    function loadMocks() {
        mockSalesDataService = require('../mocks/mockSalesDataService.js')(sandbox);
        mockCommonService = require('../mocks/mockCommonService.js')(sandbox);
    }

    //endregion

});