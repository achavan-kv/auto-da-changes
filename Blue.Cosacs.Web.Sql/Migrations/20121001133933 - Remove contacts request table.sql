ALTER TABLE Service.Request
	DROP COLUMN CustomerContact1, CustomerContactType1, CustomerContact2, CustomerContactType2, CustomerContact3, CustomerContactType3, CustomerContact4, CustomerContactType4
GO
ALTER TABLE Service.Request SET (LOCK_ESCALATION = TABLE)
GO