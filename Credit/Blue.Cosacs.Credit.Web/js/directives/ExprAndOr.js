'use strict';

var exprandor = function ($compile) {

    var template =
        '<div class="exprBlock">' +
        '<div class="function click">' +
        '<span ng-click="changeEvaluator()">{{rules.class}}</span>' +
        '</div>' +
        '<div class="expression">' +
        '<div ng-repeat="rule in rules.expression">' +
        '<span ng-if="classFilter(rule)" class="glyphicons remove_2 pull-right click" ng-click="removeExpr($index)"></span><exprAndOr ng-if="classFilter(rule)" rules="rule.expression" data="data"></exprAndOr>' +
        '<span ng-if="!classFilter(rule) && rule.hasOwnProperty(\u0027class\u0027)" class="glyphicons option_vertical pull-left click" ng-click="linkExpr($index)">&nbsp;</span>' +
        '<span ng-if="!classFilter(rule) && rule.hasOwnProperty(\u0027class\u0027)" class="glyphicons remove_2 pull-right click" ng-click="removeExpr($index)"></span>' +
        '<expr ng-if="!classFilter(rule) && rule.hasOwnProperty(\u0027class\u0027)" rule="rule" data="data"></expr>' +
        '</div>' +
        '<div><span class="glyphicons plus click" ng-click="add(rules)"></span></div>' +
        '</div>' +
        '</div>';

    return {
        template: template,
        restrict: 'E',
        scope: {
            rules: '=',
            data: '='
        },
        compile: function (tElem, tAttrs, transclude) {
            var contents = tElem.contents().remove();
            var compiledContents;

            return {
                pre: function (scope, element, attributes) {
                    if (!compiledContents) {
                        compiledContents = $compile(contents, transclude);
                    }

                    compiledContents(scope, function (clone) {
                        element.append(clone);
                    });
                },
                post: function (scope) {
                    scope.rules.class = scope.rules.class ? scope.rules.class : 'And';

                    scope.classFilter = function (rule) {
                        return (rule.class === 'And' || rule.class === 'Or' || rule.class === 'Not');
                    };

                    scope.changeEvaluator = function () {
                        switch (scope.rules.class) {
                            case 'And':
                                scope.rules.class = 'Or';
                                break;
                            case 'Or':
                                scope.rules.class = 'Not';
                                break;
                            default :
                                scope.rules.class = 'And';
                                break;
                        }
                    };

                    scope.add = function (rules) {
                        if (_.isUndefined(rules.expression)) {
                            rules.expression = [];
                        }
                        rules.expression.push({
                            "class": '',
                            "expression": []
                        });
                    };

                    scope.removeExpr = function (index) {
                        scope.rules.expression.splice(index, 1);
                    };

                    scope.linkExpr = function (index) {
                        scope.rules.expression[index] = {
                            class: 'And',
                            expression: [scope.rules[index]]
                        };
                    }

                }
            }
        }
    };
};
exprandor.$inject = ['$compile'];
module.exports = exprandor;