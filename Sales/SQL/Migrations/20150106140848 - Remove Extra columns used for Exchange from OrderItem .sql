-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME='OrderItem' AND TABLE_SCHEMA = 'Sales' AND COLUMN_NAME='OriginalId') BEGIN
		ALTER TABLE Sales.OrderItem
			DROP COLUMN OriginalId

END	

GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME='OrderItem' AND TABLE_SCHEMA = 'Sales' AND COLUMN_NAME='ManualItemId') BEGIN
		ALTER TABLE Sales.OrderItem
			DROP COLUMN ManualItemId

END	

GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME='OrderItem' AND TABLE_SCHEMA = 'Sales' AND COLUMN_NAME='Exchanged') BEGIN
		ALTER TABLE Sales.OrderItem
			DROP COLUMN Exchanged

END	

GO