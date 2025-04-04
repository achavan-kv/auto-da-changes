
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_GetDeliveryConfirmation]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_GetDeliveryConfirmation]
END
GO

Create PROCEDURE [dbo].[VE_GetDeliveryConfirmation]
	  @AccountNo VARCHAR(20) = N''
	, @Id VARCHAR(20) = N''
	, @Message varchar(MAX) output
	, @Status varchar(5) output
AS
BEGIN
SET NOCOUNT ON;
SELECT DISTINCT
		 T1.acctno AS 'AccountNo'
		,@Id AS 'CheckOutId'
		,T1.OrderNo AS 'OrderId'
		,(CASE WHEN t.id in (70,72) THEN 'F' ELSE
			CASE WHEN t.id in (57) THEN 'S' ELSE
					CASE WHEN t.id in (33) THEN 'C' ELSE 'O'
					END
			END
		END) AS 'Type'	
		,T2.itemno AS 'Itemno'
		,T1.Quantity AS 'QuantityOrdered'
		,T1.Quantity AS 'QuantityDelivered'
		,T2.deliveryaddress AS 'AddressType'
		,T1.quantity AS 'Delivered'
		,'' AS 'Comments'
		
		,ISNULL((SELECT TOP 1 DeliveryConfirmedOnDate FROM warehouse.booking WHERE acctno=T1.acctno AND ItemId=T1.ItemId),(SELECT TOP 1 datedel FROM delivery WHERE acctno=T1.acctno AND ItemId=T1.ItemId)) AS 'DeliveryDate' 
		,(SELECT TOP 1 CustomerName FROM warehouse.booking WHERE acctno=T1.acctno AND ItemId=T1.ItemId) AS 'Name'
		--,ISNULL((SELECT TOP 1 DeliveryConfirmedBy FROM warehouse.booking WHERE acctno=T1.acctno AND ItemId=T1.ItemId and DeliveryOrCollection ='D'),(SELECT TOP 1 NotifiedBy FROM delivery WHERE acctno=T1.acctno AND ItemId=T1.ItemId)) AS 'employeeId'
		,(ISNULL(T1.MVEUserID,1)) AS 'employeeId'

FROM VE_lineitem T1 
INNER JOIN lineitem T2 ON T1.acctno=T2.acctno AND T1.itemno=T2.itemno AND T2.parentitemno=''
LEFT OUTER JOIN Merchandising.Product MProduct ON MProduct.ID=T2.itemid		
LEFT OUTER JOIN Merchandising.ProductHierarchy h ON MProduct.Id = h.ProductId
LEFT OUTER JOIN Merchandising.HierarchyTag t on t.Id = h.HierarchyTagId AND t.levelid=2
where T1.acctno=@AccountNo
	AND ISNULL(T1.IsDeliver,'false')='true'
	AND T2.itemno not in ('DT','STAX')
	AND T1.itemno!='ADMIN'
	AND ISNULL(T1.OldCheckOutId,T1.CheckOutId)=@Id
	AND ISNULL(T1.DeliveryPercent,'0')='1'
	AND ISNULL(T1.ShipmentCancel,'false')='false'
	AND T1.BillType!='Return'
UNION ALL 

SELECT DISTINCT
		 T1.acctno AS 'AccountNo'
		,@Id AS 'CheckOutId'
		,T1.OrderNo AS 'OrderId'
		,(CASE WHEN t.id in (70,72) THEN 'F' ELSE
			CASE WHEN t.id in (57) THEN 'S' ELSE
					CASE WHEN t.id in (33) THEN 'C' ELSE 'O'
					END
			END
		END) AS 'Type'	
		,T2.itemno AS 'Itemno'
		,T1.Quantity AS 'QuantityOrdered'
		,T1.Quantity AS 'QuantityDelivered'
		,T2.deliveryaddress AS 'AddressType'
		,T1.quantity AS 'Delivered'
		,'' AS 'Comments'
		
		,ISNULL((SELECT TOP 1 DeliveryConfirmedOnDate FROM warehouse.booking WHERE acctno=T1.acctno AND ItemId=T1.ItemId),(SELECT TOP 1 datedel FROM delivery WHERE acctno=T1.acctno AND ItemId=T1.ItemId)) AS 'DeliveryDate' 
		,(SELECT TOP 1 CustomerName FROM warehouse.booking WHERE acctno=T1.acctno AND ItemId=T1.ItemId) AS 'Name'
		--,ISNULL((SELECT TOP 1 DeliveryConfirmedBy FROM warehouse.booking WHERE acctno=T1.acctno AND ItemId=T1.ItemId and DeliveryOrCollection ='D'),(SELECT TOP 1 NotifiedBy FROM delivery WHERE acctno=T1.acctno AND ItemId=T1.ItemId)) AS 'employeeId'
		,(ISNULL(T1.MVEUserID,1)) AS 'employeeId'

FROM VE_lineitem T1 
INNER JOIN lineitem T2 ON T1.acctno=T2.acctno AND T1.itemno=T2.itemno AND T2.parentitemno=''
LEFT OUTER JOIN Merchandising.Product MProduct ON MProduct.ID=T2.itemid		
LEFT OUTER JOIN Merchandising.ProductHierarchy h ON MProduct.Id = h.ProductId
LEFT OUTER JOIN Merchandising.HierarchyTag t on t.Id = h.HierarchyTagId AND t.levelid=2
where T1.acctno=@AccountNo
	AND ISNULL(T1.IsDeliver,'false')='true'
	AND T2.itemno not in ('DT','STAX')
	AND T1.itemno!='ADMIN'
	AND T1.OldCheckOutId=@Id
	AND T1.BillType='IdenticalEx'
	AND ISNULL(T1.ShipmentCancel,'false')='false'

UNION ALL 

SELECT DISTINCT
		 T1.acctno AS 'AccountNo'
		,@Id AS 'CheckOutId'
		,T1.OrderNo AS 'OrderId'
		,(CASE WHEN t.id in (70,72) THEN 'F' ELSE
			CASE WHEN t.id in (57) THEN 'S' ELSE
					CASE WHEN t.id in (33) THEN 'C' ELSE 'O'
					END
			END
		END) AS 'Type'	
		,T2.itemno AS 'Itemno'
		,T1.Quantity AS 'QuantityOrdered'
		,T1.Quantity AS 'QuantityDelivered'
		,T2.deliveryaddress AS 'AddressType'
		,T1.quantity AS 'Delivered'
		,'' AS 'Comments'
		
		,ISNULL((SELECT TOP 1 DeliveryConfirmedOnDate FROM warehouse.booking WHERE acctno=T1.acctno AND ItemId=T1.ItemId),(SELECT TOP 1 datedel FROM delivery WHERE acctno=T1.acctno AND ItemId=T1.ItemId)) AS 'DeliveryDate' 
		,(SELECT TOP 1 CustomerName FROM warehouse.booking WHERE acctno=T1.acctno AND ItemId=T1.ItemId) AS 'Name'
		--,ISNULL((SELECT TOP 1 DeliveryConfirmedBy FROM warehouse.booking WHERE acctno=T1.acctno AND ItemId=T1.ItemId and DeliveryOrCollection ='D'),(SELECT TOP 1 NotifiedBy FROM delivery WHERE acctno=T1.acctno AND ItemId=T1.ItemId)) AS 'employeeId'
		,(ISNULL(T1.MVEUserID,1)) AS 'employeeId'
FROM VE_lineitem T1 
INNER JOIN lineitem T2 ON T1.acctno=T2.acctno AND T1.itemno=T2.itemno AND T2.parentitemno=''
LEFT OUTER JOIN Merchandising.Product MProduct ON MProduct.ID=T2.itemid		
LEFT OUTER JOIN Merchandising.ProductHierarchy h ON MProduct.Id = h.ProductId
LEFT OUTER JOIN Merchandising.HierarchyTag t on t.Id = h.HierarchyTagId AND t.levelid=2
where T1.acctno=@AccountNo
	AND ISNULL(T1.IsDeliver,'false')='true'
	AND T2.itemno not in ('DT','STAX')
	AND T1.itemno!='ADMIN'
	AND T1.OldCheckOutId=@Id
	AND T1.BillType='Return'
	AND ISNULL(T1.ShipmentCancel,'false')='false'

UNION ALL 

SELECT DISTINCT
		 T1.acctno AS 'AccountNo'
		,@Id AS 'CheckOutId'
		,T1.OrderNo AS 'OrderId'
		,(CASE WHEN t.id in (70,72) THEN 'F' ELSE
			CASE WHEN t.id in (57) THEN 'S' ELSE
					CASE WHEN t.id in (33) THEN 'C' ELSE 'O'
					END
			END
		END) AS 'Type'	
		,T2.itemno AS 'Itemno'
		,T1.Quantity AS 'QuantityOrdered'
		,T1.Quantity AS 'QuantityDelivered'
		,T2.deliveryaddress AS 'AddressType'
		,T1.quantity AS 'Delivered'
		,'' AS 'Comments'
		
		,ISNULL((SELECT TOP 1 DeliveryConfirmedOnDate FROM warehouse.booking WHERE acctno=T1.acctno AND ItemId=T1.ItemId),(SELECT TOP 1 datedel FROM delivery WHERE acctno=T1.acctno AND ItemId=T1.ItemId)) AS 'DeliveryDate' 
		,(SELECT TOP 1 CustomerName FROM warehouse.booking WHERE acctno=T1.acctno AND ItemId=T1.ItemId) AS 'Name'
		--,ISNULL((SELECT TOP 1 DeliveryConfirmedBy FROM warehouse.booking WHERE acctno=T1.acctno AND ItemId=T1.ItemId and DeliveryOrCollection ='D'),(SELECT TOP 1 NotifiedBy FROM delivery WHERE acctno=T1.acctno AND ItemId=T1.ItemId)) AS 'employeeId'
		,(ISNULL(T1.MVEUserID,1)) AS 'employeeId'
FROM VE_lineitem T1 
INNER JOIN lineitem T2 ON T1.acctno=T2.acctno AND T1.itemno=T2.itemno AND T2.parentitemno=''
LEFT OUTER JOIN Merchandising.Product MProduct ON MProduct.ID=T2.itemid		
LEFT OUTER JOIN Merchandising.ProductHierarchy h ON MProduct.Id = h.ProductId
LEFT OUTER JOIN Merchandising.HierarchyTag t on t.Id = h.HierarchyTagId AND t.levelid=2
where T1.acctno=@AccountNo
	AND ISNULL(T1.IsDeliver,'false')='true'
	AND T2.itemno not in ('DT','STAX')
	AND T1.itemno!='ADMIN'
	AND T1.OldCheckOutId=@Id
	AND T1.BillType='Exchange'
	AND ISNULL(T1.ExchgAmtDiff,0.00)=0.00
	AND ISNULL(T1.ShipmentCancel,'false')='false'
END

