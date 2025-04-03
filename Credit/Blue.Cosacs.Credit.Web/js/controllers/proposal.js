'use strict';
var common = require('./common')();

var proposalController = function ($scope, $http, settingsService, $window, $q, $routeParams, navService, $location) {
        $scope.newAddress = {};
        var proposalId = $routeParams.Id;
        $scope.pageState = $routeParams.State;
        var sectionManager = {};

        var title = $scope.pageState === 'edit' ? 'Edit Proposal' : 'View Proposal';
        if (proposalId === 'new') {
            title = 'New Proposal';
        }
        $scope.title = title;

        function newProposal() {
            var proposal = {};
            proposal.Applicant1 = {};
            proposal.Addresses = [];
            proposal.Applicant1.ApplicationType = 'Sole';
            return proposal;
        }

        function addAddress(ad) {
            var address = {
                "AddressType": ad.AddressType,
                "Line1": ad.Line1,
                "Line2": ad.Line2,
                "City": ad.City,
                "PostCode": ad.PostCode,
                "DeliveryArea": ad.DeliveryArea
            };
            $scope.proposal.Addresses.push(address);
        }

        $scope.addAddress = function () {
            _.remove($scope.proposal.Addresses, function (address) {
                return address.AddressType === $scope.newAddress.AddressType;
            });
            addAddress($scope.newAddress);
            $scope.newAddress = {};
        };


        $scope.removeAddress = function (i) {
            $scope.proposal.Addresses.splice(i, 1);
        };

        $scope.unLink = function () {
            $scope.proposal.Applicant1 = {};
            $scope.newAddress = {};
        };

        $scope.linkCustomer = function (customer) {
            common.assign(['FirstName', 'LastName', 'Title', 'Alias', 'Email', 'CustomerId'], customer, $scope.proposal.Applicant1);
            $scope.proposal.Applicant1.HomePhone = customer.HomePhoneNumber;
            $scope.proposal.Applicant1.MobilePhone = customer.MobilePhoneNumber;
            $scope.newAddress.AddressType = 'Home';
            $scope.newAddress.Line1 = customer.AddressLine1;
            $scope.newAddress.Line2 = customer.AddressLine2;
            $scope.newAddress.City = customer.TownOrCity;
            $scope.newAddress.PostCode = customer.PostCode;
            $scope.isUnchangedLinkedBasic = common.clone($scope.proposal.Applicant1);
            $scope.isUnchangedLinkedAddress = common.clone($scope.newAddress);
            $scope.moreResults = $scope.search = {};
        };

        var searchFields = ['FirstName', 'LastName', 'HomePhone', 'MobilePhone', 'Email'];
        $scope.isSearchField = function (field) {
            return _.contains(searchFields, field) && $scope.pageState === 'edit';
        };

        function customerSearch(fields) {
            var search = {
                FirstName: fields[0],
                LastName: fields[1],
                Email: fields[2],
                HomePhone: fields[3]
            };
            if (!$scope.proposal.Applicant1.CustomerId && _.find(_.values(search), function (value) {
                    return value;
                })) {
                common.customerSearch($http, $scope, search);
            }
            else {
                $scope.moreResults = {};
                $scope.search = {};
            }
        }

//Build Async call list
        var aCalls = [$http.get('/Credit/api/CustomizeFields/'), settingsService.credit()];
        if (proposalId != 'new') {
            aCalls.push($http.get('/Credit/api/BasicDetails/' + proposalId));
        }

        $q.all(aCalls)
            .then(function (result) {

                var scheme = common.getSettings(result[0].data[0], result[1].data);
                $scope.basicDetailsScheme = scheme.sections[0];
                $scope.addressScheme = scheme.sections[1];

                if ($scope.pageState == 'edit') {
                    $scope.$watchGroup(['proposal.Applicant1.FirstName', 'proposal.Applicant1.LastName',
                        'proposal.Applicant1.Email', 'proposal.Applicant1.HomePhone'], _.debounce(function (newValues) {
                        customerSearch(newValues);
                    }, 1000));
                }

                $scope.proposal = result[2] ? result[2].data.proposal : newProposal();
                sectionManager = common.sectionManager($scope.proposal.Stage || 0);

                $scope.$watchGroup(['proposal.Applicant1.FirstName', 'proposal.Applicant1.LastName'], function (newValues, oldValues, scope) {
                    navService.setApp1Name(newValues[0] && newValues[1] ? newValues[0] + ' ' + newValues[1] : null);
                });

                function setApplicant2(newValue) {
                    navService.setHasApplicant2(newValue != 'Sole');
                    navService.setApplicationType(newValue);
                    save.partial($scope.proposal.Applicant1.ApplicationType &&
                    $scope.proposal.Applicant1.ApplicationType != 'Sole' ? '/applicant2/' : '/sanctionStage1/applicant1/');
                }

                $scope.$watch('proposal.Applicant1.ApplicationType', function (newValue, oldvalue,scope) {
                    if (newValue && newValue !== oldvalue) {
                        sectionManager.changeApplicationType(newValue);
                        setApplicant2(newValue);
                    }
                });



                $scope.$watchGroup(['basicDetailsForm.$valid', 'proposal.Addresses.length'], function (newValues) {
                    var addressField = _.find($scope.addressScheme.fields, function (field) {
                        return field.id === 'AddressType'
                    });
                    var addressTypes = _.pluck($scope.proposal.Addresses, 'AddressType');
                    $scope.addressTypesRequired = _.difference(addressField.requiredValues, addressTypes);
                    var hasRequired = $scope.addressTypesRequired.length === 0;
                    sectionManager.changeSection('BasicDetailsApp1Details', $scope.basicDetailsForm.$valid);
                    sectionManager.changeSection('BasicDetailsApp1Address', $scope.proposal.Addresses.length > 0 && hasRequired);
                    navService.setSectionManager(sectionManager);
                });

                var proposalSend = function () {
                    $scope.proposal.Stage = sectionManager.getCompletedValue();
                    return $scope.proposal;
                };

                function customSave(callback, saveUrl, proposal) {
                    $http.post(saveUrl, proposal)
                        .success(function (result) {
                            callback(result.Message.Id);
                        });
                }

                var save = common.go(
                    {
                        saveUrl: '/Credit/api/BasicDetails',
                        partialRedirect: '/sanctionStage1/applicant1/',
                        proposalId: proposalId,
                        pageState: $scope.pageState,
                        customSave: proposalId === 'new' ? customSave : null,
                        proposal: proposalSend
                    }, $http, $window, $location, $scope);

                $scope.go = function () {
                    save.go();
                };

                navService.init(
                    {
                        screen: "BasicDetailsApp1",
                        applicant2Name: $scope.proposal.Applicant2Name,
                        stage1Sections: common.getSectionList,
                        sanction1AppSections: common.getSectionList(result[0].data),
                        save: save
                    });
                setApplicant2($scope.proposal.Applicant1.ApplicationType);
                if (proposalId === 'new') {
                    sectionManager.changeApplicationType('Sole');
                }
            });
    };

proposalController.$inject = ['$scope', '$http', 'settingsService', '$window', '$q', '$routeParams', 'navService', '$location'];
module.exports = proposalController;
