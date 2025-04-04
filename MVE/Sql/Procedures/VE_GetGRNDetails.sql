if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_GetGRNDetails]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_GetGRNDetails]
END
GO


Create PROCEDURE [dbo].[VE_GetGRNDetails]
	  @GRNNo VARCHAR(20) = N''
	, @Message varchar(MAX) output
	, @Status varchar(5) output
AS
BEGIN
	SET NOCOUNT ON;
SELECT 
	 'GRN' AS 'ResourceType'
	,'MVE' AS 'Source'
	,T0.Id AS 'GRNID',
	(select SalesId from  [Merchandising].Location where ID= T0.LocationId) 'LocationId',
	1 AS 'ReceivedById'
	,T0.VendorInvoiceNumber AS 'VendorInvoiceNumber'
	,CONVERT(varchar, T3.CreatedDate,126) AS 'vendorInvoiceDate'
	,T3.CorporatePoNumber AS 'Po-Id'
	,CONVERT(varchar, T0.DateReceived,126) AS 'DateReceived'	
	,T0.Comments AS 'Comments' 
	,(CASE WHEN t.id in (70,72) THEN 'F' ELSE
              CASE WHEN t.id in (57) THEN 'S' ELSE
                     CASE WHEN t.id in (33) THEN 'C' ELSE 'O'
                     END
              END
       END)    AS ProductType
	,T2.ProductId AS 'ProductCode'
	,T2.Description AS 'Description'		
	,sum(T1.QuantityReceived) AS 'QuantityReceived'
	,T1.QuantityBackOrdered AS 'QuantityBackOrdered'
	,T1.QuantityCancelled AS 'QuantityCancelled'
	,T1.LastLandedCost AS 'LastLandedCost'
	,T1.ReasonForCancellation AS 'ReasonForCancellation'
	,T0.VendorDeliveryNumber AS 'vendorDeliveryNumber'
	FROM merchandising.GoodsReceipt T0
	INNER JOIN Merchandising.GoodsReceiptProduct T1 ON T0.Id=T1.GoodsReceiptId
	LEFT OUTER JOIN [Merchandising].[PurchaseOrderProduct] T2 ON T1.PurchaseOrderProductId=T2.Id
	LEFT OUTER JOIN [Merchandising].[PurchaseOrder] T3 ON T2.PurchaseOrderId=T3.Id
	INNER JOIN Merchandising.Product MProduct ON MProduct.ID=T2.ProductId		
	INNER JOIN Merchandising.ProductHierarchy h ON MProduct.Id = h.ProductId
	INNER JOIN Merchandising.HierarchyTag t on t.Id = h.HierarchyTagId 	
	WHERE T0.Id=@GRNNo AND T1.QuantityReceived>0
	AND t.levelid=2
	Group by T2.ProductId ,T0.Id,T0.VendorInvoiceNumber,T3.CorporatePoNumber,T3.CreatedDate,T0.DateReceived,T0.Comments
	,T2.ProductId,T2.Description,T1.QuantityBackOrdered,T1.QuantityCancelled,T1.LastLandedCost,T1.ReasonForCancellation,T0.LocationId
	,t.id ,T0.VendorDeliveryNumber
END