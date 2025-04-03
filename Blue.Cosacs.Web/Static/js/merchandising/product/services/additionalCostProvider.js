define(['angular'],
    function () {
        "use strict";

        return function ($q, $resource, apiResourceHelper) {
            var that = this,
                actions = apiResourceHelper.getDefaultActions(),
                costsResource = apiResourceHelper.createResource('Merchandising/AdditionalCostPrice/:id', { id: '@id' }, actions),
                ashleyEnable = apiResourceHelper.createResource('Merchandising/AshleySetup/getAshleyEnable', { id: '@id' }, actions),
                vendorcostpricedetail = apiResourceHelper.createResource('Merchandising/AdditionalCostPrice/GetByVendorDetail/:id', { id: '@id' }, actions);

            apiResourceHelper.createActions(actions);

            that.save = function (model) {
                if (model.id) {
                    return apiResourceHelper.update(costsResource, { id: model.id, model: model });
                } else {
                    return apiResourceHelper.create(costsResource, { model: model });
                }
            };

            that.getAshleyEnabled = function () {
                return apiResourceHelper.get(ashleyEnable);
            }

            //that.getvendorcostpricedetail = function () {
            //    return apiResourceHelper.get(vendorcostpricedetail, { id: model.id });
            //    //return "";
            //}
            
            that.GetAshleyEnable1 = function () {
                return apiResourceHelper.get(vendorcostpricedetail);
            };
        };
    });