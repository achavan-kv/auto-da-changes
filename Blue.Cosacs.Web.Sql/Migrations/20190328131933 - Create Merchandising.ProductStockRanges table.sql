IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'ProductStockRanges'
               AND TABLE_SCHEMA = 'Merchandising')
BEGIN
CREATE TABLE [Merchandising].[ProductStockRanges](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[SKU] [varchar](20) NOT NULL,
	[MinVal] [int] NOT NULL CONSTRAINT [DF_ProductStockRanges_MinVal]  DEFAULT ((0)),
	[MaxVal] [int] NOT NULL CONSTRAINT [DF_ProductStockRanges_MaxVal]  DEFAULT ((0)),
 CONSTRAINT [PK_Merchandising.ProductStockRanges] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

