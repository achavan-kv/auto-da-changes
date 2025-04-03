-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE TYPE dbo.BlueAmount FROM decimal(19, 3) NULL
GO
CREATE TYPE dbo.BluePercentage FROM decimal(5, 2) NULL
GO
