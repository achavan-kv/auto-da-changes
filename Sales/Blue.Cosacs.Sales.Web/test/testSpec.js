"use strict";

var fakes = require('./support/fakes');
//
var fs = require('fs');

describe('Infrastructure Tests', function () {
    it('should import a helper module', function () {
        expect(fakes.CartItemToAdd).toBeDefined();
        expect(fakes.OrderToSave).toBeDefined();
        expect(fakes.CurrentUser).toBeDefined();
    });

    it('should imported lodash lib', function () {
        var obj = {'a': 1, 'b': 2, 'c': 3};

        expect(_.has(obj, 'c')).toBeTruthy();
    });

    it('should imported string lib', function () {
        expect(S('JP').capitalize().s).toEqual('Jp');
    });

});