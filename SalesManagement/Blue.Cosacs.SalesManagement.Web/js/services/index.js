'use strict';
angular.module('SalesManagement.services', [])
    .factory('csrList', require('./csrList'))
    .factory('branchManagerUnallocatedCallsService', require('./branchManagerUnallocatedCallsService'));