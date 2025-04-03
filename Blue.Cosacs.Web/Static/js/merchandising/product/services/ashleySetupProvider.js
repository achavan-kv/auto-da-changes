define(['angular'],
    function () {
        "use strict";

        return function ($q, $resource, apiResourceHelper) {
            var that = this,
                actions = apiResourceHelper.getDefaultActions(),
                productRangeResource = apiResourceHelper.createResource('Merchandising/AshleySetup/:id', { id: '@id' }, actions),
                ashleyEnable = apiResourceHelper.createResource('Merchandising/AshleySetup/getAshleyEnable', { id: '@id' }, actions);

            apiResourceHelper.createActions(actions);

            that.save = function (model) {
                if (model.id) {
                    return apiResourceHelper.update(productRangeResource, { id: model.id, model: model });
                } else {
                    console.log('Hi I am calling Save of ashley setup provider, Model = ' + model.id);
                    return apiResourceHelper.create(productRangeResource, { model: model });
                }
            };

            that.remove = function (model) {
                return apiResourceHelper.remove(productRangeResource, { id: model.id });
            };

            that.getAshleyEnabled = function () {
                return apiResourceHelper.get(ashleyEnable);
            }


            that.getAll = function () {
                return apiResourceHelper.get(allResource);
            };
        };
    });