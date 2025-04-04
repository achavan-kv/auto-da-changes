if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_VendorReturn]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_VendorReturn]
END
GO

Create PROCEDURE [dbo].[VE_VendorReturn]
	  @VendorReturnID VARCHAR(20) = N''
	, @message varchar(max) output
	, @status varchar(5) output
AS
BEGIN 
SET NOCOUNT ON;
SELECT 
		 'VendorReturn' AS 'ResourceType'
		,'MVE' AS 'Source'
	    ,vr.ID as externalVendorReturnId
		, vr.GoodsReceiptId as externalGRNId
		,1 as CreatedById
		,1 as ApprovedById
		,vr.CreatedBy
		,vr.ApprovedBy
		,vr.CreatedDate
		,vr.ApprovedDate			
		,vr.ReferenceNumber		
		,(CASE WHEN t.id in (70,72) THEN 'F' ELSE
              CASE WHEN t.id in (57) THEN 'S' ELSE
                     CASE WHEN t.id in (33) THEN 'C' ELSE 'O'
                     END
              END
       END)    AS  ProductType
	   ,pop.ProductId as externalItemNo
	   ,pop.description
	   ,vrp.Comments 
	   ,vrp.quantityReturned
	   ,grp.LastLandedCost as unitLandedCost
	   ,grph.VendorDeliveryNumber AS 'vendorDeliveryNumber' 
	   ,grph.vendorInvoiceNumber AS 'vendorInvoiceNumber' 
	   ,pop.PurchaseOrderId AS 'externalPONumber' 
	from merchandising.vendorreturn vr
	INNER JOIN [Merchandising].[VendorReturnProduct] vrp ON vr.Id=vrp.VendorReturnId
	INNER JOIN [Merchandising].[GoodsReceiptProduct] grp ON vr.GoodsReceiptId=grp.GoodsReceiptId
	INNER JOIN [Merchandising].[GoodsReceipt] grph ON grp.GoodsReceiptId=grph.id
	INNER JOIN [Merchandising].[PurchaseOrderProduct] pop ON pop.id=grp.PurchaseOrderProductId
	INNER JOIN Merchandising.ProductHierarchy h ON pop.ProductId = h.ProductId
	INNER JOIN Merchandising.HierarchyTag t on t.Id = h.HierarchyTagId 	
	where  vr.ID=@VendorReturnID  and grp.Id=vrp.GoodsReceiptProductId AND t.levelid=2
	   group by t.id, pop.ProductId,vrp.Comments 
	   ,vrp.quantityReturned
	    ,vr.ID 
		,vr.GoodsReceiptId 
		,vr.CreatedById
		,vr.ApprovedById
		,vr.CreatedBy
		,vr.ApprovedBy
		,vr.CreatedDate
		,vr.ApprovedDate		
		,vr.ReferenceNumber	
		,grp.LastLandedCost
		,pop.description
		,grph.VendorDeliveryNumber
		,grph.vendorInvoiceNumber
		,pop.PurchaseOrderId
	END