'use strict';

require('angular-mocks');
require('../../js/index.js');

var sinon = require('../../node_modules/sinon/lib/sinon.js');

describe('Controller :: SalesController ::', function () {
    var mockCommonService,
            mockBasketService,
            mockSalesDataService,
            mockLocalisationService,
            mockUsersService,
            mockPrinterService,
            mockLookupDataService,
            salesController,
            q,
            scope,
            sandbox,
            routeParams
        ;

    var currentUser = {
        "BranchNumber": 900,
        "BranchName": "BRIDGETOWN 900",
        "userId": 99999,
        "name": "System Administrator",
        "permissions": []
    };

    var localisationSettings = {"CurrencySymbol": "$", "DecimalPlaces": 2};

    //region Setup

    beforeEach(function () {
        sandbox = sinon.sandbox.create();
    });

    beforeEach(angular.mock.module('Sales', 'Sales.common'));

    beforeEach(inject(function ($rootScope, $controller, $q) {
        scope = $rootScope.$new();
        q = $q;
        routeParams = {};

        initMocks();

        var dependInjects = {
            $scope: scope,
            CommonService: mockCommonService,
            BasketService: mockBasketService,
            SalesDataService: mockSalesDataService,
            LocalisationService: mockLocalisationService,
            UsersService: mockUsersService,
            PrinterService: mockPrinterService,
            LookupDataService: mockLookupDataService,
            $routeParams : routeParams
        };

        salesController = $controller('SalesController', dependInjects);

    }));

    afterEach(function () {
        sandbox.restore();
    });

    //endregion

    //region Tests

    describe('when initialized', function () {

        it('should provide the controller', function () {
            expect(salesController).toBeDefined();
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

        scope.checkUser = function () {
            scope.branchName = currentUser.BranchName;
            scope.currentUser = currentUser;
        };
        //scope.cart = new Basket();

        var defer = q.defer();
        defer.resolve(localisationSettings);

        mockLocalisationService.getSettings.returns(defer.promise);

        mockUsersService.getCurrentUser.returns(currentUser);

        var settingsData ={
            "returnReason": [
                "Mistaken Purchase",
                "Defective Product",
                "Wrong Product",
                "For Fun (Change of Mind)"
            ],
            "taxRate": 17.5,
            "taxType": "I"
        };

        q.defer();
        defer.resolve(settingsData);

        mockSalesDataService.getSettingsData.returns(defer.promise);
    }

    function loadMocks(){
        mockCommonService = require('../mocks/mockCommonService.js')(sandbox);
        mockBasketService = require('../mocks/mockBasketService.js')(sandbox);
        mockSalesDataService = require('../mocks/mockSalesDataService.js')(sandbox);
        mockLocalisationService = require('../mocks/mockLocalisationService.js')(sandbox);
        mockUsersService = require('../mocks/mockUsersService.js')(sandbox);
        mockPrinterService = require('../mocks/mockPrinterService.js')(sandbox);
        mockLookupDataService = require('../mocks/mockLookupDataService.js')(sandbox);
    }

    //endregion

});