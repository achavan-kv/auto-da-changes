'use strict';

var dependInjects = ['xhr', 'CommonService', '$http', 'UsersService'];

// TODO: put the service url in a global config
function posDataService(xhr, CommonService, $http, UsersService) {
    return {
        getItemWarranties: getItemWarranties,
        getItemDiscounts: getItemDiscounts,
        getItemKitDiscount: getItemKitDiscount,
        getAssociatedProducts: getAssociatedProducts,
        getInternalInstallations: getInternalInstallations,
        getItemKitProducts: getItemKitProducts,
        saveSales: saveSales,
        forceIndexing: forceIndexing,
        getExchangeRateData: getExchangeRateData,
        getAllProductsData: getAllProductsData,
        getProductByBarcodeData: getProductByBarcodeData,
        getSettingsData: getSettingsData,
        getPaymentMethodsData: getPaymentMethodsData,
        checkReturnedValue: checkReturnedValue,
        getCurrencyCodesData: getCurrencyCodesData,
        fetchCustomerDetails: fetchCustomerDetails,
        checkStoreCardBalance: checkStoreCardBalance,
        getWarrantyContractNosData: getWarrantyContractNosData,
        checkGiftVoucherDetails: checkGiftVoucherDetails,
        getOrderByReceipt: getOrderByReceipt,
        getExistingOrders: getExistingOrders,
        printReceipt: printReceipt,
        printWarrantyContract: printWarrantyContract,
        getBranchDetails: getBranchDetails,
        getWarrantyReturnPercentage: getWarrantyReturnPercentage,
        getContractsSetup: getContractsSetup,
        updateContractsSetup: updateContractsSetup,
        getStoreCardCustomerId: getStoreCardCustomerId,
        getBranchDetailsList: getBranchDetailsList,
        insertDiscountLimit: insertDiscountLimit,
        updateDiscountLimit: updateDiscountLimit,
        deleteDiscountLimit: deleteDiscountLimit,
        getExistingDiscountLimitData: getExistingDiscountLimitData,
        getDiscountLimit: getDiscountLimit,
        getActiveBanks: getActiveBanks,
        getCardTypesData: getCardTypesData,
        hasPermission: hasPermission,
        isCashierBalanceOutstanding: isCashierBalanceOutstanding,
        GetAvailableQuantity: GetAvailableQuantity,
        getTax: getTax,
        getCSRList: getCSRList
    };

    //region Private methods

    function getItemWarranties(params) {
        params.WarrantyTypeCode = params.WarrantyTypeCode || null;

        return getData('Cosacs/Warranty/Link/Search', params);
    }

    function getItemDiscounts(params) {
        return getData('Courts.NET.WS/Sales/GetLinkDiscounts', params);
    }

    function getItemKitDiscount(params) {
        return getData('Courts.NET.WS/Sales/GetKitDiscount', params);
    }

    function getAssociatedProducts(params) {
        return getData('sales/api/AssociatedProducts', params);
        // return getData('sales/api/products/GetAssociatedProducts', params);
    }

    function getInternalInstallations(params) {
        return getData('Courts.NET.WS/Sales/GetInternalInstallations', params);
    }

    function getItemKitProducts(params) {
        return getData('Courts.NET.WS/Sales/GetItemKitProducts', params);
    }

    function forceIndexing() {
        return getData('sales/api/customers/ForceIndex');
    }

    function getExchangeRateData(params) {
        return getData('payments/api/ForeignCurrency', params);
    }

    function getAllProductsData() {
        return getData('sales/api/products');
    }

    function getProductByBarcodeData(barcode, isUpc) {
        var params = {
            q: {"query": barcode},
            rows: 1,
            upc: isUpc ? barcode : ''
        };

        return getData('sales/api/products/Search', params);
    }

    function getSettingsData() {
        return getData('sales/api/settings');
    }

    //region Payment

    function getPaymentMethodsData() {
        return getData('payments/api/PaymentSetup/GetActivePaymentMethods');
    }

    //endregion

    function hasPermission(permission) {
        var deferred = CommonService.$q.defer();
        var params = {
            permission: permission || 0
        };

        getData('/api/Authorisation/HasPermission', params).then(function (data) {
            if (data) {
                deferred.resolve(data.Result || false);
            } else {
                deferred.resolve(false);
            }
        });
        return deferred.promise;
    }

    function getData(url, params) {
        var request = xhr.get(url, {
            params: params
        });
        return ( request.then(CommonService.handleAjaxSuccess, CommonService.handleAjaxError) );
    }

    function saveSales(order) {
        var request = xhr({
                              method: "post",
                              url: "/Sales/api/Orders",
                              data: order
                          });

        return ( request.then(CommonService.handleAjaxSuccess, CommonService.handleAjaxError) );
    }

    function checkReturnedValue(data) {
        if (data) {
            if (!data.valid) {
                return false;
            }
            else {
                return true;
            }

        }
    }

    function getCurrencyCodesData() {
        return getData('payments/api/ForeignCurrency');
    }

    function fetchCustomerDetails(customerDetails) {
        return getData('/Customer/api/Reindex', customerDetails);
    }

    function checkStoreCardBalance(params) {
        return getData('Courts.NET.WS/Sales/GetStoreCardAvailableBalance', params);
    }

    function getStoreCardCustomerId(params) {
        return getData('Courts.NET.WS/Sales/GetStoreCardCustomerId', params);
    }

    function getWarrantyContractNosData(params) {
        return getData('Courts.NET.WS/Sales/GetWarrantyContractNos', params);
    }

    function checkGiftVoucherDetails(params) {
        return getData('Courts.NET.WS/Sales/GetGiftVoucherDetails', params);
    }

    function getOrderByReceipt(params) {
        return getData('/Sales/api/Orders/', params);
    }

    function getExistingOrders(params) {
        return getData('/Sales/api/Orders/', params);
    }

    function printReceipt(params) {
        return getData('/Sales/print/invoices', params);
    }

    function printWarrantyContract(params) {
        return getData('/Sales/print/WarrantyContracts', params);
    }

    function getBranchDetails(params) {
        return getData('Cosacs/Service/Requests/GetBranchDetails/branchNo', params);
    }

    function getContractsSetup() {
        return getData('/sales/api/ContractsSetup');
    }

    function updateContractsSetup(params) {
        var request = xhr({
                              method: "put",
                              url: "/sales/api/ContractsSetup",
                              data: params
                          });

        return ( request.then(CommonService.handleAjaxSuccess, CommonService.handleAjaxError) );
    }

    function getWarrantyReturnPercentage(params) {
        return getData('Cosacs/Warranty/WarrantyReturn/Get', params);
    }

    function getBranchDetailsList() {
        return getData('/Courts.NET.WS/DBOInfo/Branches');
    }

    function insertDiscountLimit(params) {
        var request = xhr({
                              method: "post",
                              url: "/sales/api/discountLimitSetup",
                              data: params
                          });

        return ( request.then(CommonService.handleAjaxSuccess, CommonService.handleAjaxError) );
    }

    function updateDiscountLimit(params) {
        var request = xhr({
                              method: "put",
                              url: "/sales/api/discountLimitSetup",
                              data: params
                          });

        return ( request.then(CommonService.handleAjaxSuccess, CommonService.handleAjaxError) );
    }

    function deleteDiscountLimit(params) {

        var request = $http({
                                url: "/sales/api/discountLimitSetup",
                                method: "delete",
                                params: params
                            });
        return ( request.then(CommonService.handleAjaxSuccess, CommonService.handleAjaxError) );
    }

    function getExistingDiscountLimitData(params) {
        return getData('/sales/api/discountLimitSetup', params);
    }

    function getDiscountLimit(params) {
        return getData('/sales/api/discountLimitSetup', params);
    }

    function isCashierBalanceOutstanding(params) {
        return getData('Courts.NET.WS/Sales/isCashierBalanceOutstanding', params);
    }

    function getActiveBanks() {
        return getData('Payments/Api/BankMaintenance/GetActiveBanks');
    }

    function getCardTypesData() {
        return getData('Payments/Api/PaymentSetup/CardTypes');
    }

    function GetAvailableQuantity(item, branch) {
        var params = {
            itemNo: item,
            branchNo: branch
        };
        return getData('Courts.NET.WS/Sales/GetAvailableQuantity', params);
    }

    function getTax() {
        return getData('Cosacs/Merchandising/Tax/GetTaxSettings');
    }


    function getCSRList() {
        return getData('/Cosacs/Admin/Users/LoadPickListUsers?branch=' + UsersService.getCurrentUser().BranchNumber);
    }

    //endregion

}

posDataService.$inject = dependInjects;

module.exports = posDataService;
