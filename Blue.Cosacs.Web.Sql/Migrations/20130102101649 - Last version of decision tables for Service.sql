-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT [Config].[DecisionTable] ([Key], [CreatedUtc], [Value]) 
values ('SR.DecisionTable.Charge', getdate(), 
'{
  "conditions": [
    {
      "expression": "this.serviceRequest.ResolutionPrimaryCharge"
    },
    {
      "expression": "this.MasterData.Settings.TaxType"
    },
    {
      "expression": "\"Inside Supplier Warranty\" &&\ninsideWarranty(this.serviceRequest, this.serviceRequest.ManWarrantyLength || 12)"
    },
    {
      "expression": "\"Inside Extended Warranty\" &&\n\nthis.serviceRequest.WarrantyContractId &&\ninsideWarranty(this.serviceRequest, this.serviceRequest.WarrantyLength || 0)"
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
      "expression": "var sr = this.serviceRequest,\n    matrix = this.MasterData.SupplierCostMatrix,\n    account = this.MasterData.Settings.ServiceInternal;\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: \"Internal\",\n  Account:    account ,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsChargeWithSupplierMatrix(sr, not(partNumberIsEmpty), matrix).remaining\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: \"Internal\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsChargeWithSupplierMatrix(sr, partNumberIsEmpty, matrix).remaining\n});\n\nsr.Charges.push({\n  Label:      \"Labour and Additional\",\n  ChargeType: \"Internal\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      labourSupplierRemaining(sr, sum([sr.ResolutionLabourCost, sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]), matrix)\n});"
    },
    {
      "expression": "var sr = this.serviceRequest,\n    matrix = this.MasterData.SupplierCostMatrix,\n    account = this.MasterData.Settings.ServiceWarranty;\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: \"EW\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsChargeWithSupplierMatrix(sr, not(partNumberIsEmpty), matrix).remaining\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: \"EW\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsChargeWithSupplierMatrix(sr, partNumberIsEmpty, matrix).remaining\n});\n\nsr.Charges.push({\n  Label:      \"Labour and Additional\",\n  ChargeType: \"EW\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      labourSupplierRemaining(sr, sum([sr.ResolutionLabourCost, sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]), matrix)\n});"
    },
    {
      "expression": "var sr = this.serviceRequest,\n    matrix = this.MasterData.SupplierCostMatrix;\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: \"Customer\",\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsChargeWithSupplierMatrix(sr, not(partNumberIsEmpty), matrix).remaining\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: \"Customer\",\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsChargeWithSupplierMatrix(sr, partNumberIsEmpty, matrix).remaining\n});\n\nsr.Charges.push({\n  Label:      \"Labour and Additional\",\n  ChargeType: \"Customer\",\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      labourSupplierRemaining(sr, sum([sr.ResolutionLabourCost, sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]), matrix)\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\nthis.OutstandingBalance = sum(sr.Charges, function (c) { return c.Value; });"
    },
    {
      "expression": "this.OutstandingBalance = 0;"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar charges = _.filter(sr.Charges, function (c) {\n  return c.ChargeType === \"Customer\" || c.ChargeType === \"Deliverer\"\n});\n\nthis.OutstandingBalance = sum(charges, function (c) { return c.Value; }) - (sr.PaymentBalance || 0);"
    }
  ],
  "rules": [
    {
      "values": [
        "Customer",
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
        true,
        false,
        true
      ]
    },
    {
      "values": [
        "Internal",
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
        false,
        false,
        true,
        false
      ]
    },
    {
      "values": [
        "Deliverer",
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
        true,
        false,
        true
      ]
    },
    {
      "values": [
        "EW",
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
        false,
        false,
        true,
        false
      ]
    },
    {
      "values": [
        "Customer",
        "E",
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
        false
      ]
    },
    {
      "values": [
        "Supplier",
        null,
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
        true,
        false,
        false,
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
        "true"
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
        "false"
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
        true
      ]
    }
  ],
  "extensions": "// this.MasterData.Settings.TaxType: Stock includes tax or not.(I or E)\nvar addMonths = function (d, months) {\n  d = new Date(+d);\n  d.setMonth(d.getMonth() + months);\n  return d;\n};\n\nvar insideWarranty = function (sr, months) {\n  return addMonths(sr.ItemDeliveredOn, months) >= new Date()\n};\n\nvar sum = function (list, f) {\n  f = f || function(v) { return v; };\n  return _.reduce(list, function(memo, e){ return memo + (f(e) || 0); }, 0);\n};\n\nvar empty = function (s) {\n    return !s;\n};\n\nvar iif = function (expr, whenTrue, whenFalse) {\n    if (expr) {\n        return whenTrue;\n    } else {\n        return whenFalse;\n    }\t\t\n};\n\nvar not = function(f) {\n    return function(s) {\n        return !f(s);\n    };\n};\n\nvar partNumberIsEmpty = function (part) {\n    return empty(part.number);\n};\n\nvar partTotalPrice = function (part) {\n    return part.quantity * part.price;\n};\n\nvar findCost = function (sr, part, matrix) {\n  return _.find(matrix, function (row) {\n    return row.partType === part.type && insideWarranty(sr, row.year * 12);\n  });\n};\n\n// part cost covered by supplier\nvar partSupplierCovered = function (sr, part, matrix, price) {\n  var cost = findCost(sr, part, matrix);\n  if (cost) {\n    if (cost.partPcent) {\n      price = price * cost.partPcent / 100;\n    } \n    if (cost.partVal) {\n      price =  Math.min(price, cost.partVal);\n    }\n    return price;\n  } \n  return 0;\n};\n\nvar labourSupplierCovered = function (sr, labour, matrix) {\n  if (matrix == null) {\n    return 0;\n  }\n  return _.max(_.map(matrix, function (row) {\n    if (insideWarranty(sr, row.year * 12) && (row.labourPcent || row.labourVal)) {\n      var v = labour;\n      if (row.labourPcent) {\n        v = v * row.labourPcent / 100;\n      }\n      if (row.labourVal) {\n        v = Math.min(v, row.labourVal);\n      }\n      return v;\n    }\n    return 0;\n  })) || 0;\n};\n\nvar labourSupplierRemaining = function (sr, labour, matrix) {\n  var v = labourSupplierCovered(sr, labour, matrix) || 0;\n  return labour - v;\n};\n\nvar partsSumCatalog = function (sr) {\n  return sum(_.filter(sr.Parts, not(partNumberIsEmpty)), partTotalPrice);\n};\n\nvar partsSumNonCatalog = function (sr) {\n  return sum(_.filter(sr.Parts, partNumberIsEmpty), partTotalPrice);\n};\n\nvar partsChargeWithSupplierMatrix = function (sr, filter, matrix) {\n  var partSplit = _.map(_.filter(sr.Parts, filter), function (part) {\n    var price = partTotalPrice(part),\n        covered = partSupplierCovered(sr, part, matrix, price);\n    return {\n      covered: covered,\n      remaining: price - covered\n    };\n  });\n  return { \n    covered: sum(partSplit, function (s) { return s.covered; }),\n    remaining: sum(partSplit, function (s) { return s.remaining; })\n  };  \n};\n\nvar addTax = function (scope) {\n  _.each (scope.serviceRequest.Charges, function (c) {\n    c.Tax = (c.Value * scope.MasterData.Settings.TaxRate / 100.0).toFixed(2);\n  });\n};"
}')

go

INSERT [Config].[DecisionTable] ([Key], [CreatedUtc], [Value]) 
values ('SR.DecisionTable.Payment', getdate(), 
'{
  "conditions": [
    {
      "expression": "lastDigit(this.payMethod) === ''2'' && parseInt(this.payMethod) < 100"
    },
    {
      "expression": "lastDigit(this.payMethod) === ''3'' || lastDigit(this.payMethod) === ''4'' || lastDigit(this.payMethod) === ''9''"
    },
    {
      "expression": "parseInt(this.payMethod) === 100"
    },
    {
      "expression": "lastDigit(this.payMethod) === ''1''"
    },
    {
      "expression": "parseInt(this.payMethod) === 102"
    },
    {
      "expression": "lastDigit(this.payMethod) === ''5''"
    },
    {
      "expression": "lastDigit(this.payMethod) === ''7''"
    },
    {
      "expression": "lastDigit(this.payMethod) === ''8''"
    }
  ],
  "actions": [
    {
      "expression": "this.sections.bank.visible = true"
    },
    {
      "expression": "this.sections.bank.visible = false"
    },
    {
      "expression": "this.sections.cardType.visible = true"
    },
    {
      "expression": "this.sections.cardType.visible = false"
    },
    {
      "expression": "this.sections.cardNumber.visible = true"
    },
    {
      "expression": "this.sections.cardNumber.visible = false"
    },
    {
      "expression": "this.sections.bankAccountNumber.visible = true"
    },
    {
      "expression": "this.sections.bankAccountNumber.visible = false"
    },
    {
      "expression": "this.sections.amountToPay.visible = true"
    },
    {
      "expression": "this.sections.amountToPay.visible = false"
    },
    {
      "expression": "this.sections.tendered.visible = true"
    },
    {
      "expression": "this.sections.tendered.visible = false"
    },
    {
      "expression": "this.sections.change.visible = true"
    },
    {
      "expression": "this.sections.change.visible = false"
    },
    {
      "expression": "this.sections.chequeNumber.visible = true"
    },
    {
      "expression": "this.sections.chequeNumber.visible = false"
    }
  ],
  "rules": [
    {
      "values": [
        "true",
        null,
        null,
        null,
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
        true,
        false,
        true,
        true,
        false,
        true,
        false,
        false,
        true,
        true,
        false
      ]
    },
    {
      "values": [
        "",
        "true",
        null,
        null,
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
        true,
        false,
        false,
        true,
        true,
        false,
        true,
        false,
        false,
        true,
        false,
        true
      ]
    },
    {
      "values": [
        null,
        null,
        "true",
        null,
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
        true,
        false,
        false,
        true,
        true,
        false,
        true,
        false,
        true,
        false,
        false,
        true
      ]
    },
    {
      "values": [
        null,
        null,
        null,
        "true",
        null,
        null,
        null,
        null
      ],
      "actions": [
        false,
        true,
        false,
        true,
        false,
        true,
        false,
        true,
        true,
        false,
        true,
        false,
        true,
        false,
        false,
        true
      ]
    },
    {
      "values": [
        null,
        null,
        null,
        null,
        "true",
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
        true,
        false,
        true,
        true,
        false,
        true,
        false,
        false,
        true,
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
        null,
        "true",
        null,
        null
      ],
      "actions": [
        true,
        false,
        false,
        true,
        false,
        true,
        true,
        false,
        true,
        false,
        true,
        false,
        false,
        true,
        false,
        true
      ]
    },
    {
      "values": [
        null,
        null,
        null,
        null,
        null,
        null,
        "true",
        null
      ],
      "actions": [
        true,
        false,
        false,
        true,
        false,
        true,
        false,
        true,
        true,
        false,
        true,
        false,
        false,
        true,
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
        null,
        null,
        null,
        "true"
      ],
      "actions": [
        true,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
        true,
        false
      ]
    }
  ],
  "extensions": "var lastDigit = function(value) {\n  if (value) {\n    value = value.toString();\n    return value.substring(value.length - 1, value.length);\n  }\n  return null;\n};"
}')

go

INSERT [Config].[DecisionTable] ([Key], [CreatedUtc], [Value]) 
values ('SR.DecisionTable.ServiceStatus', getdate(), 
'{
  "conditions": [
    {
      "expression": "\"Estimate section is completed\"&& !isEmpty(this.serviceRequest.EstimateDateRecieved)&& dateIsValid(this.serviceRequest.EstimateDateRecieved)&& !isEmpty(this.serviceRequest.EstimateLabourCost)&& !isEmpty(this.serviceRequest.EstimateAdditionalLabourCost)&& !isEmpty(this.serviceRequest.EstimateTransportCost)"
    },
    {
      "expression": "\"Evaluation section is completed\"&& !isEmpty(this.serviceRequest.EvaluationLocation)&& !isEmpty(this.serviceRequest.EvaluationAction)&& !isEmpty(this.serviceRequest.EvaluationClaimFoodLoss)&& !isEmpty(this.serviceRequest.ScriptAnswer)"
    },
    {
      "expression": "\"Allocation section is completed\"&&\n!isEmpty(this.serviceRequest.AllocationItemReceivedOn)&& dateIsValid(this.serviceRequest.AllocationItemReceivedOn)&& !isEmpty(this.serviceRequest.AllocationServiceScheduledOn)&& dateIsValid(this.serviceRequest.AllocationServiceScheduledOn)&& !isEmpty(this.serviceRequest.AllocationTechnician)"
    },
    {
      "expression": "\"parts date is in the future\" && !isEmpty(this.serviceRequest.AllocationPartExpectOn)&& dateIsValid(this.serviceRequest.AllocationPartExpectOn)&&\nthis.serviceRequest.AllocationPartExpectOn >= (new Date())"
    },
    {
      "expression": "\"allocation date is in the future\" && this.serviceRequest.AllocationServiceScheduledOn!==null &&\ndateIsValid(this.serviceRequest.AllocationServiceScheduledOn)"
    },
    {
      "expression": "\"Resolution section is completed\" &&\n!isEmpty(this.serviceRequest.ResolutionDate)&&\ndateIsValid(this.serviceRequest.ResolutionDate)&&\n!isEmpty(this.serviceRequest.ResolutionPrimaryCharge)&&\n\n!isEmpty(this.serviceRequest.ResolutionLabourCost)&&\n!isEmpty(this.serviceRequest.ResolutionAdditionalCost)&&\n!isEmpty(this.serviceRequest.ResolutionTransportCost)&&\n\n(this.serviceRequest.ResolutionPrimaryCharge !== ''Supplier'' || this.serviceRequest.ResolutionPrimaryCharge === ''Supplier'' && !isEmpty(this.serviceRequest.ResolutionCategory))"
    },
    {
      "expression": "\"Finalize section is completed\"&& !isEmpty(this.serviceRequest.FinalisedFailure)&& dateIsValid(this.serviceRequest.FinaliseReturnDate)"
    },
    {
      "expression": "isEmpty(this.serviceRequest.State)"
    },
    {
      "expression": "this.OutstandingBalance > 0"
    }
  ],
  "actions": [
    {
      "expression": "this.serviceRequest.State = ''New''"
    },
    {
      "expression": "this.serviceRequest.State = ''Awaiting estimate''"
    },
    {
      "expression": "this.serviceRequest.State = ''Awaiting allocation''"
    },
    {
      "expression": "this.serviceRequest.State = ''Estimate overdue''"
    },
    {
      "expression": "this.serviceRequest.State = ''Awaiting deposit''"
    },
    {
      "expression": "this.serviceRequest.State = ''Awaiting spare parts''"
    },
    {
      "expression": "this.serviceRequest.State = ''Awaiting repair''"
    },
    {
      "expression": "this.serviceRequest.State = ''Repair overdue''"
    },
    {
      "expression": "this.serviceRequest.State = ''Resolved''"
    },
    {
      "expression": "this.serviceRequest.State = ''Awaiting payment''"
    },
    {
      "expression": "this.serviceRequest.State = ''Closed''"
    },
    {
      "expression": "this.serviceRequest.IsClosed = true"
    }
  ],
  "rules": [
    {
      "values": [
        "false",
        "false",
        "false",
        "false",
        "false",
        "false",
        "false",
        "",
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
        false
      ]
    },
    {
      "values": [
        "",
        "true",
        "false",
        "false",
        "false",
        "false",
        "false",
        "",
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
        false,
        false,
        false,
        false,
        false
      ]
    },
    {
      "values": [
        "",
        "",
        "true",
        "true",
        null,
        null,
        null,
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
        false,
        false,
        false,
        false,
        false
      ]
    },
    {
      "values": [
        "",
        "",
        "true",
        "false",
        "true",
        null,
        null,
        null,
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
        false,
        false,
        false,
        false,
        false
      ]
    },
    {
      "values": [
        "",
        "",
        "true",
        "",
        "false",
        "false",
        "false",
        null,
        null
      ],
      "actions": [
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
        "",
        "",
        "",
        "",
        "",
        "true",
        "false",
        null,
        null
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
        true,
        false,
        false,
        false
      ]
    },
    {
      "values": [
        "",
        "",
        "",
        "",
        "",
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
        false,
        false,
        false,
        false,
        false,
        false,
        true,
        true
      ]
    },
    {
      "values": [
        null,
        null,
        null,
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
        null,
        null,
        null,
        null,
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
        true,
        false,
        false
      ]
    }
  ],
  "extensions": "var isEmpty = function(val) {\n    return typeof val === ''undefined'' ||\n                   val === null ||\n                   val === \"\" ||\n                   (typeof val.length === ''undefined'' && val.length === 0);\n};\nvar dateIsValid = function(val) {\n    return window.moment(val).isValid();\n};"
}')

go

INSERT [Config].[DecisionTable] ([Key], [CreatedUtc], [Value]) 
values ('SR.DecisionTable.Workflow', getdate(), 
'{
  "conditions": [
    {
      "expression": "this.srType"
    },
    {
      "expression": "this.serviceRequest.Type"
    },
    {
      "expression": "!isEmpty(this.serviceRequest.WarrantyContractNumber)"
    },
    {
      "expression": "((new Date(+this.serviceRequest.ItemSoldOn +\n (new Date((365.25/12)*(24*3600*1000)) *\n (this.serviceRequest.WarrantyLength || 0) ))) >= (new Date()))"
    },
    {
      "expression": "this.serviceRequest.Id === 0"
    },
    {
      "expression": "this.CustomerSearch.Type === ''Invoice''"
    },
    {
      "expression": "this.OutstandingBalance > 0"
    },
    {
      "expression": "!this.OutstandingBalance"
    },
    {
      "expression": "this.serviceRequest.ResolutionPrimaryCharge"
    },
    {
      "expression": "this.serviceRequest.ResolutionPrimaryCharge === ''Deliverer'' || this.serviceRequest.ResolutionPrimaryCharge === ''Supplier''"
    }
  ],
  "actions": [
    {
      "expression": "this.sections.screen.visible = true"
    },
    {
      "expression": "this.sections.pageHeading.visible = false"
    },
    {
      "expression": "this.sections.searchSelectorInput.visible = true"
    },
    {
      "expression": "this.sections.searchSelectorInput.visible = false"
    },
    {
      "expression": "this.sections.customer.visible = true"
    },
    {
      "expression": "this.sections.customer.visible = false"
    },
    {
      "expression": "this.sections.evaluation.visible = true"
    },
    {
      "expression": "this.sections.evaluation.visible = false"
    },
    {
      "expression": "this.sections.estimate.visible = true"
    },
    {
      "expression": "this.sections.estimate.visible = false"
    },
    {
      "expression": "this.sections.product.enabled = true"
    },
    {
      "expression": "this.sections.product.enabled = false"
    },
    {
      "expression": "this.sections.product.stockLocationVisible = true"
    },
    {
      "expression": "this.sections.product.stockLocationVisible = false"
    },
    {
      "expression": "this.sections.warranty.visible = true"
    },
    {
      "expression": "this.sections.warranty.visible = false"
    },
    {
      "expression": "this.sections.showPrintInvoice.visible = true"
    },
    {
      "expression": "this.sections.showPrintInvoice.visible = false"
    },
    {
      "expression": "this.sections.foodLoss.visible = true"
    },
    {
      "expression": "this.sections.foodLoss.visible = false"
    },
    {
      "expression": "this.sections.customerDetail.visible = true"
    },
    {
      "expression": "this.sections.customerDetail.visible = false"
    },
    {
      "expression": "this.sections.outOfWarrantyCoverMessage.visible = true"
    },
    {
      "expression": "this.sections.outOfWarrantyCoverMessage.visible = false"
    },
    {
      "expression": "this.sections.formReferenceField.visible = true"
    },
    {
      "expression": "this.sections.formReferenceField.visible = false"
    },
    {
      "expression": "chargeTos(this, [''Customer'',''Deliverer'',''EW'',''Internal'',''Supplier'']);"
    },
    {
      "expression": "chargeTos(this, [''Customer'',''Deliverer'',''EW'',''Internal'',''Supplier'']);"
    },
    {
      "expression": "chargeTos(this, [''Customer'',''Deliverer'',''EW'',''Internal'',''Supplier'']);"
    },
    {
      "expression": "chargeTos(this, [''Customer'',''Deliverer'',''EW'',''Internal'',''Supplier'']);"
    },
    {
      "expression": "chargeTos(this, [''Internal'',''Deliverer'',''Supplier'']);"
    },
    {
      "expression": "this.sections.branchSearch.visible = true"
    },
    {
      "expression": "this.sections.branchSearch.visible = false"
    },
    {
      "expression": "this.sections.payment.visible = true"
    },
    {
      "expression": "this.sections.payment.visible = false"
    },
    {
      "expression": "this.sections.resolutionSection.supplierVisible = true;\nthis.sections.resolutionSection.delivererVisible = false;\nthis.sections.resolutionSection.productVisible = true;"
    },
    {
      "expression": "this.sections.resolutionSection.delivererVisible = true;\nthis.sections.resolutionSection.supplierVisible = false;\nthis.sections.resolutionSection.productVisible = false;"
    },
    {
      "expression": "this.sections.resolutionSection.supplierVisible = false;\nthis.sections.resolutionSection.delivererVisible = false;\nthis.sections.resolutionSection.productVisible = false;"
    }
  ],
  "rules": [
    {
      "values": [
        "SI",
        null,
        null,
        "",
        null,
        null,
        null,
        null,
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
        false,
        false,
        false,
        false,
        false
      ]
    },
    {
      "values": [
        "SE",
        null,
        null,
        null,
        null,
        null,
        null,
        null,
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
        false,
        false,
        false,
        false,
        false
      ]
    },
    {
      "values": [
        "II",
        "",
        null,
        null,
        null,
        null,
        null,
        null,
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
        false,
        false,
        false,
        false,
        false
      ]
    },
    {
      "values": [
        "IE",
        null,
        "",
        null,
        null,
        null,
        null,
        null,
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
        false,
        false,
        false,
        false,
        false
      ]
    },
    {
      "values": [
        "S",
        null,
        null,
        null,
        null,
        null,
        null,
        null,
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
        false,
        false,
        false,
        false,
        false
      ]
    },
    {
      "values": [
        "SI",
        "",
        "true",
        null,
        null,
        null,
        null,
        null,
        null,
        null
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
        false
      ]
    },
    {
      "values": [
        "SI",
        null,
        "",
        "true",
        null,
        null,
        null,
        null,
        null,
        null
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
        false
      ]
    },
    {
      "values": [
        null,
        "SI",
        null,
        null,
        null,
        null,
        null,
        null,
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
        true,
        false,
        true,
        false,
        false,
        true,
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
        false
      ]
    },
    {
      "values": [
        null,
        "SE",
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null
      ],
      "actions": [
        false,
        false,
        false,
        true,
        true,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
        false,
        false,
        true,
        false,
        false,
        true,
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
        "II",
        null,
        null,
        null,
        null,
        null,
        null,
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
        true,
        false,
        true,
        false,
        true,
        false,
        false,
        true,
        false,
        true,
        false,
        false,
        true,
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
        "IE",
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null
      ],
      "actions": [
        false,
        false,
        false,
        true,
        true,
        false,
        false,
        true,
        false,
        true,
        true,
        false,
        false,
        false,
        true,
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
        false
      ]
    },
    {
      "values": [
        null,
        "S",
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null
      ],
      "actions": [
        false,
        false,
        false,
        true,
        false,
        true,
        false,
        true,
        true,
        false,
        true,
        false,
        false,
        false,
        true,
        false,
        false,
        true,
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
        null,
        null,
        null,
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
        null,
        "true",
        null,
        null,
        null,
        null
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
        false
      ]
    },
    {
      "values": [
        null,
        null,
        null,
        null,
        null,
        null,
        "true",
        null,
        null,
        null
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
        null,
        null,
        null,
        null,
        null,
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
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        "Deliverer",
        null
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
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        "Supplier",
        null
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
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
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
        false,
        false,
        false,
        false,
        true
      ]
    }
  ],
  "extensions": "var isEmpty = function(val) {\n    return _.isEmpty(val);\n};\n\nvar chargeTos = function(scope, list) {\n  if (scope.serviceRequest.ResolutionPrimaryCharge) {\n    return;\n  }\n  scope.MasterData.ServiceChargeTos = list;\n  scope.serviceRequest.ResolutionPrimaryCharge = list[0];\n};"
}')

go
