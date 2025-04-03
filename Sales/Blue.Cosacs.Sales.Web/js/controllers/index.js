'use strict';

angular.module('Sales.controllers', [])
    .controller('BasketController', require('./basketController'))
    .controller('SearchOrderController', require('./searchOrderController'))
    .controller('PosController', require('./posController'))
    .controller('ReceiptEntryController', require('./receiptEntryController'))
    .controller('ContractsSetupController', require('./contractsSetupController'))
    .controller('CustomerController', require('./customerController'))
    .controller('DiscountLimitSetupController', require('./discountLimitSetupController'))
    .controller('PaymentController', require('./paymentController'))
    .controller('ProductController', require('./productController'))
    .controller('SalesController', require('./salesController'));