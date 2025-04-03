'use strict';

module.exports = ['$modal', function ($modal) {
    return {
        template: '<button class="btn btn-default" title="Select Icon">Select Icon <span class="glyphicons {{icon}}"></span></button>',
        restrict: 'E',
        scope: { icon: '=icon' },
        link: function (scope, element) {

            element.bind('click', function () {
                var modalInstance = $modal.open({
                    templateUrl: '/SalesManagement/views/templates/Glyphicons.html',
                    controller: ['$scope', '$modalInstance', function ($scope, $modalInstance) {
                        $scope.cancel = function (e) {
                            e.preventDefault();
                            $modalInstance.dismiss();
                        };
                        $scope.iconClicked = function (icon) {
                            console.log(icon);
                            scope.icon = icon;
                            $modalInstance.dismiss();
                        };
                    }]
                });
            });
        }
    };
}];
