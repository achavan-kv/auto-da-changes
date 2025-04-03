-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #12829


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
      "expression": "\"Inside Supplier Warranty\" &&\nresolutionValidForWarranty(this.serviceRequest.Resolution) &&\ninsideWarranty(this.serviceRequest, this.serviceRequest.ManWarrantyLength || 12)"
    },
    {
      "expression": "\"Inside Extended Warranty\" &&\nresolutionValidForWarranty(this.serviceRequest.Resolution) &&\nthis.serviceRequest.WarrantyContractId &&\ninsideWarranty(this.serviceRequest, this.serviceRequest.WarrantyLength || 0)"
    },
    {
      "expression": "validSRType(this.serviceRequest.Type)"
    }
  ],
  "actions": [
    {
      "expression": "chargeTos(this, [{\n\tkey: ''Customer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Deliverer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''FYW'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Internal'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Supplier'',\n\tauthorisationRequired: false\n}]);\n\nthis.serviceRequest.ResolutionPrimaryCharge = ''Customer'';"
    },
    {
      "expression": "chargeTos(this, [{\n\tkey: ''Customer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Deliverer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''FYW'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Internal'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Supplier'',\n\tauthorisationRequired: false\n}]);\n\nthis.serviceRequest.ResolutionPrimaryCharge = ''Deliverer'';"
    },
    {
      "expression": "chargeTos(this, [{\n\tkey: ''Customer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Deliverer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''FYW'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Internal'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Supplier'',\n\tauthorisationRequired: false\n}]);\n\nthis.serviceRequest.ResolutionPrimaryCharge = ''Internal'';"
    },
    {
      "expression": "chargeTos(this, [{\n\tkey: ''Customer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Deliverer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''FYW'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Internal'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Supplier'',\n\tauthorisationRequired: true\n}]);\n\nthis.serviceRequest.ResolutionPrimaryCharge = ''Customer'';"
    },
    {
      "expression": "chargeTos(this, [{\n\tkey: ''Customer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Deliverer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''FYW'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Internal'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Supplier'',\n\tauthorisationRequired: true\n}]);\n\nthis.serviceRequest.ResolutionPrimaryCharge = ''Supplier'';"
    },
    {
      "expression": "chargeTos(this, [{\n\tkey: ''Customer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Deliverer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''EW'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''FYW'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Internal'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Supplier'',\n\tauthorisationRequired: false\n}]);\n\nthis.serviceRequest.ResolutionPrimaryCharge = ''EW'';"
    },
    {
      "expression": "chargeTos(this, [{\n\tkey: ''Customer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Deliverer'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''FYW'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Internal'',\n\tauthorisationRequired: false\n}, {\n\tkey: ''Supplier'',\n\tauthorisationRequired: true\n}]);\n\nthis.serviceRequest.ResolutionPrimaryCharge = ''Customer'';"
    }
  ],
  "rules": [
    {
      "values": [
        "",
        null,
        null,
        "true"
      ],
      "actions": [
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
        "Damage On Delivery",
        null,
        null,
        "true"
      ],
      "actions": [
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
        "Beyond Economic Repair",
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
        false
      ]
    },
    {
      "values": [
        "Misuse by the customer",
        null,
        null,
        "true"
      ],
      "actions": [
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
        "Event or terms NOT covered",
        null,
        null,
        "true"
      ],
      "actions": [
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
        "true",
        "",
        "true"
      ],
      "actions": [
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
        "true",
        "true"
      ],
      "actions": [
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
        null
      ],
      "actions": [
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
  "extensions": "var addMonths = function (d, months) {\n  d = new Date(+d);\n  d.setMonth(d.getMonth() + months);\n  return d;\n};\n\nvar insideWarranty = function (sr, months) {\n  return addMonths(sr.ItemDeliveredOn, months) >= new Date();\n};\n\nvar chargeTos = function(scope, list) {\n scope.MasterData.ServiceChargeTos = list;\n};\n\nvar resolutionValidForWarranty = function (resolution) {\n  return resolution !== ''Damage On Delivery'' && resolution !== ''Beyond Economic Repair'' && resolution !== ''Misuse by the customer'' && resolution !== ''Event or terms NOT covered'';\n};\n\nvar validSRType = function (srType) {\n  return srType === null || srType === ''SI'' || srType === ''SE'' || srType === ''S''\n};"
}')
          
          
