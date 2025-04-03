'use strict';

angular.module('Sales.services', [])
    .factory('BasketService', require('./basketService'))
    .factory('PosDataService', require('./posDataService'))
    .factory('ModelTransformer', require('./modelTransformer'))
    .factory('SearchOrderService', require('./searchOrderService'))
    .constant('PaymentViewConst', require('./paymentViewConst'))
    .factory('PaymentService', require('./paymentService'))
    .factory('ProductsService', require('./productsService'));