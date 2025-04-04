if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_SupplierEOD]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_SupplierEOD]
END
GO
  

CREATE PROCEDURE [dbo].[VE_SupplierEOD] 
@VendorCode VARCHAR(30)
AS
BEGIN

SET NOCOUNT ON;
 SELECT 
(SELECT 
                STUFF( 
                                                ( SELECT DISTINCT ',' +
                                                                CASE WHEN PHV.TAGID in (70,72) THEN 'F' 
                                                                                 WHEN PHV.TAGID in (57) THEN 'S' 
                                                                                 WHEN PHV.TAGID in (33) THEN 'C'  
                                                                                 WHEN PHV.TAGID in (84,65) THEN 'O'                                                                                                          
                                                                END
                                                FROM     Merchandising.Product AS product INNER JOIN
                                                                Merchandising.ProductSupplierConcatView AS Vendor ON Vendor.ProductId = product.Id LEFT OUTER JOIN
                                                                Merchandising.RepossessedProductConditionView AS condition ON condition.ProductId = product.Id LEFT OUTER JOIN
                                                                Merchandising.ProductHierarchyConcatView AS h ON h.ProductId = product.Id 
                                                                INNER JOIN Merchandising.ProductHierarchyView PHV ON product.Id=PHV.ProductId 
                                                                INNER JOIN Merchandising.Supplier primaryVendor ON primaryVendor.Id=product.PrimaryVendorId
                                                                AND PHV.LevelID='2' AND primaryVendor.code=A.ExternalVendorID
																AND PHV.ProductType <> '' 
																AND primaryVendor.code = @VendorCode
                                                                FOR XML PATH('')                               
                                                ), 1, 1, ''
                                ) [ProductType]) as SupplierType,
                A.*
FROM
(select distinct
                primaryVendor.Status as Active,
                primaryVendor.code as ExternalVendorID,
                primaryVendor.Name as SupplierName,
                primaryVendor.Contacts as ContactName,
                '' as ContactTitle,
                primaryVendor.AddressLine1,
                primaryVendor.AddressLine2,
                primaryVendor.City as AddressLine3,
                PostCode as PostalCode,
                primaryVendor.state as StateorProvince,
                '' as PhoneNumber,
                '' as FaxNumber, 
                null as EmailAddress,
                '' as Notes, 
                'Ben, Sales' as LastUpdatedBy
FROM         Merchandising.Product AS product INNER JOIN
                                                  Merchandising.ProductSupplierConcatView AS Vendor ON Vendor.ProductId = product.Id LEFT OUTER JOIN
              Merchandising.RepossessedProductConditionView AS condition ON condition.ProductId = product.Id LEFT OUTER JOIN
              Merchandising.ProductHierarchyConcatView AS h ON h.ProductId = product.Id 
                                                  INNER JOIN Merchandising.ProductHierarchyView PHV ON product.Id=PHV.ProductId 
                                                  INNER JOIN Merchandising.Supplier primaryVendor ON primaryVendor.Id=product.PrimaryVendorId
                                                  AND PHV.LevelID='2'
                AND PHV.TAGID IN (33,57,65,70,72,84)
				 where primaryVendor.code = @VendorCode
                )  AS A
 END