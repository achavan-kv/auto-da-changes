/*global angular, module */

var deps = ['$scope', 'promiseDataService', 'CommonService'];

function paymentSetupController($scope, promiseDataService, CommonService) {
    'use strict';

    $scope.paymentMethodsData = {};
    $scope.removeSpaces = removeSpaces;
    $scope.changeStatus = changeStatus;

    activate();

    function activate() {
        angular.element('body').removeClass('full-screen');
        promiseDataService.getAllPaymentMethodsData().then(function (data) {
            if (data) {
                $scope.paymentMethodsData = data.PaymentMethods;
            }
        });
    }


    function removeSpaces(text) {
        return text.replace(' ', '');
    }

    function changeStatus(paymentMethod) {
        paymentMethod.IsCashReturnAllowed = paymentMethod.IsReturnAllowed ? paymentMethod.IsCashReturnAllowed : false;

        promiseDataService.updatePaymentMethodStatus(paymentMethod).then(function (data) {
            if (data.Result === "Success") {
                CommonService.addGrowl({
                                           timeout: 3,
                                           type: 'success',
                                           content: "Status for " + data.PaymentMethods.Description + " updated"
                                       });
            }
        });
    }


}

paymentSetupController.$inject = deps;

module.exports = paymentSetupController;
