'use strict';

exports.roundFloat = roundFloat;
exports.includeTax = includeTax;
exports.excludeTax = excludeTax;

function roundFloat(no, precision) {
    precision = precision||4;

    var ret = bankersRounding(no, precision);

    return ret;
}

function bankersRounding(num, decimalPlaces){
    num = num || 0;
    var d = decimalPlaces || 0;
    var m = Math.pow(10, d);
    var n = +(d ? num * m : num).toFixed(8); // Avoid rounding errors
    var i = Math.floor(n), f = n - i;
    var e = 1e-8; // Allow for rounding errors in f
    var r = (f > 0.5 - e && f < 0.5 + e) ?
            ((i % 2 == 0) ? i : i + 1) : Math.round(n);
    return d ? r / m : r;
}

function includeTax(price, rate) {
    if (!price) {
        return 0;
    }

    var rate = (rate * 0.01) + 1;

    return price * rate;
}

function excludeTax(price, rate) {
    if (!price) {
        return 0;
    }

    if (!rate) {
        return price;
    }

    return (price * 100) / (100 + rate);
}