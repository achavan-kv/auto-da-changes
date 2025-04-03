-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if exists (select * from sys.indexes where name = 'ix_delivery_agrmtno_delorcoll')
begin 
    drop index ix_delivery_agrmtno_delorcoll on [dbo].[delivery]
end
go

CREATE NONCLUSTERED INDEX [ix_delivery_agrmtno_delorcoll] ON [dbo].[delivery]
(
	[agrmtno] ASC,
	[delorcoll] ASC,
	[datetrans] ASC
)
INCLUDE ( 	[acctno],
	[datedel],
	[stocklocn],
	[buffno],
	[transvalue],
	[contractno],
	[ftnotes],
	[ItemID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

if exists (select * from sys.indexes where name = 'ix_delivery_datetrans')
begin 
    drop index ix_delivery_datetrans on [dbo].[delivery]
end
go

CREATE NONCLUSTERED INDEX [ix_delivery_datetrans] ON [dbo].[delivery]
(
	[datetrans] ASC
)
INCLUDE ( 	[acctno],
	[agrmtno],
	[datedel],
	[delorcoll],
	[stocklocn],
	[buffno],
	[transvalue],
	[contractno],
	[ftnotes],
	[ItemID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

if exists (select * from sys.indexes where name = 'ix_acct_accttype_incl')
begin 
    drop index ix_acct_accttype_incl on [dbo].[acct]
end
go

CREATE NONCLUSTERED INDEX [ix_acct_accttype_incl] ON [dbo].[acct]
(
	[accttype] ASC
)
INCLUDE ( 	[acctno],
	[dateacctopen]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

if exists (select * from sys.indexes where name = 'ix_agrmtno')
begin 
    drop index ix_agrmtno on [dbo].[agreement]
end
go

CREATE NONCLUSTERED INDEX [ix_agrmtno] ON [dbo].[agreement]
(
	[agrmtno] ASC
)
INCLUDE ( 	[acctno],
	[dateagrmt],
	[empeenosale]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

if exists (select * from sys.indexes where name = 'ix_StockInfo_RepoItem_Incl')
begin 
    drop index ix_StockInfo_RepoItem_Incl on [dbo].[StockInfo]
end
go

CREATE NONCLUSTERED INDEX [ix_StockInfo_RepoItem_Incl] ON [dbo].[StockInfo]
(
	[RepossessedItem] ASC
)
INCLUDE ( 	[IUPC],
	[Class],
	[SubClass],
	[Id]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

