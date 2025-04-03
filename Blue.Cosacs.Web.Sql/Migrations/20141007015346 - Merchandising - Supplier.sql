-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE [Merchandising].[Supplier](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SupplierTypeId] [int] NOT NULL,
	[AddressLine1] [varchar](100) NOT NULL,
	[AddressLine2] [varchar](100) NOT NULL,
	[City] [varchar](100) NOT NULL,
	[PostCode] [varchar](15) NOT NULL,
	[PaymentTerms] [varchar](15) NULL,
	[OrderEmail] [varchar](100) NULL,
	[Contacts] [varchar](Max) NULL,
 CONSTRAINT [pk_supplier] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [Merchandising].[Supplier]  WITH CHECK ADD  CONSTRAINT [FK_Supplier_SupplierType] FOREIGN KEY([SupplierTypeId])
REFERENCES [Merchandising].[SupplierType] ([Id])

ALTER TABLE [Merchandising].[Supplier] CHECK CONSTRAINT [FK_Supplier_SupplierType]
