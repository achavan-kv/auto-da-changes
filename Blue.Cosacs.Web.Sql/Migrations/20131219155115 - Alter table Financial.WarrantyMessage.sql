-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
DROP TABLE [Financial].[WarrantyMessage]

go

CREATE TABLE [Financial].[WarrantyMessage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ContractNumber] [varchar](20) NOT NULL,
	[DeliveredOn] [date] NOT NULL,
	[AccountType] [char](3) NOT NULL,
	[Department] [char](3) NOT NULL,
	[SalePrice] [decimal](12, 4) NOT NULL,
	[CostPrice] [decimal](12, 4) NOT NULL,
	[BranchNo] [smallint] NOT NULL,
	[WarrantyNo] varchar(20) NOT NULL,
	[WarrantyLength] smallint NOT NULL,
	[MessageId] [int] NOT NULL,
 CONSTRAINT [PK_Financial_WarrantyMessage_Id] PRIMARY KEY CLUSTERED (	[Id] ASC)
)

GO

