'use strict';

var exportSmsController = function ($scope, $http) {

    $scope.noData = true;

    $http({
        url: '/Communication/api/ExportSms/LoadData',
        method: "GET"
    })
    .then(function (resp) {
        $scope.noData = (_.parseInt(resp.data.NotExportedYet) === 0);
        $scope.previewsExports = _.map(resp.data.PreviousExports, function(current){
            return {
                FriendlyExportedOn: moment(current.Item1).format('DD-MM-YYYY hh:mm'),
                ExportedOn: current.Item1,
                Total: current.Item2
            };
        });
    });

};

exportSmsController.$inject = ['$scope', '$http'];
module.exports = exportSmsController;