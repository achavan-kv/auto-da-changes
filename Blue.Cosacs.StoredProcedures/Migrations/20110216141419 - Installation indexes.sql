IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Installation]') AND name = N'IX_Installation_AcctNo')
	CREATE NONCLUSTERED INDEX [IX_Installation_AcctNo] ON [dbo].[Installation] 
	(
		[AcctNo] ASC, [AgreementNo] ASC, [StockLocation] ASC, [ItemNo] ASC
	)
GO


IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Installation]') AND name = N'IX_Installation_Status')
	CREATE NONCLUSTERED INDEX [IX_Installation_Status] ON [dbo].[Installation] 
	(
		[Status] ASC
	)
GO