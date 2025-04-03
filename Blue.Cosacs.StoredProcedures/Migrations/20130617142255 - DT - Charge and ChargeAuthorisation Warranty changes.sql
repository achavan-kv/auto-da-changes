INSERT INTO Config.DecisionTable
        ( [Key], CreatedUtc, Value )
VALUES  ( 'SR.DecisionTable.Charge', -- Key - varchar(50)
Getdate(), -- CreatedUtc - datetime
'{
  "conditions": [
    {
      "expression": "this.serviceRequest.ResolutionPrimaryCharge"
    },
    {
      "expression": "this.MasterData.Settings.TaxType"
    },
    {
      "expression": "\"Inside Supplier Warranty\" &&\nthis.serviceRequest.WarrantyNumber &&\ninsideWarranty(this.serviceRequest, this.serviceRequest.ManWarrantyLength || 12)"
    },
    {
      "expression": "\"Inside Extended Warranty\" &&\n\nthis.serviceRequest.WarrantyNumber &&\ninsideWarranty(this.serviceRequest, this.serviceRequest.WarrantyLength || 0)"
    },
    {
      "expression": "this.isItemBer && this.serviceRequest.ReplacementIssued"
    }
  ],
  "actions": [
    {
      "expression": "this.serviceRequest.Charges = [];"
    },
    {
      "expression": "var sr = this.serviceRequest;\n\nsr.Charges.push({\n  CustomerId: sr.ResolutionDelivererToCharge,\n  Label:      \"Parts Cosacs\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsSumCatalog(sr)\n});\n\nsr.Charges.push({\n  CustomerId: sr.ResolutionDelivererToCharge,\n  Label:      \"Parts Other\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsSumNonCatalog(sr)\n});\n\nsr.Charges.push({\n  CustomerId: sr.ResolutionDelivererToCharge,\n  Label:      \"Labour\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sr.ResolutionLabourCost || 0\n});\n\nsr.Charges.push({\n  CustomerId: sr.ResolutionDelivererToCharge,\n  Label:      \"Additional\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost])\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceInternal,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsSumCatalog(sr)\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceInternal,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsSumNonCatalog(sr)\n});\n\nsr.Charges.push({\n  Label:      \"Labour\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceInternal,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sr.ResolutionLabourCost || 0\n});\n\nsr.Charges.push({\n  Label:      \"Additional\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceInternal,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost])\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceWarranty,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsSumCatalog(sr)\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceWarranty,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsSumNonCatalog(sr)\n});\n\nsr.Charges.push({\n  Label:      \"Labour\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceWarranty,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sr.ResolutionLabourCost || 0\n});\n\nsr.Charges.push({\n  Label:      \"Additional\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceWarranty,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost])\n});"
    },
    {
      "expression": "addTax(this);"
    },
    {
      "expression": "var sr = this.serviceRequest,\n    matrix = this.MasterData.SupplierCostMatrix,\n    account = this.MasterData.ServiceSupplierAccount[sr.ResolutionSupplierToCharge];\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: \"Supplier\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsChargeWithSupplierMatrix(sr, not(partNumberIsEmpty), matrix).covered\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: \"Supplier\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsChargeWithSupplierMatrix(sr, partNumberIsEmpty, matrix).covered\n});\n\nsr.Charges.push({\n  Label:      \"Labour and Additional\",\n  ChargeType: \"Supplier\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      labourSupplierCovered(sr, sum([sr.ResolutionLabourCost, sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]), matrix)\n});"
    },
    {
      "expression": "var sr = this.serviceRequest,\n    matrix = this.MasterData.SupplierCostMatrix,\n    account = this.MasterData.Settings.ServiceFyw,\n    chargeType = ''Unicomer Warranty'';\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: chargeType,\n  Account:    account ,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsChargeWithSupplierMatrix(sr, not(partNumberIsEmpty), matrix).remaining\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: chargeType,\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsChargeWithSupplierMatrix(sr, partNumberIsEmpty, matrix).remaining\n});\n\nsr.Charges.push({\n  Label:      \"Labour and Additional\",\n  ChargeType: chargeType,\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      labourSupplierRemaining(sr, sum([sr.ResolutionLabourCost, sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]), matrix)\n});"
    },
    {
      "expression": "var sr = this.serviceRequest,\n    matrix = this.MasterData.SupplierCostMatrix,\n    account = this.MasterData.Settings.ServiceWarranty;\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: \"EW\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsChargeWithSupplierMatrix(sr, not(partNumberIsEmpty), matrix).remaining\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: \"EW\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsChargeWithSupplierMatrix(sr, partNumberIsEmpty, matrix).remaining\n});\n\nsr.Charges.push({\n  Label:      \"Labour and Additional\",\n  ChargeType: \"EW\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      labourSupplierRemaining(sr, sum([sr.ResolutionLabourCost, sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]), matrix)\n});"
    },
    {
      "expression": "var sr = this.serviceRequest,\n    matrix = this.MasterData.SupplierCostMatrix;\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: \"Customer\",\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsChargeWithSupplierMatrix(sr, not(partNumberIsEmpty), matrix).remaining\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: \"Customer\",\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsChargeWithSupplierMatrix(sr, partNumberIsEmpty, matrix).remaining\n});\n\nsr.Charges.push({\n  Label:      \"Labour and Additional\",\n  ChargeType: \"Customer\",\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      labourSupplierRemaining(sr, sum([sr.ResolutionLabourCost, sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]), matrix)\n});"
    },
    {
      "expression": "this.OutstandingBalance = 0;"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar berMarkup = sr.StockItem.CostPrice * this.MasterData.Settings.BerMarkup/100;\nvar berCost = (sr.StockItem.CostPrice + berMarkup).toFixed(2);\nsr.Charges.push({\n    Label: ''Additional'',\n    ChargeType: sr.ResolutionPrimaryCharge,\n    Account: this.MasterData.Settings.ServiceInternal,\n    ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n    Value: berCost\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar berCost = calculateBerCostExtendedWarranty(this);\nsr.Charges.push({\n    Label: ''Additional'',\n    ChargeType: sr.ResolutionPrimaryCharge,\n    Account: this.MasterData.Settings.ServiceInternal,\n    ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n    Value: berCost\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar charges = _.filter(sr.Charges, function (c) {\n  return c.ChargeType === \"Customer\" || c.ChargeType === \"Deliverer\"\n});\n\nthis.OutstandingBalance = sum(charges, function (c) { return c.Value; }) - (sr.PaymentBalance || 0);"
    },
    {
      "expression": "var sr = this.serviceRequest,\nmatrix = this.MasterData.SupplierCostMatrix,\naccount = this.MasterData.Settings.InstallationElectricalAccount;\n\nsr.Charges.push({\nLabel: ''PartsCosacs'',\nChargeType: ''InstallationElectrical'',\nAccount: account,\nItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\nValue: partsSumCatalog(sr)\n});\n\nsr.Charges.push({\nLabel: ''PartsOther'',\nChargeType: ''InstallationElectrical'',\nAccount: account,\nItemNo: this.MasterData.Settings.ServiceItemPartsOther,\nValue: partsSumNonCatalog(sr)\n});\n\nsr.Charges.push({\nLabel: ''LabourandAdditional'',\nChargeType: ''InstallationElectrical'',\nAccount: account,\nItemNo: this.MasterData.Settings.ServiceItemLabour,\nValue: labourSupplierRemaining(sr,\n\tsum([sr.ResolutionLabourCost,\n\tsr.ResolutionAdditionalCost,\n\tsr.ResolutionTransportCost]),\n\tmatrix)\n});"
    },
    {
      "expression": "var sr = this.serviceRequest,\nmatrix = this.MasterData.SupplierCostMatrix,\naccount = this.MasterData.Settings.InstallationFurnitureAccount;\n\nsr.Charges.push({\nLabel: ''PartsCosacs'',\nChargeType: ''InstallationFurniture'',\nAccount: account,\nItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\nValue: partsSumCatalog(sr)\n});\n\nsr.Charges.push({\nLabel: ''PartsOther'',\nChargeType: ''InstallationFurniture'',\nAccount: account,\nItemNo: this.MasterData.Settings.ServiceItemPartsOther,\nValue: partsSumNonCatalog(sr)\n});\n\nsr.Charges.push({\nLabel: ''LabourandAdditional'',\nChargeType: ''InstallationFurniture'',\nAccount: account,\nItemNo: this.MasterData.Settings.ServiceItemLabour,\nValue: labourSupplierRemaining(sr,\n\tsum([sr.ResolutionLabourCost,\n\tsr.ResolutionAdditionalCost,\n\tsr.ResolutionTransportCost]),\n\tmatrix)\n\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar chargeType = ''Unicomer Warranty'';\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: chargeType,\n  Account:    this.MasterData.Settings.ServiceFyw,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsSumCatalog(sr)\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: chargeType,\n  Account:    this.MasterData.Settings.ServiceFyw,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsSumNonCatalog(sr)\n});\n\nsr.Charges.push({\n  Label:      \"Labour\",\n  ChargeType: chargeType,\n  Account:    this.MasterData.Settings.ServiceFyw,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sr.ResolutionLabourCost || 0\n});\n\nsr.Charges.push({\n  Label:      \"Additional\",\n  ChargeType: chargeType,\n  Account:    this.MasterData.Settings.ServiceFyw,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost])\n});"
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
        false,
        false,
        false,
        false,
        true,
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
        ""
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
        false,
        false,
        false,
        false,
        true,
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
        ""
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
        false,
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
        true,
        false,
        false,
        false,
        true,
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
        "false",
        "true"
      ],
      "actions": [
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
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
        "Supplier",
        null,
        "false",
        "true",
        "true"
      ],
      "actions": [
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
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
        "Installation Charge Furniture",
        null,
        null,
        null,
        null
      ],
      "actions": [
        true,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        true,
        false
      ]
    },
    {
      "values": [
        "Installation Charge Electrical",
        null,
        null,
        null,
        null
      ],
      "actions": [
        true,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        true,
        false,
        false
      ]
    },
    {
      "values": [
        "Unicomer Warranty",
        null,
        null,
        null,
        null
      ],
      "actions": [
        true,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        true,
        false,
        false,
        false,
        false,
        false,
        true
      ]
    }
  ],
  "extensions": "// this.MasterData.Settings.TaxType: Stock includes tax or not.(I or E)\nvar addMonths = function (d, months) {\n  d = new Date(+d);\n  d.setMonth(d.getMonth() + months);\n  return d;\n};\n\nvar insideWarranty = function (sr, months) {\n  return addMonths(sr.ItemDeliveredOn, months) >= new Date()\n};\n\nvar sum = function (list, f) {\n  f = f || function(v) { return v; };\n  return _.reduce(list, function(memo, e){ return memo + (f(e) || 0); }, 0);\n};\n\nvar empty = function (s) {\n    return !s;\n};\n\nvar iif = function (expr, whenTrue, whenFalse) {\n    if (expr) {\n        return whenTrue;\n    } else {\n        return whenFalse;\n    }\t\t\n};\n\nvar not = function(f) {\n    return function(s) {\n        return !f(s);\n    };\n};\n\nvar partNumberIsEmpty = function (part) {\n    return empty(part.number);\n};\n\nvar partTotalPrice = function (part) {\n    return part.quantity * part.price;\n};\n\nvar findCost = function (sr, part, matrix) {\n  return _.find(matrix, function (row) {\n    return row.partType === part.type && insideWarranty(sr, row.year * 12);\n  });\n};\n\n// part cost covered by supplier\nvar partSupplierCovered = function (sr, part, matrix, price) {\n  var cost = findCost(sr, part, matrix);\n  if (cost) {\n    if (cost.partPcent) {\n      price = price * cost.partPcent / 100;\n    } \n    if (cost.partVal) {\n      price =  Math.min(price, cost.partVal);\n    }\n    return price;\n  } \n  return 0;\n};\n\nvar labourSupplierCovered = function (sr, labour, matrix) {\n  if (matrix == null) {\n    return 0;\n  }\n  return _.max(_.map(matrix, function (row) {\n    if (insideWarranty(sr, row.year * 12) && (row.labourPcent || row.labourVal)) {\n      var v = labour;\n      if (row.labourPcent) {\n        v = v * row.labourPcent / 100;\n      }\n      if (row.labourVal) {\n        v = Math.min(v, row.labourVal);\n      }\n      return v;\n    }\n    return 0;\n  })) || 0;\n};\n\nvar labourSupplierRemaining = function (sr, labour, matrix) {\n  var v = labourSupplierCovered(sr, labour, matrix) || 0;\n  return labour - v;\n};\n\nvar partsSumCatalog = function (sr) {\n  return sum(_.filter(sr.Parts, not(partNumberIsEmpty)), partTotalPrice);\n};\n\nvar partsSumNonCatalog = function (sr) {\n  return sum(_.filter(sr.Parts, partNumberIsEmpty), partTotalPrice);\n};\n\nvar partsChargeWithSupplierMatrix = function (sr, filter, matrix) {\n  var partSplit = _.map(_.filter(sr.Parts, filter), function (part) {\n    var price = partTotalPrice(part),\n        covered = partSupplierCovered(sr, part, matrix, price);\n    return {\n      covered: covered,\n      remaining: price - covered\n    };\n  });\n  return { \n    covered: sum(partSplit, function (s) { return s.covered; }),\n    remaining: sum(partSplit, function (s) { return s.remaining; })\n  };  \n};\n\nvar addTax = function (scope) {\n  _.each (scope.serviceRequest.Charges, function (c) {\n    c.Tax = (c.Value * scope.MasterData.Settings.TaxRate / 100.0).toFixed(2);\n  });\n};\n\nvar calculateBerCostExtendedWarranty = function(scope) {\n  var sr = scope.serviceRequest;\n  var berMarkup = sr.StockItem.CostPrice * scope.MasterData.Settings.BerMarkup / 100;\n  var berCost = (sr.StockItem.CostPrice + berMarkup - scope.previousEWCost).toFixed(2);\n  if (berCost < 0) {\n  berCost = 0;\n  }\n\n  return berCost;\n};"
}')

INSERT INTO Config.DecisionTable
        ( [Key], CreatedUtc, Value )
VALUES  ( 'SR.DecisionTable.ChargeToAuthorisation', -- Key - varchar(50)
Getdate(), -- CreatedUtc - datetime
'{
  "conditions": [
    {
      "expression": "this.serviceRequest.Resolution"
    },
    {
      "expression": "\"Inside Supplier Warranty\" &&\nresolutionValidForWarranty(this.serviceRequest.Resolution) &&\nthis.serviceRequest.WarrantyNumber &&\ninsideWarranty(this.serviceRequest, this.serviceRequest.ManWarrantyLength || 12)"
    },
    {
      "expression": "\"Inside Extended Warranty\" &&\nresolutionValidForWarranty(this.serviceRequest.Resolution) &&\nthis.serviceRequest.WarrantyNumber &&\ninsideWarranty(this.serviceRequest, this.serviceRequest.WarrantyLength || 0)"
    },
    {
      "expression": "isServiceRequest(this.serviceRequest.Type);"
    },
    {
      "expression": "\"Internal installation\" && (this.serviceRequest.Type === \"II\" || this.srType === \"II\")"
    },
    {
      "expression": "\"External installation\" && (this.serviceRequest.Type === \"IE\" || this.srType === \"IE\")"
    }
  ],
  "actions": [
    {
      "expression": "chargeTos(this, [{\n\tkey: ''Customer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Deliverer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Internal'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Supplier'',\n\tauthorisationRequired: false\n}]);\n\nsetDefaultPrimaryCharge(''Customer'', this.serviceRequest);"
    },
    {
      "expression": "chargeTos(this, [{\n\tkey: ''Customer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Deliverer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Internal'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Supplier'',\n\tauthorisationRequired: false\n}]);\n\nsetDefaultPrimaryCharge(''Deliverer'', this.serviceRequest);"
    },
    {
      "expression": "chargeTos(this, [{\n\tkey: ''Customer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Deliverer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Internal'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Supplier'',\n\tauthorisationRequired: false\n}]);\n\nsetDefaultPrimaryCharge(''Internal'', this.serviceRequest);"
    },
    {
      "expression": "chargeTos(this, [{\n\tkey: ''Customer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Deliverer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Internal'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Supplier'',\n\tauthorisationRequired: true\n}]);\n\nsetDefaultPrimaryCharge(''Customer'', this.serviceRequest);"
    },
    {
      "expression": "chargeTos(this, [{\n\tkey: ''Customer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Deliverer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Unicomer Warranty'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Internal'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Supplier'',\n\tauthorisationRequired: true\n}]);\n\nsetDefaultPrimaryCharge(''Supplier'', this.serviceRequest);"
    },
    {
      "expression": "chargeTos(this, [{\n\tkey: ''Customer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Deliverer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''EW'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Internal'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Supplier'',\n\tauthorisationRequired: false\n}]);\n\nsetDefaultPrimaryCharge(''EW'', this.serviceRequest);"
    },
    {
      "expression": "chargeTos(this, [{\n\tkey: ''Customer'',\n\tauthorisationRequired: true\n}, {\n\tkey: ''Installation Charge Electrical'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Installation Charge Furniture'',\n\tauthorisationRequired: false\n}]);\n\nsetDefaultPrimaryCharge(''Installation Charge Electrical'', this.serviceRequest);"
    },
    {
      "expression": "chargeTos(this, [{\n\tkey: ''Customer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Installation Charge Electrical'',\n\tauthorisationRequired: true\n}, {\n\tkey: ''Installation Charge Furniture'',\n\tauthorisationRequired: true\n}]);\n\nsetDefaultPrimaryCharge(''Customer'', this.serviceRequest);"
    }
  ],
  "rules": [
    {
      "values": [
        "",
        null,
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
        false,
        false,
        false
      ]
    },
    {
      "values": [
        "Damage On Delivery",
        null,
        null,
        "true",
        null,
        null
      ],
      "actions": [
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
        "Beyond Economic Repair",
        null,
        null,
        "true",
        null,
        null
      ],
      "actions": [
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
        "Misuse by the customer",
        null,
        null,
        "true",
        null,
        null
      ],
      "actions": [
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
        "Event or terms NOT covered",
        null,
        null,
        "true",
        null,
        null
      ],
      "actions": [
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
        null,
        "true",
        "",
        "true",
        null,
        null
      ],
      "actions": [
        false,
        false,
        false,
        false,
        true,
        false,
        false,
        false
      ]
    },
    {
      "values": [
        null,
        "false",
        "true",
        "true",
        null,
        null
      ],
      "actions": [
        false,
        false,
        false,
        false,
        false,
        true,
        false,
        false
      ]
    },
    {
      "values": [
        null,
        null,
        null,
        null,
        "false",
        "false"
      ],
      "actions": [
        false,
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
        null,
        null,
        null,
        null,
        "true",
        null
      ],
      "actions": [
        false,
        false,
        false,
        false,
        false,
        false,
        true,
        false
      ]
    },
    {
      "values": [
        null,
        null,
        null,
        null,
        "",
        "true"
      ],
      "actions": [
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        true
      ]
    }
  ],
  "extensions": "var addMonths = function (d, months) {\n  d = new Date(+d);\n  d.setMonth(d.getMonth() + months);\n  return d;\n};\n\nvar insideWarranty = function (sr, months) {\n  return addMonths(sr.ItemDeliveredOn, months) >= new Date();\n};\n\nvar chargeTos = function(scope, list) {\n scope.MasterData.ServiceChargeTos = list;\n};\n\nvar resolutionValidForWarranty = function (resolution) {\n  return resolution !== ''Damage On Delivery'' && resolution !== ''Beyond Economic Repair'' && resolution !== ''Misuse by the customer'' && resolution !== ''Event or terms NOT covered'';\n};\n\nvar isServiceRequest = function (srType) {\n  return srType === ''SI'' || srType === ''SE'' || srType === ''S'';\n};\n\nvar isInstallationRequest = function (srType) {\n  return srType === ''II'' || srType === ''IE'';\n};\n\nvar isPrimaryChargeNotSet = function (resolutionPrimaryCharge) {\n  return resolutionPrimaryCharge === undefined || resolutionPrimaryCharge === null || resolutionPrimaryCharge === '''';\n};\n\nvar setDefaultPrimaryCharge = function (charge, serviceRequest) {\n  if (isPrimaryChargeNotSet(serviceRequest.ResolutionPrimaryCharge)) {\n    serviceRequest.ResolutionPrimaryCharge = charge;\n  }\n};"
}')