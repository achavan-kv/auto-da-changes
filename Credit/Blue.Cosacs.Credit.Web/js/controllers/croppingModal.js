'use strict';

var common = require('./common')();

var croppingModalController = function($scope, $modalInstance, $timeout, Cropper, fileService, $routeParams, $http, $q) {

    /**
     * Cropper - https://github.com/koorgoo/ngCropper
     * This is a wrapper over - https://github.com/fengyuanchen/cropper
     * More on options - https://github.com/fengyuanchen/cropper#options
     *
     * NOTE!!!
     * - Keep in mind regarding the options that ngCropper uses version 0.7.7 of the jQuery cropper, while the options listed
     * on the github page refer to the latest version of the jQuery cropper.
     * - If you add inexistent/incorrect options to the options object they will SILENTLY be ignored
     * - You can find the correct list of options for version 0.7.7 @ https://github.com/fengyuanchen/cropper/blob/0e502f6aa6cf19990e8abe1ffc5f636108fa64fb/README.md#options
     * - If the above fail you can find them listed in ngCropper.all.js@1432 where the DEFAULTS are defined.
     *
     * The behavior we are currently using is the following:
     * - minimum crop size: 150x150
     * - aspect ratio 1 (should look like a square)
     * - the crop box is resizable without going under the minimum size
     * - if the picture is cropped with a size greater than 150 it is first scaled on the client before we send it to the server
     */

    var minSize = 150;
    $scope.options = {
        aspectRatio: 1,
        resizable: true,
        zoomable: false,
        minWidth: 150,
        minHeight: 150,
        done: function(dataNew) {
            data = dataNew;
        }
    };

    $scope.cancel = function () {
        hideCropper();
        $modalInstance.dismiss('cancel');
    };

    $scope.triggerInput = function() {

    };

    var file, data;

    /**
     * Method is called every time file input's value changes.
     * Because of Angular has not ng-change for file inputs a hack is needed -
     * call `angular.element(this).scope().onFile(this.files[0])`
     * when input's event is fired.
     */
    $scope.onFile = function(inputFileObj) {
        Cropper.encode((file = inputFileObj)).then(function(dataUrl) {
            $scope.dataUrl = dataUrl;
            $timeout(showCropper);  // wait for $digest to set image's src
        });
    };

    /**
    * The cropped image is uploaded to the server and a new record is created in our DB.
    * If this is successful we receive a new GUID which we will then try and associate
    * with the current customer record.
    * */
    $scope.saveImage = function() {
        if (!file) {
            return;
        }

        Cropper.crop(file, data).then(function(blob) {

            Cropper.encode(blob).then(_createImage).then(function(image) {
                var defer = $q.defer();
                var blob = Cropper.decode(image.src);

                //If the cropped image is greater than our presets we scale it
                if(image.width > minSize || image.height > minSize) {
                    Cropper.scale(blob, {width : 150}).then(function(scaledBlob) {
                        defer.resolve(scaledBlob);
                    });
                }
                //Otherwise we return it as it is
                else {
                    defer.resolve(blob);
                }

                return defer.promise;
            }).then(uploadAndAssociateImage);
        });
    };

    /**
    * Upload the image to our store and if that is successful associate its new
    * unique identifier with the current customer record.
    * */
    function uploadAndAssociateImage(imgData) {
        var fd = new FormData();
        fd.append('file', imgData, file.name);
        fd.append('content-type', imgData.type);

        fileService.post(fd).success(function(result) {
            var newBackendImgIdentifier = result;

            $http.post('/credit/api/CustomerPhoto/', {
                CustomerId: $routeParams.Id,
                PhotoIdentifier: newBackendImgIdentifier
            }).success(function() {
                $modalInstance.close(newBackendImgIdentifier);
            });
        });
    }

    /**
     * Create an image from a blob. We use this to inspect the image's size
     * after cropping in order to determine if we also need to scale it.
     * */
    function _createImage(blob) {
        var defer = $q.defer();
        var image = new Image();
        image.onload = function(e) { defer.resolve(e.target); };
        image.src = blob;
        return defer.promise;
    }

    /**
     * Showing (initializing) and hiding (destroying) of a cropper are started by
     * events. The scope of the `ng-cropper` directive is derived from the scope of
     * the controller. When initializing the `ng-cropper` directive adds two handlers
     * listening to events passed by `ng-show` & `ng-hide` attributes.
     * To show or hide a cropper `$broadcast` a proper event.
     */
    $scope.showEvent = 'show';
    $scope.hideEvent = 'hide';

    function showCropper() {
        $scope.$broadcast('show');
    }

    function hideCropper() {
        $scope.$broadcast('hide');
    }
};

croppingModalController.$inject = ['$scope', '$modalInstance', '$timeout', 'Cropper', 'fileService', '$routeParams', '$http', '$q'];
module.exports = croppingModalController;
