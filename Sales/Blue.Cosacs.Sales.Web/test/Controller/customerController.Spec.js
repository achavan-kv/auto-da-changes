'use strict';

require('angular-mocks');
require('../../js/index.js');

var sinon = require('../../node_modules/sinon/lib/sinon.js');

// TODO: fake this
var customerDetails = [];

describe('Controller :: CustomerController ::', function () {
    var customerController,
            mockSalesDataService,
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
            SalesDataService: mockSalesDataService
        };

        customerController = $controller('CustomerController', dependInjects);

    }));

    afterEach(function () {
        sandbox.restore();
    });

    //endregion

    //region Tests

    describe('when initialized', function () {

        it('should provide the controller', function () {
            expect(customerController).toBeDefined();
        });

    });

    //endregion

    //region Init

    function initMocks() {

        loadMocks();

        var defer = q.defer();
        defer.resolve(customerDetails);

        mockSalesDataService.fetchCustomerDetails.returns(defer.promise);

    }

    function loadMocks() {
        mockSalesDataService = require('../mocks/mockSalesDataService.js')(sandbox);
    }

    //endregion


});