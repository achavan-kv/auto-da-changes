-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'DepositRequired' AND [object_id] = OBJECT_ID(N'Admin.User'))
BEGIN
ALTER TABLE [Admin].[User]
ADD 
	[DepositRequired] BIT NOT NULL DEFAULT(0)
END