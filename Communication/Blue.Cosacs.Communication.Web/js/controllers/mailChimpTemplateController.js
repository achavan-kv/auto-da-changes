'use strict';

var mailChimpTemplateController = function ($scope, $http, CommonService) {

    $scope.addingTemplate = false;
    $scope.newTemplate = newEmptyTemplate();

    $http({
        url: '/Communication/api/MailchimpTemplate',
        method: "GET"
    })
        .then(function (resp) {
            $scope.results = resp.data;
        });

    $scope.IsValidTemplate = IsValidTemplate;

    function IsValidTemplate(template){

        if (!template){
            return false;
        }

        return !(S(template.Name).isEmpty() || S(template.TemplateId).isEmpty());
    }

    function saveTemplate (template, callBack) {
        if (!IsValidTemplate(template)) {
            return null;
        }

        $http.post('/Communication/api/MailchimpTemplate', template)
            .then(function (data) {
                callBack(data);
            });
    }

    $scope.updateTemplate =  function(template){
        saveTemplate(template, function(data){
            CommonService.addSuccessGrowl('Successfully saved.');
        });
    };

    $scope.insertTemplate = function (template) {
        saveTemplate(template, function(data){
            CommonService.addSuccessGrowl('Successfully created.');

            $scope.results.push(template);
            cancelTemplate();
        })
    };

    $scope.deleteTemplate = function (id) {
        $http.delete('/Communication/api/MailchimpTemplate/' + id)
            .success(function () {
                $scope.results = _.filter($scope.results, function (current) {
                    return current.Id != id;
                });
            })
            .error(function(data){
                CommonService.addDangerGrowl(data.Message);
            });
    };

    $scope.createNewTemplate = function () {
        $scope.newTemplate = newEmptyTemplate();
        $scope.addingTemplate = true;
    };

    function cancelTemplate() {
        $scope.newTemplate = null;
        $scope.addingTemplate = false;
    }

    $scope.cancelTemplate = cancelTemplate;

    function newEmptyTemplate() {
        return {
            Id: 0,
            Name: null,
            TemplateId: null,
            CanSetBody: false
        };
    }
};

mailChimpTemplateController.$inject = ['$scope', '$http', 'CommonService'];
module.exports = mailChimpTemplateController;