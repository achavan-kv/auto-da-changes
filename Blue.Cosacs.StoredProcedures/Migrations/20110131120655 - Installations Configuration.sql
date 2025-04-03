-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists(select * from transtype where transtypecode='INE')
Insert into Transtype (origbr, transtypecode, tccodedr, tccodecr, description, balordue, exportfilesuffix, batchtype, interfaceaccount, interfacesecaccount, branchsplit, isdeposit, interfacebalancing, IncludeinGFT, empeenochange, referencemandatory, referenceunique, interfacesecbalancing, branchsplitbalancing)
values (null,'INE',0,0,'Installation - Electrical','A','C','','','',1,0,'',0,0,0,0,'',1)

if not exists(select * from transtype where transtypecode='INF')
Insert into Transtype (origbr, transtypecode, tccodedr, tccodecr, description, balordue, exportfilesuffix, batchtype, interfaceaccount, interfacesecaccount, branchsplit, isdeposit, interfacebalancing, IncludeinGFT, empeenochange, referencemandatory, referenceunique, interfacesecbalancing, branchsplitbalancing)
values (null,'INF',0,0,'Installation - Furniture','A','C','','','',1,0,'',0,0,0,0,'',1)

go

declare @category INT
select @category=code from code where category='CMC' and codedescript='Service request'

if not exists(select * from CountryMaintenance where codename='InstalChgAcct')
insert into CountryMaintenance (
	CountryCode,
	ParameterCategory,
	[Name],
	Value,
	[Type],
	[Precision],
	OptionCategory,
	OptionListName,
	Description,
	CodeName
) 

select CountryCode,@category,'Installation Charge Account','','text',0,'','','The special account number for Installation Charges','InstalChgAcct'
from country

go

if not exists (SELECT * FROM sys.objects 
	WHERE object_id = OBJECT_ID(N'[dbo].[InstallationChargeTo]') AND type in (N'U'))

CREATE TABLE [dbo].[InstallationChargeTo](
	[InstallationNo] [int] NOT NULL,
	[SortOrder] [smallint] NOT NULL,
	[Electrical] [money] NOT NULL,
	[Furniture] [money] NOT NULL
 CONSTRAINT [PK_InstallationChargeTo] PRIMARY KEY CLUSTERED 
(
	[InstallationNo] ASC,
	[SortOrder] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


GO
