-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

drop table Financial.TransactionMappingWarranty

GO

CREATE TABLE [Financial].[TransactionMappingWarranty](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccountType] [varchar](6) NOT NULL,
	[Department] [char](3) NOT NULL,
	[Percentage] [decimal](3, 2) NOT NULL,
	[Cancelation] [bit] NOT NULL,
	[CostOrSale] [varchar](4) NOT NULL,
	[Account] [varchar](4) NOT NULL,
	[TransactionType] [char](3) NOT NULL,
 CONSTRAINT [PK_TransactionMappingWarranty] PRIMARY KEY CLUSTERED ( [Id] ASC)
)

GO

insert into [Financial].[TransactionMappingWarranty]
(AccountType, Department, Percentage, Cancelation, CostOrSale, Account, TransactionType)
values 
('Credit', 'PCE', 1, 0, 'Sale', '1301', 'BHW'),
('Credit', 'PCE', -1, 0, 'Sale', '5112', 'BHW'),
('Credit', 'PCE', 1, 0, 'Cost', '6012', 'COW'),
('Credit', 'PCE', -1, 0, 'Cost', '2910', 'COW'),
('Cash', 'PCE', 1, 0, 'Sale', '1301', 'BCW'),
('Cash', 'PCE', -1, 0, 'Sale', '5212', 'BCW'),
('Cash', 'PCE', 1, 0, 'Cost', '6012', 'COW'),
('Cash', 'PCE', -1, 0, 'Cost', '2910', 'COW'),
('Cash', 'PCF', 1, 0, 'Sale', '1301', 'BCW'),
('Cash', 'PCF', -1, 0, 'Sale', '5282', 'BCW'),
('Cash', 'PCF', 1, 0, 'Cost', '6082', 'COW'),
('Cash', 'PCF', -1, 0, 'Cost', '2910', 'COW'),
('Credit', 'PCF', 1, 0, 'Sale', '1301', 'BHW'),
('Credit', 'PCF', -1, 0, 'Sale', '5182', 'BHW'),
('Credit', 'PCF', 1, 0, 'Cost', '6082', 'COW'),
('Credit', 'PCF', -1, 0, 'Cost', '2910', 'COW'),
('Credit', 'PCE', -1, 1, 'Sale', '1301', 'CRE'),
('Credit', 'PCE', 1, 1, 'Sale', '5112', 'CRE'),
('Credit', 'PCE', -1, 1, 'Cost', '6012', 'COW'),
('Credit', 'PCE', 1, 1, 'Cost', '2910', 'COW'),
('Cash', 'PCE', -1, 1, 'Sale', '1301', 'CRE'),
('Cash', 'PCE', 1, 1, 'Sale', '5212', 'CRE'),
('Cash', 'PCE', -1, 1, 'Cost', '6012', 'COW'),
('Cash', 'PCE', 1, 1, 'Cost', '2910', 'COW'),
('Cash', 'PCF', -1, 1, 'Sale', '1301', 'CRF'),
('Cash', 'PCF', 1, 1, 'Sale', '5282', 'CRF'),
('Cash', 'PCF', -1, 1, 'Cost', '6082', 'COW'),
('Cash', 'PCF', 1, 1, 'Cost', '2910', 'COW'),
('Credit', 'PCF', -1, 1, 'Sale', '1301', 'CRF'),
('Credit', 'PCF', 1, 1, 'Sale', '5182', 'CRF'),
('Credit', 'PCF', -1, 1, 'Cost', '6082', 'COW'),
('Credit', 'PCF', 1, 1, 'Cost', '2910', 'COW')
GO

drop table Financial.WarrantyMessage

go

CREATE TABLE [Financial].[WarrantyMessage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ContractNumber] [varchar](20) NOT NULL,
	[DeliveredOn] date NOT NULL,
	[AccountType] [char](3) NOT NULL,
	[Department] [char](3) NOT NULL,
	[SalePrice] [decimal](12, 4) NOT NULL,
	[CostPrice] [decimal](12, 4) NOT NULL,
	[BranchNo] [smallint] NOT NULL,
	[MessageId] [int] NOT NULL,
 CONSTRAINT [PK_Financial_WarrantyMessage_Id] PRIMARY KEY CLUSTERED ([id] ASC)
)

GO
