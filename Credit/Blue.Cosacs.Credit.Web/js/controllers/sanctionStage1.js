'use strict';
var common = require('./common')();

var sanctionStage1Controller;
sanctionStage1Controller = function ($scope, $http, settingsService, $q, $window, $routeParams, navService) {

    $scope.proposal = {};
    var proposalId = $routeParams.Id;
    var isApplicant2 = $routeParams.Applicant == 'applicant2';
    $scope.pageState = $routeParams.State;
    var applicantType = isApplicant2 ? 'Applicant2' : 'Applicant1';


    var sectionManager = {};

    $scope.title = $scope.pageState === 'edit' ? 'Edit Sanction Stage 1 ' : 'View Sanction Stage 1';




    function setDate(fields, source) {
        _.forEach(fields, (function (field) {
            source[field] = source[field] ? new Date(source[field]) : null;
        }));
    }

    var aCalls = [$http.get('/Credit/api/CustomizeFields/'),
        settingsService.credit(),
        $http({
            method: "GET",
            url: "/Credit/api/sanctionStage1/" + proposalId,
            params: {
                "isApplicant2": isApplicant2
            }
        })];
        aCalls.push(settingsService.payment());
    
    var templateMap = {
        Sole: "SanctionStage1Applicant1",
        Joint: "SanctionStage1JointApplicant",
        "Sole With Spouse": "SanctionStage1SpouseApplicant"
    };

    $q.all(aCalls).then(function (result) {
        //Applicant 1
        var template = result[0].data;
        var settingsData = result[1].data;
        var propData = result[2].data;

        $scope.proposal = propData;

        var payments = result[3].data;
        settingsData.PaymentMethod = _.map(payments.PaymentMethods, "Description");

        if (!isApplicant2) {

            //Sole
            $scope.scheme = common.getSettings(template[2], settingsData);
        }
        else {
            var app2 = _.filter(template, function (current) {
                return current.screenId === templateMap[propData.ApplicationType];
            });
            $scope.scheme = common.getSettings(app2[0], settingsData);
        }

        setDate(['DateOfBirth', 'CurrentEmploymentDate', 'DateBankAccountOpened', 'DateInCurrentAddress', 'CurrentEmploymentDate', 'DateBankAccountOpened'], $scope.proposal.Applicant);
        if (propData.Filled) {
            $scope.isUnchangedLinked = common.clone($scope.proposal.Applicant);
            $scope.filled = true;
        }

        $scope.$watchGroup(['proposal.Applicant.PayFrequency', 'proposal.Applicant.PayAmount'], function () {
            calculateMonthlyIncome();
        });

        sectionManager = common.sectionManager($scope.proposal.Stage || 0, $scope.scheme, 'stage1');

        $scope.$watchGroup(['Form.PersonalForm.$valid', 'Form.ResidentialForm.$valid', 'Form.CurrentEmploymentForm.$valid', 'Form.FinancialForm.$valid'], function (newValues, oldValues, scope) {
            sectionManager.changeSection('Sanction1' + applicantType + 'Personal', newValues[0]);
            sectionManager.changeSection('Sanction1' + applicantType + 'Residential', newValues[1]);
            sectionManager.changeSection('Sanction1' + applicantType + 'Employment', newValues[2]);
            sectionManager.changeSection('Sanction1' + applicantType + 'Financial', newValues[3]);
            navService.setSectionManager(sectionManager);
        });

        $scope.nextEnabled = function() {
            return sectionManager.canScore() || !isApplicant2 && $scope.proposal.ApplicationType !== 'Sole';
        };

        var proposalSend = function () {
            $scope.proposal.Stage = sectionManager.getCompletedValue();
            return $scope.proposal;
        };

        var save = common.go(
            {
                saveUrl: '/Credit/api/sanctionStage1',
                partialRedirect: $scope.proposal.ApplicationType != 'Sole' && !isApplicant2 ? '/sanctionStage1/applicant2/' : '/score/',
                proposalId: $scope.proposal.Id,
                pageState: $scope.pageState,
                proposal: proposalSend
            }, $http, $window);

        $scope.go = function (button) {
            if (button == 'back') {
                if (isApplicant2) {
                    save.partial('/sanctionStage1/applicant1/')
                }
                save.partial($scope.proposal.ApplicationType == 'Sole' ? '/' : '/applicant2/')
            }
            save.go();
        };

        navService.init(
            {
                screen: "Sanction1" + applicantType,
                hasApplicant2: $scope.proposal.ApplicationType != 'Sole',
                applicant1Name: $scope.proposal.Applicant1Name,
                applicant2Name: $scope.proposal.Applicant2Name,
                applicationType: $scope.proposal.ApplicationType,
                sanction1AppSections: common.getSectionList(template),
                save: save
            });

    });

    function calculateMonthlyIncome() {
        var monthlyIncome = {
            Weekly: 52 / 12,
            Fortnightly: 26 / 12,
            Annual: 1 / 12,
            Monthly: 1
        };
        var frequency = $scope.proposal.Applicant.PayFrequency ? $scope.proposal.Applicant.PayFrequency.trim() : null;
        var amount = $scope.proposal.Applicant.PayAmount;
        if (frequency && amount) {
            $scope.proposal.Applicant.MonthlyIncome = ( amount * monthlyIncome[frequency]).toFixed(0);
        }
    }

    $scope.showFieldsForDirectDebitPaymentMethod = function (field) {
        return (field.id == "AccountNumber" || field.id == "AccountBranch")
            && $scope.proposal.Applicant.PaymentMethod == 'Direct Debit';
    };

    $scope.setFormName = function (text) {
        return text.replace(' ', '') + 'Form';
    };

    $scope.addEmployment = function () {
        $scope.proposal.EmploymentHistory.push(
            {
                occupation: $scope.proposal.Applicant.OccupationHistory,
                dateStart: $scope.proposal.Applicant.StartDateOccupationHistory,
                dateEnd: $scope.proposal.Applicant.EndDateOccupationHistory,
                isApplicant2: isApplicant2,
                employerName : $scope.proposal.Applicant.EmployerName
            });
        $scope.proposal.Applicant.OccupationHistory = null;
        $scope.proposal.Applicant.StartDateOccupationHistory = null;
        $scope.proposal.Applicant.EndDateOccupationHistory = null;
        $scope.proposal.Applicant.EmployerName = null;
    };

    $scope.removeEmployment = function (index) {
        $scope.proposal.EmploymentHistory.splice(index, 1);
    };

    $scope.formatDate = function (date) {
        return moment(date).format('D MMM YYYY');
    };

    $scope.isEmploymentFilled = function () {
        return $scope.proposal.Applicant.OccupationHistory &&
            $scope.proposal.Applicant.StartDateOccupationHistory &&
            $scope.proposal.Applicant.EndDateOccupationHistory;
    };
};
sanctionStage1Controller.$inject = ['$scope', '$http', 'settingsService', '$q', '$window', '$routeParams', 'navService'];
module.exports = sanctionStage1Controller;