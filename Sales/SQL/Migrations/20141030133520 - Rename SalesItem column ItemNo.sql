-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME='OrderItem' AND TABLE_SCHEMA = 'Sales' AND COLUMN_NAME='ItermNo') BEGIN
	EXECUTE sp_rename N'Sales.OrderItem.ItermNo', N'Tmp_ItemNo_1', 'COLUMN' 

	EXECUTE sp_rename N'Sales.OrderItem.Tmp_ItemNo_1', N'ItemNo', 'COLUMN' 

END
GO