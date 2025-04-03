'use strict';

var customerSearchController = function ($scope, xhr, CommonService, UsersService, $routeParams, $location, csrList) {
    $scope.action = '';
    $scope.allocateFrom = null;
    $scope.allocateTo = null;
    $scope.csrId = '';
    $scope.title = '';

    $scope.results = {
        response: {
            docs: []
        }
    };

    function searchParameters(){
        var y = $location.search().q;

        return y;
    };

    $scope.callLog = {
        toCallAt: null,
        toCallAtHour: new Date(0),
        scheduleDisabled: false,
        reasonForCalling: null,
        clear: function(){
            $scope.callLog.toCallAt = null;
            $scope.callLog.toCallAtHour = new Date(0);
            $scope.callLog.scheduleDisabled = false;
            $scope.callLog.reasonForCalling = null;
        },
        canLog: function() {
            return countSelected() > 0 && $scope.callLog.toCallAt && $scope.callLog.toCallAtHour && $scope.callLog.reasonForCalling;
        },
        canCallAll: function(){
            return $scope.callLog.toCallAt && $scope.callLog.toCallAtHour && $scope.callLog.reasonForCalling;
        },
        logCall: function () {
            if (!$scope.callLog.canLog() && !$scope.scheduleDisabled) {
                return;
            }
            var customers = [];
            var time = moment($scope.callLog.toCallAtHour);

            _.each($scope.results.response.docs, function (current) {
                if (current.selected) {
                    customers.push({
                        CustomerId: current.CustomerId,
                        SalesPersonId: current.SalesPersonId,
                        CustomerFirstName: current.FirstName,
                        CustomerLastName: current.LastName,
                        MobileNumber: current.MobileNumber,
                        LandLinePhone: current.HomePhoneNumber
                    })
                }
            });

            var objectToSave = {
                Customers: customers,
                ReasonForCalling: $scope.callLog.reasonForCalling,
                ToCallAt: moment($scope.callLog.toCallAt).set('hour', time.get('hour')).set('minute', time.get('minute'))._d
            };
            $scope.scheduleDisabled = true;
            xhr.put('/SalesManagement/api/ScheduleCallsBulk/BranchManagerBulk', objectToSave)
                .then(function (data) {
                    $scope.callLog.clear();
                    CommonService.addGrowl({
                        timeout: 5,
                        type: 'success',
                        content: 'Calls created'
                    });
                    $scope.scheduleDisabled = false;
                    unSelectAll();
                });
        },
        callAll: function(){

            if (!$scope.callLog.canCallAll() && !$scope.scheduleDisabled){
                return;
            }

            var time = moment($scope.callLog.toCallAtHour);
            var objectToSave = {
                CustomerFilter: searchParameters(),
                ReasonForCalling: $scope.callLog.reasonForCalling,
                ToCallAt: moment($scope.callLog.toCallAt).set('hour', time.get('hour')).set('minute', time.get('minute'))._d
            };

            xhr.put('/SalesManagement/api/ScheduleCallsBulk/BranchManagerCallsAll', objectToSave)
                .then(function (data) {
                    $scope.callLog.clear();
                    CommonService.addSuccessGrowl('Calls created');
                    $scope.scheduleDisabled = false;
                    unSelectAll();
                });
        }
    };

    $scope.mailSettings = {
        mailchimpTemplateID: null,
        body: null,
        toMailAt: moment()._d,
        templates: null,
        mailsConfirm: false,
        customersSelected: countSelected,
        unsubscriptions: 0,
        allTemplates: null,
        canSendMails: canSendMails,
        canSendMailsAll: canSendMailsAll,
        nextMailStep: {
            next: function (sendToAll){
                if (sendToAll || (!sendToAll && $scope.mailSettings.canSendMails())){
                    $scope.mailSettings.mailsConfirm = true;
                    this.sendAll = sendToAll;
                }
            },
            sendAll: null
        },
        scheduleEmails: scheduleEmails,
        subject: null,
        clearEmailSettings: function(){
            $scope.mailSettings.mailchimpTemplateID = null;
            $scope.mailSettings.body = null;
            $scope.mailSettings.toMailAt = moment()._d;
            $scope.mailSettings.mailsConfirm = false;
            $scope.mailSettings.unsubscriptions = 0;
            $scope.mailSettings.subject = null;
            $scope.mailSettings.nextMailStep.sendAll = null;
            unSelectAll();
        }
    };

    $scope.smsSettings = {
        body: null,
        sendSmsAt: null,
        scheduleSms: scheduleSms,
        scheduleSmsAll: scheduleSmsAll,
        canSendSms: canSendSms,
        canSendSmsAll: canSendSmsAll,
        clearSmsSettings: function(){
            $scope.smsSettings.body = null;
            $scope.smsSettings.sendSmsAt = null;
            unSelectAll();
      },
      customersSelected: countSelected,
      unsubscriptions: 0
    };

    xhr.get('/Communication/api/MailchimpTemplate')
        .then(function(resp){
            var results = {};
            $scope.mailSettings.allTemplates = {};

            _.forEach(resp.data, function (current) {
                if (current.CanSetBody) {
                    results[current.Id] = current.Name + '(body template)';
                }
                else{
                    results[current.Id] = current.Name;
                }

                $scope.mailSettings.allTemplates[current.Id] = current.CanSetBody;
            });

            $scope.mailSettings.templates = results;
        });


    $scope.action = $routeParams.action;

    if ($scope.action === "ReallocateCustomers") {
        $scope.title = "Reallocate Customers ";
    }
    else {
        if ($scope.action === "ScheduleCalls") {
            $scope.title = "Branch Customers";
        }
    }

    if ($scope.action === "ReallocateCustomers") {
        csrList()
            .then(function (resp) {
                var results = {};

                _.forEach(resp.data, function (current) {
                    results[current.k] = current.k + ' - ' + current.v;
                });

                $scope.csrList = results;
            });
    }

    function unSelectAll() {
        _.each($scope.results.response.docs, function (current) {
            current.selected = false;
        });
    }

    $scope.itemSelect_click = function(){
        if (canSendMails() == 0) {
            $scope.mailSettings.mailsConfirm = false;
        }
    }

    function countSelected(){
        var counter = 0;
        var unsubscriptions = 0;
        var unsubscriptionsSms = 0;

        if ($scope.results && $scope.results.response && $scope.results.response.docs) {
            _.each($scope.results.response.docs, function (current) {

                if (current.selected){
                    counter += 1;
                    unsubscriptions += current.ReceiveEmails === 'No' ? 1 : 0;
                    unsubscriptionsSms += current.ReceiveSms === 'No' ? 1 : 0;
                }

                return counter;
            });
        }

        $scope.mailSettings.unsubscriptions = unsubscriptions;
        $scope.smsSettings.unsubscriptions = unsubscriptionsSms;

        return counter;
    }

    function formatDateToServer(date){
        return moment(date).format('YYYY-MM-DD') + 'T00:00:00.000Z';
    }

    function scheduleEmails(){
        var done = function(){
            CommonService.addSuccessGrowl('E-mails were successfully scheduled');
        };

        if ($scope.mailSettings.nextMailStep.sendAll){

            if (!$scope.mailSettings.canSendMailsAll()){
                return;
            }

            $scope.mailSettings.clearEmailSettings();

            scheduleEmailsAll()
                .then(done);
        }
        else{
            var objectToSend = {
                ToMailAt: formatDateToServer($scope.mailSettings.toMailAt),
                Body: $scope.mailSettings.body,
                Subject: $scope.mailSettings.subject,
                MailchimpTemplateID : $scope.mailSettings.mailchimpTemplateID,
                Customers: _.chain($scope.results.response.docs)
                    .filter(function(current){
                        return current.selected && current.ReceiveEmails === 'Yes' && !S(current.Email).isEmpty();
                    })
                    .map(function(value){
                        return {
                            Id: value.CustomerId,
                            Address: value.Email,
                            Name: value.FirstName + ' ' + value.LastName
                        };
                    })
                    .value()
            };

            $scope.mailSettings.clearEmailSettings();

            xhr.post('/SalesManagement/api/BulkMails/Selected', objectToSend)
                .then(done);
        }
    }

    function scheduleEmailsAll(){

        var objectToSend = {
            ToMailAt: formatDateToServer($scope.mailSettings.toMailAt),
            Body: $scope.mailSettings.body,
            Subject: $scope.mailSettings.subject,
            MailchimpTemplateID : $scope.mailSettings.mailchimpTemplateID,
            CustomerFilter: searchParameters()
        };

        return xhr.post('/SalesManagement/api/BulkMails/All', objectToSend);
            // .then(function(){
            //     $scope.mailSettings.clearEmailSettings();
            //     CommonService.addSuccessGrowl('E-mails were successfully scheduled');
            //     $scope.mailSettings.clearEmailSettings();
            // });
    }

    function canSendMails(){

        var hasBody = false;
        var result = true;

        if (!_.isNull($scope.mailSettings.allTemplates) && !_.isNull($scope.mailSettings.mailchimpTemplateID)){
            hasBody = $scope.mailSettings.allTemplates[$scope.mailSettings.mailchimpTemplateID];
        }

        if (hasBody &&  !$scope.mailSettings.body){
            result = false;
        }

        return ($scope.mailSettings.customersSelected() - $scope.mailSettings.unsubscriptions) > 0
            && result
            && !S($scope.mailSettings.subject).isEmpty()
            && !_.isUndefined($scope.mailSettings.toMailAt)
            && $scope.mailSettings.mailchimpTemplateID;
    };

    function canSendMailsAll(){

        var hasBody = false;
        var result = true;

        if (!_.isNull($scope.mailSettings.allTemplates) && !_.isNull($scope.mailSettings.mailchimpTemplateID)){
            hasBody = $scope.mailSettings.allTemplates[$scope.mailSettings.mailchimpTemplateID];
        }

        if (hasBody &&  !$scope.mailSettings.body){
            result = false;
        }

        return result
            && !S($scope.mailSettings.subject).isEmpty()
            && !_.isUndefined($scope.mailSettings.toMailAt)
            && $scope.mailSettings.mailchimpTemplateID;
    }

    function scheduleSms(){

        if(!canSendSms()){
            return;
        }

        var objectToSend = {
            SmsText: $scope.smsSettings.body.trim(),
            ToSendAt: formatDateToServer($scope.smsSettings.sendSmsAt),
            Customers: _.chain($scope.results.response.docs)
                .filter(function (current) {
                    return current.selected && current.ReceiveSms === 'Yes' && !S(current.MobileNumber).isEmpty();
                })
                .map(function (value) {
                    return {
                        CustomerId: value.CustomerId,
                        PhoneNumber: value.MobileNumber
                    };
                })
                .value()
        };

        xhr.post('/SalesManagement/api/BulkSms/Selected', objectToSend)
            .then(function(){
                $scope.mailSettings.clearEmailSettings();
                CommonService.addSuccessGrowl('Sms were successfully scheduled');
                $scope.smsSettings.clearSmsSettings();
            });
    }

    function scheduleSmsAll(){

        if(!canSendSmsAll()){
            return;
        }

        var objectToSend = {
            SmsText: $scope.smsSettings.body.trim(),
            ToSendAt: formatDateToServer($scope.smsSettings.sendSmsAt),
            CustomerFilter: searchParameters()
        };

        xhr.post('/SalesManagement/api/BulkSms/All', objectToSend)
            .then(function(){
                $scope.mailSettings.clearEmailSettings();
                CommonService.addSuccessGrowl('Sms were successfully scheduled');
                $scope.smsSettings.clearSmsSettings();
            });
    }

    function canSendSms(){

        return ($scope.smsSettings.customersSelected() - $scope.smsSettings.unsubscriptions) > 0
            && !_.isNull($scope.smsSettings.body)
            &&  $scope.smsSettings.body.trim().length > 0
            && !_.isNull($scope.smsSettings.sendSmsAt)
    }

    function canSendSmsAll(){

        return !_.isNull($scope.smsSettings.body)
            &&  $scope.smsSettings.body.trim().length > 0
            && !_.isNull($scope.smsSettings.sendSmsAt)
    }

    $scope.$on('facetsearch:action:clear', function () {
        $scope.callLog.clear();
        $scope.mailSettings.clearEmailSettings();
    });

    $scope.$watch(function (scope) {
        return scope.results.response.docs;
    }, function (newValue, oldValue) {

        _.each(newValue, function (current) {

            var index = _.findIndex(oldValue, function (c) {
                return c.CustomerId === current.CustomerId;
            });

            if (index !== -1) {
                current["selected"] = oldValue[index]["selected"];
            }
            else {
                current["selected"] = false
            }

            return current;
        });
    });

    $scope.allocateCustomersToCSR = function () {

        var customers = [];
        _.each($scope.results.response.docs, function (current) {
            if (current.selected) {
                customers.push({
                    CustomerId: current.CustomerId,
                    SalesPersonId: current.SalesPersonId,
                    Phone: current.MobileNumber ? current.MobileNumber : current.HomePhoneNumber,
                    AllocateFrom: $scope.allocateFrom,
                    AllocateTo: $scope.allocateTo,
                    CSRId: $scope.csrId
                })
            }
        });

        var objectToSave = {
            Customers: customers
        };
        xhr.put('/SalesManagement/api/ReallocateCustomer/AllocateCustomersToCSR', objectToSave)
            .then(function (data) {
                $scope.allocateFrom = null,
                $scope.allocateTo = null;
                $scope.csrId = '';

                var customerIds = _.pluck(customers, 'CustomerId');

                xhr.post('/Customer/Api/Reindex', customerIds);

                CommonService.addGrowl({
                    timeout: 5,
                    type: 'success', // (optional - info, danger, success, warning)
                    content: 'Successfully reallocated the CSR !'
                });

                unSelectAll();
            });
    };
};

customerSearchController.$inject = ['$scope', 'xhr', 'CommonService', 'UsersService', '$routeParams', '$location', 'csrList'];
module.exports = customerSearchController;