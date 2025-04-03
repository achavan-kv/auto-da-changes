'use strict';

var dependInjects = [];

function modelTransformer() {
    return {
        transform: transformResult
    };

    //region Private methods

    function transformResult(jsonResult, constructor) {
        if (angular.isArray(jsonResult)) {
            var models = [];

            angular.forEach(jsonResult, function(object) {
                models.push(transformObject(object, constructor));
            });

            return models;
        } else {
            return transformObject(jsonResult, constructor);
        }
    }

    function transformObject(jsonResult, constructor) {
        var model = new constructor();

        angular.extend(model, jsonResult);

        return model;
    }

    //endregion

}

modelTransformer.$inject = dependInjects;

module.exports = modelTransformer;
