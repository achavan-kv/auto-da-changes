'use strict';

var dependInjects = ['$scope', 'PosDataService', 'CommonService', '$http'];

function contractsSetupController($scope, PosDataService, CommonService, $http) {

    $scope.listContracts = {};
    $scope.contractsSetup = [];
    $scope.disableSave = false;

    $scope.save = save;
    $scope.checkRequired = checkRequired;
    $scope.checkDisableSave = checkDisableSave;

    activate();

    function loadData() {

        PosDataService.getContractsSetup().then(function (data) {
            $scope.contractsSetup = data;
        });
    }

    function save() {
        $scope.disableSave =true;

        PosDataService.updateContractsSetup($scope.contractsSetup).then(function (data) {
            if (data === "") {
                CommonService.addGrowl({
                                           timeout: 3,
                                           type: 'success',
                                           content: "Contracts Setup saved successfully."
                                       });
                loadData();
                $scope.disableSave = false;
                //singleClick.reset();
            }
        });

    }

    function checkRequired(items, categories) {
        if ((items === null || items === "") &&
            (categories === null || categories === "")) {
            return true;
        }

        return false;
    }

    function checkDisableSave() {
        if ( $scope.disableSave || _.size($scope.contractsSetup) === 0) {
            return true;
        }

        return false;
    }

    function activate() {
        // angular.element('body').removeClass('full-screen');
        loadData();
    }

}

contractsSetupController.$inject = dependInjects;

module.exports = contractsSetupController;