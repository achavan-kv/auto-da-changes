'use strict'

var branchManagerController = function ($scope, $http, UsersService, $q, CommonService, xhr, csrList) {

    $scope.scheduledDateFrom = '';
    $scope.scheduledDateTo = '';
    $scope.customerName = '';
    $scope.reasonForCalling = '';
    $scope.callTypeId = '';
    $scope.emptyResults = 0;
    $scope.displayCallLog = 0;
    $scope.csrId = '';
    $scope.unavailableCSR = false;
    $scope.noCSR = false;
    $scope.lockedCSR = false;
    $scope.isSearchClicked = false;
    $scope.selected = false;
    $scope.selectAll = false;
    var noOfCallsToDisplay = 50;

    var lockedCSRList = [];
    var unavailableCSRList = [];

    $scope.clearFilter = function () {
        $scope.scheduledDateFrom = '';
        $scope.scheduledDateTo = '';
        $scope.customerName = '';
        $scope.reasonForCalling = '';
        $scope.callTypeId = '';
        $scope.emptyResults = 0;
        $scope.displayCallLog = 0;
        $scope.csrId = '';
        $scope.unavailableCSR = false;
        $scope.noCSR = false;
        $scope.lockedCSR = false;
        $scope.isSearchClicked = false;
        $scope.selected = false;
        $scope.newCsrId = '';
        $scope.selectAll = false;
    };

    branchManagerCalls();

    $http({
        url: '/SalesManagement/api/BranchManager/GetCallTypesForBranchManager',
        method: "GET"
    })
        .then(function (resp) {
            var results = {};

            _.forEach(resp.data, function (current) {
                results[current.Id] = current.Name;
            });

            $scope.callTypes = results;
        });

    function getGroupedDates(calls) {
        var dates = _.pluck(calls, 'ToCallAt');

        var formatedDates = _.map(dates, function (value) {

            var date = new Date(value);
            date.setHours(0, 0, 0);
            return date; 
        });

        var returnValue = _.uniq(formatedDates, false, function (current) {
            return moment(current).format('DDMMYYYY');
        });

        return returnValue;
    }

    $scope.search = function () {
        $scope.displayCallLog = 0;
        if ($scope.noCSR || $scope.unavailableCSR || $scope.lockedCSR) {
            $scope.csrId = '';
        }

        $scope.isSearchClicked = true;
        branchManagerCalls();
    };

    function branchManagerCalls() {
        var objectToSave = {
            CallTypeId: _.isNull($scope.callTypeId) ? '' : $scope.callTypeId,
            ScheduledDateFrom: $scope.scheduledDateFrom,
            ScheduledDateTo: _.isNull($scope.scheduledDateTo) ? '' : $scope.scheduledDateTo,
            CustomerName: $scope.customerName,
            ReasonForCalling: $scope.reasonForCalling,
            csrId: _.isNull($scope.csrId) ? '' : $scope.csrId,
            Take: noOfCallsToDisplay
        };
        $scope.selectAll = false;

        xhr.get('/SalesManagement/api/BranchManager', {
            params: objectToSave
        }).then(function (resp) {

            var calls = _.map(resp.data.BranchManagerCalls, function (current) {
                current['Selected'] = false;
                current.ToCallAt = moment.utc(current.ToCallAt).local()._d;
                return current;
            });

            $scope.results = calls;
            if ($scope.results.length === 0) {
                $scope.emptyResults = 1;
            }
            else {

                $scope.noOfCallsDisplayed = resp.data.BranchManagerCalls.length;
                $scope.noOfCalls = resp.data.NoOfScheduledCalls;

                $scope.emptyResults = 2;
                var groupedDates = getGroupedDates($scope.results);
                $scope.getGroupedDates = changeDateFormat(groupedDates);
            }
        });
    };

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

    function isCallOutDated(date) {
        return date < moment()._d.setHours(0, 0, 0, 0);
    };

    $scope.getBranchManagerCallDetailsByDate = function (date) {
        var scheduledCallsDetails = $scope.results;

        var t = _.filter(scheduledCallsDetails, function (current) {
            return moment(current.ToCallAt).dayOfYear() === moment(date).dayOfYear();
        });
        var scheduledCallDetails = _.map(t, function (current) {
            current['Hour'] = moment(current.ToCallAt).format('HH:mm');
            return current;
        });

        return scheduledCallDetails;
    };

    $scope.getDateRowClass = function (date) {
        if (isCallOutDated(date)) {
            return 'danger';
        }

        return 'success';
    };

    $scope.clearCSRList = function (isChecked) {
        if (isChecked) {
            $scope.csrId = '';
            $scope.isSearchClicked = false;
        }
    };

    $scope.allocateCallsToCSR = function () {
        var callIds = getSelectedCalls();

        var objectToSave = {
            SelectedCalls: callIds,
            SalesPersonId: $scope.newCsrId
        };


        $http.post('/SalesManagement/api/BranchManager/Post', objectToSave)
        .then(function (resp) {
            if (resp.data.Message == "Calls updated") {
                $scope.newCsrId = '';
                branchManagerCalls();
                CommonService.addGrowl({
                    timeout: 5,
                    type: 'success', // (optional - info, danger, success, warning)
                    content: 'Successfully allocated call(s) to CSR !'
                });
            }
        });
    };

    function getSelectedCalls() {

        var selectedCalls = _.filter($scope.results, { 'Selected': true });
        return _.pluck(selectedCalls, 'CallId');
    };

    function getAllCSRs() {

        return csrList()
            .then(function (resp) {
                var results = {};

                _.forEach(resp.data, function (current) {
                    results[current.k] = current.k + ' - ' + current.v;
                });

                $scope.csrList = results;
            });
    };

    function getLockedCSRs() {
        var x = $http({
            url: '/cosacs/Admin/Users/SearchInstant?q={"facetFields":{"HomeBranchName":{"Values":["' + UsersService.getCurrentUser().BranchName + '" ]},"Locked":{"Values":["True"]}},"dateFields":{},"customQuery":[]}&start=0&rows=99999',
            method: "GET"
        });
        return x
        .then(function (resp) {
            lockedCSRList = _.pluck(resp.data.response.docs, 'UserId');
        });
    };

    function getUnavailableCSRs() {
        var x = $http({
            url: '/SalesManagement/api/BranchManager/GetUnavailableCSR',
            method: "GET"
        })
        return x
        .then(function (resp) {
            unavailableCSRList = resp.data;
        });
    }

    var deferred = $q.defer();
    var promises = [getAllCSRs(), getLockedCSRs(), getUnavailableCSRs()];

    $q.all(promises).then(
                function (results) {
                    deferred.resolve(results)
                },
                function (errors) {
                    deferred.reject(errors);
                },
                function (updates) {
                    deferred.update(updates);
                });

    deferred.promise.then(function (data) {

        var allNot = lockedCSRList.concat(unavailableCSRList);
        var finalList = $scope.csrList;

        _.each(allNot, function (current) {
            delete finalList[current];
        });

        $scope.newCsrList = finalList;

    });

    $scope.selectAllCalls = function () {
        _.forEach($scope.results, function (current) {
            current.Selected = $scope.selectAll;
        });
    };

    $scope.enableAllocateCalls = function () {
        return _.any($scope.results, function (current) {
            return current.Selected
        });
    };
};

branchManagerController.$inject = ['$scope', '$http', 'UsersService', '$q', 'CommonService', 'xhr', 'csrList'];
module.exports = branchManagerController;
