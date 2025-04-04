if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_GetSyncData]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_GetSyncData]
END
GO


Create PROCEDURE [dbo].[VE_GetSyncData]
	  @Message varchar(MAX) output
	, @Status varchar(MAX) output
AS
BEGIN
	SET NOCOUNT ON;
	SELECT DISTINCT
		ServiceCode AS ServiceCode,
		Code AS Code,
		IsInsertRecord AS IsInsertRecord,
		IsEODRecords AS IsEODRecords,
		
		CASE	
				WHEN 
					(	SELECT COUNT(ShipmentCancel)
						FROM VE_LineItem 
						WHERE acctno=T0.Code AND itemno NOT IN ('ADMIN','DT')
						AND ISNULL(oldCheckoutID,'')=T0.CheckOutID 
						AND ISNULL(ShipmentCancel,'0')='1'
					)>0  THEN 'ReturnReject'
				
				
				WHEN 
					(	SELECT COUNT(DeliveryPercent)
						FROM VE_LineItem 
						WHERE acctno=T0.Code AND itemno NOT IN ('ADMIN','DT')
							--AND ISNULL(OldCheckOutId,0)=0
						AND ISNULL(CheckoutID,'')=T0.CheckOutID 
						AND ISNULL(ShipmentCancel,'0')!='1'
					)>0  THEN 'PUT'							
		
				WHEN 
					(	SELECT COUNT(DeliveryPercent)
						FROM VE_LineItem 
						WHERE acctno=T0.Code AND itemno NOT IN ('ADMIN','DT')
							AND ISNULL(OldCheckOutId,0)=0
					)=0  THEN 'POST'

				WHEN (	SELECT  COUNT(DeliveryPercent)
						FROM VE_LineItem 
						WHERE acctno=T0.Code AND ISNULL(oldcheckoutid,CheckoutID)=T0.CheckOutID 
							AND ISNULL(OldCheckOutId,'')='' 
							AND itemno NOT IN ('ADMIN','DT')
							AND ISNULL(ShipmentCancel,'0')!='1'
					)>=1  THEN 'PUT'

				WHEN (	SELECT COUNT(DeliveryPercent)
						FROM VE_LineItem 
						WHERE ISNULL(DeliveryPercent,'0')='1' 
							AND acctno=T0.Code AND ISNULL(oldcheckoutid,CheckoutID)=T0.CheckOutID
							AND ISNULL(OldCheckOutId,'')!=''
							AND itemno NOT IN ('ADMIN','DT')
							AND ISNULL(ShipmentCancel,'0')!='1'
				)>=1  THEN 'POST'

				WHEN (	SELECT COUNT(orderno)
						FROM VE_LineItem 
						WHERE ISNULL(DeliveryPercent,'0')='0' 
							AND acctno=T0.Code AND ISNULL(oldcheckoutid,CheckoutID)=T0.CheckOutID
							AND ISNULL(OldCheckOutId,'')!='' 
							AND ISNULL(ExchgAmtDiff,0.0)!=0.0
							AND itemno NOT IN ('ADMIN','DT')
							AND ISNULL(ShipmentCancel,'0')!='1'
				)>=1  THEN 'ExchPayConfirm'

				WHEN (	SELECT COUNT(Orderno)
						FROM VE_LineItem 
						WHERE ISNULL(DeliveryPercent,'0')='0' 
							AND acctno=T0.Code AND ISNULL(oldcheckoutid,CheckoutID)=T0.CheckOutID
							AND ISNULL(OldCheckOutId,'')!='' 
							AND ISNULL(ExchgAmtDiff,0.0)=0.0
							AND itemno NOT IN ('ADMIN','DT')
							AND ISNULL(ShipmentCancel,'0')!='1'
				)>=1  THEN 'ExchConfirm'
				ELSE 'PUTyy'
END AS 'Method',
		ID AS 'ID',
		ISNULL(T0.CheckOutID,'') AS 'CheckOutID'
	FROM VE_TaskSchedular T0
	WHERE T0.Status=0
END

