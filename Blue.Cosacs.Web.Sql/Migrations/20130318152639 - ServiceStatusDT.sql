INSERT INTO Config.DecisionTable
        ( [Key], CreatedUtc, Value )
VALUES  ( 'SR.DecisionTable.ServiceStatus', -- Key - varchar(50)
          Getdate(), -- CreatedUtc - datetime
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
        "false",
        "",
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
        false
      ]
    },
    {
      "values": [
        "false",
        "true",
        "false",
        "false",
        "false",
        "false",
        "false",
        "",
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
        false
      ]
    },
    {
      "values": [
        "true",
        "true",
        "false",
        "false",
        false,
        false,
        false,
        false,
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
        "",
        "true",
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
        true
      ]
    }
  ],
  "extensions": "var isEmpty = function(val) {\n    return typeof val === ''undefined'' ||\n                   val === null ||\n                   val === \"\" ||\n                   (typeof val.length === ''undefined'' && val.length === 0);\n};\nvar dateIsValid = function(val) {\n    return window.moment(val).isValid();\n};"
}'
		  )