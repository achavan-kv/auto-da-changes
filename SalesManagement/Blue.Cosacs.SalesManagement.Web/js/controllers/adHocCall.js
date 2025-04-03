'use strict';

var adHocCallController = function ($scope, $http, $modalInstance, UsersService) {
    $scope.call.ReasonToCallAgain = $scope.call.Subject + ' - ' + $scope.call.Id;

    $scope.logCall = function (e) {
        e.preventDefault();

        sendCallToServer($http.put, '/SalesManagement/api/CallLog/Put', $scope.call);
        $modalInstance.close($scope.call);
    };

    $scope.cancel = function (e) {
        e.preventDefault();
        $modalInstance.close(null);
    };

    function getCurrentDateTime() {
        return $http({
            url: '/SalesManagement/api/CallLog/GetCurrentDateTime',
            method: "GET"
        });
    };

    getCurrentDateTime()
               .then(function (resp) {
                   $scope.call.CalledAt = new Date(resp.data.Date);
                   $scope.call.CalledAtHour = new Date(moment($scope.call.CalledAt).format('DD MMMM YYYY HH:mm'));
               });


    function sendCallToServer(method, url, objectToSend) {

        var scheduleCallback = null;

        if (objectToSend.ScheduleCallback != null) {
            var callbackHour = moment(objectToSend.ScheduleCallbackHour).get('hour');
            var callbackMinutes = moment(objectToSend.ScheduleCallbackHour).get('minute');

            scheduleCallback = moment(objectToSend.ScheduleCallback).set('hour', callbackHour).set('minute', callbackMinutes)._d;
        }

        var calledAtHour = moment(objectToSend.CalledAtHour).get('hour');

        var calledAtMinutes = moment(objectToSend.CalledAtHour).get('minute');

        var objectToSave = {
            CalledAt: moment(objectToSend.CalledAt).set('hour', calledAtHour).set('minute', calledAtMinutes)._d,
            SpokeToCustomer: _.isUndefined(objectToSend.SpokeToCustomer) ? false : objectToSend.SpokeToCustomer,
            Comments: _.isUndefined(objectToSend.Comments) ? '' : objectToSend.Comments,
            ScheduleCallBack: scheduleCallback,
            ReasonToCallAgain: objectToSend.ReasonToCallAgain,
            ReasonToCall: objectToSend.ReasonToCallAgain,
            PreviousReasonToCall : objectToSend.ReasonToCallAgain,
            CustomerFirstName: objectToSend.CustomerFirstName,
            CustomerLastName: objectToSend.CustomerLastName,
            CustomerId: objectToSend.CustomerId,
            DoNotCallAgain: _.isUndefined(objectToSend.DoNotCallAgain) ? false : objectToSend.DoNotCallAgain,
            SalesPersonId: UsersService.getCurrentUser().userId,
            PendingCalls: []
        };

        method(url, objectToSave)
         .success(function (resp) {
         });
    }
};
adHocCallController.$inject = ['$scope', '$http', '$modalInstance', 'UsersService'];

module.exports = adHocCallController;