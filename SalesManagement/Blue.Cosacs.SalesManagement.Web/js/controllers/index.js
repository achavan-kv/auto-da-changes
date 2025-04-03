'use strict';

angular.module('SalesManagement.controllers', [])
    .controller('callLogController', require('./callLogController'))
    .controller('followUpCallController', require('./followUpCalls'))
    .controller('branchManagerController', require('./branchManagerCall'))
    .controller('csrUnavailableController', require('./csrUnavailable'))
    .controller('quickDetailsCaptureController', require('./quickDetailsCapture'))
    .controller('customerSearchController', require('./customerSearch'))
    .controller('csrTargetsController', require('./csrTargets'))
    .controller('csrCustomerSearchController', require('./csrCustomerSearch'))
    .controller('callIconController', require('./callIcon'))
    .controller('csrDashboardController', require('./csrDashboard'))
    .controller('branchManagerDashboardController', require('./branchManagerDashboard'))
    .controller('branchManagerUnallocatedCallsController', require('./branchManagerUnallocatedCalls'))
    .controller('adHocCallController', require('./adHocCall'))
    .controller('inactiveCustomerController', require('./InactiveCustomerController'));
