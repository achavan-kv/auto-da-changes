define(['angular'],
function () {
    "use strict";

    return function ($q, $resource, apiResourceHelper) {
        var that = this,
            actions = apiResourceHelper.getDefaultActions(),
            resource = apiResourceHelper.createResource('Merchandising/Purchase/:id', { id: '@id' }, actions),
            notReceivedResource = apiResourceHelper.createResource('Merchandising/Purchase/NotReceived/', {}, actions),
            getForReceiptResource = apiResourceHelper.createResource('Merchandising/Purchase/GetForReceipt/', {}, actions),
            cancelResource = apiResourceHelper.createResource('Merchandising/Purchase/Cancel/:id', { id: '@id' }, actions),
            productResource = apiResourceHelper.createResource('Merchandising/Purchase/CancelProduct/:id', { id: '@id' }, actions),
            emailResource = apiResourceHelper.createResource('Merchandising/Purchase/Email/:id', { id: '@id' }, actions),
            labelResource = apiResourceHelper.createResource('Merchandising/Purchase/Labels/', { }, actions),
            firstapproveResource = apiResourceHelper.createResource('Merchandising/Purchase/POFirstLevelApprove/:id', { id: '@id' }, actions),
            secondapproveResource = apiResourceHelper.createResource('Merchandising/Purchase/POSecondLevelApprove/:id', { id: '@id' }, actions),
            bothapproveResource = apiResourceHelper.createResource('Merchandising/Purchase/POBothLevelApprove/:id', { id: '@id' }, actions),
            firstrejectResource = apiResourceHelper.createResource('Merchandising/Purchase/POFirstLevelReject/:id', { id: '@id' }, actions),
            secondrejectResource = apiResourceHelper.createResource('Merchandising/Purchase/POSecondLevelReject/:id', { id: '@id' }, actions),
            bothrejectResource = apiResourceHelper.createResource('Merchandising/Purchase/POBothLevelReject/:id', { id: '@id' }, actions),
            rejectResource = apiResourceHelper.createResource('Merchandising/Purchase/Reject/:id', { id: '@id' }, actions),
            pocancelreasonlistResource = apiResourceHelper.createResource('Merchandising/Purchase/GetList/', actions),
            savePOcancellationReasonResource = apiResourceHelper.createResource('Merchandising/Purchase/SavePOCancellationReason/:id', { id: '@id' }, actions),
            GetAshleyEnableResource = apiResourceHelper.createResource('Merchandising/Purchase/GetAshleyEnabled/', actions),
            //vendorcostpricedetail = apiResourceHelper.createResource('Merchandising/AdditionalCostPrice/vendorcostpricedetail', { id: '@id' }, actions);
            vendorcostpricedetail = apiResourceHelper.createResource('Merchandising/AdditionalCostPrice/GetByVendorDetail/:id', { id: '@id'}, actions);

        apiResourceHelper.createActions(actions);

        that.get = function (id) {
            return apiResourceHelper.get(resource, { id: id });
        };

        that.getForReceipt = function (id) {
            return apiResourceHelper.get(getForReceiptResource, { id: id });
        };

        that.save = function (model) {
            if (model.id) {
                return apiResourceHelper.update(resource, { id: model.id, model: model });
            } else {
                return apiResourceHelper.create(resource, { model: model });
            }
        };

        that.notReceived = function (vendorId) {
            return apiResourceHelper.get(notReceivedResource, { vendorId: vendorId });
        };
        //---- Code Added By Abhijeet for 
        // 1. M1/M2 Authorization to Approve/Reject PO
        // 2. Add Cancel PO crId Parameter 
        // 3. SavePOCancellationReason
        // 4. Get PO Cancellation Reason List
        that.cancel = function (id, cancelReasonId) {
            return apiResourceHelper.update(cancelResource, { id: id, cancelReasonId: cancelReasonId });
        };
        
        that.firstapprove = function (id) {
            return apiResourceHelper.update(firstapproveResource, { id: id });
        };

        that.secondapprove = function (id) {
            return apiResourceHelper.update(secondapproveResource, { id: id });
        };

        that.bothapprove = function (id) {
            return apiResourceHelper.update(bothapproveResource, { id: id });
        };

        that.firstreject = function (id) {
            return apiResourceHelper.update(firstrejectResource, { id: id });
        };

        that.secondreject = function (id) {
            return apiResourceHelper.update(secondrejectResource, { id: id });
        };

        that.bothreject = function (id) {
            return apiResourceHelper.update(bothrejectResource, { id: id });
        };

        that.getList = function () {
            return apiResourceHelper.get(pocancelreasonlistResource);
        };

        that.SavePOCancellationReason = function (id) {
            return apiResourceHelper.update(savePOcancellationReasonResource, { id: id });
        };

        that.GetAshleyEnable = function () {
            return apiResourceHelper.get(GetAshleyEnableResource);
        };

        that.GetACPPriceDetail = function (vendorId) {
            return apiResourceHelper.get(vendorcostpricedetail, { vendorId: vendorId });
        };

        //---- 

        that.email = function (id) {
            return apiResourceHelper.update(emailResource, { id: id });
        };

        that.getLabels = function(products) {
            return apiResourceHelper.create(labelResource, { products: products });
        };

        that.cancelProduct = function (id, poProductId) {
            return apiResourceHelper.create(productResource, { id: id, purchaseOrderProductId: poProductId });
        };


        
    };
});