-- transaction: true

IF EXISTS(SELECT * FROM syscolumns WHERE name='TaxAmt' AND object_name(id)='RequestPart')
BEGIN
	ALTER TABLE Service.RequestPart
	DROP COLUMN TaxAmt
END
