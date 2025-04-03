var cintError = function ($scope, $http) {

        function reset () {
            $scope.results =  { bulk : [], validation : []};
            $scope.query = {};
            $scope.query.bulk = true;
        }


        $scope.search = function () {
           $http.get('CintError/Search', $scope.query)
               .success(function(result) {
                    if ($scope.query.bulk) {
                        $scope.results.bulk = result.data;
                    }
                    else
                    {
                        $scope.results.validation = result.data;
                    }
               });
        };

        $scope.reset = reset;
        reset();


        $scope.dateFormat = pageHelper.dateFormat;
        $scope.query = {};
    };
	
cintError.$inject = ['$scope', '$http', ];
module.exports = cintError;
