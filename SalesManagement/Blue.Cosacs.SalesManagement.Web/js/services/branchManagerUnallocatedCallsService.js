'use strict';

var branchManagerUnallocatedCallsService = function($http, UsersService) {

    return {
        getLockedCSRs: getLockedCSRs,
        getUnavailableCSRs: getUnavailableCSRs,
        search: search,
        allocateCallsToCSR: allocateCallsToCSR,
        userWithPermissionForCallLog: userWithPermissionForCallLog
    };

    function userWithPermissionForCallLog(){
        return $http.get('/cosacs/Admin/Users/LoadPickListUsers?permissions=2400&branch=' + UsersService.getCurrentUser().BranchNumber);
    }

    function allocateCallsToCSR(calls){
        return  $http.post('/SalesManagement/api/BranchManager/Post', calls);
    }

    function getLockedCSRs(){
        return $http({
            url: '/cosacs/Admin/Users/SearchInstant?q={"facetFields":{"HomeBranchName":{"Values":["'
                + UsersService.getCurrentUser().BranchName
                + '" ]},"Locked":{"Values":["True"]}},"dateFields":{},"customQuery":[]}&start=0&rows=99999',
            method: "GET"
        });
    }
    function search(criteria){
        return $http.put('/SalesManagement/api/BranchManagerUnallocatedCalls', criteria)
    }

    function getUnavailableCSRs(){
        return $http({
            url: '/SalesManagement/api/BranchManager/GetUnavailableCSR',
            method: "GET"
        });
    }
}

branchManagerUnallocatedCallsService.$inject = ['$http', 'UsersService'];
module.exports = branchManagerUnallocatedCallsService;