'use strict';

var expr = function ($compile) {

    var typesList =
    {
        Numeric: [
            {
                name: "Exists",
                input: "None"
            },
            {
                name: "Not Exists",
                input: "None"
            },
            {
                name: "Is",
                input: "Numeric"
            },
            {
                name: "Is Not",
                input: "Numeric"
            },
            {
                name: "<",
                input: "Numeric"
            },
            {
                name: ">",
                input: "Numeric"
            },
            {
                name: ">=",
                input: "Numeric"
            },
            {
                name: "<=",
                input: "Numeric"
            }],
        String: [
            {
                name: "Exists",
                input: "None"
            },
            {
                name: "Not Exists",
                input: "None"
            },
            {
                name: "Is",
                input: "Lookup"
            },
            {
                name: "Is Not",
                input: "Lookup"
            },
            {
                name: "Not Like",
                input: "Text"
            },
            {
                name: "Like",
                input: "Text"
            }]
    };

    var template = '<div> ' +
        '<div class="form-group">' +
        '<list ng-show="editing.operand" scope="name" ng-model="rule.class" class="col-lg-6"></list>' +
        '<span  ng-hide="editing.operand" class="click changable pull-left" ng-click="select(\u0027operand\u0027)">{{rule.class ? rule.class : "Click"}}</span>' +
        '<span  ng-show="rule.class" class="click changable pull-left" ng-click="toggleOperator()">{{rule.operator ? rule.operator : "Click"}}</span>' +
        '<input type="{{variableType}}" ng-show="rule.operator && editing.variable && !hideVariable" ng-model="rule.variable" ng-blur="editing.variable = false;"/>' +
        '<span ng-show="rule.operator && !editing.variable && !hideVariable" ng-model="rule.variable" ng-click="select(\u0027variable\u0027)" class="changable">{{rule.variable ? rule.variable : "Enter Value"}}</span>' +
        '<list ng-show="variableLookup" ng-model="rule.variable" scope="variableInput" class="col-lg-4"></list>' +
        '</div>' +
        '</div>';

    return {
        template: template,
        restrict: 'E',
        scope: {
            rule: '=',
            data: '='
        },
        link: function (scope, element) {
            scope.editing = {};
            scope.editing.variable = false;
            scope.variable = '';
            scope.variableType = '';
            var operatorPosition = 0;
            var types = [];
            var config = {};
            var settings = {};
            var lookup = '';


            scope.data.config.then(function (result) {
                scope.name = _.pluck(result.data, 'name');
                config = result.data;
                setOperand(scope.rule.class, true);
            });

            scope.data.settings.then(function (result) {
                settings = result.data;
            });

            scope.select = function (type) {
                scope.editing[type] = true;
                if (lookup) {
                    scope.hideVariable = true;
                    scope.variableInput = settings[lookup];
                    scope.variableLookup = true;
                }
            };

            function setOperator(init) {
                if (types) {
                    scope.hideVariable = false;
                    scope.variableLookup = false;
                    if (init) {
                        return
                    }
                    scope.rule.operator = types[operatorPosition].name;
                    switch (types[operatorPosition].input) {
                        case 'None':
                            scope.hideVariable = true;
                            break;
                        case 'Lookup':
                            if (lookup) {
                                scope.hideVariable = true;
                                scope.variableInput = settings[lookup];
                                scope.variableLookup = true;
                            }
                            else {
                                scope.variableType = 'text';
                            }
                            break;
                        case 'Numeric':
                            scope.variableType = 'number';
                            break;
                        case 'Text' :
                            scope.variableType = 'text';
                            break;
                        default:
                    }
                }
            }

            function setOperand(value, init) {
                if (!_.isEmpty(config) && value) {
                    scope.editing.operand = false;
                    var selected = _.find(config, function (operand) {
                        return operand.name === value;
                    });
                    types = typesList[selected.type];
                    lookup = selected.lookupField;
                    operatorPosition = 0;
                    setOperator(init);
                }
            }


            scope.$watch('rule.class', function (newValue, oldValue) {
                if (newValue) {
                setOperand(newValue);
                }
            });

            scope.$watch('rule.variable', function (newValue, oldValue) {
                if (newValue != oldValue) {
                    scope.variableLookup = false;
                    scope.hideVariable = false;
                }
            });

            scope.toggleOperator = function () {
                operatorPosition++;
                if (operatorPosition >= types.length) {
                    operatorPosition = 0;
                }
                setOperator();
            };
        }
    };
};

expr.$inject = ['$compile'];
module.exports = expr;