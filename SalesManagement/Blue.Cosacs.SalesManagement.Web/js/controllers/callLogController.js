'use strict'

var callLogController = function ($scope, $http, $dialog, UsersService, CommonService) {

    $scope.scheduledDateFrom = '';
    $scope.scheduledDateTo = '';
    $scope.customerName = '';
    $scope.customerName = '';
    $scope.reasonForCalling = '';
    $scope.callTypeId = '';
    $scope.emptyResults = 0;
    $scope.displayCallLog = 0;
    $scope.isLogCallSection = false;
    $scope.customerPreviousCalls = {};
    var csrList = [];
    $scope.hasItemInService = false;
    $scope.arrearsAndUndeliveredAccount = false;

    var noOfCallsToDisplay = 50;

    //this query may take a few seconds so it will be better do it right now
    getCustomers();

    $scope.clearFilter = function () {

        if ($scope.isLogCallSection) {
            var confirmationMessage = 'Clear'
            displayConfirmationMessage(confirmationMessage)
            .then(function (result) {
                if (result == 'yes') {
                    clearControls();
                }
                else {
                    //Cancel
                    $scope.displayCallLog = 4;
                }
            });
        }
        else {
            clearControls();
        }
    };

    function clearControls() {
        $scope.scheduledDateFrom = '';
        $scope.scheduledDateTo = '';
        $scope.customerName = '';
        $scope.reasonForCalling = '';
        $scope.callTypeId = '';
        $scope.emptyResults = 0;
        $scope.displayCallLog = 0;
        $scope.isLogCallSection = false;
    };

    searchForCalls();

    function displayConfirmationMessage(message, isLogCallSection) {
        return $dialog.prompt('The call was not logged. Are you sure you want to ' + message + ' ?', 'Log Unscheduled Call')
    }

    clearNewCall();

    function clearNewCall() {
        $scope.newCall = {
            Id: 0,
            CustomerId: '',
            MobileNumber: '',
            LandLinePhone: '',
            PendingCalls: null,
            ReasonToCall: '',
            CalledAt: null,
            SpokeToCustomer: null,
            ScheduleCallback: null,
            DoNotCallAgain: false,
            Comments: '',
            ScheduleCallbackHour: null,
            CustomerFirstName: '',
            CustomerLastName: ''
        };
    };

    $scope.search = function () {

        if ($scope.isLogCallSection) {
            var confirmationMessage = 'Search for Calls'
            displayConfirmationMessage(confirmationMessage)
            .then(function (result) {
                if (result == 'yes') {
                    searchForCalls();
                }
                else {
                    //Cancel
                    $scope.displayCallLog = 4;
                }
            });
        }
        else {
            searchForCalls();
        }
    };

    function searchForCalls() {
        $scope.isLogCallSection = false;
        $scope.displayCallLog = 0;
        refreshScheduledCalls()
        .then(fillResults);
    }

    $http({
        url: '/SalesManagement/api/CallLog/GetCallTypes',
        method: "GET"
    })
    .then(function (resp) {
        var results = {};

        _.forEach(resp.data, function (current) {
            results[current.Id] = current.Name;
        });

        $scope.callTypes = results;
    });

    $scope.getScheduledCallsDetailsByDate = function (date) {
        var scheduledCallsDetails = $scope.results;

        var t = _.filter(scheduledCallsDetails, function (current) {
            return moment(new Date(current.ToCallAt)).dayOfYear() === moment(date).dayOfYear();
        });

        var scheduledCallDetails = _.map(t, function (current) {
            current['Hour'] = moment(current.ToCallAt).format('HH:mm');
            current['Icon'] = current.Icon;
            return current;
        });

        return scheduledCallDetails;
    };

    function getGroupedDates(calls) {
        var dates = _.pluck(calls, 'ToCallAt');

        var formattedDates = _.map(dates, function (value) {

            var date = new Date(value);
            date.setHours(0, 0, 0);
            return date;
        });

        var returnValue = _.uniq(formattedDates, false, function (current) {
            return moment(current).format('DDMMYYYY');
        });

        return returnValue;
    }

    function changeDateFormat(date) {
        var values = [];
        _.forEach(date, function (current) {
            var obj = {
                Date: current,
                FormatedDate: moment(current).format('ddd Do MMMM YYYY') + (isCallOutDated(current) ? ' - Outdated call(s) ' : '')
            }
            values.push(obj);
        });

        return values;
    };

    $scope.getDateRowClass = function (date) {
        if (isCallOutDated(date)) {
            return 'danger';
        }

        return 'success';
    };

    function getPendingCalls(customerId, currentCallId) {

        var values = _.filter($scope.results, function (current) {
            return current.CustomerId === customerId && current.Id != currentCallId && current.CustomerId != null;
        });

        var results = [];

        _.forEach(values, function (current) {
            var item = {
                CallId: current.Id,
                Message: moment(current.ToCallAt).format('DD MMMM YYYY HH:mm') + ' ' + current.ReasonToCall,
                Selected: true
            };

            results.push(item);
        });

        return results;
    };

    $scope.displayCallLogSection = function (Id) {
        if ($scope.isLogCallSection) {
            var confirmationMessage = 'Log Calls '
            displayConfirmationMessage(confirmationMessage)
            .then(function (result) {
                if (result == 'yes') {
                    displayLogCall(Id);
                }
                else {
                    //Cancel
                    $scope.displayCallLog = 4;
                }
            });
        }
        else {
            displayLogCall(Id);
        }
    };

    function displayLogCall(Id) {
        $scope.isLogCallSection = false;
        getCurrentDateTime()
            .then(function (resp) {
                $scope.displayCallLog = 1;
                $scope.currentCall = _.filter($scope.results, function (current) {
                    return current.Id === Id
                })[0];

                $scope.currentCall.CalledAt = new Date(resp.data.Date);
                $scope.currentCall.CalledAtHour = new Date(moment($scope.currentCall.calledAt).format('DD MMMM YYYY HH:mm'));
                $scope.currentCall.customerFullNameLog = $scope.currentCall.CustomerFirstName + ' ' + $scope.currentCall.CustomerLastName;
                $scope.currentCall.PendingCalls = getPendingCalls($scope.currentCall.CustomerId, Id);
                $scope.currentCall.ReasonToCallAgain = '';

                if ($scope.currentCall.CustomerId != null && $scope.currentCall.CustomerId != '') {
                    getCustomerDetails($scope.currentCall.CustomerId)
                    .then(function (resp) {
                            if (!_.isNull(resp.data)) {
                                $scope.currentCall.Phone = resp.data.PhoneNumber;
                                $scope.currentCall.Email = resp.data.Email;
                            }
                    });
                }

                getCustomerPreviousCalls($scope.currentCall.CustomerId);
                hasItemInService($scope.currentCall.CustomerId);
                isInTheArrearsAndHasUndeliveredAccount($scope.currentCall.CustomerId);
               
            });
    };

    function isCallOutDated(date) {
        return date < moment()._d.setHours(0, 0, 0, 0)
    };

    function refreshScheduledCalls() {
        return $http({
            url: '/SalesManagement/api/CallLog/GetScheduledCalls',
            method: "GET",
            params: {
                CallTypeId: _.isNull($scope.callTypeId) ? '' : $scope.callTypeId,
                FromScheduledDate: _.isNull($scope.scheduledDateFrom) ? '' : $scope.scheduledDateFrom,
                ToScheduledDate: _.isNull($scope.scheduledDateTo) ? '' : $scope.scheduledDateTo,
                CustomerName: $scope.customerName,
                ReasonForCalling: $scope.reasonForCalling,
                Take: noOfCallsToDisplay
            }
        });
    };

    $scope.logCall = function () {
        $scope.isLogCallSection = false;
        sendCallToServer($http.post, '/SalesManagement/api/CallLog/Post', $scope.currentCall);
    };

    $scope.displayUnscheduledLog = function () {
        clearNewCall();
        $scope.displayCallLog = 4;
        $scope.isLogCallSection = true;

        //getCustomers();

        getCurrentDateTime()
                .then(function (resp) {
                    $scope.newCall.CalledAt = new Date(resp.data.Date);
                    $scope.newCall.CalledAtHour = new Date(moment($scope.newCall.CalledAt).format('DD MMMM YYYY HH:mm'));
                });
    };

    function getCustomers() {
        //i have no way to say how many rows should it bring, so i guess 1K rows is fine...lets hope so
        var url =  '/Customer/Api/Reindex/SearchByCsr?branch=' + UsersService.getCurrentUser().BranchNumber + '&start=0&rows=1000';

        return $http({
            url: url,
            method: "GET"
        })
         .then(function (resp) {
             var docs = resp.data.response.docs;
             var customers = _.uniq(docs, false, function (current) {
                 return current.CustomerId;
             });

             var results = {};

             _.forEach(customers, function (current) {
                 results[current.CustomerId] = current.FirstName + ' ' + current.LastName;
             });

             $scope.newCall.CustomerList = results;
         });
    };

    function getCustomerDetails(CustomerId) {
        return $http({
            url: '/SalesManagement/api/CallLog/GetCustomerDetails',
            method: "GET",
            params: {
                customerId: CustomerId
            }
        })
    };

    function getNewCallPendingCalls(CustomerId) {
        return $http({
            url: '/SalesManagement/api/CallLog/GetCallsDetails',
            method: "GET",
            params: {
                customerId: CustomerId
            }
        }).then(function (resp) {
            var results = [];

            _.forEach(resp.data, function (current) {
                var item = {
                    CallId: current.Id,
                    Message: moment(current.ToCallAt).format('DD MMMM YYYY HH:mm') + ' ' + current.ReasonToCall,
                    Selected: true
                };

                results.push(item);
            });

            $scope.newCall.PendingCalls = results;
        });
    };

    function getCurrentDateTime() {
        return $http({
            url: '/SalesManagement/api/CallLog/GetCurrentDateTime',
            method: "GET"
        });
    };

    $scope.cancelLogCall = function () {
        $scope.displayCallLog = 0;
    };

    $scope.$watch(function (scope) {
        return scope.newCall.CustomerId;
    }, function (current) {

        if (current != "" && current != null) {
            getCustomerDetails(current)
               .then(function (resp) {
                    $scope.newCall.MobileNumber = resp.data.MobileNumber;
                    $scope.newCall.LandLinePhone = resp.data.LandLinePhone;
                    $scope.newCall.CustomerFirstName = resp.data.CustomerFirstName;
                    $scope.newCall.CustomerLastName = resp.data.CustomerLastName;
               });

            getNewCallPendingCalls(current);
        }
        else {
            if (current == null) {
                clearNewCall();
                getCurrentDateTime()
                .then(function (resp) {
                    $scope.newCall.CalledAt = new Date(resp.data.Date);
                    $scope.newCall.CalledAtHour = new Date(moment($scope.newCall.CalledAt).format('DD MMMM YYYY HH:mm'));
                });
            }
        }
    });

    $scope.isDataValid = function (newCall) {
        if ((!newCall.ScheduleCallbackHour && newCall.ScheduleCallback) || !newCall.CustomerId) {
            return false;
        }

        if (!newCall.ReasonToCallAgain) {
            return false;
        }

        return true;
    }

    $scope.isHourValid = function (callbackHour, scheduleCallbackUnscheduledCall) {
        if (!callbackHour && scheduleCallbackUnscheduledCall) {
            return false;
        }

        return true;
    }

    function fillResults(newData){
        $scope.results = _.map(newData.data.Calls, function(current){
            current.ToCallAt = moment.utc(current.ToCallAt).local()._d;

            return current;
        });
        if ($scope.results.length === 0) {
            $scope.emptyResults = 1;
        }
        else {
            $scope.noOfCallsDisplayed = $scope.results.length;
            $scope.noOfCalls = newData.data.NoOfScheduledCalls;

            $scope.emptyResults = 2;
            var groupedDates = getGroupedDates($scope.results);
            $scope.getGroupedDates = changeDateFormat(groupedDates);
        }

        $scope.isLogCallSection = false;
    }

    function sendCallToServer(method, url, objectToSend) {

        var callIds = _.filter(objectToSend.PendingCalls, { 'Selected': true });
        var pendingCallsToSave = _.pluck(callIds, 'CallId');

        var scheduleCallback = null;

        if (objectToSend.ScheduleCallback != null) {
            var callbackHour = moment(objectToSend.ScheduleCallbackHour).get('hour');
            var callbackMinutes = moment(objectToSend.ScheduleCallbackHour).get('minute');

            scheduleCallback = moment(objectToSend.ScheduleCallback).set('hour', callbackHour).set('minute', callbackMinutes)._d;
        }

        var calledAtHour = moment(objectToSend.CalledAtHour).get('hour');

        var calledAtMinutes = moment(objectToSend.CalledAtHour).get('minute');

        var objectToSave = {
            Id: objectToSend.Id,
            CalledAt: moment(objectToSend.CalledAt).set('hour', calledAtHour).set('minute', calledAtMinutes)._d,
            SpokeToCustomer: objectToSend.SpokeToCustomer,
            Comments: _.isNull(objectToSend.Comments) ? '' : objectToSend.Comments,
            ScheduleCallBack: scheduleCallback,
            ReasonToCallAgain: objectToSend.ReasonToCallAgain,
            PreviousReasonToCall: objectToSend.ReasonToCall,
            PendingCalls: pendingCallsToSave,
            CustomerFirstName: objectToSend.CustomerFirstName,
            CustomerLastName: objectToSend.CustomerLastName,
            CustomerId: objectToSend.CustomerId,
            DoNotCallAgain: objectToSend.DoNotCallAgain,
            SalesPersonId : objectToSend.SalesPersonId,
            CallTypeId: objectToSend.CallTypeId,
            MobileNumber: objectToSend.MobileNumber,
            LandLinePhone: objectToSend.LandLinePhone,
            AlternativeContactMeanId: objectToSend.AlternativeContactMeanId,
            EmailSubject: objectToSend.EmailSubject,
            MailchimpTemplateID: objectToSend.MailchimpTemplateID,
            SmsText: objectToSend.SmsText
        };

        method(url, objectToSave)
         .success(function () {
             $scope.displayCallLog = 0;
             refreshScheduledCalls().then(function (newData) {
                 fillResults(newData);

                 CommonService.addGrowl({
                     timeout: 5,
                     type: 'success', // (optional - info, danger, success, warning)
                     content: 'Call has been logged'
                 });
             });
         });
    }

    $scope.logUnscheduledCall = function () {

        $scope.newCall.SpokeToCustomer = true;
        sendCallToServer($http.put, '/SalesManagement/api/CallLog/Put', $scope.newCall);
    };

    $scope.changeCallLabel = function (SpokeToCustomer) {
        if (SpokeToCustomer) {
            return 'Schedule Callback';

        }
        else {
            return 'Reschedule Call';
        }
    }

    function getCustomerPreviousCalls(customerId) {
        if (customerId) {
            return $http({
                url: '/SalesManagement/api/PreviousCalls/GetPreviousCalls',
                method: "GET",
                params: {
                    customerId: customerId
                }
            }).then(function (resp) {
                var data = resp.data;
                var results = [];

                _.forEach(data, function (current) {
                    var item = {
                        RescheduledOn: _.isNull(current.RescheduledOn) ? null : moment(current.RescheduledOn).format('DD MMMM YYYY HH:mm'),
                        CalledAt: moment(current.CalledAt).format('DD MMMM YYYY HH:mm'),
                        CallTypeName: current.CallTypeName,
                        ReasonForCalling: current.ReasonForCalling,
                        SpokeToCustomer: current.SpokeToCustomer,
                        Comments: current.Comments,
                        SalesPersonName: getSalesPersonName(current.SalesPersonId)
                    };

                    results.push(item);
                });

                $scope.customerPreviousCalls = results;
            });
        }

        $scope.customerPreviousCalls = null;

        return null;
    };

    function getSalesPersonName(salesPersonId) {

        var salesPerson = _.filter(csrList, function (current) {
            return current.k == salesPersonId
        });

        return _.pluck(salesPerson, function (current) {
            return current.v;
        })[0];

    };

    $http.get('/Cosacs/Admin/Users/LoadPickListUsers?branch=' + UsersService.getCurrentUser().BranchNumber)
     .success(function (data) {
         csrList = data;
     });

    function hasItemInService(customerId) {

        if(customerId) {
            return $http.get('/cosacs/SalesManagement/HasItemInService/?customerId=' + customerId)
                .then(function (resp) {
                    $scope.hasItemInService = resp.data.HasServiceRequest;
                });
        }
        $scope.hasItemInService = false;

        return null;
    };

    function isInTheArrearsAndHasUndeliveredAccount(customerId) {
        if (customerId){
            return $http.get('/Courts.NET.WS/SalesManagement/CustomerInArrearsAndUndeliveredAccount?customerId=' + customerId)
            .then(function (resp) {
                $scope.arrearsAndUndeliveredAccount = resp.data;
            });
        }
        $scope.arrearsAndUndeliveredAccount = false;

        return null;
    };
};

callLogController.$inject = ['$scope', '$http', '$dialog', 'UsersService', 'CommonService'];

module.exports = callLogController;