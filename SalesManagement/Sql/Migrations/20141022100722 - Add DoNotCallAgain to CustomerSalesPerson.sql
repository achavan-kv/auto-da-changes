-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE [SalesManagement].[CustomerSalesPerson]
	ADD DoNotCallAgain Bit NULL
GO
UPDATE [SalesManagement].[CustomerSalesPerson]
	SET DoNotCallAgain = 0
GO
ALTER TABLE [SalesManagement].[CustomerSalesPerson]
	ALTER COLUMN DoNotCallAgain Bit NOT NULL