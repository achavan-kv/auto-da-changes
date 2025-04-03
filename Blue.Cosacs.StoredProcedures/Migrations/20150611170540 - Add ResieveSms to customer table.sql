-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE dbo.customer 
	ADD ResieveSms bit NULL

go

ALTER TABLE dbo.customer ADD CONSTRAINT
	DF_customer_ResieveSms DEFAULT 1 FOR ResieveSms
GO

UPDATE dbo.Customer 
SET ResieveSms = 1
GO