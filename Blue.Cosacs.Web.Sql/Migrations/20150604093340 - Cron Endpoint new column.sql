-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'Module' AND [object_id] = OBJECT_ID(N'[Cron].[Endpoint]')) 
	BEGIN
	ALTER TABLE [Cron].[Endpoint] 
	ADD Module Varchar(50) NULL
END
GO

UPDATE [Cron].[Endpoint] 
SET
Module = 'Sales Management'
WHERE Id in (100,101,102,103,104,105,107,109)

UPDATE [Cron].[Endpoint] 
SET
Module = 'Customer'
WHERE Id = 106


UPDATE [Cron].[Endpoint] 
SET
Module = 'Merchandising'
WHERE Id in (200,201,202,203,204,205,206,207,208,209)

UPDATE [Cron].[Endpoint] 
SET
Module = 'Non Stocks'
WHERE Id = 250

UPDATE [Cron].[Endpoint] 
SET
Module = 'Sales'
WHERE Id = 800

UPDATE [Cron].[Endpoint] 
SET
Module = 'Communication'
WHERE Id = 108


ALTER TABLE [Cron].[Endpoint] 
ALTER COLUMN Module Varchar(50) NOT NULL

