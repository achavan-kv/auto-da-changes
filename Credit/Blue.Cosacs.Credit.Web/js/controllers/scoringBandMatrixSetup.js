var scoringBandMatrixController;
scoringBandMatrixController = function ($scope, $http, $dialog, CommonService) {

    $scope.scorecardList = ["Applicant", "Behavioural"];
    $scope.newScoringBandMatrix = newScoringBandMatrix();
    $scope.addNew = false;
    $scope.scoringBandMatrix = {};

    $http.get('/Credit/api/ScoringBandMatrix/')
        .then(function (result) {
            $scope.scoringBandMatrixList = result.data;
        });

    $scope.add = function () {
        $scope.newScoringBandMatrix = newScoringBandMatrix();
        $scope.addNew = true;
    };

    $scope.cancel = function () {
        $scope.newScoringBandMatrix = null;
        $scope.addNew = false;
    };

    $scope.save = function (item) {
        $http.post('/Credit/api/ScoringBandMatrix', item)
            .success(function () {
                
                $http.get('/Credit/api/ScoringBandMatrix/')
                    .then(function (result) {
                        $scope.scoringBandMatrixList = result.data;
                    });

                $scope.addNew = false;
                CommonService.addGrowl({
                    timeout: 1,
                    type: 'success', // (optional - info, danger, success, warning)
                    content: 'Successfully saved changes!'
                });
            });
    };

    $scope.delete = function (id) {
        confirmDelete()
            .then(function (result) {
                if (result == 'yes') {
                    $http.delete('/Credit/api/ScoringBandMatrix/' + id)
                        .success(function () {
                            _.remove($scope.scoringBandMatrixList, function(item) {
                                return item.Id == id;
                            });
                            CommonService.addGrowl({
                                timeout: 5,
                                type: 'success', // (optional - info, danger, success, warning)
                                content: 'Successfully deleted!'
                            });
                        });
                }
            });
    };

    function newScoringBandMatrix() {
        return {
            ScoreCard: null,
            PointsFrom: null,
            PointsTo: null,
            Band: null
        };
    }

    function confirmDelete() {
        return $dialog.prompt('Are you sure you want to delete the Scoring Band Matrix ?', 'Score Band Matrix');
    }

};
scoringBandMatrixController.$inject = ['$scope', '$http', '$dialog', 'CommonService'];
module.exports = scoringBandMatrixController;