 
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_StockTransferDetails]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
DROP PROCEDURE [dbo].[VE_StockTransferDetails]
END
GO

Create PROCEDURE [dbo].[VE_StockTransferDetails]
                  @StkTrfNo VARCHAR(20) = N''
                , @Message varchar(MAX) output
                , @Status varchar(5) output
AS
BEGIN
                SET NOCOUNT ON;
SELECT 
                 'StockTransfer' AS 'resourceType'
                ,'COSACS' AS 'source' 
                ,T0.Id AS 'stocktransferId'
 				,T2.SalesId AS 'sendingLocation'
                ,T3.SalesId AS 'receivingLocation'
                ,T4.SalesId AS 'vialocation'
                ,T0.ReferenceNumber AS 'documentReferenceNo'
                ,T0.Comments AS 'comments'
                ,'1' AS 'createdById'
                ,T1.ProductId AS 'productid'
                ,T1.Quantity AS 'quantity' 
                ,T1.ReferenceNumber AS 'reference' 
                ,T1.Comments AS 'linecomments'
				,CONVERT(varchar, T0.CreatedDate,126) AS 'createdDate' 
				,(	CASE WHEN t.id in (70,72) THEN 'F' ELSE
					CASE WHEN t.id in (57) THEN 'S' ELSE
					CASE WHEN t.id in (33) THEN 'C' ELSE 'O'
						END
						END
				   END)    AS 'productType'
                FROM merchandising.StockTransfer T0
                INNER JOIN Merchandising.StockTransferProduct T1 ON T0.Id=T1.stocktransferid
				INNER JOIN Merchandising.Location T2 ON T0.SendingLocationId=T2.Id
				INNER JOIN Merchandising.Location T3 ON T0.ReceivingLocationId=T3.Id
				LEFT OUTER JOIN Merchandising.Location T4 ON T0.ViaLocationId=T4.Id
				INNER JOIN Merchandising.Product MProduct ON MProduct.ID=T1.ProductId		
				INNER JOIN Merchandising.ProductHierarchy h ON MProduct.Id = h.ProductId
				INNER JOIN Merchandising.HierarchyTag t on t.Id = h.HierarchyTagId 	
                WHERE T0.ID=@StkTrfNo  AND t.levelid=2
END