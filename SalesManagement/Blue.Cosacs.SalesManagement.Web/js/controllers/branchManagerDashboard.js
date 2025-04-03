'use strict';

var branchManagerDashboardController = function ($scope, $http, UsersService, csrList) {
    $scope.undeliveredAccounts = {};

    var displayDataByBranch = function () {
        $http.get('/SalesManagement/api/BranchManagerDashboard')
            .then(function (resp) {
                $scope.IsLoading = resp.data.IsLoading
                //Call Summary
                $scope.callSummary = resp.data.CallsSummary;
                $scope.isRefreshing = resp.data.IsRefreshing;

                //Sales KPI
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

                if (resp.data.HitRate.length > 0) {
                    $scope.hitRateKpi = resp.data.HitRate;
                }

                //Sales Summary
                if (resp.data.SalesSummary.length > 0) {
                    $scope.salesSummary = JSON.parse(resp.data.SalesSummary);
                }
            });
    }

    csrList().then(function (resp) {
            var results = {};
            var list = {};

            _.forEach(resp.data, function (current) {
                results[current.k] = current.k + ' - ' + current.v;
                list[current.k] = current.v;
            });

            $scope.csrList = results;
        });

    $scope.search = function () {
        if (!$scope.csrId) {
            displayDataByBranch();
            return;
        }
        
        $http.get('/SalesManagement/api/BranchManagerDashboard?userId=' + $scope.csrId)
            .then(function (resp) {
                $scope.IsLoading = resp.data.IsLoading;
                $scope.callSummary = resp.data.CallsSummary;
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
    };

    $scope.$watch(function (scope) {
        return scope.csrId;
    }, function () {
        $scope.callSummary = [];
        $scope.slowSrs = [];
        $scope.salesSummary = [];
        $scope.newCustomerAcquisitionAndCancellations = [];
        $scope.salesKPIFirstWeek = [];
        $scope.hitRateKpi = [];
        $scope.rewritesKPI = [];
        $scope.undeliveredAccounts = [];
        $scope.creditMixKip = [];
        $scope.search();
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
}

branchManagerDashboardController.$inject = ['$scope', '$http', 'UsersService', 'csrList'];

module.exports = branchManagerDashboardController;