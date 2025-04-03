"use strict";

require("./controllers");
require("./services");
require("./directives");

var deps = [
  "Merchandising.controllers",
  "Merchandising.services",
  "Merchandising.directives"
];

var merch = angular.module("Merchandising", deps);
module.exports = merch;