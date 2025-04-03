-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 


insert into Config.DecisionTable ("Key", CreatedUtc, Value)
values ('SR.DecisionTable.Charge', '2012-12-19 15:54:00', '{
  "conditions": [
    {
      "expression": "this.serviceRequest.ResolutionPrimaryCharge"
    },
    {
      "expression": "this.MasterData.Settings.TaxType"
    },
    {
      "expression": "\"Inside Supplier Warranty\" &&\n((new Date(+this.serviceRequest.ItemSoldOn +\n (new Date((365.25/12)*(24*3600*1000)) *\n this.serviceRequest.ManWarrantyLth))) >= (new Date()))"
    },
    {
      "expression": "\"Inside Extended Warranty\" &&\n\n(this.serviceRequest.WarrantyContractNumber !== null &&\n!isNaN(this.serviceRequest.WarrantyContractNumber)) &&\n\n((new Date(+this.serviceRequest.ItemSoldOn +\n (new Date((365.25/12)*(24*3600*1000)) *\n this.serviceRequest.WarrantyLength))) >= (new Date()))"
    },
    {
      "expression": "/* This is here just to introduce a dependency on the Supplier to re-calculate */ this.serviceRequest.ResolutionSupplierToCharge"
    }
  ],
  "actions": [
    {
      "expression": "this.serviceRequest.Charges = [];"
    },
    {
      "expression": "var sr = this.serviceRequest;\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsSumCatalog(sr)\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsSumNonCatalog(sr)\n});\n\nsr.Charges.push({\n  Label:      \"Labour\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sr.ResolutionLabourCost\n});\n\nsr.Charges.push({\n  Label:      \"Additional\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sr.ResolutionAdditionalCost + sr.ResolutionTransportCost\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceInternal,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsSumCatalog(sr)\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceInternal,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsSumNonCatalog(sr)\n});\n\nsr.Charges.push({\n  Label:      \"Labour\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceInternal,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sr.ResolutionLabourCost\n});\n\nsr.Charges.push({\n  Label:      \"Additional\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceInternal,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sr.ResolutionAdditionalCost + sr.ResolutionTransportCost\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceWarranty,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsSumCatalog(sr)\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceWarranty,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsSumNonCatalog(sr)\n});\n\nsr.Charges.push({\n  Label:      \"Labour\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceWarranty,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sr.ResolutionLabourCost\n});\n\nsr.Charges.push({\n  Label:      \"Additional\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceWarranty,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sr.ResolutionAdditionalCost + sr.ResolutionTransportCost\n});"
    },
    {
      "expression": "addTax(this);"
    },
    {
      "expression": "var sr = this.serviceRequest,\n    matrix = this.MasterData.SupplierCostMatrix,\n    account = this.MasterData.ServiceSupplierAccount[sr.ResolutionSupplierToCharge];\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: \"Supplier\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsChargeWithSupplierMatrix(sr, not(partNumberIsEmpty), matrix).covered\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: \"Supplier\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsChargeWithSupplierMatrix(sr, partNumberIsEmpty, matrix).covered\n});\n\nsr.Charges.push({\n  Label:      \"Labour and Additional\",\n  ChargeType: \"Supplier\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      labourSupplierCovered(sr.ResolutionLabourCost + sr.ResolutionAdditionalCost + sr.ResolutionTransportCost, matrix)\n});"
    },
    {
      "expression": "var sr = this.serviceRequest,\n    matrix = this.MasterData.SupplierCostMatrix,\n    account = this.MasterData.Settings.ServiceInternal;\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: \"Internal\",\n  Account:    account ,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsChargeWithSupplierMatrix(sr, not(partNumberIsEmpty), matrix).remaining\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: \"Internal\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsChargeWithSupplierMatrix(sr, partNumberIsEmpty, matrix).remaining\n});\n\nsr.Charges.push({\n  Label:      \"Labour and Additional\",\n  ChargeType: \"Internal\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      labourSupplierRemaining(sr.ResolutionLabourCost + sr.ResolutionAdditionalCost + sr.ResolutionTransportCost, matrix)\n});"
    },
    {
      "expression": "var sr = this.serviceRequest,\n    matrix = this.MasterData.SupplierCostMatrix,\n    account = this.MasterData.Settings.ServiceWarranty;\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: \"EW\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsChargeWithSupplierMatrix(sr, not(partNumberIsEmpty), matrix).remaining\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: \"EW\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsChargeWithSupplierMatrix(sr, partNumberIsEmpty, matrix).remaining\n});\n\nsr.Charges.push({\n  Label:      \"Labour and Additional\",\n  ChargeType: \"EW\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      labourSupplierRemaining(sr.ResolutionLabourCost + sr.ResolutionAdditionalCost + sr.ResolutionTransportCost, matrix)\n});"
    },
    {
      "expression": "var sr = this.serviceRequest,\n    matrix = this.MasterData.SupplierCostMatrix;\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: \"Customer\",\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsChargeWithSupplierMatrix(sr, not(partNumberIsEmpty), matrix).remaining\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: \"Customer\",\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsChargeWithSupplierMatrix(sr, partNumberIsEmpty, matrix).remaining\n});\n\nsr.Charges.push({\n  Label:      \"Labour and Additional\",\n  ChargeType: \"Customer\",\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      labourSupplierRemaining(sr.ResolutionLabourCost + sr.ResolutionAdditionalCost + sr.ResolutionTransportCost, matrix)\n});"
    }
  ],
  "rules": [
    {
      "values": [
        "Customer",
        null,
        null,
        null,
        null
      ],
      "actions": [
        true,
        true,
        false,
        false,
        false,
        false,
        false,
        false,
        false
      ]
    },
    {
      "values": [
        "Internal",
        null,
        null,
        null,
        null
      ],
      "actions": [
        true,
        false,
        true,
        false,
        false,
        false,
        false,
        false,
        false
      ]
    },
    {
      "values": [
        "Deliverer",
        null,
        null,
        null,
        null
      ],
      "actions": [
        true,
        true,
        false,
        false,
        false,
        false,
        false,
        false,
        false
      ]
    },
    {
      "values": [
        "EW",
        null,
        null,
        null,
        null
      ],
      "actions": [
        true,
        false,
        false,
        true,
        false,
        false,
        false,
        false,
        false
      ]
    },
    {
      "values": [
        "Customer",
        "E",
        null,
        null,
        null
      ],
      "actions": [
        true,
        false,
        false,
        false,
        true,
        false,
        false,
        false,
        false
      ]
    },
    {
      "values": [
        "Supplier",
        null,
        "true",
        null,
        null
      ],
      "actions": [
        true,
        false,
        false,
        false,
        false,
        true,
        true,
        false,
        false
      ]
    },
    {
      "values": [
        "Supplier",
        null,
        "false",
        "true",
        null
      ],
      "actions": [
        true,
        false,
        false,
        false,
        false,
        true,
        false,
        true,
        false
      ]
    },
    {
      "values": [
        "Supplier",
        null,
        "false",
        "false",
        null
      ],
      "actions": [
        true,
        false,
        false,
        false,
        false,
        true,
        false,
        false,
        true
      ]
    }
  ],
  "extensions": "// this.MasterData.Settings.TaxType: Stock includes tax or not.(I or E)\n\nvar sum = function (list, f) {\n    return _.reduce(list, function(memo, e){ return memo + f(e); }, 0);\n};\n\nvar empty = function (s) {\n    return !s;\n};\n\nvar iif = function (expr, whenTrue, whenFalse) {\n    if (expr) {\n        return whenTrue;\n    } else {\n        return whenFalse;\n    }\t\t\n};\n\nvar not = function(f) {\n    return function(s) {\n        return !f(s);\n    };\n};\n\nvar yearsSold = function(sr) {\n    var yearMs = 365.25 * 24 * 60 * 60 * 1000;\n    return (new Date() - sr.ItemSoldOn) / yearMs;\n};\n\nvar partNumberIsEmpty = function (part) {\n    return empty(part.number);\n};\n\nvar partTotalPrice = function (part) {\n    return part.quantity * part.price;\n};\n\nvar findCost = function (sr, part, matrix) {\n  return _.find(matrix, function (row) {\n    return row.partType === part.type && yearsSold(sr) <= row.year;\n  });\n};\n\n// part cost covered by supplier\nvar partSupplierCovered = function (sr, part, matrix, price) {\n  var cost = findCost(sr, part, matrix);\n  if (cost) {\n    if (cost.partPcent) {\n      price = price * cost.partPcent / 100;\n    } \n    if (cost.partVal) {\n      price =  Math.min(price, cost.partVal);\n    }\n    return price;\n  } \n  return 0;\n};\n\nvar labourSupplierCovered = function (labour, matrix) {\n  return _.max(_.map(matrix, function (row) {\n    var v = labour;\n    if (row.labourPcent) {\n      v = v * row.labourPcent / 100;\n    }\n    if (row.labourVal) {\n      v = Math.min(v, row.labourVal);\n    }\n    return v;\n  })) || 0;\n};\n\nvar labourSupplierRemaining = function (labour, matrix) {\n  var v = labourSupplierCovered(labour, matrix) || 0;\n  return labour - v;\n};\n\nvar partsSumCatalog = function (sr) {\n  return sum(_.filter(sr.Parts, not(partNumberIsEmpty)), partTotalPrice);\n};\n\nvar partsSumNonCatalog = function (sr) {\n  return sum(_.filter(sr.Parts, partNumberIsEmpty), partTotalPrice);\n};\n\nvar partsChargeWithSupplierMatrix = function (sr, filter, matrix) {\n  var partSplit = _.map(_.filter(sr.Parts, filter), function (part) {\n    var price = partTotalPrice(part),\n        covered = partSupplierCovered(sr, part, matrix, price);\n    return {\n      covered: covered,\n      remaining: price - covered\n    };\n  });\n  return { \n    covered: sum(partSplit, function (s) { return s.covered; }),\n    remaining: sum(partSplit, function (s) { return s.remaining; })\n  };  \n};\n\nvar addTax = function (scope) {\n  _.each (scope.serviceRequest.Charges, function (c) {\n    c.Tax = (c.Value * scope.MasterData.Settings.TaxRate / 100.0).toFixed(2);\n  });\n};"
}')