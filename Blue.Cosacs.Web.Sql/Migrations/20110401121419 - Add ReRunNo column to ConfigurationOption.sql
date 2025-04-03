-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ReRunNo'
               AND OBJECT_NAME(id) = 'EodConfigurationOption')
BEGIN
  ALTER TABLE EodConfigurationOption ADD ReRunNo int
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ReRunNo'
               AND OBJECT_NAME(id) = 'EodConfigurationOptionAudit')
BEGIN
  ALTER TABLE EodConfigurationOptionAudit ADD ReRunNo int
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ReRunTimes'
               AND OBJECT_NAME(id) = 'InterfaceControl')
BEGIN
  ALTER TABLE interfacecontrol ADD ReRunTimes int
END

