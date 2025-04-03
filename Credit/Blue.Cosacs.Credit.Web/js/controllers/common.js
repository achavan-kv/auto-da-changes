'use strict';

var common = function () {

        function sectionManager(completedValue, schema, stage) {
            // Set up complete flags with binary bit shifts.
            var sections =
            {
                'BasicDetailsApp1Details': 1 << 0,
                'BasicDetailsApp1Address': 1 << 1,
                'BasicDetailsApp2': 1 << 2,
                'Sanction1Applicant1Personal': 1 << 3,

                'Sanction1Applicant1Residential': 1 << 4,
                'Sanction1Applicant1Employment': 1 << 5,
                'Sanction1Applicant1Financial': 1 << 6,
                'Sanction1Applicant2Personal': 1 << 7,

                'Sanction1Applicant2Residential': 1 << 8,
                'Sanction1Applicant2Employment': 1 << 9,
                'Sanction1Applicant2Financial': 1 << 10,
                'Scored': 1 << 11,

                'Sanction2Applicant1Previous': 1 << 12,
                'Sanction2Applicant1Landlord': 1 << 13,
                'Sanction2Applicant1Employer': 1 << 14,
                'Sanction2Applicant1References': 1 << 15,

                'Sanction2Applicant2References': 1 << 16,
                'DocumentConfirmation': 1 << 17,
                'UnderWriter' : 1 << 18,
                'Referred': 1 << 19,

                'Declined': 1 << 20,
                'Accepted': 1 << 21
            };

            var reDirections =
                [
                    ['BasicDetailsApp1Details', {name : 'Basic Details', route: '/'}],
                    ['BasicDetailsApp1Address', {name : 'Basic Details', route: '/'}],
                    ['BasicDetailsApp2', {name : 'Basic Details Applicant 2', route:  '/applicant2/'}],
                    ['Sanction1Applicant1Personal', {name : 'Sanction Stage 1', route: '/sanctionStage1/applicant1/'}],
                    ['Sanction1Applicant1Residential', {name : 'Sanction Stage 1', route: '/sanctionStage1/applicant1/'}],
                    ['Sanction1Applicant1Employment', {name : 'Sanction Stage 1', route: '/sanctionStage1/applicant1/'}],
                    ['Sanction1Applicant1Financial', {name : 'Sanction Stage 1', route: '/sanctionStage1/applicant1/'}],
                    ['Sanction1Applicant2Personal', {name : 'Sanction Stage 1 Applicant 2', route: '/sanctionStage1/applicant2/'}],
                    ['Sanction1Applicant2Residential', {name : 'Sanction Stage 1 Applicant 2', route: '/sanctionStage1/applicant2/'}],
                    ['Sanction1Applicant2Employment', {name : 'Sanction Stage 1 Applicant 2', route: '/sanctionStage1/applicant2/'}],
                    ['Sanction1Applicant2Financial', {name : 'Sanction Stage 1 Applicant 2', route: '/sanctionStage1/applicant2/'}],
                    ['Scored', {name : 'Score', route: '/score/'}],
                    ['Sanction2Applicant1Previous', {name : 'Sanction Stage 2', route: '/sanctionStage1/applicant1/'}],
                    ['Sanction2Applicant1Landlord', {name : 'Sanction Stage 2', route: '/sanctionStage1/applicant1/'}],
                    ['Sanction2Applicant1Employer', {name : 'Sanction Stage 2', route: '/sanctionStage1/applicant1/'}],
                    ['Sanction2Applicant1References', {name : 'Sanction Stage 2', route: '/sanctionStage2/applicant1/'}],
                    ['Sanction2Applicant2References', {name : 'Sanction Stage 2 Applicant2', route: '/sanctionStage2/applicant2/'}],
                    ['DocumentConfirmation', {name : 'Document Confirmation', route: '/documentConfirmation/'}],
                    ['UnderWriter', {name : 'UnderWriter Screen', route: '/UnderWriter/'}]
                ];

            var applicationTypeCompleteSections = {
                Sole: ['BasicDetailsApp2', 'Sanction1Applicant2Personal', 'Sanction1Applicant2Residential',
                    'Sanction1Applicant2Employment', 'Sanction1Applicant2Financial', 'Sanction2Applicant2References'],

                Joint: ['Sanction1Applicant2Personal', 'Sanction1Applicant2Residential']
            };

            var basicDetails = 7; // 1 - 3 bits.
            var preScore = 2047; // 1 - 11 bits before scored;
            var preDoc = 131071; // 1 - 17 bits set before document check;


            var completed = completedValue;

            if (schema && stage) {
                // These are sections that are missing when you change application types.
                // setting them all to completed so not to confuse complete check algorithm.
                var sectionMap = {
                    stage1: {
                        'Current Employment': function () {
                            changeSection('Sanction1Applicant1Employment', true)
                        },
                        'Financial': function () {
                            changeSection('Sanction1Applicant1Financial', true)
                        },
                        'Personal': function () {
                            changeSection('Sanction1Applicant1Personal', true)
                        },
                        'Residential': function () {
                            changeSection('Sanction1Applicant1Residential', true)
                        }
                    },
                    stage2: {
                        'Employer Details': function () {
                            changeSection('Sanction2Applicant1Employer', true)
                        },
                        'Landlord Details': function () {
                            changeSection('Sanction2Applicant1Landlord', true)
                        },
                        'Previous Address': function () {
                            changeSection('Sanction2Applicant1Previous', true)
                        }
                    }
                };

                // Set values of sections that are not present as completed.
                var schemaNames = _.pluck(schema.sections, 'sectionName');
                _.forEach(sectionMap[stage], function (val, key) {
                    if (!_.contains(schemaNames, key)) {
                        val();
                    }
                });
            }

            // Set completed for unneeded sections in application types.
            function changeApplicationType(applicationType) {
                //Reset in case changing after completing other sections.
                _.forEach(applicationTypeCompleteSections['Sole'], function (section) {
                    changeSection(section, false);
                });
                _.forEach(applicationTypeCompleteSections[applicationType], function (section) {
                    changeSection(section, true);
                });
            }

            function isSectionCompleted(section) {
                return (completed & sections[section]) !== 0;
            }

            // Check all flags lower to see if completed. If not present should be set to true already.
            function canScore() {
                return (completed & preScore) === preScore;
            }

            function canDC() {
                return (completed & preDoc) === preDoc;
            }

            function changeSection(section, value) {
                if (_.isUndefined(value)) {
                    return;
                }
                if (value) {
                    completed = completed | sections[section]; // Set flag.
                }
                else {
                    completed = completed & ~sections[section]; // Remove flag.
                }
            }

            function calCompletedValue() {
                return completed;
            }

            //Find first uncompleted section.
            function lastCompleted(stageValue) {
                for (var x = 0; x < reDirections.length; x++) {
                    if ((sections[reDirections[x][0]] & stageValue) === 0) {
                        return reDirections[x][1];
                    }
                }
            }


            return {
                "isSectionCompleted": isSectionCompleted,
                "changeSection": changeSection,
                "getCompletedValue": calCompletedValue,
                "changeApplicationType": changeApplicationType,
                "canScore": canScore,
                "canDC": canDC,
                lastCompleted: lastCompleted
            }
        }

        function settings(schema, settings) {

            function split(values) {
                if (values) {
                    return _.map(values.split(','), function (value) {
                        return value.trim();
                    });
                }
                return null;
            }

            _.forEach(schema.sections, function (a) {
                _.map(a.fields, function (current) {
                    current.values = split(current.values);
                    current.requiredValues = split(current.requiredValues);
                    if (current.lookup) {
                        current.values = settings[current.fieldLookup];
                        if (current.fieldLookup == 'Nationality') {
                            current.values.push(settings['LocalNationality']);
                        }
                    }
                });
            });
            return schema;
        }


        function copyObject(object) {
            var copy = {};

            for (var name in object) {
                if (object.hasOwnProperty(name)) {
                    copy[name] = object[name];
                }
            }

            function hasChanged(name, value) {
                if (!value) {
                    return false;
                }
                return copy[name] === value;
            }

            return hasChanged;
        }

        function getCustomerMatches($http, $scope, search) {
            $http({
                url: '/Credit/api/Customers',
                method: "GET",
                params: search
            }).success(function (result) {
                if (result.response) {
                    $scope.search = result.response.docs;
                    $scope.moreResults = result.response.numFound - 7;
                }
            });
        }

        function assign(names, source, dest) {
            _.forEach(names, function (name) {
                dest[name] = source[name];
            });
        }

        function go(saveParams, $http, $window, $location, $scope) {
            var saveUrl = saveParams.saveUrl;
            var partialRedirect = saveParams.partialRedirect;
            var proposalId = saveParams.proposalId;
            var pageState = saveParams.pageState;
            var getProposal = saveParams.proposal;

            var save = saveParams.customSave || baseSave;

            function changePartial(url) {
                partialRedirect = url;
            }

            function baseSave(callback, saveUrl, proposal, proposalId) {
                $http.put(saveUrl + '/' + proposalId, proposal)
                    .success(function () {
                        callback(proposalId);
                    });
            }

            function goHref(id, partial, state) {
                $window.location.href = '#/Credit/proposals/' + id + partial + state;
            }

            function redirect() {
                if (pageState != 'edit') {
                    goHref(proposalId, partialRedirect, pageState);
                }
                else {
                    save(function (id) {
                        if ($location) {
                            $location.path('#/Credit/proposals/' + id + '/' + pageState).replace();
                        }
                        goHref(id, partialRedirect, pageState)
                    }, saveUrl, getProposal(), proposalId);
                }
            }

            return {
                go: redirect,
                partial: changePartial
            };
        }

        function getSectionList(customFields) {
            var types = {
                'Sole': 'SanctionStage1Applicant1',
                'Joint': 'SanctionStage1JointApplicant',
                'Sole With Spouse': 'SanctionStage1SpouseApplicant'
            };
            var sectionList = {};
            _.forEach(types, function (value, key) {
                sectionList[key] = _.pluck(_.find(customFields, function (screen) {
                    return screen.screenId == value
                }).sections, 'sectionName')
            });
            return sectionList;
        }

        return {
            assign: assign,
            customerSearch: getCustomerMatches,
            getSettings: settings,
            stages: {
                'BasicDetails': 1,
                'BasicDetailsApplicant2': 2,
                'SanctionStage1': 3,
                'SanctionStage1Applicant2': 4,
                'SanctionStage2': 5,
                'DocumentConfirmation': 6,
                'Accepted': 7,
                'Rejected': 8
            },
            clone: copyObject,
            sectionManager: sectionManager,
            go: go,
            getSectionList: getSectionList
        };
    }
    ;
module.exports = common;