'use strict';

var Basket = require('../model/Basket.js');
var SalesEnums = require('../model/SalesEnums');

var dependInjects = ['PosDataService', 'CommonService', 'PrinterService', 'LookupService', 'padNoFilter'];

var basketService = function (PosDataService, CommonService, PrinterService, LookupService, padNoFilter) {
    return {
        cart: new Basket(),
        startNewSale: startNewSale,
        completeSales: completeSales,
        getWarrantyContract: getWarrantyContract,
        getOrderForReceipt: getOrderForReceipt,
        getNewBasket: getNewBasket,
        printReceipt: getPrint,
        printWarrantyContract: printContract,
        printWarrantyContracts: printContracts,
        isWarrantyExpired: isWarrantyExpired,
        isFreeWarrantyExpired: isFreeWarrantyExpired,
        getItemWarrantyRemainingCoverage: getItemWarrantyRemainingCoverage,
        canDoIrExchange: canDoIrExchange,
        getWarrantyRemainingCoverage: getWarrantyRemainingCoverage
    };

    var cart;

    function startNewSale() {
        var self = this;

        self.cart = new Basket();

        self.cart.basketChangesAllowed = true;
    }

    function completeSales(cart, printer, emptyItems) {
        cart = cart || this.cart;
        var warranties = cart.getCartWarranties();
        var newWarranties = _.filter(warranties, function (w) {
            return !w.warrantyContractNo;
        });
        var noOfContracts = newWarranties.length;

        var params = {
            branch: cart.BranchNo,
            noOfContracts: noOfContracts
        };

        PosDataService.getWarrantyContractNosData(params).then(function (warrantyContractNos) {
            if (warrantyContractNos && warrantyContractNos.length > 0) {
                _.each(newWarranties, function (warranty, index) {
                    var no = warrantyContractNos[index] || '';

                    warranty.warrantyContractNo = cart.BranchNo.toString() + no;
                });
            }

        }).then(function () {
            PosDataService.saveSales(cart).then(function (data) {
                if (data && data.valid) {
                    cart.InvoiceNo = data.invoiceNo;
                    cart.saleComplete = true;

                    if (emptyItems) {
                        cart.items = [];
                    }
                    var copyCount = cart.isReturn ? 2 : 1;

                    getPrint(data.invoiceNo, copyCount, undefined, printer);

                    // Print Contracts
                    printContracts(data.invoiceNo, cart);
                }
                else if (data && !data.valid && data.failedPayments.errorMessage) {
                    CommonService.addGrowl({
                                               timeout: 3,
                                               type: 'danger',
                                               content: data.failedPayments.errorMessage
                                           });
                    cart.payments = _.reject(cart.payments, function (payment) {
                        return payment.tempPaymentId === data.failedPayments.tempPaymentId;
                    });
                    cart.paymentsTotal = cart.paymentsTotal - data.failedPayments.tenderedAmount;
                    cart.updateTotalAmount();
                }
                else {
                    CommonService.addGrowl({
                                               timeout: 3,
                                               type: 'danger',
                                               content: data.errors
                                           });
                }
            });
        });


    }

    function printContracts(invoiceNo, order) {
        var extendedWarranties = order.getExtendedCartWarranties();
        var allWarranties = order.getCartWarranties();

        if (!allWarranties || allWarranties.length === 0) {
            return;
        }

        var contractNos = [];

        _.each(allWarranties, function (warranty, index) {
            var contractNo = warranty.warrantyContractNo;

            if (!warranty.returned && contractNo && invoiceNo) {
                contractNos.push(contractNo);
            }
        });

        if (!contractNos || contractNos.length === 0) {
            return;
        }

        printContract(invoiceNo, contractNos, true);
    }

    function getOrderForReceipt(receiptNo) {
        var self = this;
        var params = {
            invoiceNo: receiptNo
        };

        PosDataService.getOrderByReceipt(params).then(function (data) {
            if (data) {
                self.cart.setOriginalOrder(data);
            }
            else {
                CommonService.error('Sale not found for requested invoice no', 'Sale not found');
            }
        }, function () {
            CommonService.error(
                'Either input was not valid or there was an error while trying to process your request.',
                'Application Error');
        });
    }

    function getWarrantyContract() {
        var data = {
            firstname: 'Chani',
            lastname: 'Diakidis'

        };

        return data;
    }

    function getPrint(invoiceNo, copyCount, receiptType, printer) {
        var params = {
            invoiceNo: invoiceNo,
            copyCount: copyCount || 1,
            receiptType: receiptType,
            isThermalPrint: PrinterService.canUseThermalPrinter
        };

        PosDataService.printReceipt(params).then(function (data) {

            if (PrinterService.canUseThermalPrinter) {
                printer = printer || "EPSON TM-T70 Receipt";

                PrinterService.thermalPrint(printer, data);
            } else {
                PrinterService.printHtml(data);
            }
        });
    }

    function printContract(agreementNo, contractNo, multiple) {
        var params = {
            agreementNo: agreementNo,
            contractNos: contractNo,
            multiple: multiple || true
        };

        PosDataService.printWarrantyContract(params).then(function (data) {
            if (data) {

                if (_.isArray(data)) {
                    _.each(data, function (model) {
                        if (model) {
                            PrinterService.printHtml(model);
                        }
                    });
                } else {
                    CommonService.addGrowl({
                                               timeout: 3,
                                               type: 'danger',
                                               content: data
                                           });
                }
            }
        });
    }

    function enrichAndPrint(data, userId, branchNo, cart) {
        var order = data;
        LookupService.populate('USERS').then(function (data) {
            if (data) {
                order.id = padNoFilter(order.id, 10);
                order.currentUser = data[userId];
                order.createdBy = data[order.createdBy];
                order.currentDateTime = new Date();
                order.loggedInBranchNo = branchNo;
                var positiveAmountSum = 0,
                    negativeAmountSum = 0;

                _.each(order.payments, function (payment) {
                    payment.paymentMethodId = findKeyByValue(SalesEnums.PaymentMethods, payment.paymentMethodId);
                    positiveAmountSum += payment.amount > 0 ? payment.amount : 0;
                    negativeAmountSum += payment.amount < 0 ? payment.amount : 0;
                });

                if (positiveAmountSum > 0 && negativeAmountSum < 0) {
                    order.changeGiven = true;
                }
                order.positiveAmountSum = positiveAmountSum;
                order.negativeAmountSum = negativeAmountSum;

                PrinterService.print('/sales/views/templates/print/receiptSale.html', order);
            }
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

    function getNewBasket() {
        return new Basket();
    }

    //region Warranties

    function isWarrantyExpired(warranty) {
        var effectiveDate = moment(warranty.warrantyEffectiveDate),
            expireDate = effectiveDate.add(warranty.warrantyLengthMonths, 'M');

        return (moment() >= expireDate);

    }

    function isFreeWarrantyExpired(item) {
        var ret = true;

        var freeWarranty = item.getItemFreeWarranty();

        if (freeWarranty) {
            ret = isWarrantyExpired(freeWarranty);
        }
        return ret;
    }

    function getWarrantyRemainingCoverage(warranty) {
        var effectiveDate = moment(warranty.warrantyEffectiveDate),
            expireDate = effectiveDate.add(warranty.warrantyLengthMonths, 'M'),
            diff = expireDate.diff(moment(), 'M');

        return diff > 0 ? Math.ceil(diff) : 0;

    }

    function getItemWarrantyRemainingCoverage(item, orderCreatedOn) {
        var ret = 0,
            purchasedDate = moment(orderCreatedOn).format('YYYY-MM-DD'),
            elapsedMonths = moment().diff(purchasedDate, 'month');

        if (elapsedMonths > 0) {
            ret = item.totalWarrantyCoverage - elapsedMonths;
        }


        return ret;
    }

    function canDoIrExchange(item) {
        if (!item.hasInstantReplacementWarranty) {
            return false;
        }

        var unClaimedWarranty = _.first(item.unClaimedInstantReplacementWarranties);

        return (unClaimedWarranty && isFreeWarrantyExpired(item));
    }

    //endregion

};

basketService.$inject = dependInjects;

module.exports = basketService;
