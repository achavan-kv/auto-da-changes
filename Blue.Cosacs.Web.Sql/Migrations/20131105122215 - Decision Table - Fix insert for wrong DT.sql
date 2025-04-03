insert into Config.DecisionTable
([Key], CreatedUtc, [Value])
values ('SR.DecisionTable.ServiceStatus', getdate(),
'{
  "conditions": [
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
      "expression": "\"Resolution section is completed\" &&\n!isEmpty(this.serviceRequest.ResolutionDate)&&\ndateIsValid(this.serviceRequest.ResolutionDate)&&\n!isEmpty(this.serviceRequest.ResolutionPrimaryCharge)&&\n\n!isEmpty(this.serviceRequest.ItemSerialNumber)&&\n!isEmpty(this.serviceRequest.ResolutionLabourCost)&&\n!isEmpty(this.serviceRequest.ResolutionAdditionalCost)&&\n!isEmpty(this.serviceRequest.ResolutionTransportCost)&&\n\n(this.serviceRequest.ResolutionPrimaryCharge !== ''Supplier'' || this.serviceRequest.ResolutionPrimaryCharge === ''Supplier'' && !isEmpty(this.serviceRequest.ResolutionCategory))"
    },
    {
      "expression": "\"Finalize section is completed\"&& !isEmpty(this.serviceRequest.FinalisedFailure)&& dateIsValid(this.serviceRequest.FinaliseReturnDate)"
    },
    {
      "expression": "isEmpty(this.serviceRequest.State)"
    },
    {
      "expression": "this.OutstandingBalance > 0"
    },
    {
      "expression": "this.isItemBer"
    },
    {
      "expression": "this.serviceRequest.DepositRequired > 0\n&& (this.serviceRequest.PaymentBalance < this.serviceRequest.DepositRequired)"
    }
  ],
  "actions": [
    {
      "expression": "this.serviceRequest.State = ''New''"
    },
    {
      "expression": "this.serviceRequest.State = ''Awaiting allocation''"
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
    },
    {
      "expression": "this.serviceRequest.State = ''BER''"
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
        "",
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
        false
      ]
    },
    {
      "values": [
        "true",
        "false",
        "false",
        "false",
        "false",
        "false",
        "",
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
        false
      ]
    },
    {
      "values": [
        "",
        "true",
        "false",
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
        "true",
        "",
        "false",
        "false",
        "false",
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
        false
      ]
    },
    {
      "values": [
        "",
        "",
        "",
        "",
        "true",
        "false",
        null,
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
        null,
        "false",
        "",
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
        "true",
        "",
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
        "false",
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
        false
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
        "true"
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
        false
      ]
    }
  ],
  "extensions": "var isEmpty = function(val) {\n    return typeof val === ''undefined'' ||\n                   val === null ||\n                   val === \"\" ||\n                   (typeof val.length === ''undefined'' && val.length === 0);\n};\nvar dateIsValid = function(val) {\n    return val !== undefined && val !== null && window.moment(val).isValid();\n};"
}')

insert into Config.DecisionTable
([Key], CreatedUtc, [Value])
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
      "expression": "this.serviceRequest.Charges = [];\nthis.serviceRequest.DepositRequired = 0;"
    },
    {
      "expression": "var sr = this.serviceRequest;\n\nmarkupParts(sr.Parts, ''ChargeCustomer'', this);\n\nsr.Charges.push({\n  CustomerId: sr.ResolutionDelivererToCharge,\n  Label:      \"Parts Cosacs\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsSumCatalog(sr, this.MasterData.Settings.ServicePartsMarkupEW)\n});\n\nsr.Charges.push({\n  CustomerId: sr.ResolutionDelivererToCharge,\n  Label:      \"Parts Other\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsSumNonCatalog(sr)\n});\n\nsr.Charges.push({\n  CustomerId: sr.ResolutionDelivererToCharge,\n  Label:      \"Labour\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sr.ResolutionLabourCost || 0\n});\n\nsr.Charges.push({\n  CustomerId: sr.ResolutionDelivererToCharge,\n  Label:      \"Additional\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    null,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost])\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceInternal,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsSumCatalog(sr, this.MasterData.Settings.ServicePartsMarkupEW)\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceInternal,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsSumNonCatalog(sr)\n});\n\nsr.Charges.push({\n  Label:      \"Labour\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceInternal,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sr.ResolutionLabourCost || 0\n});\n\nsr.Charges.push({\n  Label:      \"Additional\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceInternal,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost])\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceWarranty,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsSumCatalog(sr, this.MasterData.Settings.ServicePartsMarkupEW)\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceWarranty,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsSumNonCatalog(sr)\n});\n\nsr.Charges.push({\n  Label:      \"Labour\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceWarranty,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sr.ResolutionLabourCost || 0\n});\n\nsr.Charges.push({\n  Label:      \"Additional\",\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account:    this.MasterData.Settings.ServiceWarranty,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost])\n});"
    },
    {
      "expression": "addTax(this);"
    },
    {
      "expression": "var sr = this.serviceRequest,\n    matrix = this.MasterData.SupplierCostMatrix,\n    account = this.MasterData.ServiceSuppliers[sr.ResolutionSupplierToCharge];\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: \"Supplier\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsChargeWithSupplierMatrix(sr, not(partNumberIsEmpty), matrix).covered\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: \"Supplier\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsChargeWithSupplierMatrix(sr, partNumberIsEmpty, matrix).covered\n});\n\nsr.Charges.push({\n  Label:      \"Labour and Additional\",\n  ChargeType: \"Supplier\",\n  Account:    account,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      labourSupplierCovered(sr, sum([sr.ResolutionLabourCost, sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]), matrix)\n});"
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
      "expression": "var sr = this.serviceRequest,\nmatrix = this.MasterData.SupplierCostMatrix,\naccount = this.MasterData.Settings.InstallationElectricalAccount;\n\nsr.Charges.push({\nLabel: ''PartsCosacs'',\nChargeType: ''InstallationElectrical'',\nAccount: account,\nItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\nValue: partsSumCatalog(sr, this.MasterData.Settings.ServicePartsMarkupEW)\n});\n\nsr.Charges.push({\nLabel: ''PartsOther'',\nChargeType: ''InstallationElectrical'',\nAccount: account,\nItemNo: this.MasterData.Settings.ServiceItemPartsOther,\nValue: partsSumNonCatalog(sr)\n});\n\nsr.Charges.push({\nLabel: ''LabourandAdditional'',\nChargeType: ''InstallationElectrical'',\nAccount: account,\nItemNo: this.MasterData.Settings.ServiceItemLabour,\nValue: labourSupplierRemaining(sr,\n\tsum([sr.ResolutionLabourCost,\n\tsr.ResolutionAdditionalCost,\n\tsr.ResolutionTransportCost]),\n\tmatrix)\n});"
    },
    {
      "expression": "var sr = this.serviceRequest,\nmatrix = this.MasterData.SupplierCostMatrix,\naccount = this.MasterData.Settings.InstallationFurnitureAccount;\n\nsr.Charges.push({\nLabel: ''PartsCosacs'',\nChargeType: ''InstallationFurniture'',\nAccount: account,\nItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\nValue: partsSumCatalog(sr, this.MasterData.Settings.ServicePartsMarkupEW)\n});\n\nsr.Charges.push({\nLabel: ''PartsOther'',\nChargeType: ''InstallationFurniture'',\nAccount: account,\nItemNo: this.MasterData.Settings.ServiceItemPartsOther,\nValue: partsSumNonCatalog(sr)\n});\n\nsr.Charges.push({\nLabel: ''LabourandAdditional'',\nChargeType: ''InstallationFurniture'',\nAccount: account,\nItemNo: this.MasterData.Settings.ServiceItemLabour,\nValue: labourSupplierRemaining(sr,\n\tsum([sr.ResolutionLabourCost,\n\tsr.ResolutionAdditionalCost,\n\tsr.ResolutionTransportCost]),\n\tmatrix)\n\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar chargeType = ''Unicomer Warranty'';\n\nsr.Charges.push({\n  Label:      \"Parts Cosacs\",\n  ChargeType: chargeType,\n  Account:    this.MasterData.Settings.ServiceFyw,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsCourts,\n  Value:      partsSumCatalog(sr, this.MasterData.Settings.ServicePartsMarkupEW)\n});\n\nsr.Charges.push({\n  Label:      \"Parts Other\",\n  ChargeType: chargeType,\n  Account:    this.MasterData.Settings.ServiceFyw,\n  ItemNo:     this.MasterData.Settings.ServiceItemPartsOther,\n  Value:      partsSumNonCatalog(sr)\n});\n\nsr.Charges.push({\n  Label:      \"Labour\",\n  ChargeType: chargeType,\n  Account:    this.MasterData.Settings.ServiceFyw,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sr.ResolutionLabourCost || 0\n});\n\nsr.Charges.push({\n  Label:      \"Additional\",\n  ChargeType: chargeType,\n  Account:    this.MasterData.Settings.ServiceFyw,\n  ItemNo:     this.MasterData.Settings.ServiceItemLabour,\n  Value:      sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost])\n});"
    },
    {
      "expression": "_.each(this.serviceRequest.Parts, function (part) {\n  part.price = part.CostPrice || part.price;\n});"
    },
    {
      "expression": "_.each(this.serviceRequest.Parts, function (part) {\n  part.price = part.CashPrice || part.price;\n});"
    },
    {
      "expression": "var markup = this.MasterData.Settings.ServicePartsMarkupEW / 100;\n\n_.each(this.serviceRequest.Parts, function (part) {\n  part.price = Math.round((part.CostPrice + (part.CostPrice * markup)) * 100) / 100;\n});"
    },
    {
      "expression": "setDepositRequiredAmount(this);"
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
        false,
        false,
        true,
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
        false,
        true,
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
        false,
        false,
        true,
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
        false,
        false,
        true,
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
        false,
        true,
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
        false,
        false,
        true,
        false,
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
        true,
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
        "false",
        "false",
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
        true
      ]
    }
  ],
  "extensions": "// this.MasterData.Settings.TaxType: Stock includes tax or not.(I or E)\nvar addMonths = function (d, months) {\n  d = new Date(+d);\n  d.setMonth(d.getMonth() + months);\n  return d;\n};\n\nvar insideWarranty = function (sr, months) {\n  return addMonths(sr.ItemDeliveredOn, months) >= new Date()\n};\n\nvar sum = function (list, f, primaryCharge, ewPartsMarkup) {\n  f = f || function (v) {\n    return v;\n  };\n  return _.reduce(list, function (memo, e) {\n    return memo + (f(e, primaryCharge, ewPartsMarkup) || 0);\n  }, 0);\n};\n\nvar empty = function (s) {\n  return !s;\n};\n\nvar iif = function (expr, whenTrue, whenFalse) {\n  if (expr) {\n    return whenTrue;\n  } else {\n    return whenFalse;\n  }\n};\n\nvar not = function (f) {\n  return function (s) {\n    return !f(s);\n  };\n};\n\nvar partNumberIsEmpty = function (part) {\n  return empty(part.number);\n};\n\nvar partTotalPrice = function (part, primaryCharge, ewPartsMarkup) {\n    if (part.number) {\n        if (primaryCharge === ''EW'' && part.CostPrice) {\n            var priceIncludingMarkup = part.CostPrice + (part.CostPrice * (ewPartsMarkup / 100));\n            return part.quantity * priceIncludingMarkup;\n        } else if ((primaryCharge === ''Internal'' || primaryCharge === ''Supplier'') && part.CostPrice) {\n            return part.quantity * part.CostPrice;\n        } else {\n            return part.quantity * (part.CashPrice || part.price);\n        }\n    } else {\n        return part.quantity * part.MarkupPrice;\n    }\n};\n\nvar findCost = function (sr, part, matrix) {\n  return _.find(matrix, function (row) {\n    return row.partType === part.type && insideWarranty(sr, row.year * 12);\n  });\n};\n\n// part cost covered by supplier\nvar partSupplierCovered = function (sr, part, matrix, price) {\n  var cost = findCost(sr, part, matrix);\n  if (cost) {\n    if (cost.partPcent) {\n      price = price * cost.partPcent / 100;\n    }\n    if (cost.partVal) {\n      price = Math.min(price, cost.partVal);\n    }\n    return price;\n  }\n  return 0;\n};\n\nvar labourSupplierCovered = function (sr, labour, matrix) {\n  if (matrix == null) {\n    return 0;\n  }\n  return _.max(_.map(matrix, function (row) {\n    if (insideWarranty(sr, row.year * 12) && (row.labourPcent || row.labourVal)) {\n      var v = labour;\n      if (row.labourPcent) {\n        v = v * row.labourPcent / 100;\n      }\n      if (row.labourVal) {\n        v = Math.min(v, row.labourVal);\n      }\n      return v;\n    }\n    return 0;\n  })) || 0;\n};\n\nvar labourSupplierRemaining = function (sr, labour, matrix) {\n  var v = labourSupplierCovered(sr, labour, matrix) || 0;\n  return labour - v;\n};\n\nvar partsSumCatalog = function (sr, ewPartsMarkup) {\n  return sum(_.filter(sr.Parts, not(partNumberIsEmpty)), partTotalPrice, sr.ResolutionPrimaryCharge, ewPartsMarkup);\n};\n\nvar partsSumNonCatalog = function (sr) {\n  return sum(_.filter(sr.Parts, partNumberIsEmpty), partTotalPrice);\n};\n\nvar partsChargeWithSupplierMatrix = function (sr, filter, matrix) {\n  var partSplit = _.map(_.filter(sr.Parts, filter), function (part) {\n    var price = partTotalPrice(part, sr.ResolutionPrimaryCharge),\n      covered = partSupplierCovered(sr, part, matrix, price);\n    return {\n      covered: covered,\n      remaining: price - covered\n    };\n  });\n  return {\n    covered: sum(partSplit, function (s) {\n      return s.covered;\n    }),\n    remaining: sum(partSplit, function (s) {\n      return s.remaining;\n    })\n  };\n};\n\nvar addTax = function (scope) {\n  _.each(scope.serviceRequest.Charges, function (c) {\n    c.Tax = (c.Value * scope.MasterData.Settings.TaxRate / 100.0).toFixed(2);\n  });\n};\n\nvar calculateBerCostExtendedWarranty = function (scope) {\n  var sr = scope.serviceRequest;\n  var berMarkup = sr.StockItem.CostPrice * scope.MasterData.Settings.BerMarkup / 100;\n  var berCost = (sr.StockItem.CostPrice + berMarkup - scope.previousEWCost).toFixed(2);\n  if (berCost < 0) {\n    berCost = 0;\n  }\n\n  return berCost;\n};\n\nvar setDepositRequiredAmount = function (scope) {\n    if (!scope.MasterData.LabourCostMatrix || scope.MasterData.LabourCostMatrix.length === 0) {\n        scope.serviceRequest.DepositRequired = 0;\n        return;\n    }\n\n    var sortedCharges = _.sortBy(scope.MasterData.LabourCostMatrix, function (cost) {\n        return cost.ChargeCustomer;\n    });\n    scope.serviceRequest.DepositRequired = sortedCharges[0].ChargeCustomer;\n};\n\nvar markupCostForPart = function (part, markupField, scope) {\n    var cost = part.quantity * part.price\n    if (part.number || !scope.MasterData.PartsCostMatrix) {\n        part.MarkupPrice = cost;\n        return;\n    }\n\n    var charge = _.find(scope.MasterData.PartsCostMatrix, function (row) {\n        return row.RepairType === scope.serviceRequest.RepairType;\n    });\n\n    if (!charge) {\n        part.MarkupPrice = cost;\n        return;\n    }\n\n    cost += cost * (charge[markupField] / 100);\n    part.MarkupPrice = cost;\n};\n\nvar markupParts = function (parts, markupField, scope) {\n    _.each(parts, function (part) {\n        markupCostForPart(part, markupField, scope);\n    });\n};"
}')