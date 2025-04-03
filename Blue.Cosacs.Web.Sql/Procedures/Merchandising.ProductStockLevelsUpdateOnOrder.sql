SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'ProductStockLevelsUpdateOnOrder'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE [Merchandising].[ProductStockLevelsUpdateOnOrder]	
END
GO

-- =========================================================================
-- Author:		Abhijeet Gawali 
-- Create date: 14/02/2020
-- Description:	
-- version :	001 - PO Id's sent to procedure to update stock on order.  
-- =========================================================================

CREATE PROCEDURE [Merchandising].[ProductStockLevelsUpdateOnOrder]		
@POIds IntTVP READONLY		
AS
BEGIN

;WITH GR
AS 
(
	SELECT 
		SUM(rp.QuantityReceived + rp.QuantityCancelled) Received, 
		PurchaseOrderProductId
	FROM Merchandising.GoodsReceiptProduct rp with(nolock)
	GROUP BY PurchaseOrderProductId
)
select StockOnOrder = 
	CASE 
		WHEN Totals.OnOrder < 0 THEN 0 
		ELSE ISNULL(Totals.OnOrder, 0) 
	END, p.ProductId, p.LocationId, Totals.ReceivingLocationId into #TempProductStockLevel
FROM Merchandising.ProductStockLevel p  with(nolock)
LEFT JOIN 
(
	SELECT
		SUM(ISNULL(QuantityOrdered, 0) - ISNULL(QuantityCancelled, 0) - ISNULL(GR.Received, 0)) OnOrder, 
		ProductId, 
		ReceivingLocationId 
	FROM Merchandising.PurchaseOrder po  with(nolock)
	INNER JOIN Merchandising.PurchaseOrderproduct pop  with(nolock)
		ON pop.PurchaseOrderId = po.Id
	LEFT JOIN GR with(nolock) 
		ON GR.PurchaseOrderProductId = pop.id
	WHERE  po.id IN(select Id from @POIds)
	GROUP BY 
		ProductId, 
		ReceivingLocationId
) AS Totals 
	ON p.LocationId = Totals.ReceivingLocationId 
	AND p.ProductId = Totals.ProductId
	
UPDATE ProdstLvl SET stockonOrder = tProdstLvl.stockonorder 
FROM Merchandising.ProductStockLevel  ProdstLvl with (nolock) INNER JOIN #TempProductStockLevel tProdstLvl
ON tProdstLvl.ProductId = ProdstLvl.ProductId AND ProdstLvl.LocationId = tProdstLvl.ReceivingLocationId

DROP TABLE #TempProductStockLevel

END