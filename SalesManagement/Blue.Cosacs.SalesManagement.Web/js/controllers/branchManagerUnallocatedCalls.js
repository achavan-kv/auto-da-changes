'use strict'

var branchManagerUnallocatedCallsController = function ($scope, $http, UsersService, $q, CommonService, csrList, unallocatedCallsService) {
    var lockedCSRList = [];
    var unavailableCSRList = [];
    var noOfCallsToDisplay = 50;

    function branchManagerCalls() {

        var searchDeferred = [];

        function search(){

            var d = $q.defer();
            var objectToSend = {
                Take: noOfCallsToDisplay,
                NoCSR: $scope.noCSR,
                UnavailableCSR: $scope.unavailableCSR,
                LockedCSR: $scope.lockedCSR,
                lockedCSRList: lockedCSRList
            };

            unallocatedCallsService.search(objectToSend)
                .success(function(data){
                    d.resolve(data);
                })
                .error(function(error){
                    d.reject();
                });
            return d.promise;
        };

        searchDeferred.push(search());

        // if ($scope.userNoPermission){
        function getUserPermissons() {

            var d = $q.defer();

            unallocatedCallsService.userWithPermissionForCallLog()
                .success(function(data){
                    d.resolve(data);
                })
                .error(function(error){
                    d.reject();
                });

            return d.promise;
        }

        searchDeferred.push(getUserPermissons());
        // }

        $q.all(searchDeferred).then(
            function (results) {
                var allCalls = [];
                var calls = _.map(results[0].BranchManagerCalls, function (current) {
                    current['Selected'] = false;
                    current.ToCallAt = moment.utc(current.ToCallAt).local()._d;
                    return current;
                });

                if ($scope.userNoPermission){
                    var userIds = _.map(results[1], function(current){
                        return current.k;
                    });

                    _.each(calls, function(c){
                        var hasPermission = false;
                        //check on the user with permissions if the current sales person (on c variable) has permission
                        if (!_.isNull(c.SalesPersonId)) {
                            //if the sales person is not in the array of users means that he/she/it doesn't have permission
                            //in this case we want these "people"
                            for (var i = 0, len = userIds.length; i < len; i++) {
                                if (c.SalesPersonId === userIds[i]){
                                    hasPermission = true;
                                    break;
                                }
                            }
                        }

                        if (!hasPermission){
                            allCalls.push(c);
                        }
                    });
                }
                else{
                    allCalls = calls;
                }

                $scope.results = allCalls;
                if ($scope.results.length === 0) {
                    $scope.emptyResults = 1;
                }
                else {
                    $scope.noOfCallsDisplayed = results[0].BranchManagerCalls.length;
                    $scope.noOfCalls = results[0].NoOfScheduledCalls;

                    $scope.emptyResults = 2;
                    $scope.getGroupedDates = changeDateFormat(getGroupedDates($scope.results));
                }
            });
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

    function isCallOutDated(date) {
        return date < moment()._d.setHours(0, 0, 0, 0);
    };

    function canSearch(){
        return $scope.unavailableCSR || $scope.noCSR || $scope.lockedCSR || $scope.userNoPermission;
    }

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
        return unallocatedCallsService.getLockedCSRs()
        .then(function (resp) {
            lockedCSRList = _.pluck(resp.data.response.docs, 'UserId');
        });
    };

    function getUnavailableCSRs() {
        return unallocatedCallsService.getUnavailableCSRs()
        .then(function (resp) {
            unavailableCSRList = resp.data;
        });
    }

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

    function getSelectedCalls() {

        var selectedCalls = _.filter($scope.results, { 'Selected': true });
        return _.pluck(selectedCalls, 'CallId');
    }

    function toggleOption(option){
        $scope[option] = !$scope[option];
    }

    $scope.emptyResults = 0;
    $scope.unavailableCSR = true;
    $scope.noCSR = true;
    $scope.lockedCSR = true;
    $scope.userNoPermission = true;

    $scope.clearFilter = function () {
        $scope.unavailableCSR = false;
        $scope.noCSR = false;
        $scope.lockedCSR = false;
        $scope.results = [];
    };

    $scope.canSearch = canSearch;

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

        branchManagerCalls();

        var allNot = lockedCSRList.concat(unavailableCSRList);
        var finalList = $scope.csrList;

        _.each(allNot, function (current) {
            delete finalList[current];
        });

        $scope.newCsrList = finalList;

    });

    $scope.enableAllocateCalls = function () {
        return _.any($scope.results, function (current) {
            return current.Selected
        });
    };

    $scope.search = function () {
        if (!canSearch()){
            return;
        }

        $scope.displayCallLog = 0;
        if ($scope.noCSR || $scope.unavailableCSR || $scope.lockedCSR) {
            $scope.csrId = '';
        }

        $scope.isSearchClicked = true;
        branchManagerCalls();
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

    $scope.selectAllCalls = function () {
        _.forEach($scope.results, function (current) {
            current.Selected = $scope.selectAll;
        });
    };

    $scope.allocateCallsToCSR = function () {
        var callIds = getSelectedCalls();

        var objectToSave = {
            SelectedCalls: callIds,
            SalesPersonId: $scope.newCsrId
        };

        unallocatedCallsService.allocateCallsToCSR(objectToSave)
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

    $scope.toggleOption = toggleOption;
};

branchManagerUnallocatedCallsController.$inject = ['$scope', '$http', 'UsersService', '$q', 'CommonService', 'csrList', 'branchManagerUnallocatedCallsService'];
module.exports = branchManagerUnallocatedCallsController;