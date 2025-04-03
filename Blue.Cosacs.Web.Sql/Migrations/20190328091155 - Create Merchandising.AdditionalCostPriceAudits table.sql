IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'AdditionalCostPriceAudits'
               AND TABLE_SCHEMA = 'Merchandising')
BEGIN
CREATE TABLE [Merchandising].[AdditionalCostPriceAudits](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AdditionalCostId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[VendorId] [nvarchar](50) NOT NULL,
	[VendorCost] [decimal](18, 5) NOT NULL,
	[VendorCurrency] [nvarchar](50) NOT NULL,
	[LastLandedCost] [decimal](18, 5) NOT NULL,
	[AvgWeightedCost] [decimal](18, 5) NOT NULL,
	[DateAdded] [datetime] NOT NULL,
	[AddedBy] [nvarchar](50) NOT NULL
) ON [PRIMARY]
END
GO
