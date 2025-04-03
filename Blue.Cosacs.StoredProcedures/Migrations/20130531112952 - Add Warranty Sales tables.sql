create table Warranty.WarrantySale
(
	Id int IDENTITY(1,1) NOT NULL,
	[InvoiceNumber] [varchar](50) NULL,
	[SaleBranch] [smallint] NOT NULL,
	[SoldOn] [smalldatetime] NULL,
	[SoldBy] [varchar](50) NULL,
	[CustomerAccount] [char](12) NULL,
	[CustomerId] [varchar](50) NULL,
	[CustomerTitle] [varchar](25) NULL,
	[CustomerFirstName] [varchar](50) NULL,
	[CustomerLastName] [varchar](50) NULL,
	[CustomerAddressLine1] [varchar](50) NULL,
	[CustomerAddressLine2] [varchar](50) NULL,
	[CustomerAddressLine3] [varchar](50) NULL,
	[CustomerPostcode] [varchar](10) NULL,
	[ItemId] int NULL,
	[ItemNumber] [varchar](25) NULL,
	[ItemUPC] [varchar](25) NULL,
	[ItemPrice] [money] NULL,
	[ItemDescription] [varchar](100) NULL,
	[ItemBrand] [varchar](50) NULL,
	[ItemModel] [varchar](50) NULL,
	[ItemSupplier] [varchar](50) NULL,
	[WarrantyContractNo] [varchar](10) NULL,
	[WarrantyId] [int] NULL,
	[WarrantyNumber] [varchar](20) NULL,
	[WarrantyLength] [smallint] NULL,
	[WarrantyTaxRate] [decimal](4, 2) NOT NULL,
	[WarrantyIsFree] [bit] NULL,
	[WarrantyCostPrice] [money] NOT NULL,
	[WarrantyRetailPrice] [money] NOT NULL,
	[WarrantySalePrice] [money] NOT NULL
)

ALTER TABLE Warranty.WarrantySale
ADD CONSTRAINT [PK_WarrantySale_Id] PRIMARY KEY (Id)

CREATE TABLE [Warranty].[WarrantyContact](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WarrantySaleId] [int] NOT NULL,
	[Type] [varchar](20) NOT NULL,
	[Value] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Warranty_Contact] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [Warranty].[WarrantyContact]  WITH CHECK ADD  CONSTRAINT [FK_WarrantySale_Contact] FOREIGN KEY([WarrantySaleId])
REFERENCES [Warranty].[Warranty] ([Id])
ON DELETE CASCADE
