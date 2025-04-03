if exists ( select *  from  dbo.sysobjects   where  id = object_id('[Merchandising].[AshleyExportXML]')    and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [Merchandising].[AshleyExportXML]
GO   
create PROCEDURE  [Merchandising].[AshleyExportXML]
as 
Begin
declare @id varchar(50)='AshleyEnabled'
If @id=(select Id from config.setting where Id='AshleyEnabled' and ValueBit=1 )
DECLARE @PoNumber int
SET @PoNumber = (SELECT TOP 1
  id
FROM TempPoCount
WHERE Statusupdate <> 1)
SELECT
  ROW_NUMBER() OVER (ORDER BY t.id ASC) AS RowNumber,
  t.Id,
  t.VendorId,
  t.Vendor,
  t.RequestedDeliveryDate,
  t.ReceivingLocationId,
  t.ReceivingLocation,
  t.ReferenceNumbers,
  t.Currency,
  t.Comments,
  t.Status,
  t.OriginalPrint,
  t.CreatedDate,
  t.CreatedById,
  t.CreatedBy,
  t.PaymentTerms,
  t.OriginSystem,
  t.CorporatePoNumber,
  t.ShipDate,
  t.ShipVia,
  t.PortOfLoading,
  t.Attributes,
  t.CommissionChargeFlag,
  t.CommissionPercentage,
  t.ExpiredDate,
  pp.VendorStyleLong as CorporateUPC,----Vendor UPC
  o.id AS Productount,
  o.ProductId,
  o.Sku,
  o.Description,
  o.QuantityOrdered,
  o.UnitCost,
  o.EstimatedDeliveryDate,
  o.PurchaseOrderId,
  o.PreLandedUnitCost,
  o.PreLandedExtendedCost,
  o.LabelRequired,
  o.QuantityCancelled,
 '7773300' as BuyerpartyIdentifierCode,
 'ReceiverAssigned' as buyerpartyIdentifierQualifierCode,
 '052738531' as sellerpartyIdentifierCode,
 'SenderAssigned' as sellerpartyIdentifierQualifierCode,
 '190' as ShiptopartyIdentifierCode,
 'ReceiverAssigned' as ShiptopartyIdentifierQualifierCode,
 'ShipItemsAsAvailable'  as  shipComplete,
 'BuyerAssigned' as ProductitemNumberQualifierbuyer,
 'SellerAssigned' as ProductitemNumberQualifierseller,
 'Each' as  unitOfMeasure,
 'ORDERLEVELCUSTOMERCOMMENT' as Ordercomment,
 'Original' as [status_New]
 ---850 as [Type]
FROM [Merchandising].[PurchaseOrder] t
INNER JOIN [Merchandising].[PurchaseOrderProduct] o
  ON t.id = o.PurchaseOrderId
  inner join [Merchandising].[Product] pp  on o.Productid=pp.id
INNER JOIN [Merchandising].[ProductAttributes] v
  ON o.ProductId = v.ProductId
WHERE t.id IN (@PoNumber) --and t.[status]='Approved'
UPDATE TempPoCount
SET Statusupdate = '1'
WHERE id = @PoNumber
print 'Ashley product not configured on this country'
end 

 