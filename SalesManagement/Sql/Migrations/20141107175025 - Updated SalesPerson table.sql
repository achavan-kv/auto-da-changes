-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE SalesManagement.SalesPerson
	ALTER COLUMN BeggingUnavailable DATE NULL
GO

ALTER TABLE SalesManagement.SalesPerson
	ALTER COLUMN EndUnavailable DATE NULL
GO

ALTER TABLE SalesManagement.SalesPerson
	ALTER COLUMN CreatedBy INT NULL
GO

SP_RENAME 'SalesManagement.SalesPerson.BeggingUnavailable', 'BeginningUnavailable', 'COLUMN'