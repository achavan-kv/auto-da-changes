'use strict';
var Customer = require('../model/Customer.js');

var dependInjects = ['$scope', 'PosDataService'];

var customerController = function ($scope, PosDataService) {

    $scope.CustomerIdDisabled = false;
    $scope.customerOpType = 'NEW';
    $scope.selectedCustomerID = 0;
    $scope.showResults = false;
    $scope.allowCustomerChange = true;
    $scope.dataAvailable = false;
    $scope.readOnlyCustomerId = false;
    $scope.searchBottonEnabled = true;

    $scope.pageSize = 3; //total number records should be shown on screen
    $scope.totalItems = 0;
    $scope.currentPage = 1;
    $scope.maxSize = 0; //total number pages which should be shown in screen

    var customerDetails = new Customer();

    $scope.searchButtonClick = searchButtonClick;
    $scope.unlockButtonClick = unlockButtonClick;
    $scope.clearButtonClick = clearButtonClick;
    $scope.newBtnClick = newBtnClick;
    $scope.selectedRow = selectedRow;
    $scope.pageChanged = pageChanged;

    activate();

    function searchButtonClick() {
        $scope.currentPage = 1;
        customerDetails = new Customer();

        if (!$scope.selectedCustomerID) {
            _.merge(customerDetails, $scope.customer);
        }
        fetchData();
    }

    function clearButtonClick() {
        $scope.isCustomerDetailsLocked = false;
        $scope.readOnlyCustomerId = false;
        $scope.CustomerIdDisabled = false;
        $scope.showResults = false;
        $scope.searchBottonEnabled = true;
        $scope.currentPage = 1;
        $scope.selectedCustomerID = 0;
        $scope.customer= new Customer();
        $scope.customerOpType = 'NEW';
        cleanFunction();
    }

    function unlockButtonClick() {
        $scope.isCustomerDetailsLocked = false;
        // $scope.cart.Customer.CustomerId = null;
        $scope.customer.isSalesCustomer = true;
        $scope.readOnlyCustomerId = true;
    }

    function newBtnClick() {
        clearButtonClick();
        $scope.customerOpType = 'NEW';
        $scope.CustomerIdDisabled = true;
        $scope.searchBottonEnabled = false;
        $scope.customer.isSalesCustomer = true;
    }

    function selectedRow(item) {

        if (!item) {
            return;
        }

        $scope.selectedCustomerID = item.CustomerId;
        $scope.searchBottonEnabled = false;
        $scope.isCustomerDetailsLocked = true;

        var customer = new Customer();

        _.merge(customer, item);

        customer.isSalesCustomer = item.Id &&  _.contains (item.Id, 'Sales');
        customer.selected = true;

        cleanFunction();

        $scope.customer = customer;

        $scope.customer.DOB = new Date($scope.customer.DOB);
		$scope.customerOpType = 'UNLOCK';
    }

    function fetchData() {
        customerDetails.Start =
        (($scope.currentPage - 1) * $scope.pageSize) < 0 ? 0 : (($scope.currentPage - 1) * $scope.pageSize);
        customerDetails.Rows = $scope.pageSize;

        PosDataService.fetchCustomerDetails(customerDetails).then(function (data) {
            if (typeof data.response === 'object' && data.response.docs) {
                var queryResult = [];
                _.each(data.response.docs, function (c) {
                    var customer = new Customer();
                    _.merge(customer, c);
                    queryResult.push(customer);
                });

                $scope.customers = queryResult;

                $scope.showResults = true;

                if ($scope.customers.length < 1)
                    $scope.dataAvailable = false;
                else {
                    $scope.dataAvailable = true;
                    $scope.totalItems = data.response.numFound;
                    $scope.maxSize =
                    (Math.ceil(data.response.numFound / $scope.pageSize)) > 5 ? 5 : Math.ceil(data.response.numFound
                                                                                              / $scope.pageSize);
                    //window.scrollTo(0,400);
                }
            }
        });
    }

    function forceSelectCustomer(event, cusId, override) {
        if(!override){
            if(!$scope.customer.FirstName){
                override = true;
            }
        }
        if(override) {
            var customerDetails = new Customer();
            customerDetails.CustomerId = cusId;
            customerDetails.Start = 0;
            customerDetails.Rows = 1000;

            PosDataService.fetchCustomerDetails(customerDetails).then(function (data) {
                if (data.response && data.response.docs && data.response.docs.length > 0) {
                    var customer = new Customer();
                    _.merge(customer, data.response.docs[0]);

                    $scope.$safeApply($scope, function () {
                        selectedRow(customer);
                        $scope.customer.isCardLinked = true;
                    });
                }
            });
        }

    }

    function cleanFunction() {
        $scope.customer = new Customer();
        //window.scrollTo(0,0);
    }

    function pageChanged(currentPage) {
        $scope.currentPage = currentPage;
        fetchData();
    }

    function activate() {
        $scope.$on('pos:resetCustomerSearch', clearButtonClick);
        $scope.$on('pos:forceSelectCustomer', forceSelectCustomer);
    }
};
customerController.$inject = dependInjects;

module.exports = customerController;