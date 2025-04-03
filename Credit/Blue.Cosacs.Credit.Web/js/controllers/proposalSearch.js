'use strict';
var common = require('./common')();

var proposalSearchController = function ($scope, $http, UsersService) {
    $scope.search = {};
    $scope.permissions = {};
    $scope.stages = ['Basic Details', 'Basic Details Applicant 2','Score', 'Sanction Stage 1', 'Sanction Stage 1 Applicant 2', 'Sanction Stage 2','Sanction Stage 2 Applicant 2', 'Document Confirmation', 'Accepted', 'Rejected'];
    $scope.source = ['Credit', 'Quotation', 'Online', 'Individual Third Parties'];

    $scope.fromNow = function (date) {
        if (!date) {
            return null;
        }
        return moment(date).fromNow();
    };

    $scope.permissions.editProposal = UsersService.hasPermission(2701);
    $scope.permissions.viewProposal = UsersService.hasPermission(2702);

    $scope.clear = function () {
        $scope.search = {};
        $scope.results = {};
    };

    $scope.getResults = function () {
        if ($scope.search.currentStage) {
            $scope.search.stage = $scope.search.currentStage.replace(/ /g, '');
        }

        $http.post('/Credit/api/ProposalsSearch', $scope.search)
            .success(function (data) {
                $scope.results = data.results;
            });
    };

    $scope.convertStage = common.sectionManager().lastCompleted;

    $scope.editLink = function (applicationStage, id) {
        return '#/Credit/proposals/' + id + $scope.convertStage(applicationStage).route + 'edit';
    };
};

proposalSearchController.$inject = ['$scope', '$http', 'UsersService'];
module.exports = proposalSearchController;