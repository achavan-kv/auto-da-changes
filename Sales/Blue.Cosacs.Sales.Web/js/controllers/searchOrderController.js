'use strict';

var dependInjects = ['$scope', 'CommonService', 'PosDataService', 'PrinterService', '$location', 'SearchOrderService',
                     'LookupService', 'padNoFilter'];

var SalesEnums = require('../model/SalesEnums');

function searchOrderController($scope, CommonService, PosDataService, PrinterService, $location, SearchOrderService,
                               LookupService, padNoFilter) {

    $scope.searchObject = SearchOrderService.receiptSearch;

    $scope.orderItemsResult = [];
    $scope.selectedInvoiceNo;

    $scope.dataAvailable = false;
    $scope.showResults = false;

    $scope.itemsPerPage = 8;
    $scope.totalItems = 0;
    $scope.currentPage = 1;
    $scope.maxSize = 0;
    $scope.numPages = 0;

    $scope.pageChanged = pageChanged;
    $scope.searchOrder = searchOrder;
    $scope.selectedRow = selectedRow;
    $scope.clear = clear;
    $scope.printOrder = printOrder;
    $scope.printAll = printAll;
    $scope.printContract = printContract;
    $scope.allowPrint = false;
    activate();
    ////


    ///

    function activate() {
        $scope.searchObject.branchNo = $scope.BranchNumber.toString();
        angular.element('body').removeClass('full-screen');
        if (!_.isEmpty($location.search())) {
            $scope.searchObject.branchNo = $location.search().branchNo ? $location.search().branchNo : null;
            $scope.searchObject.dateFrom = new Date($location.search().dateFrom);
            $scope.searchObject.dateTo = new Date($location.search().dateTo);
            $scope.searchObject.invoiceNoMin =
                $location.search().invoiceNoMin ? parseInt($location.search().invoiceNoMin) : null;
            $scope.searchObject.invoiceNoMax =
                $location.search().invoiceNoMax ? parseInt($location.search().invoiceNoMax) : null;
            searchOrder();
        }

        if (!$scope.MasterData) {
            $scope.MasterData = {};
        }
        getSettings();

        PosDataService.hasPermission(SalesEnums.Permissions.ReprintOrderReceipts).then(function (data) {
            $scope.allowPrint = data || false;
        });
    }

    function getSettings() {
        PosDataService.getSettingsData().then(function (data) {
            if (data) {
                $scope.MasterData.settings = data;

            }
        });
    }

    function searchOrder() {
        if (!($scope.searchObject.dateFrom && $scope.searchObject.dateTo)) {
            CommonService.addGrowl({
                timeout: 5,
                type: 'danger',
                content: "Required field/s are missing"
            });
            return;
        }
        var params = {
            branchNo: $scope.searchObject.branchNo || 0,
            dateFrom: $scope.searchObject.dateFrom,
            dateTo: $scope.searchObject.dateTo,
            invoiceNoMin: $scope.searchObject.invoiceNoMin || 0,
            invoiceNoMax: $scope.searchObject.invoiceNoMax || 0,
            start: (($scope.currentPage - 1) * $scope.itemsPerPage) < 0 ? 0 :
                   (($scope.currentPage - 1) * $scope.itemsPerPage),
            rows: $scope.itemsPerPage
        };

        $location.search({
            branchNo: $scope.searchObject.branchNo,
            dateFrom: moment($scope.searchObject.dateFrom).format('YYYY-MM-DD'),
            dateTo: moment($scope.searchObject.dateTo).format('YYYY-MM-DD'),
            invoiceNoMin: $scope.searchObject.invoiceNoMin,
            invoiceNoMax: $scope.searchObject.invoiceNoMax
        });

        PosDataService.getExistingOrders(params).then(function (data) {
            if (data) {
                $scope.showResults = true;
            }
            if (data && data.count > 0) {
                $scope.dataAvailable = true;
                $scope.orderItemsResult = data.ordersList;
                $scope.totalItems = data.count;
                $scope.maxSize = (Math.ceil($scope.totalItems / $scope.itemsPerPage)) > 5 ? 5 :
                                 Math.ceil($scope.totalItems / $scope.itemsPerPage);
            }
            else {
                $scope.dataAvailable = false;
            }
        });
    }

    function selectedRow() {
        $scope.selectedInvoiceNo = this.item.invoiceNo;
    }

    function pageChanged(currentPage) {
        $scope.currentPage = currentPage;
        searchOrder();
    }

    function clear() {
        $scope.dataAvailable = false;
        $scope.showResults = false;

        $scope.searchObject = {
            branchNo: $scope.BranchNumber,
            dateFrom: new Date(),
            dateTo: new Date(),
            invoiceNoMin: null,
            invoiceNoMax: null,
            start: 0,
            rows: 8
        };
    }

    function printOrder() {
        $scope.selectedInvoiceNo = this.item.invoiceNo;
        var params = {
            invoiceNo: $scope.selectedInvoiceNo,
            copyCount: 1,
            receiptType: "Reprint",
            isThermalPrint: PrinterService.canUseThermalPrinter
        };

        PosDataService.printReceipt(params).then(function (data) {

            if (PrinterService.canUseThermalPrinter) {
                var printer = $scope.MasterData.settings.thermalPrinterName || "EPSON TM-T70 Receipt";

                PrinterService.thermalPrint(printer, data);
            } else {
                PrinterService.printHtml(data);
            }
        });
    }

    function printContract(agreementNo, contractNo) {
        var contractNos = [];
        contractNos.push(contractNo);

        var params = {
            agreementNo: agreementNo,
            contractNos: contractNo,
            multiple: true
        };

        PosDataService.printWarrantyContract(params).then(function (data) {

            PrinterService.printHtml(data);
        });
    }

    function printAll() {
        if (!($scope.searchObject.dateFrom && $scope.searchObject.dateTo)) {
            CommonService.addGrowl({
                timeout: 5,
                type: 'danger',
                content: "Required field/s are missing"
            });
            return;
        }

        var params = {
            branchNo: $scope.searchObject.branchNo || 0,
            dateFrom: $scope.searchObject.dateFrom,
            dateTo: $scope.searchObject.dateTo,
            invoiceNoMin: $scope.searchObject.invoiceNoMin || 0,
            invoiceNoMax: $scope.searchObject.invoiceNoMax || 0,
            isThermalPrint: PrinterService.canUseThermalPrinter
        };

        PosDataService.printReceipt(params).then(function (data) {

            if (PrinterService.canUseThermalPrinter) {
                var printer = $scope.MasterData.settings.thermalPrinterName || "EPSON TM-T70 Receipt";

                PrinterService.thermalPrint(printer, data);
            } else {
                PrinterService.printHtml(data);
            }
        });

    }

    function enrichAndPrint(data) {
        var timeout = 1000;
        var selectedOrders = {
            orders: []
        };
        if (data.constructor === Array) {
            selectedOrders.orders = data;
            timeout = 5000;
        }
        else {
            selectedOrders.orders.push(data);
        }

        LookupService.populate('USERS').then(function (data) {
            _.each(selectedOrders.orders, function (selectedOrder) {
                selectedOrder.id = padNoFilter(selectedOrder.id, 10);
                selectedOrder.currentUser = data[$scope.currentUser.userId];
                selectedOrder.createdBy = data[selectedOrder.createdBy];
                selectedOrder.currentDateTime = new Date();
                selectedOrder.receiptType = "REPRINT ";
                selectedOrder.loggedInBranchNo = $scope.BranchNumber;
                var positiveAmountSum = 0,
                    negativeAmountSum = 0;
                _.each(selectedOrder.payments, function (payment) {
                    payment.paymentMethodId = findKeyByValue(SalesEnums.PaymentMethods, payment.paymentMethodId);
                    positiveAmountSum += payment.amount > 0 ? payment.amount : 0;
                    negativeAmountSum += payment.amount < 0 ? payment.amount : 0;
                });

                if (positiveAmountSum > 0 && negativeAmountSum < 0) {
                    selectedOrder.changeGiven = true;
                }
                selectedOrder.positiveAmountSum = positiveAmountSum;
                selectedOrder.negativeAmountSum = negativeAmountSum;
            });

            $scope.orders = selectedOrders.orders;

            //  $scope.selectedInvoiceNo = this.item.invoiceNo;


            var params = {
                invoiceNo: $scope.selectedInvoiceNo
            };

            PosDataService.testPrint(params).then(function (data) {
                PrinterService.printHtml(data);
            });

            //CommonService.$timeout(function () {
            //    var el = document.getElementById('printerEl');
            //    angular.element(el).triggerHandler('click');
            //}, timeout);

        });
    }

    function findKeyByValue(set, value) {
        for (var k in set) {
            if (set.hasOwnProperty(k)) {
                if (set[k] == value) {
                    return k;
                }
            }
        }
        return undefined;
    }

}
searchOrderController.$inject = dependInjects;

module.exports = searchOrderController;