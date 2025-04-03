-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(SELECT * FROM sys.columns WHERE [name] = N'Probation' AND [object_id] = OBJECT_ID(N'Admin.User')) 
BEGIN
    ALTER TABLE [Admin].[User]
    ADD Probation BIT NULL
END

