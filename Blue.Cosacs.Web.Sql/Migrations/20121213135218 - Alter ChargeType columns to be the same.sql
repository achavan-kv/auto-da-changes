-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(select * from syscolumns where name = 'ChargeType' and object_name(id) = 'ServiceChargeAcct')
BEGIN
	ALTER TABLE ServiceChargeAcct ALTER COLUMN ChargeType VARCHAR(30) NOT NULL
END	
GO


IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'Request' AND    Column_Name = 'ResolutionPrimaryCharge'
           AND TABLE_SCHEMA = 'Service')
BEGIN
	ALTER TABLE Service.Request ALTER COLUMN ResolutionPrimaryCharge VARCHAR(30) NULL
END


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'Request' AND  Column_Name = 'IsPaymentRequired'
           AND TABLE_SCHEMA = 'Service')
BEGIN
	ALTER TABLE Service.Request ADD IsPaymentRequired BIT NULL
END
