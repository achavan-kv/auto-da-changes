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
      "expression": "\"Inside Supplier Warranty\" &&\nthis.serviceRequest.ManufacturerWarrantyNumber &&\ninsideWarranty(this.serviceRequest, this.serviceRequest.ManufacturerWarrantyLength || 12)"
    },
    {
      "expression": "\"Inside Extended Warranty\" &&\n\nthis.serviceRequest.WarrantyNumber &&\ninsideWarranty(this.serviceRequest, this.serviceRequest.WarrantyLength || 0)"
    },
    {
      "expression": "this.isItemBer && this.serviceRequest.ReplacementIssued"
    },
    {
      "expression": "this.serviceRequest.Type"
    }
  ],
  "actions": [
    {
      "expression": "this.serviceRequest.Charges = [];\nthis.serviceRequest.DepositRequired = 0;"
    },
    {
      "expression": "_.each(this.serviceRequest.Parts, function (part) {\n  part.price = part.CostPrice;\n});\n\nsetLabourChargeInternal(this);"
    },
    {
      "expression": "_.each(this.serviceRequest.Parts, function (part) {\n  part.price = part.CashPrice || part.price;\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\n\nmarkupExternalParts(sr.Parts, ''ChargeCustomer'', this);\nsetLabourChargeCustomer(this);\n\nvar totalNonCatalog = partsSumNonCatalogCostPrice(sr);\n\nsr.Charges.push({\n  CustomerId: sr.ResolutionDelivererToCharge,\n  Label: \"Parts Cosacs\",\n  IsExternal: false,\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account: null,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Cost: partsSumCatalogCostPrice(sr),\n  Value: partsSumCatalog(sr)\n});\n\nsr.Charges.push({\n  CustomerId: sr.ResolutionDelivererToCharge,\n  Label: \"Parts Other\",\n  IsExternal: true,\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account: null,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n  Cost: totalNonCatalog,\n  Value: markupAmount(totalNonCatalog, ''ChargeCustomer'', this)\n});\n\nsr.Charges.push({\n  CustomerId: sr.ResolutionDelivererToCharge,\n  Label: \"Labour\",\n  IsExternal: this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account: null,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: getLabourCost(this),\n  Value: sr.ResolutionLabourCost || 0\n});\n\nsr.Charges.push({\n  CustomerId: sr.ResolutionDelivererToCharge,\n  Label: \"Additional\",\n  IsExternal: this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account: null,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]),\n  Value: sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost])\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\n\nvar totalCatalog = partsSumCatalogCostPrice(sr);\nvar totalNonCatalog = partsSumNonCatalogCostPrice(sr);\nvar labourCost = getLabourCost(this);\n\nsr.Charges.push({\n  Label: \"Parts Cosacs\",\n  IsExternal: false,\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account: this.MasterData.Settings.ServiceInternal,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Cost: totalCatalog,\n  Value: markupAmount(totalCatalog, ''ChargeInternal'', this)\n});\n\nsr.Charges.push({\n  Label: \"Parts Other\",\n  IsExternal: true,\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account: this.MasterData.Settings.ServiceInternal,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n  Cost: totalNonCatalog,\n  Value: markupAmount(totalNonCatalog, ''ChargeInternal'', this)\n});\n\nsr.Charges.push({\n  Label: \"Labour\",\n  IsExternal: this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account: this.MasterData.Settings.ServiceInternal,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: labourCost,\n  Value: labourCost\n});\n\nsr.Charges.push({\n  Label: \"Additional\",\n  IsExternal: this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account: this.MasterData.Settings.ServiceInternal,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]),\n  Value: sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost])\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\n\nvar totalCatalog = partsSumCatalogCostPrice(sr);\nvar totalNonCatalog = partsSumNonCatalogCostPrice(sr);\nvar labourCost = getLabourCost(this);\nvar labourTotal = sum([labourCost, sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]);\nvar ewCovered = labourEWCovered(sr, this);\n\nsr.Charges.push({\n  Label: \"Parts Cosacs\",\n  IsExternal: false,\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account: this.MasterData.Settings.ServiceWarranty,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Cost: totalCatalog,\n  Value: markupAmount(totalCatalog, ''ChargeExtendedWarraty'', this)\n});\n\nsr.Charges.push({\n  Label: \"Parts Other\",\n  IsExternal: true,\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account: this.MasterData.Settings.ServiceWarranty,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n  Cost: totalNonCatalog,\n  Value: markupAmount(totalNonCatalog, ''ChargeExtendedWarraty'', this)\n});\n\nif (ewCovered > 0) {\n  sr.Charges.push({\n    Label: \"Labour and Additional\",\n    IsExternal: this.serviceRequest.AllocationTechnicianIsInternal,\n    ChargeType: sr.ResolutionPrimaryCharge,\n    Account: this.MasterData.Settings.ServiceWarranty,\n    ItemNo: this.MasterData.Settings.ServiceItemLabour,\n    Cost: ewCovered,\n    Value: ewCovered\n  });\n}\n\nif (ewCovered < labourTotal) {\n  sr.Charges.push({\n    Label: \"Labour and Additional\",\n    IsExternal: this.serviceRequest.AllocationTechnicianIsInternal,\n    ChargeType: ''FYW'',\n    Account: this.MasterData.Settings.ServiceFyw,\n    ItemNo: this.MasterData.Settings.ServiceItemLabour,\n    Cost: labourTotal - ewCovered,\n    Value: labourTotal - ewCovered\n  });\n}"
    },
    {
      "expression": "addTax(this);"
    },
    {
      "expression": "markupParts(this.serviceRequest.Parts, getPartsMarkupField(this.serviceRequest.ResolutionPrimaryCharge), this);"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar matrix = this.MasterData.SupplierCostMatrix;\nvar account = this.MasterData.ServiceSuppliers[sr.ResolutionSupplierToCharge];\nvar totalCatalog = partsChargeWithSupplierMatrix(sr, not(partNumberIsEmpty), matrix).covered;\nvar totalNonCatalog = partsChargeWithSupplierMatrix(sr, partNumberIsEmpty, matrix).covered;\nvar labourCharge = labourSupplierCovered(sr, sum([getLabourCost(this), sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]), matrix);\n\nsr.Charges.push({\n  Label: \"Parts Cosacs\",\n  IsExternal: false,\n  ChargeType: \"Supplier\",\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Cost: totalCatalog,\n  Value: markupAmount(totalCatalog, ''ChargeFirstYearWarranty'', this)\n});\n\nsr.Charges.push({\n  Label: \"Parts Other\",\n  IsExternal: true,\n  ChargeType: \"Supplier\",\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n  Value: markupAmount(totalNonCatalog, ''ChargeFirstYearWarranty'', this)\n});\n\nsr.Charges.push({\n  Label: \"Labour and Additional\",\n  IsExternal: this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: \"Supplier\",\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: labourCharge,\n  Value: labourCharge\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar matrix = this.MasterData.SupplierCostMatrix;\nvar account = this.MasterData.Settings.ServiceFyw;\nvar chargeType = ''FYW'';\nvar totalCatalog = partsChargeWithSupplierMatrix(sr, not(partNumberIsEmpty), matrix).remaining;\nvar totalNonCatalog = partsChargeWithSupplierMatrix(sr, partNumberIsEmpty, matrix).remaining;\nvar labourCharge = labourSupplierRemaining(sr, sum([getLabourCost(this), sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]), matrix);\n\nsr.Charges.push({\n  Label: \"Parts Cosacs\",\n  IsExternal: false,\n  ChargeType: chargeType,\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Cost: totalCatalog,\n  Value: markupAmount(totalCatalog, ''ChargeFirstYearWarranty'', this)\n});\n\nsr.Charges.push({\n  Label: \"Parts Other\",\n  IsExternal: true,\n  ChargeType: chargeType,\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n  Cost: totalNonCatalog,\n  Value: markupAmount(totalNonCatalog, ''ChargeFirstYearWarranty'', this)\n});\n\nsr.Charges.push({\n  Label: \"Labour and Additional\",\n  IsExternal: this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: chargeType,\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: labourCharge,\n  Value: labourCharge\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar matrix = this.MasterData.SupplierCostMatrix;\nvar account = this.MasterData.Settings.ServiceWarranty;\n\nvar totalCatalog = partsChargeWithSupplierMatrix(sr, not(partNumberIsEmpty), matrix).remaining;\nvar totalNonCatalog = partsChargeWithSupplierMatrix(sr, partNumberIsEmpty, matrix).remaining;\nvar labourCharge = labourSupplierRemaining(sr, sum([getLabourCost(this), sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]), matrix);\n\nsr.Charges.push({\n  Label: \"Parts Cosacs\",\n  IsExternal: false,\n  ChargeType: \"EW\",\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Cost: totalCatalog,\n  Value: markupAmount(totalCatalog, ''ChargeExtendedWarranty'', this)\n});\n\nsr.Charges.push({\n  Label: \"Parts Other\",\n  IsExternal: true,\n  ChargeType: \"EW\",\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n  Cost: totalNonCatalog,\n  Value: markupAmount(totalNonCatalog, ''ChargeExtendedWarranty'', this)\n});\n\nsr.Charges.push({\n  Label: \"Labour and Additional\",\n  IsExternal: this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: \"EW\",\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: labourCharge,\n  Value: labourCharge\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar matrix = this.MasterData.SupplierCostMatrix;\nvar account = this.MasterData.Settings.ServiceFyw;\nvar chargeType = ''FYW'';\n\nvar totalCatalog = partsChargeWithSupplierMatrix(sr, not(partNumberIsEmpty), matrix).remaining;\nvar totalNonCatalog = partsChargeWithSupplierMatrix(sr, partNumberIsEmpty, matrix).remaining;\nvar labourCharge = labourSupplierRemaining(sr, sum([getLabourCost(this), sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]), matrix);\n\nsr.Charges.push({\n  Label: ''Parts Cosacs'',\n  IsExternal: false,\n  ChargeType: chargeType,\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Cost: totalCatalog,\n  Value: markupAmount(totalCatalog, ''ChargeFirstYearWarranty'', this)\n});\n\nsr.Charges.push({\n  Label: ''Parts Other'',\n  IsExternal: true,\n  ChargeType: chargeType,\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n  Cost: totalNonCatalog,\n  Value: markupAmount(totalNonCatalog, ''ChargeFirstYearWarranty'', this)\n});\n\nsr.Charges.push({\n  Label: ''Labour and Additional'',\n  IsExternal: this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: chargeType,\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: labourCharge,\n  Value: labourCharge\n});"
    },
    {
      "expression": "this.OutstandingBalance = 0;"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar berMarkup = sr.StockItem.CostPrice * this.MasterData.Settings.BerMarkup/100;\nvar berCost = (sr.StockItem.CostPrice + berMarkup).toFixed(2);\nsr.Charges.push({\n    Label: ''Additional'',\n    ChargeType: sr.ResolutionPrimaryCharge,\n    Account: this.MasterData.Settings.ServiceInternal,\n    ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n    Value: berCost\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar account = this.MasterData.ServiceSuppliers[sr.ResolutionSupplierToCharge];\n\nvar labourChargeCovered = labourSupplierCovered(sr, sum([getLabourCost(this), sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]), matrix);\nvar labourChargeRemaining = labourSupplierRemaining(sr, sum([getLabourCost(this), sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]), matrix);\n\nsr.Charges.push({\n  Label: ''Parts Cosacs'',\n  IsExternal: false,\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n  Cost: sr.StockItem.CostPrice\n  Value: sr.StockItem.CostPrice\n});\n\nsr.Charges.push({\n  Label: \"Labour\",\n  IsExternal: this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: labourChargeCovered,\n  Value: labourChargeCovered\n});\n\nsr.Charges.push({\n  Label: \"Labour\",\n  IsExternal: this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: ''FYW'',\n  Account: this.MasterData.Settings.ServiceFyw,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: labourChargeRemaining,\n  Value: labourChargeRemaining\n});",
      "fn": null,
      "error": {}
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar charges = _.filter(sr.Charges, function (c) {\n  return c.ChargeType === \"Customer\" || c.ChargeType === \"Deliverer\"\n});\n\nthis.OutstandingBalance = sum(charges, function (c) { return c.Value; }) - (sr.PaymentBalance || 0);"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar matrix = this.MasterData.SupplierCostMatrix;\nvar account = this.MasterData.Settings.InstallationElectricalAccount;\n\nsr.Charges.push({\n  Label: ''Parts Cosacs'',\n  ChargeType: ''InstallationElectrical'',\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Value: partsSumCatalog(sr, this.MasterData.Settings.ServicePartsMarkupEW)\n});\n\nsr.Charges.push({\n  Label: ''Parts Other'',\n  ChargeType: ''InstallationElectrical'',\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n  Value: partsSumNonCatalog(sr)\n});\n\nsr.Charges.push({\n  Label: ''Labour and Additional'',\n  ChargeType: ''InstallationElectrical'',\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Value: labourSupplierRemaining(sr,\n  sum([sr.ResolutionLabourCost,\n  sr.ResolutionAdditionalCost,\n  sr.ResolutionTransportCost]),\n  matrix)\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar matrix = this.MasterData.SupplierCostMatrix;\nvar account = this.MasterData.Settings.InstallationFurnitureAccount;\n\nsr.Charges.push({\n  Label: ''Parts Cosacs'',\n  ChargeType: ''InstallationFurniture'',\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Value: partsSumCatalog(sr, this.MasterData.Settings.ServicePartsMarkupEW)\n});\n\nsr.Charges.push({\n  Label: ''Parts Other'',\n  ChargeType: ''InstallationFurniture'',\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n  Value: partsSumNonCatalog(sr)\n});\n\nsr.Charges.push({\n  Label: ''Labour and Additional'',\n  ChargeType: ''InstallationFurniture'',\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Value: labourSupplierRemaining(sr,\n  sum([sr.ResolutionLabourCost,\n  sr.ResolutionAdditionalCost,\n  sr.ResolutionTransportCost]),\n  matrix)\n\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar chargeType = ''FYW'';\n\nvar totalCatalog = partsSumCatalog(sr);\nvar totalNonCatalog = partsSumNonCatalog(sr);\nvar labourCost = getLabourCost(this);\n\nsr.Charges.push({\n  Label: \"Parts Cosacs\",\n  IsExternal: false,\n  ChargeType: chargeType,\n  Account: this.MasterData.Settings.ServiceFyw,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Cost: totalCatalog,\n  Value: totalCatalog\n});\n\nsr.Charges.push({\n  Label: \"Parts Other\",\n  IsExternal: true,\n  ChargeType: chargeType,\n  Account: this.MasterData.Settings.ServiceFyw,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n  Cost: totalNonCatalog,\n  Value: totalNonCatalog\n});\n\nsr.Charges.push({\n  Label: \"Labour\",\n  IsExternal: this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: chargeType,\n  Account: this.MasterData.Settings.ServiceFyw,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: labourCost,\n  Value: labourCost\n});\n\nsr.Charges.push({\n  Label: \"Additional\",\n  IsExternal: this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: chargeType,\n  Account: this.MasterData.Settings.ServiceFyw,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]),\n  Value: sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost])\n});"
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
        null,
        null
      ],
      "actions": [
        true,
        false,
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
        "Internal",
        null,
        null,
        null,
        "",
        null
      ],
      "actions": [
        true,
        true,
        false,
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
        false
      ]
    },
    {
      "values": [
        "Deliverer",
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
        true,
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
        "",
        null
      ],
      "actions": [
        true,
        true,
        false,
        false,
        false,
        true,
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
        false
      ]
    },
    {
      "values": [
        "Customer",
        "E",
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
        "Supplier",
        null,
        "true",
        null,
        null,
        "SI"
      ],
      "actions": [
        true,
        true,
        false,
        false,
        false,
        false,
        false,
        true,
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
        false
      ]
    },
    {
      "values": [
        "Supplier",
        null,
        "false",
        "true",
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
        true,
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
        false
      ]
    },
    {
      "values": [
        "Supplier",
        null,
        "false",
        "false",
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
        true,
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
        false
      ]
    },
    {
      "values": [
        "Supplier",
        null,
        "true",
        "false",
        "true",
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
        "Supplier",
        null,
        "false",
        "true",
        "true",
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
        false
      ]
    },
    {
      "values": [
        "Installation Charge Furniture",
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
        false,
        false,
        false,
        false,
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
        "Installation Charge Electrical",
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
        false,
        false,
        false,
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
        "Unicomer Warranty",
        null,
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
        false
      ]
    },
    {
      "values": [
        null,
        null,
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
        false,
        false,
        false,
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
        "Supplier",
        null,
        "true",
        null,
        null,
        "SE"
      ],
      "actions": [
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
        false,
        false,
        false,
        false,
        false,
        false
      ]
    }
  ],
  "extensions": "// this.MasterData.Settings.TaxType: Stock includes tax or not.(I or E)\nvar addMonths = function (d, months) {\n    d = new Date(+d);\n    d.setMonth(d.getMonth() + months);\n    return d;\n};\n\nvar insideWarranty = function (sr, months) {\n    return addMonths(sr.ItemDeliveredOn, months) >= new Date()\n};\n\nvar sum = function (list, f) {\n    f = f || function (v) {\n        return v;\n    };\n    return _.reduce(list, function (memo, e) {\n        return memo + (f(e) || 0);\n    }, 0);\n};\n\nvar empty = function (s) {\n    return !s;\n};\n\nvar iif = function (expr, whenTrue, whenFalse) {\n    if (expr) {\n        return whenTrue;\n    } else {\n        return whenFalse;\n        8\n    }\n};\n\nvar not = function (f) {\n    return function (s) {\n        return !f(s);\n    };\n};\n\nvar partNumberIsEmpty = function (part) {\n    return empty(part.number);\n};\n\nvar partTotalPrice = function (part) {\n    return part.quantity * part.price;\n};\n\nvar partCostPrice = function (part) {\n    return part.quantity * part.CostPrice;\n};\n\nvar findCost = function (sr, part, matrix) {\n    return _.find(matrix.costs, function (row) {\n        return row.partType === part.type && insideWarranty(sr, row.year * 12);\n    });\n};\n\n// part cost covered by supplier\nvar partSupplierCovered = function (sr, part, matrix, price) {\n    var cost = findCost(sr, part, matrix);\n    if (cost) {\n        if (cost.partPcent) {\n            price = price * cost.partPcent / 100;\n        }\n        if (cost.partVal) {\n            price = Math.min(price, cost.partVal);\n        }\n        return price;\n    }\n    return 0;\n};\n\nvar labourSupplierCovered = function (sr, labour, matrix) {\n    if (matrix == null) {\n        return 0;\n    }\n    return _.max(_.map(matrix.costs, function (row) {\n        if (insideWarranty(sr, row.year * 12) && (row.labourPcent || row.labourVal)) {\n            var v = labour;\n            if (row.labourPcent) {\n                v = v * row.labourPcent / 100;\n            }\n            if (row.labourVal) {\n                v = Math.min(v, row.labourVal);\n            }\n            return v;\n        }\n        return 0;\n    })) || 0;\n};\n\nvar labourSupplierRemaining = function (sr, labour, matrix) {\n    var v = labourSupplierCovered(sr, labour, matrix) || 0;\n    return labour - v;\n};\n\nvar partsSumCatalog = function (sr) {\n    return sum(_.filter(sr.Parts, not(partNumberIsEmpty)), partTotalPrice);\n};\n\nvar partsSumNonCatalog = function (sr) {\n    return sum(_.filter(sr.Parts, partNumberIsEmpty), partTotalPrice);\n};\n\nvar partsSumCatalogCostPrice = function (sr) {\n    return sum(_.filter(sr.Parts, not(partNumberIsEmpty)), partCostPrice);\n};\n\nvar partsSumNonCatalogCostPrice = function (sr) {\n    return sum(_.filter(sr.Parts, partNumberIsEmpty), partCostPrice);\n};\n\nvar partsChargeWithSupplierMatrix = function (sr, filter, matrix) {\n    var partSplit = _.map(_.filter(sr.Parts, filter), function (part) {\n        var price = partCostPrice(part);\n        var covered = partSupplierCovered(sr, part, matrix, price);\n        return {\n            covered: covered,\n            remaining: price - covered\n        };\n    });\n    return {\n        covered: sum(partSplit, function (s) {\n            return s.covered;\n        }),\n        remaining: sum(partSplit, function (s) {\n            return s.remaining;\n        })\n    };\n};\n;\nvar addTax = function (scope) {\n    _.each(scope.serviceRequest.Charges, function (c) {\n        c.Tax = (c.Value * scope.MasterData.Settings.TaxRate / 100.0).toFixed(2);\n    });\n};\n\nvar calculateBerCostExtendedWarranty = function (scope) {\n    var sr = scope.serviceRequest;\n    var berMarkup = sr.StockItem.CostPrice * scope.MasterData.Settings.BerMarkup / 100;\n    var berCost = (sr.StockItem.CostPrice + berMarkup - scope.previousEWCost).toFixed(2);\n    if (berCost < 0) {\n        berCost = 0;\n    }\n\n    return berCost;\n};\n\nvar setDepositRequiredAmount = function (scope) {\n    if (!scope.MasterData.LabourCostMatrix || scope.MasterData.LabourCostMatrix.length === 0) {\n        scope.serviceRequest.DepositRequired = 0;\n        return;\n    }\n\n    var sortedCharges = _.sortBy(scope.MasterData.LabourCostMatrix, function (cost) {\n        return cost.ChargeCustomer;\n    });\n\n    scope.serviceRequest.DepositFromMatrix = sortedCharges[0].ChargeCustomer;\n    scope.serviceRequest.DepositRequired = sortedCharges[0].ChargeCustomer;\n};\n\nvar markupAmount = function (amount, markupField, scope) {\n    if (!scope.MasterData.PartsCostMatrix) {\n        return amount;\n    }\n\n    var charge = _.find(scope.MasterData.PartsCostMatrix, function (row) {\n        return row.RepairType === scope.serviceRequest.RepairType;\n    });\n\n    if (!charge) {\n        return amount;\n    }\n\n    var markedUp = amount + (amount * (charge[markupField] / 100));\n    return markedUp;\n};\n\nvar markupCostForPart = function (part, markupField, scope) {\n\n    var markedUpPrice = markupAmount(part.CostPrice, markupField, scope);\n    part.price = markedUpPrice;\n}\n\nvar markupCostForExternalPart = function (part, markupField, scope) {\n    if (part.Source === ''Internal'' || !scope.MasterData.PartsCostMatrix) {\n        return;\n    }\n\n    markupCostForPart(part, markupField, scope);\n};\n\nvar markupParts = function (parts, markupField, scope) {\n    _.each(parts, function (part) {\n        markupCostForPart(part, markupField, scope);\n    });\n};\n\nvar markupExternalParts = function (parts, markupField, scope) {\n    _.each(parts, function (part) {\n        markupCostForExternalPart(part, markupField, scope);\n    });\n};\n\n\nvar getLabourChargeMatrixEntry = function (scope) {\n    if (!scope.MasterData.LabourCostMatrix) {\n        scope.serviceRequest.NoCostMatrixData = true;\n        scope.serviceRequest.ResolutionLabourCost = 0;\n        return;\n    }\n\n    if (!scope.serviceRequest.RepairType) {\n        return;\n    }\n\n    var charge = _.find(scope.MasterData.LabourCostMatrix, function (cost) {\n        return cost.RepairType === scope.serviceRequest.RepairType;\n    });\n\n    return charge;\n};\n\nvar setLabourChargeCustomer = function (scope) {\n\n    var charge = getLabourChargeMatrixEntry(scope);\n\n    if (!charge) {\n        scope.serviceRequest.NoCostMatrixData = true;\n        scope.serviceRequest.ResolutionLabourCost = 0;\n        return;\n    }\n\n    scope.serviceRequest.NoCostMatrixData = false;\n    scope.serviceRequest.ResolutionLabourCost = charge.ChargeCustomer;\n};\n\nvar setLabourChargeInternal = function (scope) {\n\n    var charge = getLabourChargeMatrixEntry(scope);\n\n    if (!charge) {\n        scope.serviceRequest.NoCostMatrixData = true;\n        scope.serviceRequest.ResolutionLabourCost = 0;\n        return;\n    }\n\n    scope.serviceRequest.NoCostMatrixData = false;\n    if (scope.serviceRequest.AllocationTechnicianIsInternal) {\n        scope.serviceRequest.ResolutionLabourCost = charge.ChargeInternalTech;\n    } else {\n        scope.serviceRequest.ResolutionLabourCost = charge.ChargeContractedTech;\n    }\n};\n\nvar getLabourCost = function (scope) {\n    var charge = getLabourChargeMatrixEntry(scope);\n    if (!charge) {\n        return 0;\n    }    \n\n    if (scope.serviceRequest.AllocationTechnicianIsInternal) {\n        return charge.ChargeInternalTech;\n    } else {\n        return charge.ChargeContractedTech;\n    }\n};\n\nvar labourEWCovered = function (sr, scope) {\n    var charge = getLabourChargeMatrixEntry(scope);\n    if (!charge) {\n        return 0;\n    }\n\n    return charge.ChargeEWClaim;\n};\n\nvar partsMarkupFieldLookup = {\n    ''Internal'': ''ChargeInternal'',\n    ''EW'': ''ChargeExtendedWarranty'',\n    ''Supplier'': ''ChargeFirstYearWarranty'',\n    ''Deliverer'': ''ChargeCustomer'',\n    ''Unicomer Warranty'': ''ChargeFirstYearWarranty''\n};\nvar getPartsMarkupField = function (chargeTo) {\n    return partsMarkupFieldLookup[chargeTo];\n};"
}')