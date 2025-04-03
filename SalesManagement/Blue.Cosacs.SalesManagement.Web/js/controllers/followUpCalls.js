'use strict';

var followUpCallController = function ($scope, $http, CommonService) {

    $scope.addingFollow = false;
    $scope.timePeriods = {
        '1': 'Day',
        '2': 'Week',
        '3': 'Month'
    };

    $scope.contactMeans = {
        'Phone': '1',
        'Email': '2',
        'Sms': '3',
        'None': '0'
    }

    $scope.newFollow = newEmptyFollow();
    $scope.Templates = [];

    function getData() {
        $http({
            url: '/SalesManagement/api/FollowUpCalls',
            method: "GET"
        })
        .then(function (resp) {
            $scope.results = _.map(resp.data, processDataFromServer);
        });

        $http.get('/Communication/api/MailchimpTemplate')
            .then(function(resp){
                var results = {};

                _.forEach(resp.data, function (current) {
                    results[current.Id] = current.Name;
                });
                $scope.Templates = results;
            });
    }

    $scope.isValidFollow = validFollow;

    function validFollow(follow) {

        if (!follow) {
            return false;
        }

        if (!follow.TimePeriod) {
            return false;
        }
        if (!_.isNumber(follow.Quantity) || follow.Quantity < 1) {
            return false;
        }

        if (follow.SelectedContactType === $scope.contactMeans.Email && (!follow.ContactViaTemplate || !follow.ContactEmailSubject)){
            return false;
        }

        if (follow.SelectedContactType === $scope.contactMeans.Sms && S(follow.ContactViaSms).isEmpty()){
            return false;
        }

        if (follow.IfFlushedContactType === $scope.contactMeans.Email && (!follow.IfFlushedTemplate || ! follow.FlushedEmailSubject)){
            return false;
        }

        if (follow.IfFlushedContactType === $scope.contactMeans.Sms && S(follow.IfFlushedSms).isEmpty()){
            return false;
        }

        return !S(follow.ReasonToCall).isEmpty();
    }

    $scope.updateFollow = function (follow) {
        if (!validFollow(follow)) {
            return null;
        }

        $http.put('/SalesManagement/api/FollowUpCalls', prepareFollowToSend(follow))
            .then(function () {
                CommonService.addSuccessGrowl('Successfully Saved the Follow Up Calls.')
            });
    };

    $scope.createNewFollowUpCall = function () {
        $scope.newFollow = newEmptyFollow();
        $scope.addingFollow = true;
    };

    function prepareFollowToSend(data){
        var followToSend =  _.assign({
            'MailchimpTemplateID': null,
            'SmsText': null
        }, data);

        followToSend.ContactMeansId = followToSend.SelectedContactType;

        if (followToSend.SelectedContactType === $scope.contactMeans.Email){
            followToSend.MailchimpTemplateID = followToSend.ContactViaTemplate;
        }
        else if (followToSend.SelectedContactType === $scope.contactMeans.Sms) {
            followToSend.SmsText = followToSend.ContactViaSms;
            followToSend.ContactEmailSubject = null;
        }
        else
        {
            followToSend.ContactEmailSubject = null;
        }

        if (followToSend.IfFlushedContactType === $scope.contactMeans.Email){
            followToSend.MailchimpTemplateID = followToSend.IfFlushedTemplate;
            followToSend.AlternativeContactMeanId = $scope.contactMeans.Email;
        }
        else if (followToSend.IfFlushedContactType === $scope.contactMeans.Sms){
            followToSend.SmsText = followToSend.IfFlushedSms;
            followToSend.AlternativeContactMeanId = $scope.contactMeans.Sms;
            followToSend.FlushedEmailSubject = null;
        }
        else
        {
            followToSend.FlushedEmailSubject = null;
        }

        delete followToSend.SelectedContactType;
        delete followToSend.IfFlushedContactType;
        delete followToSend.ContactViaTemplate;
        delete followToSend.IfFlushedTemplate;
        delete followToSend.ContactViaSms;
        delete followToSend.IfFlushedSms;

        return followToSend;
    }

    function processDataFromServer(data){
        var result = _.extend({
            'ContactViaTemplate': null,
            'IfFlushedTemplate': null,
            'ContactViaSms': null,
            'IfFlushedSms': null
        }, data);

        result.TimePeriod = S(data.TimePeriod).s;
        result.ContactMeansId = S(result.ContactMeansId).s;
        result.AlternativeContactMeanId = S(result.AlternativeContactMeanId).s;

        if (result.ContactMeansId === $scope.contactMeans.Email){
            result.ContactViaTemplate = S(result.MailchimpTemplateID).s;
            result.SelectedContactType = $scope.contactMeans.Email;
        }
        else if (result.ContactMeansId === $scope.contactMeans.Sms){
            result.ContactViaSms = result.SmsText
            result.SelectedContactType = $scope.contactMeans.Sms;
        }
        else{
            result.SelectedContactType = $scope.contactMeans.Phone;
        }

        if (result.AlternativeContactMeanId === $scope.contactMeans.Email){
            result.IfFlushedTemplate = S(result.MailchimpTemplateID).s
            result.IfFlushedContactType = $scope.contactMeans.Email;
        }
        else if (result.AlternativeContactMeanId === $scope.contactMeans.Sms){
            result.IfFlushedSms = result.SmsText
            result.IfFlushedContactType = $scope.contactMeans.Sms;
        }
        else{
            result.IfFlushedContactType = $scope.contactMeans.None;
        }

        delete result.ContactMeansId;
        delete result.AlternativeContactMeanId;
        delete result.MailchimpTemplateID
        delete result.SmsText

        return result;
    }

    $scope.insertFollowUpCall = function (follow) {
        if (!validFollow(follow)) {
            return null;
        }

        $http.post('/SalesManagement/api/FollowUpCalls', prepareFollowToSend(follow))
            .success(function (data) {
                CommonService.addGrowl({
                    timeout: 5,
                    type: 'success', // (optional - info, danger, success, warning)
                    content: 'Successfully Saved the Follow Up Calls.'
                });

                $scope.results = _.map(data, processDataFromServer);

                cancelFollowUpCall();
            }).
            error(function (data, status, headers, config) {
                //TODO:do something
            });
    };

    function cancelFollowUpCall() {
        $scope.newFollow = null;
        $scope.addingFollow = false;
    }

    $scope.deleteFollowUpCall = function (id) {
        $http.delete('/SalesManagement/api/FollowUpCalls/' + id)
            .success(function () {
                $scope.results = _.filter($scope.results, function (current) {
                    return current.Id != id;
                });
            }).
            error(function (data, status, headers, config) {
                //TODO:do something
            });
    };

    $scope.cancelnewFollow = cancelFollowUpCall;

    function newEmptyFollow() {
        return {
            Id: 0,
            TimePeriod: null,
            Quantity: null,
            ReasonToCall: null,
            SelectedContactType: $scope.contactMeans.Phone,
            IfFlushedContactType: $scope.contactMeans.None,
            ContactViaTemplate: null,
            IfFlushedTemplate: null,
            ContactViaSms: null,
            IfFlushedSms: '',
            Icon: null,
            ContactEmailSubject: null,
            FlushedEmailSubject : null
        };
    }

    getData();
};

followUpCallController.$inject = ['$scope', '$http', 'CommonService'];

module.exports = followUpCallController;