-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
ALTER TABLE SalesManagement.Call
	DROP COLUMN MobileExtension, MobileDialCode, LandLineExtension, LandLineDialCode
GO

ALTER TABLE SalesManagement.CustomerSalesPerson
	DROP COLUMN MobileExtension, MobileDialCode, LandLineExtension, LandLineDialCode
