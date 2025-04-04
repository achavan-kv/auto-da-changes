insert into Config.DecisionTable
([Key], CreatedUtc, [Value])
values ('POS.DecisionTable.Payment', getdate(),
'{
  "conditions": [
    {
      "expression": "lastDigit(this.payment.payMethod) === ''2'' && parseInt(this.payment.payMethod) < 100"
    },
    {
      "expression": "lastDigit(this.payment.payMethod) === ''3'' || lastDigit(this.payment.payMethod) === ''4'' || lastDigit(this.payment.payMethod) === ''9''"
    },
    {
      "expression": "parseInt(this.payment.payMethod) === 100"
    },
    {
      "expression": "lastDigit(this.payment.payMethod) === ''1''"
    },
    {
      "expression": "parseInt(this.payment.payMethod) === 102"
    },
    {
      "expression": "lastDigit(this.payment.payMethod) === ''5''"
    },
    {
      "expression": "lastDigit(this.payment.payMethod) === ''7''"
    },
    {
      "expression": "lastDigit(this.payment.payMethod) === ''8''"
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
        false,
        false,
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
        false
      ]
    }
  ],
  "extensions": "var lastDigit = function(value) {\n  if (value) {\n    value = value.toString();\n    return value.substring(value.length - 1, value.length);\n  }\n  return null;\n};"
}')