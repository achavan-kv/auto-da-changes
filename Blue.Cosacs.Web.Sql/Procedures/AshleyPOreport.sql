  IF EXISTS (SELECT  * FROM sys.objects WHERE object_id = OBJECT_ID(N'AshleyPOReprot') AND type IN (N'P', N'PC'))
	DROP PROCEDURE AshleyPOReprot
GO
/****** Object:  StoredProcedure [dbo].[CreateAutoPO]    Script Date: 1/17/2020 5:52:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec [AshleyPOReprot] '2020-01-01','2020-01-17','C'
CREATE PROCEDURE [dbo].[AshleyPOReprot]
	 @Fromdate DATE, -- ='2020-01-01',
	 @Todate DATE,-- ='2020-01-17',
	 @Status VARCHAR(20)--='C'
AS
BEGIN 
-------------------------------Auto Po Status Cancelled Report-------------------------------------------- 
	IF @Status='C'
	BEGIN
		SELECT  Id AS PoNumber,T.RequestedDeliveryDate,T.VendorId,T.Vendor,T.ReceivingLocation,T.Status,T.CreatedDate,CancelReason FROM [Merchandising].[PurchaseOrder]  t  
		WHERE  t.CreatedBy='Auto PO' and t.Status='Cancelled' 
		and CONVERT(DATE,CreatedDate) BETWEEN @Fromdate and @Todate
	END 
 -------------------------------Auto Po Ack Cancelled Report-------------------------------------------- 
	 IF @STATUS='U'
	 BEGIN
		SELECT  Id AS PoNumber,T.RequestedDeliveryDate,T.VendorId,T.Vendor,T.ReceivingLocation,T.Status,T.CreatedDate, '' as CancelReason FROM [Merchandising].[PurchaseOrder]  t  
		WHERE  t.CreatedBy='Auto PO'  and CreatedDate between @Fromdate and @Todate
		and CONVERT(DATE,t.CreatedDate)<>CONVERT(DATE,t. RequestedDeliveryDate)
	End
END
 
 --select *   FROM [Merchandising].[PurchaseOrder] where Id='5353'
 
 
 --Update [Merchandising].[PurchaseOrder]  set CancelReason='Testing' where Id='5356'