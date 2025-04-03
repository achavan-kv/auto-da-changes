-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME='OrderPayment' AND TABLE_SCHEMA = 'Sales' AND COLUMN_NAME='Change') BEGIN
		ALTER TABLE Sales.OrderPayment
			DROP COLUMN Change

END	

GO