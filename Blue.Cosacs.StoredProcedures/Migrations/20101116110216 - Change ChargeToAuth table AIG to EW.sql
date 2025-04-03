-- Change DefaultChargeTo from AIG to EW 

UPDATE SR_ChargeToAuthorisation set DefaultChargeTo='EW' Where DefaultChargeTo='AIG'
