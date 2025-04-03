-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF EXISTS (SELECT * FROM sysindexes WHERE NAME = 'ix_accountlocking_lockcount')
DROP INDEX accountlocking.ix_accountlocking_lockcount