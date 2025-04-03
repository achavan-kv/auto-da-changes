/*global _, moment, module, S */
var searchController = function ($scope, $http, $q, $window, stringUtils, customer) {
    'use strict';

    $scope.search = {};
    //$scope.search.orderNumber = '1137008';

    $scope.orderSearchTemplate = '/Payments/views/templates/SearchResult.html';
    $scope.mainTitle = 'Payment Search';
    $scope.moment = moment;
    $scope.dateFormatShort = 'DD-MMM-YYYY';
    $scope.searchType = 'order';

    $scope.orderSearchObj = {};

    var getSearchParams = function ($scope) {
        var ret = {};
        ret.searchType = $scope.searchType;
        ret.dateFrom = stringUtils.isNullOrWhitespace($scope.search.dateFrom) ?
            null : moment($scope.search.dateFrom).format('YYYY-MM-DD');
        ret.dateTo = stringUtils.isNullOrWhitespace($scope.search.dateTo) ?
            null : moment($scope.search.dateTo).format('YYYY-MM-DD');
        ret.orderNumber = $scope.search.orderNumber;
        ret.customerId = $scope.search.customerId;
        ret.customerFirstName = $scope.search.customerFirstName;
        ret.customerLastName = $scope.search.customerLastName;
        ret.agreementNumber = $scope.search.agreementNumber;

        return ret;
    };

    var getSearchType = function (searchParams) {
        if (!stringUtils.isNullOrWhitespace(searchParams.searchType)) {
            return searchParams.searchType;
        } else if (!stringUtils.isNullOrWhitespace(searchParams.orderNumber)) {
            return 'order';
        }
    };

    var registerDeferredHandlers = function (deferredCustomer, data) {
        deferredCustomer.then(function (result) {
                data.customerObj = result[0];
            }, function (reason) {
                window.console.log('Func:registerDeferredHandler, Error loading Customer: ' + reason);
            }, function (update) {
                window.console.log('Func:registerDeferredHandler, Error loading Customer: ' + update);
            }
        );
    };

    $scope.clearSearchResults = function () {
        $scope.agreements = [];
        $scope.orders = [];
        $scope.customers = [];
    };

    $scope.clearSearchFilters = function () {
        $scope.search = {};
        $scope.clearSearchResults();
    };

    $scope.searchPayments = function () {
        $scope.clearSearchResults();

        var searchParams = getSearchParams($scope);
        var searchType = getSearchType(searchParams);

        if (searchType === 'order') {
            if (!stringUtils.isNullOrWhitespace(searchParams.orderNumber)) { // search by order number
                $http.get('/Payments/DataMocks/Sales/Order/SearchControllerMock_1.json').
                    success(function (data) {
                        data.type = "order";
                        var deferredCustomer = customer.getById(data.customer);
                        registerDeferredHandlers(deferredCustomer, data);
                        $scope.orders = [data];
                    }).
                    error(function (data, status) {
                        window.console.log("Func:searchPayments, order, Error: " + status + ";\r\n" + data);
                    });
            } else if (!stringUtils.isNullOrWhitespace(searchParams.dateFrom) || // format
                !stringUtils.isNullOrWhitespace(searchParams.dateTo)) {
                $http.get('/Payments/DataMocks/Sales/Order/SearchByDateControllerMock_1.json').
                    success(function (data) {
                        var arrayData = data.files;
                        if (_.isArray(arrayData)) {
                            _.map(arrayData, function (order) {
                                var deferredCustomer = customer.getById(order.customer);
                                registerDeferredHandlers(deferredCustomer, order);
                                order.type = "order";
                            });
                            $scope.orders = arrayData;
                        }
                    }).
                    error(function (data, status) {
                        window.console.log("Func:searchPayments, orders, Error: " + status + ";\r\n" + data);
                    });
            }
        } else if (searchType === 'agreement') {
            if (!stringUtils.isNullOrWhitespace(searchParams.agreementNumber)) { // search by agreement number
                $http.get('/Payments/DataMocks/Credit/Agreement/AgreementSearchControllerMock_1.json').
                    success(function (data) {
                        data.type = "agreement";
                        var deferredCustomer = customer.getById(data.customer);
                        registerDeferredHandlers(deferredCustomer, data);
                        $scope.agreements = [data];
                    }).
                    error(function (data, status) {
                        window.console.log("Func:searchPayments, agreement, Error: " + status + ";\r\n" + data);
                    });
            } else if (!stringUtils.isNullOrWhitespace(searchParams.dateFrom) || // format
                !stringUtils.isNullOrWhitespace(searchParams.dateTo)) {
                $http.get('/Payments/DataMocks/Credit/Agreement/AgreementSearchByDateControllerMock_1.json').
                    success(function (data) {
                        var arrayData = data.agreements;
                        if (_.isArray(arrayData)) {
                            _.map(arrayData, function (agreement) {
                                var deferredCustomer = customer.getById(agreement.customer);
                                registerDeferredHandlers(deferredCustomer, agreement);
                                agreement.type = "agreement";
                            });
                            $scope.agreements = arrayData;
                        }
                    }).
                    error(function (data, status) {
                        window.console.log("Func:searchPayments, agreements, Error: " + status + ";\r\n" + data);
                    });
            }

        } else if (searchType === 'customer') {
            if (!stringUtils.isNullOrWhitespace(searchParams.customerFirstName) || // code format
                !stringUtils.isNullOrWhitespace(searchParams.customerLastName)) {
                $http.get('/Payments/DataMocks/Credit/Customer/CustomerSearchByNameControllerMock_1.json').
                    success(function (data) {
                        data.type = "customer";
                        $scope.customers = data;
                    }).
                    error(function (data, status) {
                        window.console.log("Func:searchPayments, order, Error: " + status + ";\r\n" + data);
                    });
            }
        }
    };

    $scope.loadCustomerAgreements = function (customerResult) {
        var customerId = customerResult.Id;

        if (stringUtils.isNullOrWhitespace(customerResult.customerAgreements)) {
            if (!stringUtils.isNullOrWhitespace(customerId)) {
                $http.get('/Payments/DataMocks/Credit/CustomerAgreement/CustomerAgreementsSearchByIdMock_1.json').
                    success(function (data) {
                        data.type = "customerAgreements";
                        if (data.Agreements) {
                            customerResult.customerAgreements = data.Agreements;
                            customerResult.showCustomerAgreements = false;
                            $scope.toggleCustomerAgreements(customerResult);
                        }
                    }).
                    error(function (data, status) {
                        window.console.log("Func:searchPayments, order, Error: " + status + ";\r\n" + data);
                    });
            }
        } else {
            $scope.toggleCustomerAgreements(customerResult);
        }
    };

    $scope.toggleCustomerAgreements = function (customerResult) {
        if (!stringUtils.isNullOrWhitespace(customerResult.customerAgreements) &&
            customerResult.customerAgreements.length > 0) {
            customerResult.showCustomerAgreements =
                customerResult.showCustomerAgreements === false;
        }
    };

    $window.handleBarcodeScanned = function (data) {
        if (data && data.barcode) {
            var isInvoice = S(data.barcode).startsWith("IN$"),
                barcode = isInvoice ? S(data.barcode).strip('IN$').s : data.barcode;

            $scope.searchType = 'order';
            $scope.search.orderNumber = barcode;
            $scope.searchPayments();
        }
    };

    $window.handleCustomerCardSwept = function (data) {
        if (data && data.cardNumber) {
            $http.get('/Payments/DataMocks/Credit/CustomerStoreCard/6367050000056985.json').
                success(function (customerData) {
                    customerData.type = "customer";
                    $scope.searchType = 'customer';
                    $scope.customers = customerData;
                    $scope.openPaymentsForCustomerAgreement(customerData.Id);
                }).
                error(function (data, status) {
                    window.console.log("Func:searchPayments, order, Error: " + status + ";\r\n" + data);
                });
        }
    };

    $scope.openPaymentsForCustomerAgreement = function (customer) {
        $window.location.href = '/#/Payments/Payments?CustomerId=' + customer.Id;
    };

    $scope.openPaymentsForOrderNumber = function (order) {
        $window.location.href = '/#/Payments/Payments?OrderNumber=' + order.id;
    };

};
searchController.$inject = ['$scope', '$http', '$q', '$window', 'stringUtils', 'customer'];
module.exports = searchController;
