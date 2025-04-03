'use strict'

var quickDetailsCaptureController = function ($scope, $http, UsersService, CommonService, xhr) {
    $scope.currentCall = {};
    $scope.unlock = true;
    $scope.keepLocked = false;
    $scope.emptyResults = 0;

    $scope.clearFilter = function () {
        $scope.unlock = true;
        $scope.keepLocked = false;
        $scope.currentCall = [];
        btnSearch.disabled = true;
        $scope.results = [];
        $scope.emptyResults = 0;
    };

    $scope.logCall = function () {
        sendCallToServer($http.post, '/SalesManagement/api/QuickDetailsCapture/InsertCall', $scope.currentCall);
        btnSearch.disabled = true;
    };

    function sendCallToServer(method, url, objectToSend) {

        if (objectToSend.ToCallAtHour != null) {
            var calledAtHour = moment(objectToSend.ToCallAtHour).get('hour');
            var calledAtMinutes = moment(objectToSend.ToCallAtHour).get('minute');
        }

        if (!$scope.keepLocked) {
            objectToSend.CustomerId = null;
        }

        var objectToSave = {
            CustomerId: objectToSend.CustomerId,
            SpokeToCustomer: false,
            ScheduleCallBack: moment(objectToSend.ToCallAt).set('hour', calledAtHour).set('minute', calledAtMinutes)._d,
            ReasonToCallAgain: objectToSend.ReasonForCalling,
            CustomerFirstName: objectToSend.CustomerFirstName,
            CustomerLastName: objectToSend.CustomerLastName,
            SalesPersonId: _.isUndefined(objectToSend.SalesPersonId) ? '' : objectToSend.SalesPersonId,
            MobileNumber: objectToSend.MobilePhone
        };

        method(url, objectToSave)
         .success(function (resp) {
             CommonService.addGrowl({
                 timeout: 5,
                 type: 'success', // (optional - info, danger, success, warning)
                 content: 'The call was successfully scheduled!'
             });

             $scope.clearFilter()
             $scope.unlock = true;
             $scope.keepLocked = false;
         });
    };

    $scope.isCallbackHourValid = function (currentCall) {
        if (!currentCall.ToCallAt) {
            return false;
        }

        if (!currentCall.ToCallAtHour) {
            return false;
        }

        return true;
    }

    function getData(url, params) {
        var request = xhr.get(url, {
            params: params
        });
        return (request.then(CommonService.handleAjaxSuccess, CommonService.handleAjaxError));
    }

    $scope.searchCustomer = function () {
        var customerDetails = {
            CustomerId: _.isUndefined($scope.currentCall.CustomerId) ? '' : $scope.currentCall.CustomerId,
            FirstName: _.isUndefined($scope.currentCall.CustomerFirstName) ? '' : $scope.currentCall.CustomerFirstName,
            LastName: _.isUndefined($scope.currentCall.CustomerLastName) ? '' : $scope.currentCall.CustomerLastName,
            MobileNumber: _.isUndefined($scope.currentCall.MobilePhone) ? '' : $scope.currentCall.MobilePhone,
            Email: _.isUndefined($scope.currentCall.Email) ? '' : $scope.currentCall.Email,
            AddressLine1: _.isUndefined($scope.currentCall.AddressLine1) ? '' : $scope.currentCall.Address,
            AddressLine2: _.isUndefined($scope.currentCall.AddressLine2) ? '' : $scope.currentCall.Address,
            TownOrCity: _.isUndefined($scope.currentCall.TownOrCity) ? '' : $scope.currentCall.TownOrCity,
            Title: _.isUndefined($scope.currentCall.Title) ? '' : $scope.currentCall.Title,
            Alias: _.isUndefined($scope.currentCall.Alias) ? '' : $scope.currentCall.Alias,
            PostCode: _.isUndefined($scope.currentCall.PostCode) ? '' : $scope.currentCall.PostCode
        }

        customerDetails.Start = 0;
        customerDetails.Rows = 50; // Display only 50 records

        getData('/Customer/api/Reindex', customerDetails)
        .then(function (data) {
            if (typeof data.response === 'object' && data.response.docs) {
                $scope.results = data.response.docs;

                if ($scope.results.length != 0) {
                    $scope.emptyResults = 2;
                }
                else {
                    $scope.emptyResults = 1;
                }
            }
        });
    };

    $scope.populateCustomerDetails = function (CustomerId) {
        $scope.currentCall = {};

        var customerDetails = _.filter($scope.results, function (current) {
            return current.CustomerId === CustomerId;
        })[0];

        $scope.currentCall.CustomerId = customerDetails.CustomerId;
        $scope.currentCall.CustomerFirstName = customerDetails.FirstName;
        $scope.currentCall.CustomerLastName = customerDetails.LastName;
        $scope.currentCall.Email = customerDetails.Email;
        $scope.currentCall.TownOrCity = customerDetails.TownOrCity;
        $scope.currentCall.Title = customerDetails.Title;
        $scope.currentCall.AddressLine1 = customerDetails.AddressLine1;
        $scope.currentCall.AddressLine2 = customerDetails.AddressLine2;
        $scope.currentCall.Alias = customerDetails.Alias;
        $scope.currentCall.PostCode = customerDetails.PostCode;
        $scope.currentCall.MobilePhone = customerDetails.MobileNumber ? customerDetails.MobileNumber : customerDetails.HomePhoneNumber;
        $scope.currentCall.SalesPersonId = customerDetails.SalesPersonId;

        $scope.unlock = false;
        $scope.keepLocked = true;
        btnSearch.disabled = false;
    };

    $scope.editDetails = function () {
        $scope.unlock = true;
        $scope.keepLocked = true;
    };

    $scope.isDataValid = function (currentCall) {
        btnSearch.disabled = false;
        if (!(currentCall.ToCallAtHour &&
             currentCall.ToCallAt &&
             currentCall.ReasonForCalling &&
            currentCall.MobilePhone)) {
            return false;
        }

        if (($scope.keepLoked && !currentCall.CustomerFirstName) || ($scope.keepLocked && !currentCall.CustomerLastName)) {
            return true;
        }

        if ((!$scope.keepLocked && !currentCall.CustomerFirstName) || (!$scope.keepLocked && !currentCall.CustomerLastName)) {
            return false;
        }

        return true;
    }
};

quickDetailsCaptureController.$inject = ['$scope', '$http', 'UsersService', 'CommonService', 'xhr'];
module.exports = quickDetailsCaptureController;