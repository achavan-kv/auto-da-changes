'use strict';

var inactiveCustomerController = function ($scope, $http, CommonService) {

    $http({
        url: '/SalesManagement/api/InactiveCustomersInteraction',
        method: "GET"
    })
    .then(function (resp) {
        $scope.data = processDataFromServer(resp.data);
    });

    $scope.contactMeans = {
        'Phone': '1',
        'Email': '2',
        'Sms': '3',
        'None': '0'
    };

    $http.get('/Communication/api/MailchimpTemplate')
        .then(function(resp){
            var results = {};

            _.forEach(resp.data, function (current) {
                results[current.Id] = current.Name;
            });
            $scope.Templates = results;
        });


    function processDataFromServer(values){

        var returnValue = [];

        _.forEach(values, function(current) {

            var result = _.extend({
                'ContactViaTemplate': null,
                'IfFlushedTemplate': null,
                'ContactViaSms': null,
                'IfFlushedSms': null,
                'Title': current.Id === 1 ? 'Inactive Customers' : 'Instalment Ending'
            }, current);

            result.ContactMeansId = S(result.ContactMeansId).s;
            result.AlternativeContactMeanId = S(result.AlternativeContactMeanId).s;

            if (result.ContactMeansId === $scope.contactMeans.Email) {
                result.ContactViaTemplate = S(result.MailchimpTemplateID).s;
                result.SelectedContactType = $scope.contactMeans.Email;
            }
            else if (result.ContactMeansId === $scope.contactMeans.Sms) {
                result.ContactViaSms = result.SmsText
                result.SelectedContactType = $scope.contactMeans.Sms;
            }
            else {
                result.SelectedContactType = $scope.contactMeans.Phone;
            }

            if (result.AlternativeContactMeanId === $scope.contactMeans.Email) {
                result.IfFlushedTemplate = S(result.MailchimpTemplateID).s
                result.IfFlushedContactType = $scope.contactMeans.Email;
            }
            else if (result.AlternativeContactMeanId === $scope.contactMeans.Sms) {
                result.IfFlushedSms = result.SmsText
                result.IfFlushedContactType = $scope.contactMeans.Sms;
            }
            else {
                result.IfFlushedContactType = $scope.contactMeans.None;
            }

            delete result.ContactMeansId;
            delete result.AlternativeContactMeanId;
            delete result.MailchimpTemplateID
            delete result.SmsText

            returnValue.push(result);
        });

        return _.sortBy(returnValue, ['Id']);
    }

    $scope.isValidInactive = validInactive;

    function validInactive(toValidate) {
        if (!toValidate) {
            return false;
        }

        if (toValidate.SelectedContactType === $scope.contactMeans.Email && (!toValidate.ContactViaTemplate || !toValidate.ContactEmailSubject)){
            return false;
        }

        if (toValidate.SelectedContactType === $scope.contactMeans.Sms && S(toValidate.ContactViaSms).isEmpty()){
            return false;
        }

        if (toValidate.IfFlushedContactType === $scope.contactMeans.Email && (!toValidate.IfFlushedTemplate || !toValidate.FlushedEmailSubject)){
            return false;
        }

        if (toValidate.IfFlushedContactType === $scope.contactMeans.Sms && S(toValidate.IfFlushedSms).isEmpty()){
            return false;
        }

        return true;
    }

    $scope.updateValues = function(toSend){
        if (!validInactive(toSend)) {
            return null;
        }

        $http.put('/SalesManagement/api/InactiveCustomersInteraction', prepareDataToSend(toSend))
            .then(function () {
                CommonService.addSuccessGrowl('Successfully Saved.')
            });
    }

    function prepareDataToSend(toPrepare){
        var inactiveToSend =  _.assign({
            'MailchimpTemplateID': null,
            'SmsText': null
        }, toPrepare);

        inactiveToSend.ContactMeansId = inactiveToSend.SelectedContactType;

        if (inactiveToSend.SelectedContactType === $scope.contactMeans.Email){
            inactiveToSend.MailchimpTemplateID = inactiveToSend.ContactViaTemplate;
        }
        else if (inactiveToSend.SelectedContactType === $scope.contactMeans.Sms) {
            inactiveToSend.SmsText = inactiveToSend.ContactViaSms;
            inactiveToSend.ContactEmailSubject = null;
        }
        else
        {
            inactiveToSend.ContactEmailSubject = null;
        }

        if (inactiveToSend.IfFlushedContactType === $scope.contactMeans.Email){
            inactiveToSend.MailchimpTemplateID = inactiveToSend.IfFlushedTemplate;
            inactiveToSend.AlternativeContactMeanId = $scope.contactMeans.Email;
        }
        else if (inactiveToSend.IfFlushedContactType === $scope.contactMeans.Sms){
            inactiveToSend.SmsText = inactiveToSend.IfFlushedSms;
            inactiveToSend.AlternativeContactMeanId = $scope.contactMeans.Sms;
            inactiveToSend.FlushedEmailSubject = null;
        }
        else{
            inactiveToSend.FlushedEmailSubject = null;
        }

        delete inactiveToSend.SelectedContactType;
        delete inactiveToSend.IfFlushedContactType;
        delete inactiveToSend.ContactViaTemplate;
        delete inactiveToSend.IfFlushedTemplate;
        delete inactiveToSend.ContactViaSms;
        delete inactiveToSend.IfFlushedSms;
        delete inactiveToSend.Title;

        return inactiveToSend;
    }

};

inactiveCustomerController.$inject = ['$scope', '$http', 'CommonService'];

module.exports = inactiveCustomerController;