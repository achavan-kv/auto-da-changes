-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #15183

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'WarrantySale' AND  Column_Name = 'LineItemIdentifier'
           AND TABLE_SCHEMA = 'Warranty')
BEGIN

EXEC sp_rename 
    @objname = 'Warranty.WarrantySale.LineItemIdentifier', 
    @newname = 'WarrantyGroupId', 
    @objtype = 'COLUMN'

END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'Request' AND  Column_Name = 'LineItemIdentifier'
           AND TABLE_SCHEMA = 'Service')
BEGIN
	EXEC sp_rename 
		@objname = 'Service.Request.LineItemIdentifier', 
		@newname = 'WarrantyGroupId', 
		@objtype = 'COLUMN'
END
GO