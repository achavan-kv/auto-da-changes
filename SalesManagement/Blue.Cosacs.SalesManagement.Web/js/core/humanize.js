'use strict';

function ordinal(value) {
    var end, leastSignificant, number, specialCase;
    number = parseInt(value, 10);
    if (number === 0) {
        return value;
    }
    specialCase = number % 100;
    if (specialCase === 11 || specialCase === 12 || specialCase === 13) {
        return "" + number + "th";
    }
    leastSignificant = number % 10;
    switch (leastSignificant) {
        case 1:
            end = "st";
            break;
        case 2:
            end = "nd";
            break;
        case 3:
            end = "rd";
            break;
        default:
            end = "th";
    }
    return "" + number + end;
};

function compactInteger(input, decimals) {
    var bigNumPrefixes, decimalIndex, decimalPart, decimalPartArray, length, number, numberLength, numberLengths, output, outputNumber, signString, unsignedNumber, unsignedNumberCharacterArray, unsignedNumberString, wholePart, wholePartArray, _i, _len, _length;
    if (decimals == null) {
        decimals = 0;
    }
    decimals = Math.max(decimals, 0);
    number = parseInt(input, 10);
    signString = number < 0 ? "-" : "";
    unsignedNumber = Math.abs(number);
    unsignedNumberString = "" + unsignedNumber;
    numberLength = unsignedNumberString.length;
    numberLengths = [13, 10, 7, 4];
    bigNumPrefixes = ['T', 'B', 'M', 'k'];
    if (unsignedNumber < 1000) {
        return "" + signString + unsignedNumberString;
    }
    if (numberLength > numberLengths[0] + 3) {
        return number.toExponential(decimals).replace('e+', 'x10^');
    }
    for (_i = 0, _len = numberLengths.length; _i < _len; _i++) {
        _length = numberLengths[_i];
        if (numberLength >= _length) {
            length = _length;
            break;
        }
    }
    decimalIndex = numberLength - length + 1;
    unsignedNumberCharacterArray = unsignedNumberString.split("");
    wholePartArray = unsignedNumberCharacterArray.slice(0, decimalIndex);
    decimalPartArray = unsignedNumberCharacterArray.slice(decimalIndex, decimalIndex + decimals + 1);
    wholePart = wholePartArray.join("");
    decimalPart = decimalPartArray.join("");
    if (decimalPart.length < decimals) {
        decimalPart += "" + (Array(decimals - decimalPart.length + 1).join('0'));
    }
    if (decimals === 0) {
        output = "" + signString + wholePart + bigNumPrefixes[numberLengths.indexOf(length)];
    } else {
        outputNumber = (+("" + wholePart + "." + decimalPart)).toFixed(decimals);
        output = "" + signString + outputNumber + bigNumPrefixes[numberLengths.indexOf(length)];
    }
    return output;
};

function normalizePrecision (value, base) {
    value = Math.round(Math.abs(value));
    if (isNaN(value)) {
        return base;
    } else {
        return value;
    }
};

function formatNumber(number, precision, thousand, decimal) {
    var base, commas, decimals, firstComma, mod, negative, usePrecision;
    if (precision == null) {
        precision = 0;
    }
    if (thousand == null) {
        thousand = ",";
    }
    if (decimal == null) {
        decimal = ".";
    }
    firstComma = function(number, thousand, position) {
        if (position) {
            return number.substr(0, position) + thousand;
        } else {
            return "";
        }
    };
    commas = function(number, thousand, position) {
        return number.substr(position).replace(/(\d{3})(?=\d)/g, "$1" + thousand);
    };
    decimals = function(number, decimal, usePrecision) {
        if (usePrecision) {
            return decimal + toFixed(Math.abs(number), usePrecision).split(".")[1];
        } else {
            return "";
        }
    };
    usePrecision = normalizePrecision(precision);
    negative = number < 0 && "-" || "";
    base = parseInt(toFixed(Math.abs(number || 0), usePrecision), 10) + "";
    mod = base.length > 3 ? base.length % 3 : 0;

    return negative + firstComma(base, thousand, mod) + commas(base, thousand, mod) + decimals(number, decimal, usePrecision);
};

function toFixed (value, precision) {
    var power;
    if (precision === null) {
        precision = normalizePrecision(precision, 0);
    }
    power = Math.pow(10, precision);
    return (Math.round(value * power) / power).toFixed(precision);
};

module.exports ={
    ordinal: ordinal,
    compactInteger: compactInteger,
    formatNumber: formatNumber,
    normalizePrecision: normalizePrecision
};