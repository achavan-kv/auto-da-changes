SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LocationStockLevelView'
           AND xtype = 'V')
BEGIN 
DROP VIEW [Merchandising].[LocationStockLevelView]
END
GO 

-- ========================================================================
-- Version:		<001> 
-- ========================================================================

CREATE VIEW [Merchandising].[LocationStockLevelView]
AS

SELECT 
ProductId,StockOnHand,StockOnOrder,StockAvailable,StockAllocated,
LocationId,LocationNumber,SalesId,Name,Fascia,StoreType,Warehouse,
VirtualWarehouse
FROM [Merchandising].[LocationStockLevelView1] With(NoLock)



GO


