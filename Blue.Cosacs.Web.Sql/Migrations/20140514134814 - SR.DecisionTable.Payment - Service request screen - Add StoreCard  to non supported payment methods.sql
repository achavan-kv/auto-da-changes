insert into Config.DecisionTable
([Key], CreatedUtc, [Value])
values ('SR.DecisionTable.Payment', getdate(),
'{
  "conditions": [
    {
      "expression": "\"Cheque\" &&\nparseInt(this.payMethod) === 2"
    },
    {
      "expression": "\"Credit Card / Debit Card\" &&\nparseInt(this.payMethod) === 3 || parseInt(this.payMethod) === 4"
    },
    {
      "expression": "\"US$\" &&\nparseInt(this.payMethod) === 100"
    },
    {
      "expression": "\"Cash\" &&\nparseInt(this.payMethod) === 1"
    },
    {
      "expression": "\"US$ Cheque\" &&\nparseInt(this.payMethod) === 102"
    },
    {
      "expression": "\"Standing Order\" &&\nparseInt(this.payMethod) === 5"
    },
    {
      "expression": "\"Cheque Bank Darft\" &&\nparseInt(this.payMethod) === 7"
    },
    {
      "expression": "\"Gift Voucher\" &&\nparseInt(this.payMethod) === 8"
    },
    {
      "expression": "\"Not Applicable\" &&\nthis.payMethod === undefined || parseInt(this.payMethod) === 0"
    },
    {
      "expression": "\"Credit Card BNS / Debit Card BNS\" &&\nparseInt(this.payMethod) === 6 || parseInt(this.payMethod) === 9"
    },
    {
      "expression": "\"EPAY-ePayment\" &&\nparseInt(this.payMethod) === 11"
    },
    {
      "expression": "\"Store Card\" &&\nparseInt(this.payMethod) === 13"
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
    },
    {
      "expression": "this.sections.paymentMethodNotSupported.visible = true"
    },
    {
      "expression": "this.sections.paymentMethodNotSupported.visible = false"
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
        true
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
        "true",
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
        false,
        true,
        false,
        true,
        false,
        true,
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
        null,
        "true",
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
        false,
        true,
        false,
        true,
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
        true,
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
        null,
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
        null,
        null,
        null,
        "true"
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
        false,
        true,
        false,
        true,
        false,
        true,
        false,
        true,
        true,
        false
      ]
    }
  ],
  "extensions": "var lastDigit = function(value) {\n  if (value) {\n    value = value.toString();\n    return value.substring(value.length - 1, value.length);\n  }\n  return null;\n};"
}')
