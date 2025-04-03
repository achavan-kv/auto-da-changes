-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

Alter TABLE SalesCommissionRates alter column ItemText VARCHAR(18) not null

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemId'
               AND OBJECT_NAME(id) = 'SalesCommissionRates')
BEGIN
  ALTER TABLE SalesCommissionRates ADD ItemId INT not null default 0
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'RepoPercentage'
               AND OBJECT_NAME(id) = 'SalesCommissionRates')
BEGIN
  ALTER TABLE SalesCommissionRates ADD RepoPercentage float not null default 0
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'RepoPercentageCash'
               AND OBJECT_NAME(id) = 'SalesCommissionRates')
BEGIN
  ALTER TABLE SalesCommissionRates ADD RepoPercentageCash float not null default 0
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'RepoValue'
               AND OBJECT_NAME(id) = 'SalesCommissionRates')
BEGIN
  ALTER TABLE SalesCommissionRates ADD RepoValue money not null default 0
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'RepoItemId'
               AND OBJECT_NAME(id) = 'SalesCommissionRates')
BEGIN
  ALTER TABLE SalesCommissionRates ADD RepoItemId INT not null default 0
END

go

Alter TABLE SalesCommissionRatesAudit alter column ItemText VARCHAR(18) not null

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemId'
               AND OBJECT_NAME(id) = 'SalesCommissionRatesAudit')
BEGIN
  ALTER TABLE SalesCommissionRatesAudit ADD ItemId INT not null default 0
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'RepoPercentage'
               AND OBJECT_NAME(id) = 'SalesCommissionRatesAudit')
BEGIN
  ALTER TABLE SalesCommissionRatesAudit ADD RepoPercentage float not null default 0
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'RepoPercentageCash'
               AND OBJECT_NAME(id) = 'SalesCommissionRatesAudit')
BEGIN
  ALTER TABLE SalesCommissionRatesAudit ADD RepoPercentageCash float not null default 0
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'RepoValue'
               AND OBJECT_NAME(id) = 'SalesCommissionRatesAudit')
BEGIN
  ALTER TABLE SalesCommissionRatesAudit ADD RepoValue money not null default 0
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'RepoItemId'
               AND OBJECT_NAME(id) = 'SalesCommissionRatesAudit')
BEGIN
  ALTER TABLE SalesCommissionRatesAudit ADD RepoItemId INT not null default 0
END

go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SalesCommissionRates]') AND name = N'pk_SalesCommissionRates')
ALTER TABLE [dbo].[SalesCommissionRates] DROP CONSTRAINT [pk_SalesCommissionRates]
GO

ALTER TABLE [dbo].[SalesCommissionRates] ADD  CONSTRAINT [pk_SalesCommissionRates] PRIMARY KEY CLUSTERED 
(
	[ItemText] ASC,
	[DateFrom] ASC,
	[CommissionType] ASC,
	[ComBranchNo] ASC,
	[ItemId] asc
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

UPDATE SalesCommissionRates
set itemid=s.id 
from SalesCommissionRates r INNER JOIN StockInfo s on ItemText = IUPC
where CommissionType='P' and s.RepossessedItem=0

UPDATE SalesCommissionRatesAudit
set itemid=s.id 
from SalesCommissionRatesAudit r INNER JOIN StockInfo s on ItemText = IUPC
where CommissionType='P' and RepossessedItem=0
go

UPDATE SalesCommissionRates
set RepoPercentage=Percentage, RepoPercentageCash=Percentagecash, RepoValue=Value
go

UPDATE SalesCommissionRates
set RepoItemId=s.id 
from SalesCommissionRates r INNER JOIN StockInfo s on ItemText = IUPC
where CommissionType='P' and s.RepossessedItem=1
go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SalesCommissionRates]') AND name = N'ix_ItemId')
DROP INDEX [ix_ItemId] ON [dbo].[SalesCommissionRates] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [ix_ItemId] ON [dbo].[SalesCommissionRates] 
(
	[ItemId] ASC,
	[DateFrom] ASC,
	[CommissionType] ASC,
	[ComBranchNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


Alter TABLE dbo.SalesCommissionMultiSPIFFRates alter column Item1 VARCHAR(18) not null
Alter TABLE dbo.SalesCommissionMultiSPIFFRates alter column Item2 VARCHAR(18) not null
Alter TABLE dbo.SalesCommissionMultiSPIFFRates alter column Item3 VARCHAR(18) not null
Alter TABLE dbo.SalesCommissionMultiSPIFFRates alter column Item4 VARCHAR(18) not null
Alter TABLE dbo.SalesCommissionMultiSPIFFRates alter column Item5 VARCHAR(18) not null

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemId1'
               AND OBJECT_NAME(id) = 'SalesCommissionMultiSPIFFRates')
BEGIN
  ALTER TABLE SalesCommissionMultiSPIFFRates ADD ItemId1 INT not null default 0
  ALTER TABLE SalesCommissionMultiSPIFFRates ADD ItemId2 INT not null default 0
  ALTER TABLE SalesCommissionMultiSPIFFRates ADD ItemId3 INT not null default 0
  ALTER TABLE SalesCommissionMultiSPIFFRates ADD ItemId4 INT not null default 0
  ALTER TABLE SalesCommissionMultiSPIFFRates ADD ItemId5 INT not null default 0
END
go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SalesCommissionMultiSPIFFRates]') AND name = N'ix_ItemId1')
DROP INDEX [ix_ItemId1] ON [dbo].[SalesCommissionMultiSPIFFRates] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [ix_ItemId1] ON [dbo].[SalesCommissionMultiSPIFFRates] 
(
	[ItemId1] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SalesCommissionMultiSPIFFRates]') AND name = N'ix_ItemId2')
DROP INDEX [ix_ItemId2] ON [dbo].[SalesCommissionMultiSPIFFRates] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [ix_ItemId2] ON [dbo].[SalesCommissionMultiSPIFFRates] 
(
	[ItemId2] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SalesCommissionMultiSPIFFRates]') AND name = N'ix_ItemId3')
DROP INDEX [ix_ItemId3] ON [dbo].[SalesCommissionMultiSPIFFRates] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [ix_ItemId3] ON [dbo].[SalesCommissionMultiSPIFFRates] 
(
	[ItemId3] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SalesCommissionMultiSPIFFRates]') AND name = N'ix_ItemId4')
DROP INDEX [ix_ItemId4] ON [dbo].[SalesCommissionMultiSPIFFRates] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [ix_ItemId4] ON [dbo].[SalesCommissionMultiSPIFFRates] 
(
	[ItemId4] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SalesCommissionMultiSPIFFRates]') AND name = N'ix_ItemId5')
DROP INDEX [ix_ItemId5] ON [dbo].[SalesCommissionMultiSPIFFRates] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [ix_ItemId5] ON [dbo].[SalesCommissionMultiSPIFFRates] 
(
	[ItemId5] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

Alter TABLE dbo.SalesCommission alter column ItemNo VARCHAR(18) not null

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemId'
               AND OBJECT_NAME(id) = 'SalesCommission')
BEGIN
  ALTER TABLE SalesCommission ADD ItemId INT not null default 0
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'RepossessedItem'
               AND OBJECT_NAME(id) = 'SalesCommission')
BEGIN
  ALTER TABLE SalesCommission ADD RepossessedItem bit not null default 0
END

go

UPDATE SalesCommission
set itemid=s.id 
from SalesCommission r INNER JOIN StockInfo s on r.ItemNo = s.itemno
--where CommissionType='P' and s.RepossessedItem=0

go
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SalesCommission]') AND name = N'pk_SalesCommission')
ALTER TABLE [dbo].[SalesCommission] DROP CONSTRAINT [pk_SalesCommission]
GO

ALTER TABLE [dbo].[SalesCommission] ADD  CONSTRAINT [pk_SalesCommission] PRIMARY KEY CLUSTERED 
(
	[Employee] ASC,
	[RunDate] ASC,
	[AcctNo] ASC,
	[AgrmtNo] ASC,
	[ItemId] ASC,
	[CommissionType] ASC,
	[CommissionAmount] ASC,
	[Buffno] ASC,
	[ContractNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

declare @ParmCat INT 

select @parmCat=code from code where category='CMC' and codedescript='Sales Commissions'

IF NOT EXISTS(select * from countrymaintenance where codename = 'ComRepoItem')
BEGIN
	insert into countrymaintenance (CountryCode, 
									ParameterCategory, 
									Name, 
									Value, 
									Type, 
									Precision, 
									OptionCategory, 
									OptionListName,
									Description, 
									CodeName)
	select
		  countrycode, 
		 @parmcat,
		 'Repossessed products commission rate',
		 'false',
		 'checkbox',
		  0,
		  '',
		  '',
		  'Allows different commission rates to be applied to Regular products and Repossessed products. If false Repossessed products will have the same commission rates as Regular products.',
		  'ComRepoItem'	  
	from country
		 
END


