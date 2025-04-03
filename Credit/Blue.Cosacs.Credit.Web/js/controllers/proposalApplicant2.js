'use strict';
var common = require('./common')();
var proposalApplicant2Controller = function ($scope, $http, settingsService, $q, $routeParams, $window, navService) {
    $scope.proposal = {};
    var sectionManager = {};
    var isApp2New = false;

    $scope.title = $routeParams.State === 'edit' ? 'Edit Proposal - Applicant 2' : 'View Proposal - Applicant 2';

    var proposalId = $routeParams.Id;
    $scope.pageState = $routeParams.State;

    function customerSearch(fields) {
        var search = {
            FirstName: fields[0],
            LastName: fields[1],
            DateOfBirth: fields[2]
        };
        if (!$scope.proposal.Applicant2.CustomerId && _.find(_.values(search), function (value) {
                return value;
            })) {
            common.customerSearch($http, $scope, search);
        }
        else {
            $scope.search = {};
        }
    }

    $q.all([
        $http.get('/Credit/api/CustomizeFields/'),
        settingsService.credit(),
        $http.get('/Credit/api/Applicant2/' + proposalId)
    ])
        .then(function (result) {
            $scope.scheme = common.getSettings(result[0].data[1], result[1].data);
            $scope.proposal = result[2].data;
            isApp2New =  jQuery.isEmptyObject(result[2].data.Applicant2);
            sectionManager = common.sectionManager($scope.proposal.Stage || 0);


            if ($scope.proposal.Applicant2) {
                $scope.proposal.Applicant2.DateOfBirth = new Date($scope.proposal.Applicant2.DateOfBirth);
            }
            else {
                $scope.proposal.Applicant2 = {};
            }
            if ($scope.pageState == 'edit') {
                $scope.$watchGroup(['proposal.Applicant2.FirstName', 'proposal.Applicant2.LastName',
                    'proposal.Applicant2.DateOfBirth'], _.debounce(function (newValues) {
                    customerSearch(newValues);
                }, 1000));
            }

            $scope.$watchGroup(['proposal.Applicant2.FirstName', 'proposal.Applicant2.LastName'], function (newValues, oldValues, scope) {
                navService.setApp2Name(newValues[0] && newValues[1] ? newValues[0] + ' ' + newValues[1] : null);
            });

            $scope.$watch('Applicant2Form.$valid', function (newValue) {
                sectionManager.changeSection('BasicDetailsApp2', newValue);
                navService.setSectionManager(sectionManager);
            });

            function customSave(callback, saveUrl, proposal) {
                $http.post(saveUrl, proposal)
                    .success(function () {
                        callback(proposalId);
                    });
            }

            var proposalSend = function () {
                $scope.proposal.Stage = sectionManager.getCompletedValue();
                return $scope.proposal;
            };

            var save = common.go(
                {
                    saveUrl: '/Credit/api/Applicant2',
                    partialRedirect: '/sanctionStage1/applicant1/',
                    proposalId: proposalId,
                    pageState: $scope.pageState,
                    customSave: isApp2New ? customSave : null,
                    proposal : proposalSend
                }, $http, $window);

            $scope.go = function (button) {
                if (button == 'back') {
                    save.partial('/')
                }
                save.go();
            };

            navService.init(
                {
                    "screen": "BasicDetailsApp2",
                    "hasApplicant2" : true,
                    "applicant1Name" : $scope.proposal.Applicant1Name,
                    "applicationType" : $scope.proposal.ApplicationType,
                    sanction1AppSections : common.getSectionList(result[0].data),
                    save : save,
                    pageState: $routeParams.State
                }
            );
        });

    var searchFields = ['FirstName', 'LastName', 'DateOfBirth'];
    $scope.isSearchField = function (field) {
        return _.contains(searchFields, field) && $scope.pageState === 'edit';
    };

    $scope.unLink = function () {
        $scope.proposal.Applicant2 = {};
    };

    $scope.linkCustomer = function (customer) {
        common.assign(['FirstName', 'LastName', 'Title', 'Alias', 'CustomerId'], customer, $scope.proposal.Applicant2);
        $scope.proposal.Applicant2.DateOfBirth = new Date(customer.DOB);
        $scope.isUnchangedLinkedApp2 = common.clone($scope.proposal.Applicant2);
        $scope.moreResults = $scope.search = {};
    };
};
proposalApplicant2Controller.$inject = ['$scope', '$http', 'settingsService', '$q', '$routeParams', '$window','navService'];
module.exports = proposalApplicant2Controller;