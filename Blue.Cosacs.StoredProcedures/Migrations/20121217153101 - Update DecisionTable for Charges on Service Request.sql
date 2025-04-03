-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

insert into Config.DecisionTable ("Key", CreatedUtc, Value) values
('SR.DecisionTable.Charge', GETDATE(), 
'{
  "conditions": [
    {
      "expression": "this.serviceRequest.ResolutionPrimaryCharge"
    },
    {
      "expression": "this.MasterData.Settings.TaxType"
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
    }
  ],
  "rules": [
    {
      "values": [
        "Customer",
        null
      ],
      "actions": [
        true,
        true,
        false,
        false,
        false
      ]
    },
    {
      "values": [
        "Internal",
        null
      ],
      "actions": [
        true,
        false,
        true,
        false,
        false
      ]
    },
    {
      "values": [
        "Deliverer",
        null
      ],
      "actions": [
        true,
        true,
        false,
        false,
        false
      ]
    },
    {
      "values": [
        "EW",
        null
      ],
      "actions": [
        true,
        false,
        false,
        true,
        false
      ]
    },
    {
      "values": [
        "Supplier",
        null
      ],
      "actions": [
        true,
        false,
        false,
        false,
        false
      ]
    },
    {
      "values": [
        "",
        "E"
      ],
      "actions": [
        false,
        false,
        false,
        false,
        true
      ]
    }
  ],
  "extensions": "// this.MasterData.Settings.TaxType: Stock includes tax or not.(I or E)\n\nvar sum = function (list, f) {\n    return _.reduce(list, function(memo, e){ return memo + f(e); }, 0);\n};\n\nvar empty = function (s) {\n    return !s;\n};\n\nvar iif = function (expr, whenTrue, whenFalse) {\n    if (expr) {\n        return whenTrue;\n    } else {\n        return whenFalse;\n    }\t\t\n};\n\nvar not = function(f) {\n    return function(s) {\n        return !f(s);\n    };\n};\n\nvar yearsSold = function() {\n    var yearMs = 365.25 * 24 * 60 * 60 * 1000;\n    return (new Date() - this.serviceRequest.ItemSoldOn) / yearMs;\n};\n\nvar partNumberIsEmpty = function (part) {\n    return empty(part.number);\n};\n\nvar partTotalPrice = function (part) {\n    return part.quantity * part.price;\n};\n\nvar partMatrixCost = function (part) {\n    var year = yearsSold();\n    var costs = _.filter(this.SupplierCostMatrix, function (cost) {\n        return cost.PartType === part.type && cost.Year >= year;\n    });\n};\n\nvar partsSumCatalog = function (sr) {\n  return sum(_.filter(sr.Parts, not(partNumberIsEmpty)), partTotalPrice);\n};\n\nvar partsSumNonCatalog = function (sr) {\n  return sum(_.filter(sr.Parts, partNumberIsEmpty), partTotalPrice);\n};\n\nvar addTax = function (scope) {\n  _.each (scope.serviceRequest.Charges, function (c) {\n    c.Tax = (c.Value * scope.MasterData.Settings.TaxRate / 100.0).toFixed(2);\n  });\n};"
}')