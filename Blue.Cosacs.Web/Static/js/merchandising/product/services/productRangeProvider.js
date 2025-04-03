define(['angular'],
    function () {
        "use strict";

        return function ($q, $resource, apiResourceHelper) {
            var that = this,
                actions = apiResourceHelper.getDefaultActions(),
                productRangeResource = apiResourceHelper.createResource('Merchandising/ProductRange/:id', { id: '@id' }, actions),
                ashleyEnable = apiResourceHelper.createResource('Merchandising/AshleySetup/getAshleyEnable', { id: '@id' }, actions);
                //taxSettingsResource = apiResourceHelper.createResource('Merchandising/ProductRange/GetTaxSettings', {}, actions);

            apiResourceHelper.createActions(actions);

            that.save = function (model) {
                if (model.id) {
                    return apiResourceHelper.update(productRangeResource, { id: model.id, model: model  });
                } else {
                    console.log('Hi I am calling Save of product range provider, Model = ' + model);
                    return apiResourceHelper.create(productRangeResource, { model: model });
                }
            };

            that.remove = function (model) {
                return apiResourceHelper.remove(productRangeResource, { id: model.id });
            };

            //that.getTaxSettings = function () {
            //    return apiResourceHelper.get(taxSettingsResource);
            //};

            that.getAshleyEnabled = function () {
                return apiResourceHelper.get(ashleyEnable);
            }

            that.getAll = function () {
                return apiResourceHelper.get(allResource);
            };
        };
    });