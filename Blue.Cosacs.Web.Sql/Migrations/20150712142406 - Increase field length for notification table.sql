-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE [Admin].[Notification]
	ALTER COLUMN Subject varchar(200) NOT NULL;

ALTER TABLE [Admin].[Notification]
	ALTER COLUMN Body varchar(350) NOT NULL;

ALTER TABLE [Admin].[Notification]
	ALTER COLUMN ComplexMessage varchar(800) SPARSE NULL;