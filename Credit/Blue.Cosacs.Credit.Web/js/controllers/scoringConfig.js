'use strict';
var scoreConfigController = function ($scope, $http, settingsService, CommonService) {
    $scope.cardTypes = ['Applicant', 'Behavioural'];
    $scope.setup = {};
    $scope.setup.config = $http.get('/Credit/api/ScoreCardConfiguration');
    $scope.setup.settings = settingsService.credit();
    $scope.scoreCard = {};

    $scope.$watch('cardType', function (newValue) {
        if (newValue) {
            $scope.scoreCard = $scope.scoreCard || {};
            $scope.scoreCard[newValue] = $scope.scoreCard[newValue] || {};
            $scope.scoreCard[newValue].name = $scope.scoreCard[newValue].name || newValue;
            $scope.scoreCard[newValue].scoringRules = $scope.scoreCard[newValue].scoringRules || [];
            $scope.scoreCard[newValue].referRules = $scope.scoreCard[newValue].referRules || [];
            $scope.scoreCard[newValue].declineRules = $scope.scoreCard[newValue].declineRules || [];
        }
    });


    $http.get('/Credit/api/ScoreCard')
        .success(function (result) {
            _.forEach(result, function (card) {
                $scope.scoreCard[card.name] = card;
            });
        });

    $scope.addRule = function (rule) {
        rule.push(
            {
                result: null,
                rules: {class: 'And', expression: [{class: '', expression: null}]}
            });
    };

    $scope.deleteRule = function (array, index) {
        array.splice(index, 1);
    }

    $scope.save = function (cardName) {

        var card = $scope.scoreCard[cardName];
        card.name = cardName;

        $http.post('/Credit/api/ScoreCard', card)
            .success(function () {
                CommonService.addGrowl({
                    timeout: 1,
                    type: 'success', // (optional - info, danger, success, warning)
                    content: 'Successfully saved changes!'
                });
            });
    }

};
scoreConfigController.$inject = ['$scope', '$http', 'settingsService', 'CommonService'];
module.exports = scoreConfigController;