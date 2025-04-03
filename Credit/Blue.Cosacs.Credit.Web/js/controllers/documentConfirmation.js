'use strict';
var common = require('./common')();


var documentConfirmationController = function ($scope, $http, $q, $routeParams, CommonService, fileService) {
    var proposalId = $routeParams.Id;
    $scope.proposalId = proposalId;
    $scope.dc = {};
    $scope.dc.ProposalId = proposalId;


    function documentConfirmationSettings(customerType, schemas, settings) {
        var customerSchema = _.find(schemas, function (schema) {
            return schema.customerType === customerType;
        });

        _.map(customerSchema.fields, function (current) {
            current.values = settings[current.fieldID];
        });
        return customerSchema;
    }

    function populateFiles() {
        _.forEach($scope.scheme.fields, function (field) {
            field.files = [];
            _.forEach($scope.dc.files, function (file) {
                if (file.DocumentType === field.fieldID) {
                    field.files.push(file);
                }
            });
        });
    }

    $q.all([
        $http.get('/credit/api/settings'),
        $http.get('/Credit/api/DocumentConfirmation/' + proposalId)
    ])
        .then(function (result) {
            //Display data for new Customer
            $scope.LocalNationality = result[0].data.LocalNationality;
            $scope.summary = result[1].data.proposalSummary;
            $scope.dc = result[1].data.documentConfirmation || {};

            $scope.dc.files = result[1].data.files || [];

            $scope.scheme = documentConfirmationSettings(result[1].data.proposalSummary.CustomerType, result[1].data.fields, result[0].data);
            $scope.scheme.fields = removeFields($scope.scheme.fields);
            populateFiles();
        });


    $scope.saveFile = function (file) {
        $http.put('/Credit/api/DocumentConfirmation/' + proposalId, file)
            .success(function () {
                CommonService.addGrowl({
                    timeout: 5,
                    type: 'success', // (optional - info, danger, success, warning)
                    content: 'Successfully upload files'
                });
            });
    };

    $scope.deleteFile = function (deletedFile) {
        $q.all([
            fileService.delete(deletedFile.Guid),
            $http.delete('/Credit/api/DocumentConfirmation/' + deletedFile.Guid)
        ]).then(function () {
            CommonService.addGrowl({
                timeout: 1,
                type: 'success', // (optional - info, danger, success, warning)
                content: 'Successfully deleted files'
            });

            _.forEach($scope.scheme.fields, function (field) {
                for (var i = 0; i < field.files.length; i++) {
                    if (field.files[i].Guid === deletedFile.Guid) {
                        field.files.splice(i, 1);
                    }
                }
            });
        });
    };

    $scope.save = function () {
        $scope.dc.ProposalId = proposalId;
        $scope.dc.Completed = $scope.documentConfirmationForm.$valid;

        $http.post('/Credit/api/DocumentConfirmation', $scope.dc)
            .success(function () {
                CommonService.addGrowl({
                    timeout: 1,
                    type: 'success', // (optional - info, danger, success, warning)
                    content: 'Successfully saved !'
                });
            });
    };

    function removeFields(scheme) {
        var fieldsToRemove = [];

        if ($scope.summary.CurrentResidentialStatus !== 'Renting') {
            fieldsToRemove.push("LandlordDetails");
        }


        if ($scope.summary.ApplicationType !== 'Sole With Spouse') {
            fieldsToRemove.push("SpouseDetails");
        }

        if ($scope.summary.Occupation !== 'Government Employees') {
            fieldsToRemove.push("GovernmentEmployees");
        }

        if ($scope.summary.Nationality == $scope.LocalNationality) {
            fieldsToRemove.push("OverseasPerson");
        }

        _.remove(scheme, function (current) {
            return _.find(fieldsToRemove, function (field) {
                return current.fieldID === field;
            });
        });

        return scheme;
    }

    $scope.typePlaceholder = function (model) {
        return model + ' Type'
    };

    $scope.type = function (model) {
        return model + 'Type'
    };

    $scope.notes = function (model) {
        return model + 'Notes';
    };

    $scope.check = function (model) {
        return model + 'Check';
    };

    $scope.$on('uploadEvent', function (event, data) {
        $scope.dc.Files.push(data);
    });
};

documentConfirmationController.$inject = ['$scope', '$http', '$q', '$routeParams', 'CommonService', 'fileService'];
module.exports = documentConfirmationController;