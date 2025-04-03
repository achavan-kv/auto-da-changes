'use strict';

var csrCustomerSearchController = function ($scope, xhr, CommonService, UsersService, $routeParams) {
    $scope.results = {
        response: {
            docs: []
        }
    };

    $scope.ToCallAt = null;
    $scope.ToCallAtHour = new Date(0);
    $scope.scheduleDisabled = false;

    $scope.selectAll = function () {
        selectAll();
    };

    function unSelectAll() {
        _.each($scope.results.response.docs, function (current) {
            current.selected = false;
        });
    }

    function canLog() {
        var returnValue = false;
        if ($scope.results && $scope.results.response && $scope.results.response.docs) {
            returnValue = _.any($scope.results.response.docs, function (current) {
                return current.selected;
            });
        }

        return returnValue && $scope.ToCallAt && $scope.ToCallAtHour && $scope.ReasonForCalling;
    }

    $scope.canLog = function () {
        return canLog();
    }

    $scope.$on('facetsearch:action:clear', function () {
        $scope.ToCallAt = null;
        $scope.ToCallAtHour = new Date(0);
        $scope.ReasonForCalling = null;
    });

    $scope.$watch(function (scope) {
        return scope.results.response.docs;
    }, function (newValue, oldValue) {

        _.each(newValue, function (current) {

            var index = _.findIndex(oldValue, function (c) {
                return c.CustomerId === current.CustomerId;
            });

            if (index !== -1) {
                current["selected"] = oldValue[index]["selected"];
            }
            else {
                current["selected"] = false
            }

            return current;
        });
    });

    $scope.logCall = function () {
        if (!canLog() && !$scope.scheduleDisabled) {
            return;
        }
        var customers = [];
        var time = moment($scope.ToCallAtHour);

        _.each($scope.results.response.docs, function (current) {
            if (current.selected) {
                customers.push({
                    CustomerId: current.CustomerId,
                    SalesPersonId: current.SalesPersonId,
                    CustomerFirstName: current.FirstName,
                    CustomerLastName: current.LastName,
                    MobileNumber: current.MobileNumber,
                    LandLinePhone: current.HomePhoneNumber,
                    Email: current.Email ? current.Email : current.Email
                })
            }
        });

        var objectToSave = {
            Customers: customers,
            ReasonForCalling: $scope.ReasonForCalling,
            ToCallAt: moment($scope.ToCallAt).set('hour', time.get('hour')).set('minute', time.get('minute'))._d
        };
        $scope.scheduleDisabled = true;
        xhr.put('/SalesManagement/api/ScheduleCallsBulk/CSRBulk', objectToSave)
            .then(function (data) {
                $scope.ToCallAt = null;
                $scope.ToCallAtHour = new Date(0);
                $scope.ReasonForCalling = null;
                CommonService.addGrowl({
                    timeout: 5,
                    type: 'success', // (optional - info, danger, success, warning)
                    content: 'Successfully scheduled the call(s) !'
                });
                $scope.scheduleDisabled = false;
            });

        unSelectAll();
    };
};

csrCustomerSearchController.$inject = ['$scope', 'xhr', 'CommonService', 'UsersService', '$routeParams'];

module.exports = csrCustomerSearchController;