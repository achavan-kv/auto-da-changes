'use strict';

function mockSalesDataService(sandbox){
    return {
        getItemWarranties: sandbox.stub(),
        getItemDiscounts: sandbox.stub(),
        getAssociatedProducts: sandbox.stub(),
        getInternalInstallations: sandbox.stub(),
        saveSales: sandbox.stub(),
        getOrderByReceipt: sandbox.stub(),
        forceIndexing: sandbox.stub(),
        getExchangeRateData: sandbox.stub(),
        getAllProductsData: sandbox.stub(),
        getProductByBarcodeData: sandbox.stub(),
        getTaxSettingsData: sandbox.stub(),
        getPaymentMethodsData:sandbox.stub(),
        checkReturnedValue:sandbox.stub(),
        saveExchangeRate:sandbox.stub(),
        getCurrencyData:sandbox.stub(),
        getExchangeRateDetails:sandbox.stub(),
        fetchCustomerDetails:sandbox.stub(),
        getWarrantyContractNosData:sandbox.stub(),
        getRefundExchangeReasons:sandbox.stub(),
        getSettingsData:sandbox.stub(),
        getExistingOrders:sandbox.stub(),
        getWarrantyReturnPercentage:sandbox.stub(),
        getBranchDetailsList:sandbox.stub(),
        insertDiscountLimit:sandbox.stub(),
        updateDiscountLimit:sandbox.stub(),
        deleteDiscountLimit:sandbox.stub(),
        getExistingDiscountLimitData:sandbox.stub(),
        getDiscountLimit:sandbox.stub()
    };
}

module.exports = mockSalesDataService;