-- transaction: true

IF NOT EXISTS(SELECT * FROM syscolumns WHERE name='TaxAmount' AND object_name(id)='RequestPart')
BEGIN
	ALTER TABLE Service.RequestPart
	ADD TaxAmount dbo.BlueAmount NULL
END
