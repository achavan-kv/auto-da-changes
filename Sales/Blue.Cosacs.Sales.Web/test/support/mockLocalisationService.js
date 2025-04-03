'use strict';
require('angular-mocks');

var q;


inject(function ( $q) {
    q = $q;

});

function mockLocalisationService(){
    return ({
        fetchLocalisationSettings: function (CommonService) {

        },
        getSettings: function () {
            console.log('HHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH');
            var deferred = q.defer();
   var data ={"CurrencySymbol":"$","DecimalPlaces":2};

                    deferred.resolve(data);
            return deferred.promise;
        }
    });
}

module.exports= mockLocalisationService;