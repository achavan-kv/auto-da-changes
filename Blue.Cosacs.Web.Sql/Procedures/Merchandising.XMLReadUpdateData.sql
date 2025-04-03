 
IF EXISTS (SELECT  *
  FROM sys.objects
  WHERE object_id = OBJECT_ID(N'[Merchandising].[XMLReadUpdateData]')
  AND type IN (N'P', N'PC'))
  DROP PROCEDURE  [Merchandising].[XMLReadUpdateData]
GO


create PROCEDURE  [Merchandising].[XMLReadUpdateData]
as 
Begin
UPDATE [Merchandising].[PurchaseOrder]
SET RequestedDeliveryDate = (SELECT DISTINCT
      CreationDate
    FROM [dbo].[Merchandising].[XMLDataStore]
    WHERE CreationDate IS NOT NULL),
    ShipDate = (SELECT TOP 1
      ShiptolocationArrialDate
    FROM [dbo].[Merchandising].[XMLDataStore]
    WHERE ShiptolocationArrialDate IS NOT NULL),
    ShipVia = (SELECT TOP 1
      Shipid
    FROM [dbo].[Merchandising].[XMLDataStore]
    WHERE Shipid IS NOT NULL)
WHERE id = (SELECT DISTINCT
  POId
FROM [dbo].[Merchandising].[XMLDataStore]
WHERE Poid IS NOT NULL)
  end 
