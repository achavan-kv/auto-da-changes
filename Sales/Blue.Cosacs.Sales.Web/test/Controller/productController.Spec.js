'use strict';

require('angular-mocks');
require('../../js/index.js');

var sinon = require('../../node_modules/sinon/lib/sinon.js');
var fakes = require('../support/fakes');
var Basket = require('../../js/model/Basket.js');

var searchedProduct ={};

describe('Controller :: ProductController ::', function () {
    var mockCommonService,
            mockProductsService,
            productController,
            scope,
            _window,
            q,
            rootScope,
            sandbox;

    //region Setup

    beforeEach(function () {
        sandbox = sinon.sandbox.create();
    });

    beforeEach(angular.mock.module('Sales', 'Sales.common'));

    beforeEach(inject(function ($rootScope, $controller, $window, $q) {
        rootScope=$rootScope;
        scope = $rootScope.$new();
        _window = $window;
        q = $q;

        initMocks();

        var dependInjects = {
            $scope: scope,
            CommonService: mockCommonService,
            ProductsService: mockProductsService
        };

        productController = $controller('ProductController', dependInjects);

    }));

    afterEach(function () {
        sandbox.restore();
    });

    //endregion

    //region Tests

    describe('when initialized', function () {

        it('should provide the controller', function () {
            expect(productController).toBeDefined();
        });

    });

    describe('when barcode scanned', function () {

        it('should broadcast scan event', function () {
            var barcode = chance.integer({min: 100, max: 500});
            spyOn(mockCommonService, '$broadcast');

            _window.handleBarcodeScanned({barcode:barcode});

           expect(mockCommonService.$broadcast).toHaveBeenCalledWith('shell:barcode:scan', barcode);
        });

        it('should handle scan event', function () {
            var barcode = chance.integer({min: 100, max: 500});
            rootScope.$broadcast('shell:barcode:scan', barcode);

            expect(mockProductsService.getProductByBarcode.called).toBeTruthy();
        });

    });

    //endregion

    //region Init

    function initMocks() {

        loadMocks();

        scope.$safeApply = saveApply;
        scope.currentUser = fakes.CurrentUser;

        var defer = q.defer();
        defer.resolve(searchedProduct);

        mockProductsService.getProductByBarcode.returns(defer.promise);
    }

    function saveApply($scope, fn) {
        fn = fn || function () {
        };
        if ($scope.$$phase) {
            fn();
        }
        else {
            $scope.$apply(fn);
        }
    }

    function loadMocks() {
        mockCommonService = require('../mocks/mockCommonService.js')(sandbox);
        mockProductsService = require('../mocks/mockProductsService.js')(sandbox);
    }

    //endregion

});