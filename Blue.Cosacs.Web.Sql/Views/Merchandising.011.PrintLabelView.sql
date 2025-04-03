GO

IF EXISTS (
		SELECT 1
		FROM dbo.sysobjects
		WHERE id = object_id(N'[Merchandising].[PrintLabelView]')
			AND OBJECTPROPERTY(id, N'IsView') = 1
		)
	DROP VIEW [Merchandising].[PrintLabelView]
GO

-- =============================================
-- CREATED BY:	   ANUSHREE URKUNDE
-- CREATE DATE:	   14/07/2020
-- SCRIPT Name:    PrintLabelView.sql
-- SCRIPT COMMENT: Created view for Capturing corporate UPC by changing vendor UPC for purchase order label print.
-- Discription:    Scanning using corporate UPC - CR
-- ==============================================
CREATE VIEW [Merchandising].[PrintLabelView]
AS
SELECT ISNULL(CONVERT(INT, ROW_NUMBER() OVER (
				ORDER BY p.Id
				)), 0) AS Id
	,pop.Id AS PurchaseOrderProductId
	,p.Id AS ProductId
	,ProductType
	,p.SKU
	,LongDescription
	,CorporateUPC
	,BoxCount
	,POSDescription
	,p.VendorStyleLong
	,b.BrandName
	,po.Vendor
	,po.Id AS PurchaseOrderId
FROM Merchandising.PurchaseOrderProduct pop
JOIN merchandising.purchaseorder po ON pop.PurchaseOrderId = po.id
JOIN merchandising.product p ON pop.productid = p.id
LEFT OUTER JOIN Merchandising.Brand b ON p.BrandId = b.id
