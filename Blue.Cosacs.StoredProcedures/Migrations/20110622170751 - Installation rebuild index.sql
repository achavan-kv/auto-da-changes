-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF EXISTS (SELECT 1 FROM syscolumns
		   WHERE name = 'ItemID' AND OBJECT_NAME(id) = 'Installation')
	EXEC sp_rename
    @objname = 'Installation.ItemID',
    @newname = 'ItemId',
    @objtype = 'COLUMN'		   

IF EXISTS (SELECT 1 FROM syscolumns
		   WHERE name = 'ItemNo' AND OBJECT_NAME(id) = 'Installation')
BEGIN
  IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Installation]') AND name = N'IX_Installation_AcctNo')
	DROP INDEX [IX_Installation_AcctNo] ON [dbo].[Installation] WITH ( ONLINE = OFF )
	
  ALTER TABLE Installation DROP COLUMN ItemNo
  
  CREATE NONCLUSTERED INDEX [IX_Installation_AcctNo] ON [dbo].[Installation] 
	(
		[AcctNo] ASC,
		[AgreementNo] ASC,
		[ItemId] ASC,
		[StockLocation] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
 
END