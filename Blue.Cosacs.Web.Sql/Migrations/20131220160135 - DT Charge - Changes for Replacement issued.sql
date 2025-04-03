insert into Config.DecisionTable
([Key], CreatedUtc, [Value])
values ('SR.DecisionTable.Charge', getdate(),
'{
  "conditions": [
    {
      "expression": "this.serviceRequest.ResolutionPrimaryCharge"
    },
    {
      "expression": "\"Inside Supplier Warranty\" &&\nthis.serviceRequest.ManufacturerWarrantyContractNo &&\ninsideWarranty(this.serviceRequest, this.serviceRequest.ManufacturerWarrantyLength || 0)"
    },
    {
      "expression": "\"Inside Extended Warranty\" &&\n\nthis.serviceRequest.WarrantyContractNo &&\ninsideWarranty(this.serviceRequest, (this.serviceRequest.WarrantyLength || 0) + (this.serviceRequest.ManufacturerWarrantyLength || 0))"
    },
    {
      "expression": "this.isItemBer"
    },
    {
      "expression": "this.serviceRequest.Type"
    },
    {
      "expression": "''Warranty covered'' &&\n(\n    (this.serviceRequest.ManufacturerWarrantyContractNo && insideWarranty(this.serviceRequest, this.serviceRequest.ManufacturerWarrantyLength || 0)) ||\n    (this.serviceRequest.WarrantyContractNo && insideWarranty(this.serviceRequest, (this.serviceRequest.WarrantyLength || 0) + (this.serviceRequest.ManufacturerWarrantyLength || 0)))\n)"
    },
    {
      "expression": "this.serviceRequest.ReplacementIssued"
    },
    {
      "expression": "\"Is service request\" &&\nthis.serviceRequest.Type === ''SI'' ||\nthis.serviceRequest.Type === ''SE''"
    }
  ],
  "actions": [
    {
      "expression": "this.serviceRequest.Charges = [];"
    },
    {
      "expression": "_.each(this.serviceRequest.Parts, function (part) {\n  part.price = part.CostPrice;\n});"
    },
    {
      "expression": "_.each(this.serviceRequest.Parts, function (part) {\n  part.price = part.CashPrice || part.price;\n});"
    },
    {
      "expression": "setLabourChargeInternal(this);"
    },
    {
      "expression": "setLabourChargeCustomer(this);"
    },
    {
      "expression": "var sr = this.serviceRequest;\n\nmarkupExternalParts(sr.Parts, ''ChargeCustomer'', this);\n\nvar totalExternal = partsSumExternalCostPrice(sr);\nvar totalSalvaged = partsSumSalvagedCostPrice(sr);\n\nsr.Charges.push({\n  CustomerId: sr.ResolutionDelivererToCharge,\n  Label: \"Parts Cosacs\",\n  IsExternal: false,\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account: null,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Cost: partsSumCatalogCostPrice(sr),\n  Value: partsSumCatalog(sr),\n  Tax: getTaxAmount(partsSumCatalog(sr), this)\n});\n\nif (totalExternal > 0) {\n  sr.Charges.push({\n    CustomerId: sr.ResolutionDelivererToCharge,\n    Label: \"Parts External\",\n    IsExternal: true,\n    ChargeType: sr.ResolutionPrimaryCharge,\n    Account: null,\n    ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n    Cost: totalExternal,\n    Value: markupAmount(totalExternal, ''ChargeCustomer'', this),\n    Tax: getTaxAmount(markupAmount(totalExternal, ''ChargeCustomer'', this), this)\n  });\n}\n\nif (totalSalvaged > 0) {\n  sr.Charges.push({\n    CustomerId: sr.ResolutionDelivererToCharge,\n    Label: \"Parts Salvaged\",\n    IsExternal: false,\n    ChargeType: sr.ResolutionPrimaryCharge,\n    Account: null,\n    ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n    Cost: totalSalvaged,\n    Value: markupAmount(totalSalvaged, ''ChargeCustomer'', this),\n    Tax: getTaxAmount(markupAmount(totalSalvaged, ''ChargeCustomer'', this), this)\n  });\n}\n\nsr.Charges.push({\n  CustomerId: sr.ResolutionDelivererToCharge,\n  Label: \"Labour\",\n  IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account: null,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: getLabourCost(this),\n  Value: sr.ResolutionLabourCost\n});\n\nsr.Charges.push({\n  CustomerId: sr.ResolutionDelivererToCharge,\n  Label: \"Additional\",\n  IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account: null,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]),\n  Value: sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost])\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\n\nvar totalCatalog = partsSumCatalogCostPrice(sr);\nvar totalExternal = partsSumExternalCostPrice(sr);\nvar totalSalvaged = partsSumSalvagedCostPrice(sr);\nvar labourCost = getLabourCost(this);\n\nsr.Charges.push({\n\tLabel: \"Parts Cosacs\",\n\tIsExternal: false,\n\tChargeType: sr.ResolutionPrimaryCharge,\n\tAccount: this.MasterData.Settings.ServiceInternal,\n\tItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n\tCost: totalCatalog,\n\tValue: markupAmount(totalCatalog, ''ChargeInternal'', this)\n});\n\nif (totalExternal > 0) {\n\tsr.Charges.push({\n\t\tLabel: \"Parts External\",\n\t\tIsExternal: true,\n\t\tChargeType: sr.ResolutionPrimaryCharge,\n\t\tAccount: this.MasterData.Settings.ServiceInternal,\n\t\tItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n\t\tCost: totalExternal,\n\t\tValue: markupAmount(totalExternal, ''ChargeInternal'', this)\n\t});\n}\n\nif (totalSalvaged > 0) {\n\tsr.Charges.push({\n\t\tLabel: \"Parts Salvaged\",\n\t\tIsExternal: false,\n\t\tChargeType: sr.ResolutionPrimaryCharge,\n\t\tAccount: this.MasterData.Settings.ServiceInternal,\n\t\tItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n\t\tCost: totalSalvaged,\n\t\tValue: markupAmount(totalSalvaged, ''ChargeInternal'', this)\n\t});\n}\n\nsr.Charges.push({\n\tLabel: \"Labour\",\n\tIsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n\tChargeType: sr.ResolutionPrimaryCharge,\n\tAccount: this.MasterData.Settings.ServiceInternal,\n\tItemNo: this.MasterData.Settings.ServiceItemLabour,\n\tCost: labourCost,\n\tValue: labourCost\n});\n\nsr.Charges.push({\n\tLabel: \"Additional\",\n\tIsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n\tChargeType: sr.ResolutionPrimaryCharge,\n\tAccount: this.MasterData.Settings.ServiceInternal,\n\tItemNo: this.MasterData.Settings.ServiceItemLabour,\n\tCost: sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]),\n\tValue: sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost])\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\n\nvar totalCatalog = partsSumCatalogCostPrice(sr);\nvar totalExternal = partsSumExternalCostPrice(sr);\nvar totalSalvaged = partsSumSalvagedCostPrice(sr);\nvar labourCost = getLabourCost(this);\nvar labourTotal = sum([labourCost, sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]);\nvar ewCovered = labourEWCovered(sr, this);\n\nsr.Charges.push({\n  Label: \"Parts Cosacs\",\n  IsExternal: false,\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account: this.MasterData.Settings.ServiceWarranty,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Cost: totalCatalog,\n  Value: markupAmount(totalCatalog, ''ChargeExtendedWarranty'', this)\n});\n\nif (totalExternal > 0) {\n\tsr.Charges.push({\n\t  Label: \"Parts External\",\n\t  IsExternal: true,\n\t  ChargeType: sr.ResolutionPrimaryCharge,\n\t  Account: this.MasterData.Settings.ServiceWarranty,\n\t  ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n\t  Cost: totalExternal,\n\t  Value: markupAmount(totalExternal, ''ChargeExtendedWarranty'', this)\n\t});\n}\n\nif (totalSalvaged > 0) {\n\tsr.Charges.push({\n\t  Label: \"Parts Salvaged\",\n\t  IsExternal: false,\n\t  ChargeType: sr.ResolutionPrimaryCharge,\n\t  Account: this.MasterData.Settings.ServiceWarranty,\n\t  ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n\t  Cost: totalSalvaged,\n\t  Value: markupAmount(totalSalvaged, ''ChargeExtendedWarranty'', this)\n\t});\n}\n\nif (ewCovered > 0) {\n  sr.Charges.push({\n    Label: \"Labour and Additional\",\n    IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n    ChargeType: sr.ResolutionPrimaryCharge,\n    Account: this.MasterData.Settings.ServiceWarranty,\n    ItemNo: this.MasterData.Settings.ServiceItemLabour,\n    Cost: ewCovered,\n    Value: ewCovered\n  });\n}\n\nif (ewCovered < labourTotal) {\n  sr.Charges.push({\n    Label: \"Labour and Additional\",\n    IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n    ChargeType: ''FYW'',\n    Account: this.MasterData.Settings.ServiceFyw,\n    ItemNo: this.MasterData.Settings.ServiceItemLabour,\n    Cost: labourTotal - ewCovered,\n    Value: labourTotal - ewCovered\n  });\n}"
    },
    {
      "expression": "markupParts(this.serviceRequest.Parts, getPartsMarkupField(this.serviceRequest.ResolutionPrimaryCharge), this);"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar matrix = this.MasterData.SupplierCostMatrix;\nvar account = this.MasterData.ServiceSuppliers[sr.ResolutionSupplierToCharge];\n\nvar totalCatalog = partsChargeWithSupplierMatrix(sr, partSourceIsInternal, matrix).covered;\nvar totalExternal = partsChargeWithSupplierMatrix(sr, partSourceIsExternal, matrix).covered;\nvar totalSalvaged = partsChargeWithSupplierMatrix(sr, partSourceIsSalvaged, matrix).covered;\n\nvar labourCharge = labourSupplierCovered(sr, sum([getLabourCost(this), sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]), matrix);\n\nsr.Charges.push({\n  Label: \"Parts Cosacs\",\n  IsExternal: false,\n  ChargeType: \"Supplier\",\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Cost: totalCatalog,\n  Value: markupAmount(totalCatalog, ''ChargeFirstYearWarranty'', this)\n});\n\nif (totalExternal > 0) {\n  sr.Charges.push({\n    Label: \"Parts External\",\n    IsExternal: true,\n    ChargeType: \"Supplier\",\n    Account: account,\n    ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n    Cost: totalExternal,\n    Value: markupAmount(totalExternal, ''ChargeFirstYearWarranty'', this)\n  });\n}\n\nif (totalSalvaged > 0) {\n  sr.Charges.push({\n    Label: \"Parts Salvaged\",\n    IsExternal: true,\n    ChargeType: \"Supplier\",\n    Account: account,\n    ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n    Cost: totalSalvaged,\n    Value: markupAmount(totalSalvaged, ''ChargeFirstYearWarranty'', this)\n  });\n}\n\nsr.Charges.push({\n  Label: \"Labour and Additional\",\n  IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: \"Supplier\",\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: labourCharge,\n  Value: labourCharge\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar matrix = this.MasterData.SupplierCostMatrix;\nvar account = this.MasterData.Settings.ServiceFyw;\nvar chargeType = ''FYW'';\n\nvar totalCatalog = partsChargeWithSupplierMatrix(sr, partSourceIsInternal, matrix).remaining;\nvar totalExternal = partsChargeWithSupplierMatrix(sr, partSourceIsExternal, matrix).remaining;\nvar totalSalvaged = partsChargeWithSupplierMatrix(sr, partSourceIsSalvaged, matrix).remaining;\n\nvar labourCharge = labourSupplierRemaining(sr, sum([getLabourCost(this), sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]), matrix);\n\nsr.Charges.push({\n  Label: \"Parts Cosacs\",\n  IsExternal: false,\n  ChargeType: chargeType,\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Cost: totalCatalog,\n  Value: markupAmount(totalCatalog, ''ChargeFirstYearWarranty'', this)\n});\n\nif (totalExternal > 0) {\n  sr.Charges.push({\n    Label: \"Parts External\",\n    IsExternal: true,\n    ChargeType: chargeType,\n    Account: account,\n    ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n    Cost: totalExternal,\n    Value: markupAmount(totalExternal, ''ChargeFirstYearWarranty'', this)\n  });\n}\n\nif (totalSalvaged > 0) {\n  sr.Charges.push({\n    Label: \"Parts Salvaged\",\n    IsExternal: false,\n    ChargeType: chargeType,\n    Account: account,\n    ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n    Cost: totalSalvaged,\n    Value: markupAmount(totalSalvaged, ''ChargeFirstYearWarranty'', this)\n  });\n}\n\nsr.Charges.push({\n  Label: \"Labour and Additional\",\n  IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: chargeType,\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: labourCharge,\n  Value: labourCharge\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar matrix = this.MasterData.SupplierCostMatrix;\nvar account = this.MasterData.Settings.ServiceWarranty;\n\nvar totalCatalog = partsChargeWithSupplierMatrix(sr, partSourceIsInternal, matrix).remaining;\nvar totalExternal = partsChargeWithSupplierMatrix(sr, partSourceIsExternal, matrix).remaining;\nvar totalSalvaged = partsChargeWithSupplierMatrix(sr, partSourceIsSalvaged, matrix).remaining;\n\nvar labourCharge = labourSupplierRemaining(sr, sum([getLabourCost(this), sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]), matrix);\nvar ewCovered = labourEWCovered(sr, this);\n\nsr.Charges.push({\n  Label: \"Parts Cosacs\",\n  IsExternal: false,\n  ChargeType: \"EW\",\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Cost: totalCatalog,\n  Value: markupAmount(totalCatalog, ''ChargeExtendedWarranty'', this)\n});\n\nif (totalExternal > 0) {\n  sr.Charges.push({\n    Label: \"Parts External\",\n    IsExternal: true,\n    ChargeType: \"EW\",\n    Account: account,\n    ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n    Cost: totalExternal,\n    Value: markupAmount(totalExternal, ''ChargeExtendedWarranty'', this)\n  });\n}\n\nif (totalSalvaged > 0) {\n  sr.Charges.push({\n    Label: \"Parts Salvaged\",\n    IsExternal: false,\n    ChargeType: \"EW\",\n    Account: account,\n    ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n    Cost: totalSalvaged,\n    Value: markupAmount(totalSalvaged, ''ChargeExtendedWarranty'', this)\n  });\n}\n\nif (ewCovered > 0) {\n  sr.Charges.push({\n    Label: \"Labour and Additional\",\n    IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n    ChargeType: \"EW\",\n    Account: account,\n    ItemNo: this.MasterData.Settings.ServiceItemLabour,\n    Cost: ewCovered,\n    Value: ewCovered\n  });\n}\n\nif (ewCovered < labourCharge) {\n    sr.Charges.push({\n      Label: \"Labour and Additional\",\n      IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n      ChargeType: \"FYW\",\n      Account: this.MasterData.Settings.ServiceFyw,\n      ItemNo: this.MasterData.Settings.ServiceItemLabour,\n      Cost: labourCharge - ewCovered,\n      Value: labourCharge - ewCovered\n    });\n}"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar matrix = this.MasterData.SupplierCostMatrix;\nvar account = this.MasterData.Settings.ServiceFyw;\nvar chargeType = ''FYW'';\n\nvar totalCatalog = partsChargeWithSupplierMatrix(sr, partSourceIsInternal, matrix).remaining;\nvar totalExternal = partsChargeWithSupplierMatrix(sr, partSourceIsExternal, matrix).remaining;\nvar totalSalvaged = partsChargeWithSupplierMatrix(sr, partSourceIsSalvaged, matrix).remaining;\n\nvar labourCharge = labourSupplierRemaining(sr, sum([getLabourCost(this), sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]), matrix);\n\nsr.Charges.push({\n  Label: ''Parts Cosacs'',\n  IsExternal: false,\n  ChargeType: chargeType,\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Cost: totalCatalog,\n  Value: markupAmount(totalCatalog, ''ChargeFirstYearWarranty'', this)\n});\n\nif (totalExternal > 0) {\n  sr.Charges.push({\n    Label: ''Parts External'',\n    IsExternal: true,\n    ChargeType: chargeType,\n    Account: account,\n    ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n    Cost: totalExternal,\n    Value: markupAmount(totalExternal, ''ChargeFirstYearWarranty'', this)\n  });\n}\n\nif (totalSalvaged > 0) {\n  sr.Charges.push({\n    Label: ''Parts Salvaged'',\n    IsExternal: true,\n    ChargeType: chargeType,\n    Account: account,\n    ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n    Cost: totalSalvaged,\n    Value: markupAmount(totalSalvaged, ''ChargeFirstYearWarranty'', this)\n  });\n}\n\nsr.Charges.push({\n  Label: ''Labour and Additional'',\n  IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: chargeType,\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: labourCharge,\n  Value: labourCharge\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar matrix = this.MasterData.SupplierCostMatrix;\nvar account = this.MasterData.Settings.ServiceFyw;\nvar chargeType = ''Customer'';\n\nvar totalCatalog = partsChargeWithSupplierMatrix(sr, partSourceIsInternal, matrix).remaining;\nvar totalExternal = partsChargeWithSupplierMatrix(sr, partSourceIsExternal, matrix).remaining;\nvar totalSalvaged = partsChargeWithSupplierMatrix(sr, partSourceIsSalvaged, matrix).remaining;\n\nvar labourCharge = labourSupplierRemaining(sr, sum([getLabourCost(this), sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]), matrix);\n\nsr.Charges.push({\n  Label: ''Parts Cosacs'',\n  IsExternal: false,\n  ChargeType: chargeType,\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Cost: totalCatalog,\n  Value: markupAmount(totalCatalog, ''ChargeCustomer'', this),\n  Tax: getTaxAmount(markupAmount(totalCatalog, ''ChargeCustomer'', this), this)\n});\n\nif (totalExternal > 0) {\n  sr.Charges.push({\n    Label: ''Parts External'',\n    IsExternal: true,\n    ChargeType: chargeType,\n    Account: account,\n    ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n    Cost: totalExternal,\n    Value: markupAmount(totalExternal, ''ChargeCustomer'', this),\n    Tax: getTaxAmount(markupAmount(totalExternal, ''ChargeCustomer'', this), this)\n  });\n}\n\nif (totalSalvaged > 0) {\n  sr.Charges.push({\n    Label: ''Parts Salvaged'',\n    IsExternal: true,\n    ChargeType: chargeType,\n    Account: account,\n    ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n    Cost: totalSalvaged,\n    Value: markupAmount(totalSalvaged, ''ChargeCustomer'', this),\n    Tax: getTaxAmount(markupAmount(totalSalvaged, ''ChargeCustomer'', this), this)\n  });\n}\n\nsr.Charges.push({\n  Label: ''Labour and Additional'',\n  IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: chargeType,\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: labourCharge,\n  Value: labourCharge\n});"
    },
    {
      "expression": "this.OutstandingBalance = 0;"
    },
    {
      "expression": "''Goodwill replacement charged to Internal'';\n\nvar sr = this.serviceRequest;\n\nsr.Charges.push({\n  Label: ''Parts Cosacs'',\n  IsExternal: false,\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account: this.MasterData.Settings.ServiceInternal,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Cost: sr.StockItem.CostPrice,\n  Value: sr.StockItem.CostPrice\n});\n\nsr.Charges.push({\n  Label: ''Labour'',\n  IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account: this.MasterData.Settings.ServiceInternal,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: getLabourCost(this),\n  Value: getLabourCost(this)\n});\n\nsr.Charges.push({\n  Label: ''Additional'',\n  IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: sr.ResolutionPrimaryCharge,\n  Account: this.MasterData.Settings.ServiceInternal,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]),\n  Value: sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost])\n});"
    },
    {
      "expression": "''BER replacement'';\n\nvar sr = this.serviceRequest;\nvar account;\nvar matrix = this.MasterData.SupplierCostMatrix;\nvar labourTotal = sum([getLabourCost(this), sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]);\nvar labourChargeCovered = labourSupplierCovered(sr, labourTotal, matrix);\nvar labourChargeRemaining = labourSupplierRemaining(sr, labourTotal, matrix);\nvar ewCovered = labourEWCovered(sr, this);\nvar chargeType = sr.ResolutionPrimaryCharge === ''Unicomer Warranty'' ? ''FYW'' : sr.ResolutionPrimaryCharge;\nif (sr.ResolutionPrimaryCharge === ''Supplier'') {\n  account = this.MasterData.ServiceSuppliers[sr.ResolutionSupplierToCharge];\n} else if (sr.ResolutionPrimaryCharge === ''Unicomer Warranty'') {\n  account = this.MasterData.Settings.ServiceFyw;\n} else if (sr.ResolutionPrimaryCharge === ''EW'') {\n  account = this.MasterData.Settings.ServiceWarranty;\n} else if (sr.ResolutionPrimaryCharge === ''Internal'') {\n  account = this.MasterData.Settings.ServiceInternal;\n}\n\nsr.Charges.push({\n  Label: ''Parts Cosacs'',\n  IsExternal: false,\n  ChargeType: chargeType,\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Cost: sr.StockItem.CostPrice,\n  Value: calculateBerCharge(this)\n});\n\nif (sr.ResolutionPrimaryCharge === ''Supplier'') {\n  sr.Charges.push({\n    Label: ''Labour and Additional'',\n    IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n    ChargeType: sr.ResolutionPrimaryCharge,\n    Account: account,\n    ItemNo: this.MasterData.Settings.ServiceItemLabour,\n    Cost: labourChargeCovered,\n    Value: labourChargeCovered\n  });\n\n  sr.Charges.push({\n    Label: ''Labour and Additional'',\n    IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n    ChargeType: ''FYW'',\n    Account: this.MasterData.Settings.ServiceFyw,\n    ItemNo: this.MasterData.Settings.ServiceItemLabour,\n    Cost: labourChargeRemaining,\n    Value: labourChargeRemaining\n  });\n\n}\n\nif (sr.ResolutionPrimaryCharge === ''EW'') {\n  if (ewCovered > 0) {\n    sr.Charges.push({\n      Label: ''Labour and Additional'',\n      IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n      ChargeType: ''EW'',\n      Account: this.MasterData.Settings.ServiceWarranty,\n      ItemNo: this.MasterData.Settings.ServiceItemLabour,\n      Cost: ewCovered,\n      Value: ewCovered\n    });\n  }\n\n  if (ewCovered < labourTotal) {\n    sr.Charges.push({\n      Label: ''Labour and Additional'',\n      IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n      ChargeType: ''FYW'',\n      Account: this.MasterData.Settings.ServiceFyw,\n      ItemNo: this.MasterData.Settings.ServiceItemLabour,\n      Cost: labourTotal - ewCovered,\n      Value: labourTotal - ewCovered\n    });\n  }\n}\n\nif (sr.ResolutionPrimaryCharge !== ''Supplier'' && sr.ResolutionPrimaryCharge !== ''EW'') {\n  sr.Charges.push({\n    Label: ''Labour'',\n    IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n    ChargeType: chargeType,\n    Account: account,\n    ItemNo: this.MasterData.Settings.ServiceItemLabour,\n    Cost: getLabourCost(this),\n    Value: getLabourCost(this)\n  });\n\n  sr.Charges.push({\n    Label: ''Additional'',\n    IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n    ChargeType: chargeType,\n    Account: account,\n    ItemNo: this.MasterData.Settings.ServiceItemLabour,\n    Cost: sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]),\n    Value: sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost])\n  });\n}"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar charges = _.filter(sr.Charges, function (c) {\n  return c.ChargeType === \"Customer\" || c.ChargeType === \"Deliverer\"\n});\n\nthis.OutstandingBalance = sum(charges, function (c) { return c.Value + (c.Tax || 0); }) - (sr.PaymentBalance || 0);"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar matrix = this.MasterData.SupplierCostMatrix;\nvar account = this.MasterData.Settings.InstallationElectricalAccount;\nthis.serviceRequest.NoCostMatrixData = true;\nthis.serviceRequest.ResolutionLabourCost = 0;\n\nsr.Charges.push({\n  Label: ''Parts Cosacs'',\n  ChargeType: ''Installation Charge Electrical'',\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Value: partsSumCatalog(sr, this.MasterData.Settings.ServicePartsMarkupEW)\n});\n\nsr.Charges.push({\n  Label: ''Parts Other'',\n  ChargeType: ''Installation Charge Electrical'',\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n  Value: partsSumNonCatalog(sr)\n});\n\nsr.Charges.push({\n  Label: ''Labour and Additional'',\n  ChargeType: ''Installation Charge Electrical'',\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Value: labourSupplierRemaining(sr,\n  sum([sr.ResolutionLabourCost,\n  sr.ResolutionAdditionalCost,\n  sr.ResolutionTransportCost]),\n  matrix)\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar matrix = this.MasterData.SupplierCostMatrix;\nvar account = this.MasterData.Settings.InstallationFurnitureAccount;\nthis.serviceRequest.NoCostMatrixData = true;\nthis.serviceRequest.ResolutionLabourCost = 0;\n\nsr.Charges.push({\n  Label: ''Parts Cosacs'',\n  ChargeType: ''Installation Charge Furniture'',\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Value: partsSumCatalog(sr, this.MasterData.Settings.ServicePartsMarkupEW)\n});\n\nsr.Charges.push({\n  Label: ''Parts Other'',\n  ChargeType: ''Installation Charge Furniture'',\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n  Value: partsSumNonCatalog(sr)\n});\n\nsr.Charges.push({\n  Label: ''Labour and Additional'',\n  ChargeType: ''Installation Charge Furniture'',\n  Account: account,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Value: labourSupplierRemaining(sr,\n  sum([sr.ResolutionLabourCost,\n  sr.ResolutionAdditionalCost,\n  sr.ResolutionTransportCost]),\n  matrix)\n\n});"
    },
    {
      "expression": "var sr = this.serviceRequest;\nvar chargeType = ''FYW'';\n\nvar totalCatalog = partsSumCatalogCostPrice(sr);\nvar totalExternal = partsSumExternalCostPrice(sr);\nvar totalSalvaged = partsSumSalvagedCostPrice(sr);\nvar labourCost = getLabourCost(this);\n\nsr.Charges.push({\n  Label: \"Parts Cosacs\",\n  IsExternal: false,\n  ChargeType: chargeType,\n  Account: this.MasterData.Settings.ServiceFyw,\n  ItemNo: this.MasterData.Settings.ServiceItemPartsCourts,\n  Cost: totalCatalog,\n  Value: markupAmount(totalCatalog, ''ChargeFirstYearWarranty'', this)\n});\n\nif (totalExternal > 0) {\n    sr.Charges.push({\n      Label: \"Parts External\",\n      IsExternal: true,\n      ChargeType: chargeType,\n      Account: this.MasterData.Settings.ServiceFyw,\n      ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n      Cost: totalExternal,\n      Value: markupAmount(totalExternal, ''ChargeFirstYearWarranty'', this)\n    });\n}\n\nif (totalSalvaged > 0) {\n    sr.Charges.push({\n      Label: \"Parts Salvaged\",\n      IsExternal: true,\n      ChargeType: chargeType,\n      Account: this.MasterData.Settings.ServiceFyw,\n      ItemNo: this.MasterData.Settings.ServiceItemPartsOther,\n      Cost: totalSalvaged,\n      Value: markupAmount(totalSalvaged, ''ChargeFirstYearWarranty'', this)\n    });\n}\n\nsr.Charges.push({\n  Label: \"Labour\",\n  IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: chargeType,\n  Account: this.MasterData.Settings.ServiceFyw,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: labourCost,\n  Value: labourCost\n});\n\nsr.Charges.push({\n  Label: \"Additional\",\n  IsExternal: !this.serviceRequest.AllocationTechnicianIsInternal,\n  ChargeType: chargeType,\n  Account: this.MasterData.Settings.ServiceFyw,\n  ItemNo: this.MasterData.Settings.ServiceItemLabour,\n  Cost: sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]),\n  Value: sum([sr.ResolutionAdditionalCost, sr.ResolutionTransportCost])\n});"
    },
    {
      "expression": "setDepositRequiredAmount(this);"
    },
    {
      "expression": "clearDepositRequiredAmount(this);"
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
        false,
        false
      ]
    },
    {
      "values": [
        "Internal",
        null,
        null,
        "",
        null,
        null,
        "false",
        null
      ],
      "actions": [
        true,
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
        false,
        true
      ]
    },
    {
      "values": [
        "EW",
        null,
        null,
        "",
        null,
        null,
        "false",
        null
      ],
      "actions": [
        true,
        true,
        false,
        true,
        false,
        false,
        false,
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
        false,
        false,
        false,
        false
      ]
    },
    {
      "values": [
        "Supplier",
        "true",
        null,
        null,
        "SI",
        null,
        "false",
        null
      ],
      "actions": [
        true,
        true,
        false,
        true,
        false,
        false,
        false,
        false,
        true,
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
        false
      ]
    },
    {
      "values": [
        "Supplier",
        "false",
        "true",
        null,
        null,
        null,
        "false",
        null
      ],
      "actions": [
        true,
        true,
        false,
        true,
        false,
        false,
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
        "false",
        "false",
        null,
        "",
        null,
        "false",
        null
      ],
      "actions": [
        true,
        true,
        false,
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
        "Internal",
        "",
        "",
        "false",
        null,
        null,
        "true",
        null
      ],
      "actions": [
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
        "",
        "",
        "",
        "true",
        null,
        null,
        "true",
        null
      ],
      "actions": [
        true,
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
        "Installation Charge Furniture",
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
        "Installation Charge Electrical",
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
        "Unicomer Warranty",
        null,
        null,
        null,
        null,
        null,
        "false",
        null
      ],
      "actions": [
        true,
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
        true,
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
        "false",
        "false",
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
        false,
        false,
        false,
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
        "Supplier",
        "true",
        null,
        null,
        "SE",
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
        true,
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
        true
      ]
    }
  ],
  "extensions": "// this.MasterData.Settings.TaxType: Stock includes tax or not.(I or E)\nvar addMonths = function (d, months) {\n    d = new Date(+d);\n    d.setMonth(d.getMonth() + months);\n    return d;\n};\n\nvar insideWarranty = function (sr, months) {\n    return addMonths(sr.ItemDeliveredOn, months) >= new Date();\n};\n\nvar sum = function (list, f) {\n    f = f || function (v) {\n        return v || 0;\n    };\n    return _.reduce(list, function (memo, e) {\n        return memo + (f(e) || 0);\n    }, 0);\n};\n\nvar empty = function (s) {\n    return !s;\n};\n\nvar iif = function (expr, whenTrue, whenFalse) {\n    if (expr) {\n        return whenTrue;\n    } else {\n        return whenFalse;\n    }\n};\n\nvar not = function (f) {\n    return function (s) {\n        return !f(s);\n    };\n};\n\nvar partSourceIsNotInternal = function (part) {\n    return part.Source !== ''Internal'';\n};\n\nvar partSourceIsInternal = function (part) {\n    return part.Source === ''Internal'';\n};\n\nvar partSourceIsExternal = function (part) {\n    return part.Source === ''External'';\n};\n\nvar partSourceIsSalvaged = function (part) {\n    return part.Source === ''Salvaged'';\n};\n\nvar partTotalPrice = function (part) {\n    return part.quantity * part.price;\n};\n\nvar partCostPrice = function (part) {\n    return part.quantity * part.CostPrice;\n};\n\nvar findCost = function (sr, part, matrix) {\n    return _.find(matrix, function (row) {\n        return row.partType === part.type && insideWarranty(sr, row.year * 12);\n    });\n};\n\n// part cost covered by supplier\nvar partSupplierCovered = function (sr, part, matrix, price) {\n    var cost = findCost(sr, part, matrix);\n    if (cost) {\n        if (cost.partPcent) {\n            price = price * cost.partPcent / 100;\n        }\n        if (cost.partVal) {\n            price = Math.min(price, cost.partVal);\n        }\n        return price;\n    }\n    return 0;\n};\n\nvar labourSupplierCovered = function (sr, labour, matrix) {\n    if (!matrix) {\n        return 0;\n    }\n    return _.max(_.map(matrix, function (row) {\n        if (insideWarranty(sr, row.year * 12) && (row.labourPcent || row.labourVal)) {\n            var v = labour;\n            if (row.labourPcent) {\n                v = v * row.labourPcent / 100;\n            }\n            if (row.labourVal) {\n                v = Math.min(v, row.labourVal);\n            }\n            return v;\n        }\n        return 0;\n    })) || 0;\n};\n\nvar labourSupplierRemaining = function (sr, labour, matrix) {\n    var v = labourSupplierCovered(sr, labour, matrix) || 0;\n    return labour - v;\n};\n\nvar partsSumCatalog = function (sr) {\n    return sum(_.filter(sr.Parts, not(partSourceIsNotInternal)), partTotalPrice);\n};\n\nvar partsSumNonCatalog = function (sr) {\n    return sum(_.filter(sr.Parts, partSourceIsNotInternal), partTotalPrice);\n};\n\nvar partsSumExternal = function (sr) {\n    return sum(_.filter(sr.Parts, partSourceIsExternal), partTotalPrice);\n};\n\nvar partsSumSalvaged = function (sr) {\n    return sum(_.filter(sr.Parts, partSourceIsSalvaged), partTotalPrice);\n};\n\nvar partsSumCatalogCostPrice = function (sr) {\n    return sum(_.filter(sr.Parts, not(partSourceIsNotInternal)), partCostPrice);\n};\n\nvar partsSumNonCatalogCostPrice = function (sr) {\n    return sum(_.filter(sr.Parts, partSourceIsNotInternal), partCostPrice);\n};\n\nvar partsSumExternalCostPrice = function (sr) {\n    return sum(_.filter(sr.Parts, partSourceIsExternal), partCostPrice);\n};\n\nvar partsSumSalvagedCostPrice = function (sr) {\n    return sum(_.filter(sr.Parts, partSourceIsSalvaged), partCostPrice);\n};\n\nvar partsChargeWithSupplierMatrix = function (sr, filter, matrix) {\n    var partSplit = _.map(_.filter(sr.Parts, filter), function (part) {\n        var price = partCostPrice(part);\n        var covered = partSupplierCovered(sr, part, matrix, price);\n        return {\n            covered: covered,\n            remaining: price - covered\n        };\n    });\n    return {\n        covered: sum(partSplit, function (s) {\n            return s.covered;\n        }),\n        remaining: sum(partSplit, function (s) {\n            return s.remaining;\n        })\n    };\n};\n\nvar getTaxAmount = function (charge, scope) {\n    if (scope.MasterData.Settings.TaxType !== ''E'') {\n        return 0;\n    }\n\n    return Math.round((charge * scope.MasterData.Settings.TaxRate / 100.0) * 100) / 100;\n};\n\nvar calculateBerCharge = function (scope) {\n    var sr = scope.serviceRequest;\n    var berCost = sr.StockItem.CostPrice;\n    \n    if (sr.ResolutionPrimaryCharge === ''EW'') {\n        var markedUpPrice = markupAmount(sr.StockItem.CostPrice, ''ChargeExtendedWarranty'', scope);\n        berCost = markedUpPrice - (scope.previousRepairCost || 0);\n    }\n\n    if (berCost < 0) {\n        berCost = 0;\n    }\n\n    return berCost;\n};\n\nvar setDepositRequiredAmount = function (scope) {\n    if (!scope.MasterData.LabourCostMatrix || scope.MasterData.LabourCostMatrix.length === 0) {\n        scope.serviceRequest.DepositRequired = 0;\n        return;\n    }\n\n    if (scope.serviceRequest.ResolutionPrimaryCharge && scope.serviceRequest.ResolutionPrimaryCharge !== ''Customer'' && scope.serviceRequest.ResolutionPrimaryCharge !== ''Deliverer'') {\n        return;\n    }\n\n    if (scope.serviceRequest.DepositFromMatrix && scope.serviceRequest.DepositRequired !== scope.serviceRequest.DepositFromMatrix) {\n        return;\n    }\n\n    var sortedCharges = _.sortBy(scope.MasterData.LabourCostMatrix, function (cost) {\n        return cost.ChargeCustomer;\n    });\n\n    scope.serviceRequest.DepositFromMatrix = sortedCharges[0].ChargeCustomer;\n\n    if (scope.serviceRequest.DepositRequired === null || scope.serviceRequest.DepositRequired === undefined) {\n      scope.serviceRequest.DepositRequired = sortedCharges[0].ChargeCustomer;\n    }\n};\n\nvar clearDepositRequiredAmount = function (scope) {\n    scope.serviceRequest.DepositRequired = 0;\n};\n\nvar markupAmount = function (amount, markupField, scope) {\n    if (!scope.MasterData.PartsCostMatrix) {\n        return amount;\n    }\n\n    var charge = _.find(scope.MasterData.PartsCostMatrix, function (row) {\n        return row.RepairType === scope.serviceRequest.RepairType || !scope.serviceRequest.RepairType;\n  });\n\n    if (!charge) {\n        return amount;\n    }\n\n    var markedUp = amount + (amount * (charge[markupField] / 100));\n    return markedUp;\n};\n\nvar markupCostForPart = function (part, markupField, scope) {\n\n    var markedUpPrice = markupAmount(part.CostPrice, markupField, scope);\n    part.price = markedUpPrice;\n};\n\nvar markupCostForExternalPart = function (part, markupField, scope) {\n    if (part.Source === ''Internal'' || !scope.MasterData.PartsCostMatrix) {\n        return;\n    }\n\n    markupCostForPart(part, markupField, scope);\n};\n\nvar markupParts = function (parts, markupField, scope) {\n    _.each(parts, function (part) {\n        markupCostForPart(part, markupField, scope);\n    });\n};\n\nvar markupExternalParts = function (parts, markupField, scope) {\n    _.each(parts, function (part) {\n        markupCostForExternalPart(part, markupField, scope);\n    });\n};\n\n\nvar getLabourChargeMatrixEntry = function (scope) {\n    if (!scope.MasterData.LabourCostMatrix) {\n        scope.serviceRequest.NoCostMatrixData = true;\n        scope.serviceRequest.ResolutionLabourCost = 0;\n        return;\n    }\n\n    if (!scope.serviceRequest.RepairType) {\n        return;\n    }\n\n    var charge = _.find(scope.MasterData.LabourCostMatrix, function (cost) {\n        return cost.RepairType === scope.serviceRequest.RepairType;\n    });\n\n    return charge;\n};\n\nvar isServiceRequest = function (sr) {\n    return sr.Type === ''SI'' || sr.Type === ''SE'';\n};\n\nvar setLabourChargeCustomer = function (scope) {\n\n    if (!isServiceRequest(scope.serviceRequest)) {\n        scope.serviceRequest.NoCostMatrixData = true;\n        scope.serviceRequest.ResolutionLabourCost = 0;\n        return;\n    }\n\n    var charge = getLabourChargeMatrixEntry(scope);\n\n    if (!charge) {\n        scope.serviceRequest.NoCostMatrixData = true;\n        scope.serviceRequest.ResolutionLabourCost = 0;\n        return;\n    }\n\n    scope.serviceRequest.NoCostMatrixData = false;\n    scope.serviceRequest.ResolutionLabourCost = charge.ChargeCustomer;\n};\n\nvar setLabourChargeInternal = function (scope) {\n\n    if (!isServiceRequest(scope.serviceRequest)) {\n        scope.serviceRequest.NoCostMatrixData = true;\n        scope.serviceRequest.ResolutionLabourCost = 0;\n        return;\n    }\n\n    var charge = getLabourChargeMatrixEntry(scope);\n\n    if (!charge) {\n        scope.serviceRequest.NoCostMatrixData = true;\n        scope.serviceRequest.ResolutionLabourCost = 0;\n        return;\n    }\n\n    scope.serviceRequest.NoCostMatrixData = false;\n\n    if (scope.serviceRequest.Type === ''SE'') {\n        scope.serviceRequest.ResolutionLabourCost = charge.ChargeCustomer;\n        return;\n    }\n    if (scope.serviceRequest.AllocationTechnicianIsInternal) {\n        scope.serviceRequest.ResolutionLabourCost = charge.ChargeInternalTech;\n    } else {\n        scope.serviceRequest.ResolutionLabourCost = charge.ChargeContractedTech;\n    }\n};\n\nvar getLabourCost = function (scope) {\n    var charge = getLabourChargeMatrixEntry(scope);\n    if (!charge) {\n        return 0;\n    }\n\n    if (scope.serviceRequest.Type === ''SE'') {\n        return charge.ChargeCustomer;\n    }\n\n    if (scope.serviceRequest.AllocationTechnicianIsInternal) {\n        return charge.ChargeInternalTech;\n    } else {\n        return charge.ChargeContractedTech;\n    }\n};\n\nvar labourEWCovered = function (sr, scope) {\n    var charge = getLabourChargeMatrixEntry(scope);\n    if (!charge) {\n        return 0;\n    }\n\n    var totalLabour = sum([sr.ResolutionLabourCost, sr.ResolutionAdditionalCost, sr.ResolutionTransportCost]);\n\n    return totalLabour > charge.ChargeEWClaim ? charge.ChargeEWClaim : totalLabour;\n};\n\nvar partsMarkupFieldLookup = {\n    ''Internal'': ''ChargeInternal'',\n    ''EW'': ''ChargeExtendedWarranty'',\n    ''Supplier'': ''ChargeFirstYearWarranty'',\n    ''Deliverer'': ''ChargeCustomer'',\n    ''Unicomer Warranty'': ''ChargeFirstYearWarranty''\n};\n\nvar getPartsMarkupField = function (chargeTo) {\n    return partsMarkupFieldLookup[chargeTo];\n};"
}')