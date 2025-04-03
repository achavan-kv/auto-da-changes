IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StoreCard]') AND name = N'IX_StoreCard_Number')
DROP INDEX [IX_StoreCard_Number] ON [dbo].[StoreCard] WITH ( ONLINE = OFF )
GO

alter table StoreCard
  alter column Number bigint not null
go

CREATE UNIQUE NONCLUSTERED INDEX [IX_StoreCard_Number] ON [dbo].[StoreCard] 
(
	[Number] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
