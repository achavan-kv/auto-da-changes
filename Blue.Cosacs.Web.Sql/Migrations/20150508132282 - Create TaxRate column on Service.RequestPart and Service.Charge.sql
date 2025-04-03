-- transaction: true

IF NOT EXISTS(SELECT * FROM syscolumns WHERE name='TaxRate' AND object_schema_name(id)='Service' AND object_name(id)='RequestPart')
BEGIN
	ALTER TABLE Service.RequestPart
	ADD TaxRate dbo.BlueAmount NULL CONSTRAINT DF_Service_RequestPart_TaxRate DEFAULT 0
END

IF NOT EXISTS(SELECT * FROM syscolumns WHERE name='TaxRate' AND object_schema_name(id)='Service' AND object_name(id)='Charge')
BEGIN
	ALTER TABLE Service.Charge
	ADD TaxRate dbo.BlueAmount NULL CONSTRAINT DF_Service_Charge_TaxRate DEFAULT 0
END
