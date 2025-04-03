-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS (SELECT 1 FROM syscolumns
			   WHERE name = 'RIFileTransfer'
               AND OBJECT_NAME(id) = 'country')
ALTER TABLE dbo.country
ADD RIFileTransfer bit NOT NULL DEFAULT(0)