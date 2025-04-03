IF EXISTS (SELECT * FROM sys.objects so
           INNER JOIN sys.schemas ss ON so.schema_id = ss.schema_id
           WHERE so.type = 'V'
           AND so.NAME = 'BookingView'
           AND ss.name = 'Warehouse')
DROP VIEW  Warehouse.BookingView
GO
--================================================================================
-- Project       : Unicomer 
-- Name          : Raj
-- Author        : Raj
-- Create Date   : 03 July 2020
-- Description   : To Get Shipment on delivery Schedule

 

-- Change Control
-- --------------
--Version :  <002>
-- Date              By                  Description      

------             	---------                ----      
-- 18/10/2019     	Raj kishore      #Added 'Distinct' Keyword to select  unique records only.
-- 23 Jun 2020 		Tosif		Selected customer accounts with hldorjnt = 'H'

--             
 --================================================================================



CREATE VIEW [Warehouse].[BookingView]
AS
WITH Merch -- This a hack. Need for release. Fix later -- Paulo.
AS
(select DISTINCT SKU, StockOnHand, l.SalesId from Merchandising.ProductStockLevel pl
INNER JOIN Merchandising.Product p on p.Id = pl.ProductId
INNER JOIN Merchandising.Location l on l.Id = pl.LocationId)
SELECT   DISTINCT
		 CASE
	  WHEN dbo.custaddress.addtype IN ('W','D') THEN ' ' 
	 ELSE ISNULL(dbo.custaddress.DELTitleC,'') + ' ' + ISNULL(dbo.custaddress.DELFirstName,'') + ' ' + ISNULL(dbo.custaddress.DELLastName,'')   END as DName,  
        b.Id, CustomerName, AddressLine1, AddressLine2, AddressLine3, PostCode, StockBranch, DeliveryBranch, DeliveryOrCollection,
        b.DeliveryOrCollectionDate,        -- #14547
        b.ItemNo, ItemId, ItemUPC, ProductDescription, ProductBrand, ProductModel, ProductArea, ProductCategory, Quantity, RepoItemId, Comment, DeliveryZone, ContactInfo, 
        CONVERT(datetime, SWITCHOFFSET(CONVERT(datetimeoffset, b.OrderedOn), DATENAME(TzOffset, SYSDATETIMEOFFSET()))) AS OrderedOn,        -- #13458 
        Damaged, AssemblyReq, b.AcctNo, OriginalId, TruckId, PickingId, PickingAssignedBy, PickQuantity, PickingComment, PickingRejectedReason, PickingRejected, ScheduleId, 
        ScheduleComment, ScheduleSequence, PickingAssignedDate, UnitPrice, Path, ScheduleRejected, ScheduleRejectedReason, DeliveryRejected, DeliveryRejectedReason, 
        DeliveryConfirmedBy, DeliveryRejectionNotes, ScheduleQuantity, DeliverQuantity, Exception, CurrentQuantity, Express, AddressNotes, BookedBy, Fascia, PickUp, 
        PickUpDatePrinted, PickUpNotePrintedBy, DeliveryConfirmedDate,
        c.userid as CancelUser, c.[date] as CancelDate, c.reason as CancelReason, CONVERT(FLOAT,ISNULL(Merch.StockOnHand,0.0)) AS StockOnHand,
        B.NonStockServiceType, B.NonStockServiceItemNo, B.NonStockServiceDescription,
        B.DeliveryOrCollectionSlot, br.StoreType, b.ReceivingLocation
        FROM Warehouse.Booking B
        LEFT OUTER JOIN warehouse.Cancellation c ON isnull(b.OriginalId,b.id) = c.id     
        LEFT OUTER JOIN Merch ON b.itemno = Merch.SKU AND b.StockBranch = Merch.SalesId
          JOIN dbo.branch br on B.DeliveryBranch = br.branchno LEFT OUTER JOIN -- Fix for Issue 5813155
                              dbo.custacct AS custadd ON custadd.acctno = B.AcctNo  and custadd.hldorjnt='H' LEFT OUTER JOIN
                              dbo.custaddress ON custadd.custid = dbo.custaddress.custid AND (dbo.custaddress.cusaddr1 = B.AddressLine1 AND --#Condition changed to And previously OR
                              dbo.custaddress.cusaddr2 = B.AddressLine2 AND dbo.custaddress.cusaddr3 = B.AddressLine3) --Added dbo.custaddress.cusaddr3 = B.AddressLine3

							   
GO							  