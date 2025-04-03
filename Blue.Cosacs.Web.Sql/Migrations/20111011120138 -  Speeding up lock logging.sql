-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS (SELECT * FROM sysindexes WHERE NAME ='ix_locklogfinal')
CREATE CLUSTERED INDEX ix_locklogfinal ON locklogfinal(dateblock,spidlocking,spidlockedout)