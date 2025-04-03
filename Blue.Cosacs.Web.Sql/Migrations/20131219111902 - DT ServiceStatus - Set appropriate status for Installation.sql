insert into Config.DecisionTable
([Key], CreatedUtc, [Value])
values ('SR.DecisionTable.ServiceStatus', getdate(),
'{
  "conditions": [
    {
      "expression": "\"Evaluation section is completed\" &&\n(!isEmpty(this.serviceRequest.EvaluationLocation) &&\n!isEmpty(this.serviceRequest.EvaluationAction) &&\n!isEmpty(this.serviceRequest.EvaluationClaimFoodLoss) &&\n!isEmpty(this.serviceRequest.ScriptAnswer)) ||\nisInstallation(this.serviceRequest)"
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
      "expression": "\"Resolution section is completed\" &&\n!isEmpty(this.serviceRequest.ResolutionDate)&&\ndateIsValid(this.serviceRequest.ResolutionDate)&&\n!isEmpty(this.serviceRequest.ResolutionPrimaryCharge)&&\n\n!isEmpty(this.serviceRequest.ItemSerialNumber)&&\n!isEmpty(this.serviceRequest.ResolutionLabourCost)&&\n\n(this.serviceRequest.ResolutionPrimaryCharge !== ''Supplier'' || this.serviceRequest.ResolutionPrimaryCharge === ''Supplier'' && !isEmpty(this.serviceRequest.ResolutionCategory))"
    },
    {
      "expression": "\"Finalize section is completed\"&& !isEmpty(this.serviceRequest.FinalisedFailure)&& dateIsValid(this.serviceRequest.FinaliseReturnDate)"
    },
    {
      "expression": "isEmpty(this.serviceRequest.State)"
    },
    {
      "expression": "(this.serviceRequest.ResolutionPrimaryCharge === ''Deliverer'' || this.serviceRequest.ResolutionPrimaryCharge === ''Customer'') &&  (this.OutstandingBalance > 0)"
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
      "expression": "if (!isInstallation(this.serviceRequest)) {\n\tthis.serviceRequest.State = ''Awaiting repair'';\n} else {\n\tthis.serviceRequest.State = ''Awaiting installation'';\n}"
    },
    {
      "expression": "if (!isInstallation(this.serviceRequest)) {\n\tthis.serviceRequest.State = ''Repair overdue'';\n} else {\n\tthis.serviceRequest.State = ''Installation overdue'';\n}"
    },
    {
      "expression": "if (!isInstallation(this.serviceRequest)) {\n\tthis.serviceRequest.State = ''Resolved'';\n} else {\n\tthis.serviceRequest.State = ''Installed'';\n}"
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
  "extensions": "var isEmpty = function(val) {\n    return typeof val === ''undefined'' ||\n                   val === null ||\n                   val === \"\" ||\n                   (typeof val.length === ''undefined'' && val.length === 0);\n};\nvar dateIsValid = function(val) {\n    return val !== undefined && val !== null && window.moment(val).isValid();\n};\n\nvar isInstallation = function (sr) {\n\treturn sr.Type === ''II'' || sr.Type === ''IE'';\n};"
}')