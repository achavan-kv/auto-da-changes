SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME = 'LocationStockLevelView1' AND xtype = 'U')
BEGIN 
CREATE TABLE [Merchandising].[LocationStockLevelView1](
	[ProductId] [int] NOT NULL,
	[StockOnHand] [int] NOT NULL,
	[StockOnOrder] [int] NOT NULL,
	[StockAvailable] [int] NOT NULL,
	[StockAllocated] [int] NULL,
	[LocationId] [int] NOT NULL,
	[LocationNumber] [varchar](100) NOT NULL,
	[SalesId] [varchar](100) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Fascia] [varchar](100) NOT NULL,
	[StoreType] [varchar](100) NOT NULL,
	[Warehouse] [bit] NOT NULL,
	[VirtualWarehouse] [bit] NOT NULL
) ON [PRIMARY]

END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Merchandising].[LocationStockLevelView1]') AND NAME = 'ind_prod_LocationStockLevelView')
BEGIN 
CREATE INDEX ind_prod_LocationStockLevelView ON [Merchandising].[LocationStockLevelView1] (ProductId); 

END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Merchandising].[LocationStockLevelView1]') AND NAME = 'ind_LocationStockLevelView')
BEGIN 
CREATE INDEX ind_LocationStockLevelView ON [Merchandising].[LocationStockLevelView1] (ProductId, VirtualWarehouse);

END
GO

