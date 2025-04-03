-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[sa_trigcourtsperson]'))
DROP TRIGGER [dbo].[sa_trigcourtsperson]

IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trig_staffmaintenance]'))
DROP TRIGGER [dbo].[trig_staffmaintenance]
