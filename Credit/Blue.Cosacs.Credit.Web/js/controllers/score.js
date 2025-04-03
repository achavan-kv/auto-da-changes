'use strict';
var common = require('./common')();
var scoreController = function ($scope, $http, settingsService, $q, $window, $routeParams, navService) {
    var proposalId = $routeParams.Id;
    $scope.pageState = $routeParams.State;
    var back = '/sanctionStage1/applicant1/';

    var save = common.go(
        {
            saveUrl: '/Credit/api/Scoring',
            partialRedirect: '/sanctionStage2/applicant1/',
            proposalId: proposalId,
            pageState: $scope.pageState,
            proposal : function () {}
        }, $http, $window);

    navService.init(
        {
            "screen": "Scoring",
            "hasApplicant2" : false
        }
    );

    $http.get('/Credit/api/Scoring/' + proposalId)
        .success(function (result) {
            if (result.hasApplicant2) {
                back = '/sanctionStage1/applicant2/';
            }
        });

    $scope.go = function (button) {
        if (button == 'back') {
            save.partial(back)
        }
        save.go();
    };
};
scoreController.$inject = ['$scope', '$http', 'settingsService', '$q', '$window', '$routeParams', 'navService'];
module.exports = scoreController;