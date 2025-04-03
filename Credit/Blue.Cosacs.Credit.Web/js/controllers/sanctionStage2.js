'use strict';
var common = require('./common')();

var sanctionStage2Controller = function ($scope, $http, settingsService, $q, $window, $routeParams, navService) {
    $scope.proposal = {};
    var proposalId = $routeParams.Id;
    var isApplicant2 = $routeParams.Applicant === 'applicant2';
    $scope.pageState = $routeParams.State;
    var applicantType = $scope.applicantType = isApplicant2 ? 'Applicant2' : 'Applicant1';
    $scope.newReference = {};
    var noOfMonthsInCurrentAddress = 0;
    var sectionManager = {};
    $scope.isApplicant2 = isApplicant2;


    $scope.title = $scope.pageState === 'edit' ? 'Edit Proposal - Sanction Stage 2 - ' +  applicantType : 'View Proposal - Sanction Stage 2 - ' +  applicantType;

    $q.all([$http.get('/Credit/api/CustomizeFields/'),
        settingsService.credit(),
        $http({
            method: "GET",
            url: "/Credit/api/sanctionStage2/" + proposalId,
            params: {
                "isApplicant2": isApplicant2
            }
        })
    ]).then(function (result) {
        var template = result[0].data;
        var settingsData = result[1].data;
        var propData = result[2].data;

        noOfMonthsInCurrentAddress = settingsData.NoOfMonthsInCurrentAddress;
        $scope.proposal = propData;


        if (isApplicant2) {
            $scope.references = common.getSettings(template[7], settingsData);
        }
        else {
            $scope.scheme = common.getSettings(template[5], settingsData);
            $scope.references = common.getSettings(template[6], settingsData);
            $scope.proposal.References = $scope.proposal.References || [];
        }

        if (propData.Filled) {
            $scope.isUnchangedLinked = common.clone($scope.proposal[applicantType]);
            $scope.filled = true;
        }

        sectionManager = common.sectionManager($scope.proposal.Stage || 0, $scope.scheme, 'stage2');

        var formWatch = ['sanctionStage2Form.PreviousAddressForm.$valid', 'sanctionStage2Form.EmployerDetailsForm.$valid','proposal.References.length'];

        if ($scope.proposal.CurrentResidentialStatus == 'Renting') {
            formWatch.push('sanctionStage2Form.LandlordDetailsForm.$valid');
        }
        else {
            sectionManager.changeSection('Sanction2Applicant1Landlord', true);
        }

        $scope.$watchGroup(formWatch, function (newValues, oldValues, scope) {
            sectionManager.changeSection('Sanction2' + applicantType + 'Previous', newValues[0]);
            sectionManager.changeSection('Sanction2' + applicantType + 'Employer', newValues[1]);
            if ($scope.proposal.CurrentResidentialStatus == 'Renting') {
                sectionManager.changeSection('Sanction2' + applicantType + 'Landlord', newValues[3]);
            }
            sectionManager.changeSection('Sanction2' + applicantType + 'References', $scope.isReferenceValid());
            navService.setSectionManager(sectionManager);
        });

        $scope.nextEnabled = function() {
            return sectionManager.canDC() || !isApplicant2 && $scope.proposal.ApplicationType !== 'Sole';
        };

        var proposalSend = function () {
            $scope.proposal.Stage = sectionManager.getCompletedValue();
            return $scope.proposal;
        };

        $scope.canDC = sectionManager.canDC;

        var save = common.go(
            {
                saveUrl: '/Credit/api/sanctionStage2',
                partialRedirect: $scope.proposal.ApplicationType != 'Sole' && !isApplicant2 ? '/sanctionStage2/applicant2/' : '/documentConfirmation/',
                proposalId: $scope.proposal.Id,
                pageState: $scope.pageState,
                proposal: proposalSend
            }, $http, $window);

        $scope.go = function (button) {
            if (button == 'back') {
                save.partial(isApplicant2 ? '/sanctionStage2/applicant1/' : '/score/')
            }
            $scope.proposal.Stage = sectionManager.getCompletedValue();
            save.go();
        };

        navService.init(
            {
                screen: "Sanction2" + applicantType,
                hasApplicant2: $scope.proposal.ApplicationType != 'Sole',
                applicant1Name: $scope.proposal.Applicant1Name,
                applicant2Name: $scope.proposal.Applicant2Name,
                applicationType: $scope.proposal.ApplicationType,
                sanction1AppSections: common.getSectionList(template),
                save: save
            });

    });

    $scope.isPreviousAddressMandatory = function () {
        return $scope.proposal.MonthsInCurrentAddress <= noOfMonthsInCurrentAddress;
    };

    $scope.addNewReference = function () {
        $scope.newReference.ProposalId = proposalId;
        $scope.newReference.IsFamily = $scope.newReference.IsFamily ? $scope.newReference.IsFamily : false;
        $scope.proposal.References.push($scope.newReference);
        $scope.newReference = {};
    };

    // At least one family and non-family reference is required !
    $scope.isReferenceValid = function () {
        var isFamilyReference = _.some($scope.proposal.References, {'IsFamily': true});
        var isNonFamilyReference = _.some($scope.proposal.References, {'IsFamily': false});
        return isFamilyReference && isNonFamilyReference

    };

    $scope.deleteReference = function (index) {
        $scope.proposal.References.splice(index, 1);
    };

    $scope.setFormName = function (text) {
        return text.replace(' ', '') + 'Form';
    };


};
sanctionStage2Controller.$inject = ['$scope', '$http', 'settingsService', '$q', '$window', '$routeParams', 'navService'];
module.exports = sanctionStage2Controller;