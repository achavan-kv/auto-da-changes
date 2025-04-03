-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE StoreCardStatus
ALTER COLUMN StatusCode VARCHAR(5) NOT NULL
GO
