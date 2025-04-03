'use strict';

function humanize() {
    return function (input) {
        return _.str.humanize(input);
    };
}

function titleize() {
    return function (input) {
        return _.str.titleize(input);
    };
}

angular.module('Payments.filters', [])
    .filter('titleize', titleize)
    .filter('humanize', humanize);
