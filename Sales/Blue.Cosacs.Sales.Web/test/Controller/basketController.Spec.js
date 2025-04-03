'use strict';

require('angular-mocks');
require('../../js/index.js');

var sinon = require('../../node_modules/sinon/lib/sinon.js');
var Basket = require('../../js/model/Basket.js');

describe('Controller :: BasketController ::', function () {
    var mockCommonService,
            mockBasketService,
            mockSalesDataService,
            mockPrinterService,
            basketController,
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
            CommonService: mockCommonService,
            BasketService: mockBasketService,
            SalesDataService: mockSalesDataService,
            PrinterService:mockPrinterService
        };

        basketController = $controller('BasketController', dependInjects);

    }));

    afterEach(function () {
        sandbox.restore();
    });

    //endregion

    //region Tests

    describe('when initialized', function () {

        it('should provide the controller', function () {
            expect(basketController).toBeDefined();
        });

    });

    //endregion

    //region Init

    function initMocks() {

        loadMocks();

        var refundExchangeReasons = ['Mistaken Purchase',
                                     'Defective Product',
                                     'Wrong Product',
                                     'For Fun (Change of Mind)'];

        scope.cart = new Basket();
        scope.MasterData = refundExchangeReasons;

        var defer = q.defer();
        defer.resolve(refundExchangeReasons);

        mockBasketService.getRefundExchangeReasons.returns(defer.promise);
    }

    function loadMocks() {
        mockCommonService = require('../mocks/mockCommonService.js')(sandbox);
        mockBasketService = require('../mocks/mockBasketService.js')(sandbox);
        mockSalesDataService = require('../mocks/mockSalesDataService.js')(sandbox);
        mockPrinterService = require('../mocks/mockPrinterService.js')(sandbox);
    }

    //endregion

});