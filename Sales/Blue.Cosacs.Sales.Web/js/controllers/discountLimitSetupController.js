'use strict';

var dependInjects = ['$scope','PosDataService','CommonService'],
    DiscountLimitData = require('../model/DiscountLimitData.js');

var discountLimitSetupController = function ($scope,PosDataService,CommonService) {

    $scope.newData = new DiscountLimitData();
    $scope.searchData = new DiscountLimitData();
    $scope.branchDetails;
    $scope.branchNameList = {};
    $scope.existingDiscountLimitList = [];
    $scope.showNewRow = false;
    $scope.storeType = {
        'C':'Courts',
        'N':'Non Courts'
    };

    $scope.isValidRate = isValidRate;

    //DB Operation Related
    $scope.insertData = insertData;
    $scope.deleteData = deleteData;
    $scope.updateData = updateData;
    $scope.searchExistingData = searchExistingData;

    //UI Related
    $scope.addNew = addNew;
    $scope.cancelNewRecord = cancelNewRecord;
    $scope.editFormVisible = editFormVisible;
    $scope.cancelEditForm = cancelEditForm;

    activate();

    function activate() {
        angular.element('body').removeClass('full-screen');
        getBranchDetails();

    }

    function getBranchDetails(){
        PosDataService.getBranchDetailsList().then(function (data) {
            if (typeof data === 'object') {
                $scope.branchDetails = data;
                _.each($scope.branchDetails, function (b) {
                    $scope.branchNameList[b.BranchNumber] = b.BranchNumber + ' ' + b.BranchName;
                });
                getExistingDiscountLimitData();
            }
        });
    }

    function getExistingDiscountLimitData() {
        PosDataService.getExistingDiscountLimitData($scope.searchData).then(function (data) {
            if (typeof data === 'object') {
                var result = [];
                _.each(data, function (d) {
                    var discLimit = new DiscountLimitData();
                    _.merge(discLimit, d);
                    var found = _.find($scope.branchDetails, function (branch) {
                        return branch.BranchNumber == discLimit.branchNumber;
                    });
                    discLimit.branchName = found ? found.BranchName : null;
                    result.push(discLimit);
                });
                $scope.existingDiscountLimitList = result;
            }
            else {
                CommonService.alert('Exception while fetching existing discount limit data...', 'Error');
            }
        });
    }

    function searchExistingData(){
        getExistingDiscountLimitData();
    }

    //Insert Data
    function insertData() {
        if (!isValidRate($scope.newData.limitPercentage)) {
            return CommonService.addGrowl({
                timeout: 3,
                type: 'danger',
                content: 'Limit Percentage must be a valid number (0 - 100) with maximum two decimal places.'
            });
        }
        PosDataService.insertDiscountLimit($scope.newData).then(function (data) {
            if (PosDataService.checkReturnedValue(data)) {
                var found = _.find($scope.branchDetails, function (branch) {
                    return branch.BranchNumber == $scope.newData.branchNumber;
                });
                $scope.newData.branchName = found ? found.BranchName : null;
                $scope.existingDiscountLimitList.splice(0, 0, $scope.newData);
                $scope.showNewRow = false;
                $scope.newData = new DiscountLimitData();
                //$scope.newRate = '';
                //$scope.newDateFrom = null;

                CommonService.addGrowl({
                    timeout: 3,
                    type: 'success',
                    content: "Record added successfully"
                });
            }
            else {
                CommonService.addGrowl({
                    timeout: 3,
                    type: 'danger',
                    content: data.customError ? data.customError : data.errors
                });
            }
        });
    }

    //Update Data
    function updateData(updateData){
        if (!isValidRate(updateData.limitPercentageChanged)) {
            return CommonService.addGrowl({
                timeout: 3,
                type: 'danger',
                content: 'Limit Percentage must be a valid number (0 - 100) with maximum two decimal places.'
            });
        }
        var tempPercentageValue = updateData.limitPercentage;
        updateData.limitPercentage =updateData.limitPercentageChanged;
        PosDataService.updateDiscountLimit(updateData).then(function (data) {
            if (PosDataService.checkReturnedValue(data)) {
                updateData.limitPercentageChanged = 0;

                CommonService.addGrowl({
                    timeout: 3,
                    type: 'success',
                    content: "Record updated successfully"
                });
                updateData.showEditFields = false;
            }
            else
            {
                updateData.limitPercentage =tempPercentageValue;
            }
        });
    }

    //Delete Data
    function deleteData(deleteData){
        var title = 'Delete Discount Limit Entry',
            msg = 'Are you sure you want to delete this Discount Limit Entry?';

        var deleteConfirmation = CommonService.$dialog.messageBox(title, msg, [
            {
                label: 'Delete',
                result: 'delete',
                cssClass: 'btn btn-primary'
            },
            {
                label: 'Cancel',
                result: 'cancel'
            }
        ]);
        deleteConfirmation.open().then(function (choice) {
            if (choice === 'delete') {

                PosDataService.deleteDiscountLimit(deleteData).then(function (data) {
                    if (data === "") {
                        $scope.existingDiscountLimitList = _.reject($scope.existingDiscountLimitList, function (discLimit) {
                            return (discLimit.branchNumber === deleteData.branchNumber && discLimit.storeType === deleteData.storeType);
                        });
                        CommonService.addGrowl({
                            timeout: 3,
                            type: 'success',
                            content: "Record deleted successfully"
                        });
                    }
                });
            }
        });
    }

    //Show Input Form
    function addNew(){
        $scope.showNewRow = true;
    }

    //Cancel New Record Changes
    function cancelNewRecord(){
        $scope.newData = new DiscountLimitData();
        $scope.showNewRow = false;
    }

    //Show Form to Edit
    function editFormVisible(data){
        data.limitPercentageChanged = data.limitPercentage;
        data.showEditFields=true;
    }

    //Cancel Edit Changes
    function cancelEditForm(data){
        data.limitPercentageChanged = 0;
        data.showEditFields=false;
    }


    function isValidRate(inputRate){
        return inputRate;
    }
};

discountLimitSetupController.$inject = dependInjects;

module.exports = discountLimitSetupController;