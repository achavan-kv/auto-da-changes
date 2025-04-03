'use strict';
var common = require('./common')();

var customizeMandatoryFieldsController = function ($scope, $http, CommonService, settingsService, $q) {

    $q.all([$http.get('/Credit/api/CustomizeFields/'),
        settingsService.credit()])
        .then(function (result) {
            $scope.scheme = result[0].data;
            $scope.addresses = result[1].data['AddressType'];

            var basicDetailsSections = _.result(_.find($scope.scheme, function (screen) {
                return screen.screenId === 'proposalApplicant1';
            }), 'sections');

            var NewAddressFields = _.result(_.find(basicDetailsSections, function (sections) {
                return sections.sectionName === 'New Address';
            }), 'fields');

            var addressObj = _.find(NewAddressFields, function (sections) {
                return sections.id === 'AddressType';
            });


            var addressType = AddressType(addressObj);
            $scope.addAddress = addressType.add;
            $scope.removeAddress = addressType.remove;
            $scope.address = addressType.list;

        });

    $scope.save = function (field, screenId) {
        field.screenId = screenId;

        $http.post('/Credit/api/CustomizeFields', field)
            .success(function () {
                CommonService.addGrowl({
                    timeout: 1,
                    type: 'success', // (optional - info, danger, success, warning)
                    content: 'Successfully saved changes!'
                });
            });
    };

    function AddressType(addressTypeField) {
        var field = addressTypeField;

        field.values = field.requiredValues && field.requiredValues.trim().length > 0 ? field.requiredValues.split(',') : [];


        function populateRequired(field) {
            field.requiredValues = field.values.join(',');
            return field;
        }

        return {
            remove: function (index) {
                field.values.splice(index, 1);
                $scope.save(populateRequired(field), 'proposalApplicant1');

            },
            add: function (addressType) {
                if (addressType && !_.some(field.values, function (address) {
                        return address === addressType
                    })) {
                    field.values.push(addressType);
                    $scope.save(populateRequired(field), 'proposalApplicant1');
                }
            },
            list: field.values
        }
    }


};

customizeMandatoryFieldsController.$inject = ['$scope', '$http', 'CommonService', 'settingsService', '$q'];
module.exports = customizeMandatoryFieldsController;