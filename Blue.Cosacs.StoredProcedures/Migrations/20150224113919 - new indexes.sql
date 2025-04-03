-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.CMWorklistsAcct') AND NAME ='idx_cmworklistsacct_dateto')
    DROP INDEX idx_cmworklistsacct_dateto ON [dbo].[CMWorklistsAcct];
GO

CREATE NONCLUSTERED INDEX idx_cmworklistsacct_dateto
ON [dbo].[CMWorklistsAcct] ([Dateto])
GO

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.follupalloc') AND NAME ='idx_follupalloc_datedealloc')
    DROP INDEX idx_follupalloc_datedealloc ON [dbo].[follupalloc];
GO

CREATE NONCLUSTERED INDEX idx_follupalloc_datedealloc
ON [dbo].[follupalloc] ([datedealloc])
GO

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.custaddress') AND NAME ='idx_custaddress_addtype_datemoved')
    DROP INDEX idx_custaddress_addtype_datemoved ON [dbo].[custaddress];
GO

CREATE NONCLUSTERED INDEX idx_custaddress_addtype_datemoved
ON [dbo].[custaddress] ([addtype],[datemoved])
INCLUDE ([custid],[zone])
GO