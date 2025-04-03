-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
DELETE FROM SalesManagement.CustomerSalesPerson

GO

ALTER TABLE SalesManagement.CustomerSalesPerson
ADD CustomerBranch smallint NOT NULL
