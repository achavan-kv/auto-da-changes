define(['angular'],
    function () {
        "use strict";

        return function ($q, $resource, apiResourceHelper) {
            var that = this,
                actions = apiResourceHelper.getDefaultActions(),
                resource = apiResourceHelper.createResource('Merchandising/StockCountStart/CreatePerpetual/:id', { id: '@id' }, actions);
                apiResourceHelper.createActions(actions);


            that.save = function (model) {
                var models = _.isArray(model) ? model : [].concat(model);
                return apiResourceHelper.update(productSave, { model: models });
            };

            that.start = function (id) {
                return apiResourceHelper.create(resource, { model: id });
            };

        };
    });
