SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'GetLocationStockLevel'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE [Merchandising].[GetLocationStockLevel]
END
GO
-- ========================================================================
-- Version:		<001> 
-- ========================================================================
CREATE PROCEDURE [Merchandising].[GetLocationStockLevel]
AS
TRUNCATE TABLE [Merchandising].[LocationStockLevelView1]

INSERT INTO [Merchandising].[LocationStockLevelView1]
SELECT p.Id as ProductId
,ISNULL(psl.StockOnHand,0) as StockOnHand
,ISNULL(psl.StockOnOrder,0) as StockOnOrder
,ISNULL(psl.StockAvailable,0) as StockAvailable
,ISNULL(psl.StockOnHand,0) - ISNULL(psl.StockAvailable,0) as StockAllocated
,l.Id as LocationId
,l.LocationId as LocationNumber
,l.SalesId
,l.Name
,l.Fascia
,l.StoreType
,l.Warehouse
,l.VirtualWarehouse
FROM Merchandising.Product p
CROSS JOIN Merchandising.Location l
LEFT JOIN Merchandising.ProductStockLevel psl
on psl.ProductId = p.id AND psl.LocationId = l.Id
WHERE p.[Status] in (1,2,3,4,6) 

