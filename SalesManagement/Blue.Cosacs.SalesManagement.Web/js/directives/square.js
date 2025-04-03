'use strict';

module.exports = function () {
    return {
        template: '<div class="col-sm-4"><div class="panel panel-default"><div class="panel-body"><div class="value">{{value}}</div><div class="kpi-text"><div class="name">{{name}}</div><div class="description">{{description}}</div></div></div></div></div>',
        restrict: 'E',
        scope: {
            value: '=value',
            name: '@name',
            description : '@description'
        }
    };
};
