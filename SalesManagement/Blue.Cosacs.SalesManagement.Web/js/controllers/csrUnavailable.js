'use strict';

var csrUnavailableController = function ($scope, $http, UsersService, CommonService, $q, csrList) {

    $scope.adding = false;
    $scope.newUnavailability = newEmptyUnavailability();
    $scope.salesPeople = null;
    var registredCsrs = null;
    $scope.filter = {
        to: null,
        from: null,
        salesPerson: null,
        numberOfRecords: 15
    };

    $scope.search = function () {
        search();
    }

    function search() {

        if ($scope.filter.to || $scope.filter.from || $scope.filter.salesPeople) {
            $scope.filter.numberOfRecords = null;
        }
        else {
            $scope.filter.numberOfRecords = 15;
        }

        $http.get('/SalesManagement/api/CsrUnavailable/', {
            params: {
                salesPerson: _.isNull($scope.filter.salesPerson) ? '' : $scope.filter.salesPerson,
                from: _.isNull($scope.filter.from) ? '' : $scope.filter.from,
                to: _.isNull($scope.filter.to) ? '' : $scope.filter.to,
                numberOfRecords: _.isNull($scope.filter.numberOfRecords) ? '' : $scope.filter.numberOfRecords
            }
        })
        .success(function (data) {
            $scope.results = data;
            _.map($scope.results, function (current) {
                current.BeggingUnavailable = new Date(current.BeggingUnavailable);
                current.EndUnavailable = new Date(current.EndUnavailable);

                return current;
            });

            cancelUnavailability();
        });
    };

    $scope.isValid = function (record) {
        return valid(record);
    };

    function valid(record) {
        if ((_.isNull(record) || _.isNull(record.EndUnavailable) || _.isNull(record.BeggingUnavailable) || _.isNull(record.SalesPersonId))) {
            return false;
        }
        return record.EndUnavailable >= record.BeggingUnavailable;
    }

    function cancelUnavailability() {
        $scope.newUnavailability = null;
        $scope.adding = false;
    }

    $scope.cancelNewUnavailability = function () {
        cancelUnavailability();
    };

    $scope.createNewUnavailability = function () {
        $scope.newUnavailability = newEmptyUnavailability();
        $scope.adding = true;
    };

    function formatData(record){
        return _.extend({}, record, function(a, b){
            if (_.isDate(b)) {
                return moment(b).format("YYYY-MM-DD");
            }
            return b;
        })
    }

    $scope.insertUnavailability = function (record) {
        if (!valid(record)) {
            return null;
        }

        $http.put('/SalesManagement/api/CsrUnavailable', formatData(record))
            .success(function () {
                search();
                CommonService.addGrowl({
                    timeout: 5,
                    type: 'success', 
                    content: 'Unavailable created'
                });
            }).
            error(function (data, status, headers, config) {
                //TODO:do something
            });
    };

    $scope.updateUnavailability = function (record) {
        if (!valid(record)) {
            return null;
        }

        $http.post('/SalesManagement/api/CsrUnavailable', formatData(record))
            .success(function () {
                search();
                CommonService.addGrowl({
                    timeout: 5,
                    type: 'success',
                    content: 'Unavailable saved'
                });
            }).
            error(function (data, status, headers, config) {
                //TODO:do something
            });
    };

    $scope.deleteUnavailability = function (id) {
        $http.delete('/SalesManagement/api/CsrUnavailable/' + id)
            .success(function () {
                search();
                CommonService.addGrowl({
                    timeout: 5,
                    type: 'success', 
                    content: 'Unavailable deleted'
                });
            }).
            error(function (data, status, headers, config) {
                //TODO:do something
            });
    };

    function fillSalesPeople() {
        var deferred = $q.defer();

        var pickListUsers = function(){
            var request = csrList();

            return request
                .then(function (data) {
                    var results = {};

                    _.forEach(data.data, function (current) {
                        results[current.k] = current.k + ' - ' + current.v;
                    });

                    $scope.salesPeople = results;
                });
        };

        var salesPeople = function(){
          var request = $http.get('/SalesManagement/api/CsrUnavailable/SalesPeople');

            return request
                .then(function(data){
                    registredCsrs = data.data;
                })
        };

        var promises = [pickListUsers(), salesPeople()];

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

        deferred.promise.then(function () {

            //get the users id's
            var keys = Object.keys($scope.salesPeople);

            _.each(keys, function(current){
                //is it in the registredCsrs array?
                if (_.where(registredCsrs, function(k){
                    return k === _.parseInt(current);
                }).length === 0){
                    //the user is not registered as a crs in our system
                    delete $scope.salesPeople[current];
                }
            });
        });
    }

    function newEmptyUnavailability() {
        return {
            Id: 0,
            Name: null,
            Beginning: null,
            End: null,
            SalesPersonId: null
        };
    }

    fillSalesPeople();
    search();
};

csrUnavailableController.$inject = ['$scope', '$http', 'UsersService', 'CommonService', '$q', 'csrList'];

module.exports = csrUnavailableController;