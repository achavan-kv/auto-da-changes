'use strict';

var dependInjects = ['PosDataService'];

function productsService(PosDataService) {

    return {
        getAllProducts: getAllProducts,
        getProductByBarcode: getProductByBarcode

    };

    function getAllProducts() {
        return PosDataService.getAllProductsData();
    }

    function getProductByBarcode(barcode, isUpc) {
        return PosDataService.getProductByBarcodeData(barcode, isUpc);
    }

}

productsService.$inject = dependInjects;

module.exports = productsService;