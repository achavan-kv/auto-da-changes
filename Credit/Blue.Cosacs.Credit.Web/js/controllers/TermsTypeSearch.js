var termsTypeSearchController = function ($scope, $http) {
    $scope.search = {};
    $scope.customerTypes = ['New', 'Returning'];
    $scope.agreementTypes = ['RF Agreement', 'HP Agreement', 'Store Card Agreement'];
    $scope.cashLoanCustomers = ['New Customer', 'Recent Customer', 'Existing Customer', 'Staff Customer'];

    $http.get('/Credit/api/ScoringBandMatrix/')
        .then(function (result) {
            $scope.customerBands = _.map(result.data, 'Band');
        });

    $scope.fromNow = function (date) {
        if (!date) {
            return null;
        }
        return moment(date.substring(0, date.length - 1)).fromNow();
    };

    $scope.clear = function () {
        $scope.search = {};
        $scope.results = {};
    };

    $scope.local = function (date) {
        return date.substring(0, date.length - 1)
    }

};
termsTypeSearchController.$inject = ['$scope', '$http'];
module.exports = termsTypeSearchController;