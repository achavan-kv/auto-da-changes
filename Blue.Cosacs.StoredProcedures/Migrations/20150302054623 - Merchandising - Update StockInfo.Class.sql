-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


/****** Object:  Index [ix_StockInfo_RepoItem_Incl]    Script Date: 2/03/2015 3:48:05 PM ******/
DROP INDEX [ix_StockInfo_RepoItem_Incl] ON [dbo].[StockInfo]
GO

ALTER TABLE StockInfo
ALTER COLUMN Class varchar(5)

/****** Object:  Index [ix_StockInfo_RepoItem_Incl]    Script Date: 2/03/2015 3:48:05 PM ******/
CREATE NONCLUSTERED INDEX [ix_StockInfo_RepoItem_Incl] ON [dbo].[StockInfo]
(
	[RepossessedItem] ASC
)
INCLUDE ( 	[IUPC],
	[Class],
	[SubClass],
	[Id]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

