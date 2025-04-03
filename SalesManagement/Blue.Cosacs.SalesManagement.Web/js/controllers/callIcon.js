'use strict';

var callIconController = function ($scope, $http, UsersService, CommonService) {
    $scope.results = '';

    $http({
        url: '/SalesManagement/api/IconTypes',
        method: "GET"
    })
          .then(function (resp) {
              $scope.results = resp.data;
          });

    $scope.save = function () {
        $http.post('/SalesManagement/api/IconTypes', $scope.results)
         .success(function (data) {
             CommonService.addGrowl({
                 timeout: 5,
                 type: 'success', // (optional - info, danger, success, warning)
                 content: 'Successfully Saved the Icons !'
             });
         });
    };
};

callIconController.$inject = ['$scope', '$http', 'UsersService', 'CommonService'];

module.exports = callIconController;