
GO

IF EXISTS (
		SELECT 1
		FROM dbo.sysobjects
		WHERE id = object_id(N'[Merchandising].[StockCountProductView]')
			AND OBJECTPROPERTY(id, N'IsView') = 1
		)
	DROP VIEW [Merchandising].[StockCountProductView]
GO

-- =============================================
-- CREATED BY:	   ANUSHREE URKUNDE
-- CREATE DATE:	   14/07/2020
-- SCRIPT Name:    StockCountProductView.sql
-- SCRIPT COMMENT: Created view for Capturing corporate UPC by changing vendor UPC for StockCount label.
-- Discription:    Scanning using corporate UPC - CR
-- ==============================================
CREATE VIEW [Merchandising].[StockCountProductView]
AS
SELECT scp.Id
	,scp.StockCountId
	,scp.ProductId
	,p.Sku
	,REPLACE(REPLACE(p.LongDescription, CHAR(10), ''), '  ', ' ') AS [LongDescription]
	--,REPLACE(p.LongDescription, char(32), '') AS [LongDescription] 
	,p.Attributes
	,p.CorporateUPC
	,scp.StartStockOnHand
	,scp.[Count]
	,scp.SystemAdjustment
	,scp.Count + scp.SystemAdjustment - scp.StartStockOnHand AS Variance
	,ISNULL(psl.StockOnHand, 0) - StartStockOnHand AS NetMovement
	,ISNULL(psl.StockOnHand, 0) AS CurrentStockOnHand
	,scp.Comments
	,sc.LocationId
	,sc.[Type]
	,sc.CountDate
	,sc.ClosedDate
	,l.Name AS LocationName
	,l.Fascia
	,scp.Hierarchy
FROM Merchandising.Product p
JOIN Merchandising.StockCountProduct scp ON scp.ProductId = p.Id
JOIN Merchandising.StockCount sc ON scp.StockCountId = sc.Id
JOIN Merchandising.Location l ON l.Id = sc.LocationId
LEFT JOIN Merchandising.ProductStockLevel psl ON psl.ProductId = p.Id
	AND psl.LocationId = l.Id
