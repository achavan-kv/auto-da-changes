'use strict';
var common = require('./common')();

var customerDetailsController = function($scope, $routeParams, $http, settingsService, $modal, LookupService) {
    /* Customer data retrieved from the backend. */
    $scope.customerDetails;
    $scope.customerAddresses;
    $scope.customerContacts;
    $scope.customerTags;

    /* Contact and Address forms are bound to these objects. */
    $scope.newContactDetails = {};
    $scope.newAddressDetails = {};
    $scope.newTag = {};

    /* Retrieved from the backend. Populates Contact Type, Delivery Area and Address Type dropdowns. */
    $scope.creditSettings;

    /* Retrieved from the backend. Constraints for the inputs on the Address form. */
    $scope.addressFormSettings;

    var customerId = $routeParams.Id;
    var getCustomerDetailsApi = '/credit/api/CustomerDetails/';
    var newContactDetailsApi = '/credit/api/CustomerContacts';
    var deleteContactDetailsApi = '/credit/api/CustomerContacts/';
    var newAddressDetailsApi = '/credit/api/CustomerAddress';
    var newTagsApi = '/credit/api/CustomerTag';

    function getCustomerDetails(id) {
        $http.get(getCustomerDetailsApi + id).success(function(resultData) {
            $scope.customerDetails = resultData.customerFullDetails.Customer;
            $scope.customerAddresses = resultData.customerFullDetails.CustomerAddresses;
            $scope.customerContacts = resultData.customerFullDetails.CustomerContacts;
            $scope.customerTags = resultData.customerFullDetails.CustomerTags;
            $scope.pageHeading = $scope.customerDetails.Title + " " + $scope.customerDetails.FirstName + " " + $scope.customerDetails.LastName + " - Customer Details";

            removeExistingTagsFromList();

            LookupService.k2v('BRANCH', $scope.customerDetails.Branch).then(function(data) {
                $scope.customerDetails.Branch = data[$scope.customerDetails.Branch];
            });
        });
    }

    /* Executed after we get the default list of tags from the settings API and the tags that the
     * user already has associated with his account.
     * Makes sure that the dropdown used to add tags to a customer doesn't contain tags already
     * associated with the customer.
     * */
    function removeExistingTagsFromList() {
        /* Create a hashtable of tags already associated with the customer for easy lookup. */
        var setOfTags = $scope.customerTags.reduce(function (setOfTags, currentItem) {
            setOfTags[currentItem.Tag] = 1;
            return setOfTags;
        }, {});

        /* Remove items from the dropdown based on the hashtable above. */
        $scope.creditSettings.CustomerTag = _.filter($scope.creditSettings.CustomerTag, function (item) {
            return !(item in setOfTags);
        });
    }

    settingsService.credit().success(function (resultData) {
        $scope.creditSettings = resultData;
        getCustomerDetails(customerId);
    });

    settingsService.fieldsSettings().success(function (resultData) {
        var proposalApplicant1Settings = _.result(_.find(resultData, function(item) {
            return item.screenId === "proposalApplicant1";
        }), "sections");

        var addressSettings = _.result(_.find(proposalApplicant1Settings, function(item) {
            return item.sectionName === "New Address";
        }), 'fields');

        /* Accumulate the collection into a single object so that we can access each setting by ID. */
        $scope.addressFormSettings = addressSettings.reduce(function(finalItem, currentItem) {
            finalItem[currentItem.id] = currentItem;
            return finalItem;
        }, { });
    });

    /* Bound to Contact Details form add button */
    $scope.addContactDetails = function(formIsValid) {
        if(!formIsValid) {
            return;
        }

        /* Add the customer ID to the object. The rest of the required properties
         * are filled in by ng-model in the UI. */
        $scope.newContactDetails.CustomerId = customerId;

        $http.post(newContactDetailsApi, $scope.newContactDetails).success(function(newId) {
            $scope.newContactDetails.Id = newId;
            $scope.customerContacts.push($scope.newContactDetails);

            //Otherwise the grid and the inputs would be bound to the same object; editing the input would update the grid
            $scope.newContactDetails = {};
        });
    };

    /* Bound to the trash icon in the contact details grid */
    $scope.removeContactDetails = function(contactId) {
        var contactDetailsToDelete = _.find($scope.customerContacts, function(item) {
            return item.Id === contactId;
        });

        $http( {
            method: "DELETE",
            url: deleteContactDetailsApi,
            data: contactDetailsToDelete,
            headers: {'Content-Type': 'application/json'}
        }).success(function() {
            _.remove($scope.customerContacts, function(item) {
               return item.Id === contactId;
            });
        });
    };

    /* Bound to Addresses form add button */
    $scope.addAddress = function(formIsValid) {
        if(!formIsValid) {
            return;
        }

        /* Add the customer ID to the object. The rest of the required properties
         * are filled in by ng-model in the UI. */
        $scope.newAddressDetails.CustomerId = customerId;

        $http.post(newAddressDetailsApi, $scope.newAddressDetails).success(function(newId) {
            $scope.newAddressDetails.Id = newId;
            $scope.customerAddresses.push($scope.newAddressDetails);

            //Otherwise the grid and the inputs would be bound to the same object; editing the input would update the grid
            $scope.newAddressDetails = {};
        });
    };

    $scope.addCustomerTag = function(formIsValid) {
        if(!formIsValid) {
            return;
        }

        /* Add the customer ID to the object. The rest of the required properties
         * are filled in by ng-model in the UI. */
        $scope.newTag.CustomerId = customerId;

        $http.post(newTagsApi, $scope.newTag).success(function(newId) {
            $scope.newTag.Id = newId;
            $scope.customerTags.push($scope.newTag);

            /* After we associate a tag with a customer, make sure we remove it from the dropdown so that
             * we can't add it again to the same customer.
             * */
            _.remove($scope.creditSettings.CustomerTag, function(item) {
               return item === $scope.newTag.Tag;
            });

            //Otherwise the grid and the inputs would be bound to the same object; editing the input would update the grid
            $scope.newTag = {};
        });
    };

    /* Cropper modal */
    $scope.show = function() {
        var modalInstance = $modal.open({
            templateUrl: '/Credit/views/croppingModal.html',
            controller: 'croppingModalController'
        });

        modalInstance.result.then(function(newPhotoIdentifier) {
            $scope.customerDetails.ProfilePhoto = newPhotoIdentifier;
        });
    };

    $scope.formatDate = function(date) {
        return moment(date).format("DD MMMM YYYY");
    };
};

customerDetailsController.$inject = ['$scope', '$routeParams', '$http', 'settingsService', '$modal', 'LookupService'];
module.exports = customerDetailsController;