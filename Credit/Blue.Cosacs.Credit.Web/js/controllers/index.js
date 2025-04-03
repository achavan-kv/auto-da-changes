'use strict';

angular.module('Credit.controllers', ['ngRoute'])
    .config([
        '$compileProvider',
        function ($compileProvider) {
            /*
             * Chrome adds unsafe in front of any type of URI which is not part of its whitelist.
             * The ngCropper tool uses base64 encoded strings as the src attribute of our image
             * element. Without this setting the cropper would not work.
             */
            $compileProvider.imgSrcSanitizationWhitelist(/^\s*(https?|local|data):/);
        }
    ])
    .controller('proposalController', require('./proposal'))
    .controller('proposalApplicant2Controller', require('./proposalApplicant2'))
    .controller('sanctionStage1Controller', require('./sanctionStage1'))
    .controller('sanctionStage2Controller', require('./sanctionStage2'))
    .controller('proposalSearchController', require('./proposalSearch'))
    .controller('customizeMandatoryFieldsController', require('./customizeMandatoryFields'))
    .controller('scoreController', require('./score'))
    .controller('documentConfirmationController', require('./documentConfirmation'))
    .controller('navController', require('./nav'))
    .controller('customerDetailsController', require('./customerDetails'))
    .controller('scoringBandMatrixController', require('./scoringBandMatrixSetup'))
    .controller('scoreConfigController', require('./scoringConfig'))
    .controller('croppingModalController', require('./croppingModal'))
    .controller('customerSearchController', require('./customerSearch'))
    .controller('termsTypeController', require('./termsType'))
    .controller('termsTypeSearchController', require('./termsTypeSearch'))
    .controller('termsTypeSimulatorController', require('./termsTypeSimulator'));
