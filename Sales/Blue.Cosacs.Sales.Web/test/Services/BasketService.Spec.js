'use strict';

require('angular-mocks');
require('../../js/index.js');
var sinon = require('../../node_modules/sinon/lib/sinon.js'),
    Fakes  = require('../support/fakes.js');

var Basket = require('../../js/model/Basket.js');

//require('../../node_modules/jasmine-sinon/lib/jasmine-sinon.js');

describe('Services :: BasketService :: ', function () {
    var _basketService,
            mockSalesDataService,
            mockCommonService,
            mockPrinterService,
            mockLookUpService,
            q,
            scope,
            sandbox;

    var taxSettingsData = {
        TaxRate: 20,
        TaxType: "I"
    };

    var saveSalesReturn = {
        valid: true,
        invoiceNo: 1,
        errors: '',
        failedPayments: {
            paymentMethodId: 0,
            availableBalance: 0,
            storeCardNo: ''
        }
    };

    //region Setup

    beforeEach(angular.mock.module('Sales', 'Sales.common'));

    beforeEach(function () {
        sandbox = sinon.sandbox.create();
    });

    // mock dependencies
    beforeEach(angular.mock.module("Sales.common", function ($provide) {
        loadMocks();

        $provide.value("SalesDataService", mockSalesDataService);
        $provide.value("CommonService", mockCommonService);
        $provide.value("PrinterService", mockPrinterService);
        $provide.value("LookupService", mockLookUpService);
        $provide.value("padNoFilter", function () {
            return sandbox.stub();
        });


    }));

    // create the service under test
    beforeEach(inject(function (CommonService, BasketService, SalesDataService,LookupService, padNoFilter,PrinterService, $q, $rootScope) {
        q = $q;
        scope = $rootScope.$new();
        _basketService = BasketService;

        initMocks();
    }));

    afterEach(function () {
        sandbox.restore();
    });

    // beforeEach(inject(function ($injector) {
    // _basketService = $injector.get('BasketService');
    //
    // }));

    //endregion

    //region Tests

    describe('when initialized', function () {

        it('should provide the service', function () {
            expect(_basketService).toBeDefined();
        });

    });

    describe('when starting a new Sale', function () {

        it('Should create a new basket', function () {
            _basketService.startNewSale();

            expect(_basketService.cart).toBeDefined();
            expect(_basketService.cart.items.length).toEqual(0);
            expect(_basketService.cart.itemsTotal).toEqual(0);
        });

        it('Should allow basket changes', function () {
            _basketService.startNewSale();

            expect(_basketService.cart).toBeDefined();
            expect(_basketService.cart.basketChangesAllowed).toBeTruthy();
        });

        xit('should call \'SalesDataService.getTaxSettingsData\' method', function () {
            _basketService.startNewSale();

            expect(mockSalesDataService.getTaxSettingsData.called).toBeTruthy();
        });

        xit('should set Basket\'s Tax Settings', function () {
            var defer = q.defer();
            defer.resolve({taxType: "I", taxRate: 50});
            mockSalesDataService.getTaxSettingsData.returns(defer.promise);

            _basketService.startNewSale();

            // we need to call a digest in order to resolve a promise
            scope.$root.$apply();

            expect(_basketService.cart).toBeDefined();
            expect(_basketService.cart.taxType).toEqual('I');
            expect(_basketService.cart.taxRate).toEqual(50);
        });

    });

    describe('when completing a Sale', function () {

        it('should call \'SalesDataService.saveSales\' method', function () {
            var defer = q.defer();
            defer.resolve(saveSalesReturn);
            mockSalesDataService.saveSales.returns(defer.promise);

            _basketService.completeSales();

            // we need to call a digest in order to resolve a promise
            scope.$root.$apply();

            expect(mockSalesDataService.saveSales.called).toBeTruthy();
        });

        xit('should set Cart\'s Invoice No', function () {
            var expected = parseInt(faker.finance.account());
            var defer = q.defer();
            saveSalesReturn.invoiceNo = expected;
            defer.resolve(saveSalesReturn);
            var cart = _basketService.cart;

            mockSalesDataService.saveSales.returns(defer.promise);

            _basketService.completeSales(cart);

            // we need to call a digest in order to resolve a promise
            scope.$root.$apply();

            expect(cart.InvoiceNo).toEqual(expected);
        });

        it('should set Cart\'s Sale to complete', function () {
            var defer = q.defer();
            defer.resolve({valid: true});
            var cart = _basketService.cart;

            mockSalesDataService.saveSales.returns(defer.promise);

            _basketService.completeSales(cart);

            // we need to call a digest in order to resolve a promise
            scope.$root.$apply();

            expect(cart.saleComplete).toBeTruthy();
        });

        it('should reset Cart\'s items when \'emptyItems\' flag is set', function () {
            var emptyItems = true;
            var defer = q.defer();
            defer.resolve({valid: true});
            var cart = _basketService.cart;

            mockSalesDataService.saveSales.returns(defer.promise);

            _basketService.completeSales(cart, emptyItems);
            scope.$root.$apply();

            expect(cart.items).toEqual([]);
        });

    });

    //endregion

    //region Init

    function initMocks() {
        var defer = q.defer();
        defer.resolve(taxSettingsData);

        mockSalesDataService.getTaxSettingsData.returns(defer.promise);

        defer = q.defer();
        defer.resolve([]);
        mockSalesDataService.getWarrantyContractNosData.returns(defer.promise);

        defer = q.defer();
        defer.resolve(null);
        mockSalesDataService.saveSales.returns(defer.promise);

        defer = q.defer();
        defer.resolve([]);
        mockSalesDataService.getExistingOrders.returns(defer.promise);

        defer = q.defer();
        defer.resolve(Fakes.Users);
        mockLookUpService.populate.returns(defer.promise);

    }

    function loadMocks() {
        mockSalesDataService = require('../mocks/mockSalesDataService.js')(sandbox);
        mockCommonService = require('../mocks/mockCommonService.js')(sandbox);
        mockPrinterService = require('../mocks/mockPrinterService.js')(sandbox);
        mockLookUpService = require('../mocks/mockLookUpService.js')(sandbox);
    }

    //endregion

});

