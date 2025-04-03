var termsTypeController;
termsTypeController = function ($scope, $http, $dialog, CommonService, $routeParams) {

    $scope.cashLoanCustomers = ['New Customer', 'Recent Customer', 'Existing Customer', 'Staff Customer'];
    $scope.customerTypes = ['New Customer', 'Returning Customer'];
    $scope.fascias = ['Courts Store', 'Non Courts Store']; // this list will change
    clean();
    $scope.termsType.customerBands = [];
    $scope.termsType.isRFAgreement = true;
    $scope.termsType.isHPAgreement = true;
    $scope.state = $routeParams.State;
    $scope.termsTypeId = $routeParams.Id;
    var productSKUsList = [];
    $scope.addNew = false;
    $scope.newCustomerBand = newCustomerBand();

    if ($routeParams.Id != 'new') {
        $http.get('/Credit/api/TermsType/' + $routeParams.Id)
            .then(function (result) {
                if (!_.isNull(result.data)) {
                    _.forEach(result.data.customerBands, function (obj) {
                        obj.startDate = new Date(obj.startDate);
                        obj.endDate = _.isNull(obj.endDate) ? null : new Date(obj.endDate) ;
                        obj.locked = true;
                        obj.readonly = true;
                    })
                }
                $scope.termsType = result.data;
            });

    }

    $http.get('/Credit/api/ScoringBandMatrix/')
        .then(function (result) {
            $scope.customerBands = _.map(result.data, function (obj) {
                return {k: {bandName: obj.Band, bandId: obj.Id}, v: obj.Band}
            });

            $scope.bands = result.data;

            if ($scope.termsTypeId == 'new') {
                _.forEach(result.data, function (result) {
                    var obj = {
                        id: result.Id,
                        scoreCard: result.ScoreCard,
                        pointsFrom: result.PointsFrom,
                        pointsTo: result.PointsTo,
                        name: result.Band,
                        interestRatePercentage: 0,
                        depositRequiredPercentage: 0,
                        cpiPercentage: 0,
                        adminPercentage: 0,
                        adminValue: 0,
                        startDate: new Date(),
                        endDate: null,
                        delete : false,

                    }
                    $scope.termsType.customerBands.push(obj);
                })
            }

        });

    $http.get('/Cosacs/Merchandising/Products/Get')
        .then(function (result) {
            productSKUsList = result.data.data;
        });

    $http.get('/Cosacs/Merchandising/Hierarchy/GetHierarchyData/')
        .then(function (result) {
            $scope.productHierarchyLevels = _.map(result.data.Levels, 'Name');
            $scope.productHierarchyTags = result.data.Tags;
        });

    $http.get('/credit/api/settings')
        .then(function (result) {
            $scope.customerTags = result.data.CustomerTag;
        });

    $scope.save = function () {
        if ($scope.termsType.isCashLoanNewCustomer ||
            $scope.termsType.isCashLoanExistingCustomer ||
            $scope.termsType.isCashLoanRecentCustomer ||
            $scope.termsType.isCashLoanStaffCustomer) {
            $scope.termsType.CustomerType = "";
        }

        $scope.termsType.state = $routeParams.State;

        $http.post('/Credit/api/TermsType', $scope.termsType)
            .success(function () {
                CommonService.addGrowl({
                    timeout: 5,
                    type: 'success', // (optional - info, danger, success, warning)
                    content: 'Successfully saved the terms type !'
                });

                clean();
                $scope.termsType.isRFAgreement = true;
                $scope.termsType.isHPAgreement = true;
                $scope.termsType.customerBands = [];
                $http.get('/Credit/api/ScoringBandMatrix/')
                    .then(function (result) {
                        _.forEach(result.data, function (result) {
                            var obj = {
                                id: result.Id,
                                scoreCard: result.ScoreCard,
                                pointsFrom: result.PointsFrom,
                                pointsTo: result.PointsTo,
                                name: result.Band,
                                interestRatePercentage: 0,
                                depositRequiredPercentage: 0,
                                cpiPercentage: 0,
                                adminPercentage: 0,
                                adminValue: 0,
                                startDate: new Date(),
                                endDate: null,
                                locked: true
                            }

                            $scope.termsType.customerBands.push(obj);
                        });
                    });
            });
    };

    $scope.remove = function (index, list) {
        list.splice(index, 1);
    };

    $scope.add = function (list, value, listModel) {
        if (!_.contains(list, value)) {
            list.push(value);

            $scope[listModel] = null;
        }
    };

    $scope.addProductHierarchy = function (value, level) {
        var obj = {
            tag: value,
            level: level
        };

        var values = _.pluck($scope.termsType.productHierarchyTags, 'Tag')
        if (!_.contains(values, value)) {
            $scope.termsType.productHierarchyTags.push(obj);
        }

        $scope.productHierarchyTag = null;
        $scope.productHierarchyLevel = null;

    };

    $scope.cancelCustomerBand = function () {
        $scope.newScoringBandMatrix = null;
        $scope.addNew = false;
    };

    $scope.deleteCustomerBand = function (index) {
        $scope.termsType.customerBands.splice(index, 1);
    };

    $scope.addCustomerBand = function () {
        $scope.newCustomerBand = newCustomerBand();
        $scope.addNew = true;
    };

    $scope.addToList = function (obj) {
        var newCustomerBand = {
            pointsFrom: obj.pointsFrom,
            pointsTo: obj.pointsTo,
            name: obj.band.bandName,
            interestRatePercentage: obj.interestRatePercentage,
            depositRequiredPercentage: obj.depositRequiredPercentage,
            cpiPercentage: obj.cpiPercentage,
            adminPercentage: obj.adminPercentage,
            adminValue: obj.adminValue,
            startDate: obj.startDate,
            endDate: null,
            locked: false,
            delete : true
        }
        $scope.termsType.customerBands.push(newCustomerBand);
        $scope.addNew = false;
    };

    $scope.addProductSKUs = function (value) {
        if (!_.contains($scope.termsType.productSkus, value)) {

            var skuDescription = _.pluck(_.filter(productSKUsList, function (item) {
                return item.skuId == value
            }), 'skuDescription')[0];

            var obj = {
                skuId: value,
                description: _.isUndefined(skuDescription) ? "" : skuDescription
            };

            $scope.termsType.productSkus.push(obj);

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

    $scope.$watch('productHierarchyLevel', function (productHierarchyLevel) {
        $scope.productHierarchyTagNames = _.pluck(_.filter($scope.productHierarchyTags, function (item) {
            return item.Level.Name == productHierarchyLevel;
        }), 'Name');
    });

    $scope.$watch('newCustomerBand.band', function (value) {
        if (!_.isUndefined(value)) {
            $scope.newCustomerBand.pointsFrom = _.map(_.filter($scope.bands, function (item) {
                return item.Band == value.bandName;
            }), 'PointsFrom')[0];

            $scope.newCustomerBand.pointsTo = _.map(_.filter($scope.bands, function (item) {
                return item.Band == value.bandName;
            }), 'PointsTo')[0];
        }
    });

    $scope.$watch('newCustomerBand.startDate', function () {
        var filteredBands = _.filter($scope.termsType.customerBands, function (band) {
            if (!_.isUndefined($scope.newCustomerBand.band)) {
                return band.name == $scope.newCustomerBand.band.bandName && $scope.newCustomerBand.startDate > band.startDate;
            }
        });

        var maxDate = null;
        if (filteredBands.length > 0) {
            maxDate = _.max(_.map(filteredBands, 'startDate'));
        }
        _.forEach($scope.termsType.customerBands, function (band) {
                if (!_.isUndefined($scope.newCustomerBand.band)) {
                    if (band.name == $scope.newCustomerBand.band.bandName && band.startDate == maxDate) {
                        band.endDate = moment($scope.newCustomerBand.startDate).add(-1, 'days')._d;
                    }
                }
            }
        );
    });

    $scope.getDate = function (band) {
        var bands = _.filter($scope.termsType.customerBands, function (customerBand) {
            if (!_.isUndefined(band.band)) {
                return band.band.bandName == customerBand.name;
            }
        });

        var maxDateForBand = _.max(_.map(bands, 'startDate'));
        return moment(maxDateForBand).add(1, 'days').format('YYYY-MM-DD');
    };

    function clean() {
        $scope.termsType = {};
        $scope.termsType.freeInstalments = [];
        $scope.termsType.productHierarchyTags = [];
        $scope.termsType.fascias = [];
        $scope.termsType.customerTags = [];
        $scope.termsType.productSkus = [];
    }

    function newCustomerBand() {
        return {
            pointsFrom: null,
            pointsTo: null,
            name: null,
            interestRatePercentage: null,
            depositRequiredPercentage: null,
            cpiPercentage: null,
            adminPercentage: null,
            adminValue: null,
            startDate: null,
            endDate: null,
            locked: false
        };
    }
};
termsTypeController.$inject = ['$scope', '$http', '$dialog', 'CommonService', '$routeParams'];
module.exports = termsTypeController;