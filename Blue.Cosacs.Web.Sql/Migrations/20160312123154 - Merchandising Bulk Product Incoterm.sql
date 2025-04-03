CREATE TABLE [Merchandising].[IncotermStaging](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[Name] [varchar](240) NULL,
	[CurrencyType] [varchar](3) NULL,
	[SupplierUnitCost] [decimal](19, 4) NULL,
	[CountryOfDispatch] [varchar](2) NULL,
	[LeadTime] [varchar](20) NULL,
 CONSTRAINT [PK_Merchandising_IncotermStaging] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
))