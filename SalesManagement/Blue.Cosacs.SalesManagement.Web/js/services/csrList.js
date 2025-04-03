'use strict';

var csrList = function($http, UsersService) {
    return function(){
        return $http.get('/Cosacs/Admin/Users/LoadPickListUsers?branch=' + UsersService.getCurrentUser().BranchNumber + '&permissions=2400');
    };
}

csrList.$inject = ['$http', 'UsersService'];
module.exports = csrList;