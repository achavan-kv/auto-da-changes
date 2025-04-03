'use strict';

require('angular-mocks');
require('../../js/index.js');

var sinon = require('../../node_modules/sinon/lib/sinon.js');

// TODO: fake this
var paymentMethods = [];
var currencyCodes = [];

describe('Controller :: PaymentController ::', function () {
    var mockCommonService,
            mockPaymentService,
            paymentController,
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
            PaymentService:mockPaymentService
        };

        paymentController = $controller('PaymentController', dependInjects);

    }));

    afterEach(function () {
        sandbox.restore();
    });

    //endregion

    //region Tests

    describe('when initialized', function () {

        it('should provide the controller', function() {
            expect(paymentController).toBeDefined();
        });

    });

    //endregion

    //region Init

    function initMocks() {

        loadMocks();

        var defer = q.defer();
        defer.resolve(paymentMethods);

        mockPaymentService.getPaymentMethods.returns(defer.promise);

        defer = q.defer();
        defer.resolve(currencyCodes);

        mockPaymentService.getCurrencyCodes.returns(defer.promise);
    }

    function loadMocks(){
        mockCommonService = require('../mocks/mockCommonService.js')(sandbox);
        mockPaymentService = require('../mocks/mockPaymentService.js')(sandbox);
    }

    //endregion

});