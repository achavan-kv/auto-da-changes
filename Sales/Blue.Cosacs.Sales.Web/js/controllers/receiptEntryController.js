'use strict';

var dependInjects = ['$scope','$modalInstance'];

function receiptEntryController($scope,$modalInstance) {
    $scope.loadReceipt = function (e) {
        e.preventDefault();

        $modalInstance.close($scope.barcode);
    };

    $scope.cancel = function (e) {
        e.preventDefault();
        $modalInstance.close();
    };
}

receiptEntryController.$inject = dependInjects;

module.exports = receiptEntryController;