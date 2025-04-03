var termsTypeSimulatorController = function ($scope, $http, CommonService) {
    $scope.search = {};
    $scope.customerTypes = ['New Customer', 'Returning Customer'];
    $scope.agreementTypes = ['RF Agreement', 'HP Agreement', 'Store Card Agreement'];
    $scope.cashLoanCustomers = ['New Customer', 'Recent Customer', 'Existing Customer', 'Staff Customer'];
    $scope.fascias = ['Courts Store', 'Non Courts Store']; // this list will change
    $scope.search.customerTags = [];
    $scope.search.productSkus = [];
    $scope.search.fascias = [];
    $scope.isTermsTypeSimulator = true;
    var productSKUsList = [];
    $scope.search.productHierarchyTags = [];

    $scope.get = function () {
        $http.post('Credit/api/TermsTypeSimulator', $scope.search)
            .success(function (result) {
                $scope.displayResults = result;
            });
    };

    $scope.add = function (list, value, listModel) {
        if (!_.contains(list, value)) {
            list.push(value);

            $scope[listModel] = null;
        }
    };

    $http.get('/Cosacs/Merchandising/Hierarchy/GetHierarchyData/')
        .then(function (result) {
            $scope.productHierarchyLevels = _.map(result.data.Levels, 'Name');
            $scope.productHierarchyTags = result.data.Tags;
        });

    $http.get('/Cosacs/Merchandising/Products/Get')
        .then(function (result) {
            productSKUsList = result.data.data;
        });

    $http.get('/credit/api/settings')
        .then(function (result) {
            $scope.customerTags = result.data.CustomerTag;
        });

    $scope.addProductSKUs = function (value) {
        if (!_.contains($scope.search.productSkus, value)) {

            var skuDescription = _.pluck(_.filter(productSKUsList, function (item) {
                return item.skuId == value
            }), 'skuDescription')[0];

            var obj = {
                skuId: value,
                description: _.isUndefined(skuDescription) ? "" : skuDescription
            };

            $scope.search.productSkus.push(obj);
            $scope.productSku = null;
        }

        if (!_.contains(_.pluck(productSKUsList, 'skuId'), value)) {
            CommonService.addGrowl({
                timeout: 5,
                type: 'danger', // (optional - info, danger, success, warning)
                content: 'SKU not found !'
            });
        }
    };

    $scope.addProductHierarchy = function (value, level) {
        var obj = {
            tag: value,
            level: level
        };

        var values = _.pluck($scope.search.productHierarchyTags, 'Tag')
        if (!_.contains(values, value)) {
            $scope.search.productHierarchyTags.push(obj);
        }

        $scope.productHierarchyTag = null;
        $scope.productHierarchyLevel = null;

    };

    $scope.$watch('productHierarchyLevel', function (productHierarchyLevel) {
        $scope.productHierarchyTagNames = _.pluck(_.filter($scope.productHierarchyTags, function (item) {
            return item.Level.Name == productHierarchyLevel;
        }), 'Name');
    });

    $scope.remove = function (index, list) {
        list.splice(index, 1);
    };

    $scope.clearSearch = function () {
        $scope.search = {};
        $scope.displayResults = {};
        $scope.search.productHierarchyTags = [];
        $scope.search.customerTags = [];
        $scope.search.productSkus = [];
        $scope.search.fascias = [];
    };

    $scope.fromNow = function (date) {
        if (!date) {
            return null;
        }
        return moment(date).fromNow();
    };

    $scope.local = function (date) {
        if (!$scope.isTermsTypeSimulator) {
            return date.substring(0, date.length - 1)
        }

        return date;
    }
};
termsTypeSimulatorController.$inject = ['$scope', '$http', 'CommonService'];
module.exports = termsTypeSimulatorController;