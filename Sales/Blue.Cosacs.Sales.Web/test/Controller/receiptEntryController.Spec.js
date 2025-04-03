'use strict';

require('angular-mocks');
require('../../js/index.js');

var sinon = require('../../node_modules/sinon/lib/sinon.js');

describe('Controller :: ReceiptEntryController ::', function () {
    var receiptEntryController,
            modalInstance,
            scope,
            sandbox;

    //region Setup

    beforeEach(function () {
        sandbox = sinon.sandbox.create();
    });

    beforeEach(angular.mock.module('Sales', 'Sales.common'));

    beforeEach(inject(function ($rootScope, $controller) {
        scope = $rootScope.$new();

        initMocks();

        var dependInjects = {
            $scope: scope,
            $modalInstance:modalInstance
        };

        receiptEntryController = $controller('ReceiptEntryController', dependInjects);

    }));

    afterEach(function () {
        sandbox.restore();
    });

    //endregion

    //region Tests

    describe('when initialized', function () {

        it('should provide the controller', function() {
            expect(receiptEntryController).toBeDefined();
        });

    });

    //endregion

    //region Init

    function initMocks() {

        loadMocks();
    }

    function loadMocks(){
        modalInstance = {                    // Create a mock object using spies
            close: jasmine.createSpy('modalInstance.close'),
            dismiss: jasmine.createSpy('modalInstance.dismiss'),
            result: {
                then: jasmine.createSpy('modalInstance.result.then')
            }
        };

    }

    //endregion

});