'use strict';

var deps = ['$scope', 'promiseDataService', 'CommonService', '$http'];

function paymentMapController($scope, promiseDataService, CommonService, $http) {
    'use strict';

    var shadowWinCosacsId = null;
    $scope.paymentMappingData = [];
    $scope.saveMapping = saveMapping;
    $scope.startEdit = startEdit;
    $scope.cancelEdit = cancelEdit;

    activate();

    function activate() {
        angular.element('body').removeClass('full-screen');
        promiseDataService.getAllPaymentMapData().then(function (data) {
            if (data) {
                $scope.paymentMappingData = data;
            }
        });
    }

    function saveMapping(paymentMapping) {
        if(!paymentMapping){
            return;
        }

        var growlTimeout = 3,
            url = 'payments/api/PaymentMethodMap?posId=' + paymentMapping.Id + '&winCosacsId=' + paymentMapping.WinCosacsId;


        $http({
                  method: 'POST',
                  url: url
              }).then(function (data) {
            if (data.data === "") {

                CommonService.addGrowl({
                                           type: 'info', // (optional - info, danger, success, warning)
                                           content: 'Mapping saved successfully',
                                           timeout: growlTimeout
                                       });
                shadowWinCosacsId = null;
                paymentMapping.isEditing = false;

            } else {
                CommonService.addGrowl({
                                           type: 'danger', // (optional - info, danger, success, warning)
                                           content: data.data,
                                           timeout: growlTimeout
                                       });
            }
        });

    }

    function startEdit(paymentMapping) {
        shadowWinCosacsId = paymentMapping.WinCosacsId;
        paymentMapping.isEditing = true;
    }

    function cancelEdit(paymentMapping) {
        paymentMapping.WinCosacsId = shadowWinCosacsId || 0;

        paymentMapping.isEditing = false;
    }
}

paymentMapController.$inject = deps;

module.exports = paymentMapController;
