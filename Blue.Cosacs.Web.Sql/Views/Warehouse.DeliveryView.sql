 /*
-Address Standardization CR2019 - 025 - To Get The Delivery Name Empty When the address type comes as W as Work the the company name should be show on any delivery print Empty.

*/

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Warehouse].[DeliveryView]'))
DROP VIEW  [Warehouse].[DeliveryView]
GO

-- =======================================================================================
-- Version :  003
-- Change Control
-- --------------
-- Date			By			Description
-- ----			--			-----------
-- 18 Oct 2019 Raj kishore  #Added 'Or' Keyword  with Customer Address.
-- 23 Jun 2020 Tosif		Selected customer accounts with hldorjnt = 'H'
-- 07 Jul 2020 Snehalata	Address Standardization - Change the condition to show the DName
--							when delivery done by Work Address Type then DName is Empty to show the Company Name blank On delivery  
-- =======================================================================================

CREATE VIEW [Warehouse].[DeliveryView]
AS
		
SELECT Distinct 
      [Id]
      ,[CustomerName]
	  , CASE WHEN dbo.custaddress.addtype IN ('W','D') 
			 THEN ' '  --Address Standardization CR2019 - 025			
			 ELSE ISNULL(dbo.custaddress.DELTitleC,'') + ' ' + ISNULL(dbo.custaddress.DELFirstName,'') + ' ' + ISNULL(dbo.custaddress.DELLastName,'')
		END as DName
      ,[AddressLine1]
      ,[AddressLine2]
      ,[AddressLine3]
      ,[PostCode]
      ,[StockBranch]
      ,[DeliveryBranch]
      ,[DeliveryOrCollection]
      ,[DeliveryOrCollectionDate]
      ,[ItemNo]
      ,[ItemId]
      ,[ItemUPC]
      ,[ProductDescription]
      ,[ProductBrand]
      ,[ProductModel]
      ,[ProductArea]
      ,[ProductCategory]
      ,[Quantity]
      ,[RepoItemId]
      ,[Comment]
      ,[DeliveryZone]
      ,[ContactInfo]
      ,[OrderedOn]
      ,[Damaged]
      ,[AssemblyReq]
      ,b.[AcctNo]
      ,[OriginalId]
      ,[TruckId]
      ,[PickingId]
      ,[PickingAssignedBy]
      ,[PickQuantity]
      ,[PickingComment]
      ,[PickingRejectedReason]
      ,[PickingRejected]
      ,[ScheduleId]
      ,[ScheduleComment]
      ,[ScheduleSequence]
      ,[PickingAssignedDate]
      ,[UnitPrice]
      ,[Path]
      ,[ScheduleRejected]
      ,[ScheduleRejectedReason]
      ,[DeliveryRejected]
      ,[DeliveryRejectedReason]
      ,[DeliveryConfirmedBy]
      ,[DeliveryRejectionNotes]
      ,[ScheduleQuantity]
      ,[DeliverQuantity]
      ,[Exception]
      ,[CurrentQuantity]
      ,[Express]
      ,[AddressNotes]
      ,[BookedBy]
      ,[Fascia]
      ,[PickUp]
      ,[PickUpDatePrinted]
      ,[PickUpNotePrintedBy]
      ,[DeliveryConfirmedDate]
      ,[CancelUser]
      ,[CancelDate]
      ,[CancelReason]
      ,[StockOnHand]
      ,[NonStockServiceType]
      ,[NonStockServiceItemNo]
      ,[NonStockServiceDescription]
      ,[DeliveryOrCollectionSlot]
      ,[StoreType]
      ,[ReceivingLocation], ISNULL(Ag.AgreementInvoiceNumber, Ag.Agrmtno) as OrderInvoiceNo
FROM Warehouse.BookingView b
		LEFT JOIN Agreement Ag on b.acctno = Ag.acctno
		LEFT join dbo.custacct AS custadd ON custadd.acctno = B.AcctNo AND custadd.hldorjnt = 'H' LEFT OUTER JOIN
          dbo.custaddress ON custadd.custid = dbo.custaddress.custid 
		  AND (dbo.custaddress.cusaddr1 = B.AddressLine1 
			   AND dbo.custaddress.cusaddr2 = B.AddressLine2
			   AND dbo.custaddress.cusaddr3 = B.AddressLine3)

GO
