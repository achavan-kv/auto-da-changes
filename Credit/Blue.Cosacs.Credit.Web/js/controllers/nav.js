'use strict';
var navController = function ($scope, navService) {

    $scope.screen = function (screen) {
        return screen === navService.screen();
    };
    $scope.applicant1Name = navService.getApp1Name;
    $scope.applicant2Name = navService.getApp2Name;
    $scope.hasApplicant2 = navService.hasApplicant2;
    $scope.completed = navService.isSectionCompleted;
    $scope.canScore = navService.canScore;
    $scope.canDC = navService.canDC;

    $scope.showSanction1Section = function (name) {
        return _.some(navService.getSanction1App2Sections(), function (section) {
            return section == name;
        });
    };

    var partialLocations = {
        'BasicDetailsApp1': '/',
        'BasicDetailsApp2': '/applicant2/',
        'Sanction1Applicant1': '/sanctionStage1/applicant1/',
        'Sanction1Applicant2': '/sanctionStage1/applicant2/',
        'Sanction2Applicant1': '/sanctionStage2/applicant1/',
        'Sanction2Applicant2': '/sanctionStage2/applicant2/',
        'Score' : '/score/',
        'DocumentConfirmation' : '/documentConfirmation/'
    };

    var scoreNotCompletedDisable = ['Score','DocumentConfirmation','Sanction2Applicant1','Sanction2Applicant2'];

    $scope.go = function (location) {
        if (!navService.canScore() && _.contains(scoreNotCompletedDisable,location)) {
            return;
        }
        if (!navService.canDC() && location === 'DocumentConfirmation') {
            return;
        }
        navService.save().partial(partialLocations[location]);
        navService.save().go();
    };
};
navController.$inject = ['$scope', 'navService'];
module.exports = navController;