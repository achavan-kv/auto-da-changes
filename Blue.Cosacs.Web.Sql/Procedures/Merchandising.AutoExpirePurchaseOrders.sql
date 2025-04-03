IF OBJECT_ID('Merchandising.AutoExpirePurchaseOrders') IS NOT NULL
	DROP PROCEDURE Merchandising.AutoExpirePurchaseOrders
GO 

-- =============================================
-- Author:		Abhijeet Gawali 
-- Create date: 14/02/2020
-- Description:	
-- version :	001 - Applied expiring date condition
-- =============================================

CREATE PROCEDURE Merchandising.AutoExpirePurchaseOrders		
@ExpireDate DateTime,
@DaysTillCancel int
AS
BEGIN
	DECLARE @ids table (id int); 

	UPDATE p
	SET P.Status = 'Expired',
		P.ExpiredDate = GETDATE()
	OUTPUT INSERTED.Id INTO @ids
	FROM Merchandising.PurchaseOrder p
	WHERE p.Status IN ('New','PartiallyReceived')
	--AND NOT EXISTS (SELECT 1
	--				FROM Merchandising.PurchaseOrderProduct pop  
	--				WHERE pop.EstimatedDeliveryDate >= @ExpireDate 
	--				AND p.Id = pop.PurchaseOrderId)
	AND EXISTS (SELECT 1
					FROM Merchandising.PurchaseOrderProduct pop  
					WHERE p.Id = pop.PurchaseOrderId)
	-- Code Added by Abhijeet 
	AND NOT EXISTS 
	(
		SELECT 1
		FROM Merchandising.PurchaseOrderProduct pop  
		WHERE pop.EstimatedDeliveryDate >= DATEADD(DAY, -@DaysTillCancel, @ExpireDate)
			AND p.Id = pop.PurchaseOrderId
	)

	SELECT id
	FROM @ids
END


