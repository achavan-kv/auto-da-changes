'use strict';

var csrDashboardController = function ($scope, $http, UsersService, CommonService) {
    $scope.csrSummary = {};
    $scope.undeliveredAccounts = {};
    $scope.IsLoading = false;
    $scope.isRefreshing = false;

    function openLogCallDialog() {
        var options = {
            templateUrl: '/SalesManagement/views/templates/ScheduleCall.html',
            controller: 'adHocCallController',
            scope: $scope
        };

        return CommonService.$dialog.dialog(options);
    };

    $scope.scheduleCall = function (item) {

        $scope.call = {
            Id : item.Id,
            CustomerFirstName: item.CustomerFirstName,
            CustomerLastName: item.CustomerLastName,
            CustomerId: item.CustomerId,
            Subject : item.Subject
        };

        var modalInstance = openLogCallDialog();

        modalInstance.open().then(function (result) {
            if (!result) {
                return;
            }
        });
    };

    $http.get('/SalesManagement/api/CsrDashboard/')
        .then(function (resp) {
            $scope.IsLoading = resp.data.IsLoading;
            $scope.csrSummary = resp.data.CallsSummary;
            $scope.isRefreshing = resp.data.IsRefreshing;

            if (resp.data.SalesSummary.length > 0) {
                $scope.salesSummary = JSON.parse(resp.data.SalesSummary);
            }

            if (resp.data.NewCustomerAcquisitionAndCancellations.length > 0) {
                $scope.newCustomerAcquisitionAndCancellations = resp.data.NewCustomerAcquisitionAndCancellations;
            }

            if (resp.data.SalesKPIFirstWeek.length > 0) {
                $scope.salesKPIFirstWeek = moment(new Date(resp.data.SalesKPIFirstWeek)).format('DD MMMM YYYY');
            }

            if (resp.data.RewritesKPI.length > 0) {
                $scope.rewritesKPI = resp.data.RewritesKPI;
            }

            if (resp.data.CreditMix.length > 0) {
                $scope.creditMixKip = resp.data.CreditMix;
            }

            if (resp.data.SlowSrs.length > 0) {
                var slowSrData = JSON.parse(resp.data.SlowSrs);
                $scope.slowSrs = _.map(_.sortBy(slowSrData, "LastUpdatedOn"));
            }

            if (resp.data.HitRate.length > 0){
                $scope.hitRateKpi = resp.data.HitRate;
            }

            if (resp.data.UndeliveredAccounts.length > 0) {
                var results = [];

                _.filter(resp.data.UndeliveredAccounts, function (current) {
                    results.push(JSON.parse(current));
                });

                var values = [];
                _.forEach(results, function (current) {
                    var obj = {
                    CustomerFirstName: current.CustomerFirstName,
                    CustomerLastName: current.CustomerLastName,
                    CustomerId : current.CustomerId,
                        CustomerAccount: current.CustomerAccount,
                        ItemNo: current.ItemNo,
                        ItemDescription: current.ItemDescription,
                        StatusDescription: current.StatusDescription,
                        Phone: current.Phone,
                        DeliveryDateFormat: moment(current.DeliveryDate).format('DD MMMM YYYY'),
                        DeliveryDate: current.DeliveryDate
                    }

                    values.push(obj);
                });

                var sortedResults = _.sortBy(values, function (current) {
                    return current.DeliveryDate;
                });

                $scope.undeliveredAccounts = sortedResults;
            }
        });

    $scope.getDateRowClass = function (date) {
        if (isCallOutDated(date)) {
            return 'danger';
        }

        return '';
    };

    function isCallOutDated(date) {
        return date < moment()._d.setHours(0, 0, 0, 0);
    };
};

csrDashboardController.$inject = ['$scope', '$http', 'UsersService', 'CommonService'];

module.exports = csrDashboardController;