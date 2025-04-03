IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'PurchaseOrderSave')
DROP PROCEDURE  PurchaseOrderSave
GO 
CREATE PROCEDURE PurchaseOrderSave
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : PurchaseOrderSave.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save Purchase Order
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 31/08/11  IP  RI - #4690 - RI System Integration
-- ================================================
@Warehouse VARCHAR(3),
@suppliercode VARCHAR(12),
@OrderNumber VARCHAR(12),
@stringdatedue VARCHAR(10),
@quantity INT ,
--@itemno VARCHAR(10)
@itemno VARCHAR(18)									--IP - 31/08/11 - RI - #4690
AS 

DECLARE @datedue DATETIME,@branchno SMALLINT 
SELECT @branchno= branchno FROM branch WHERE branchno = @warehouse 
IF  DATALENGTH(@stringdatedue)=8 AND @stringdatedue !='00000000'
BEGIN
SET @stringdatedue = LEFT(@stringdatedue,2) + '/' + LEFT (RIGHT (@stringdatedue,6),2) + '/' + RIGHT(@stringdatedue,4)

SET @datedue = CONVERT(DATETIME,@stringdatedue ,103)
	
END
ELSE
	SET @datedue = GETDATE()
-- only inserting if datedue in the future or in the past 10 days in case late
IF @branchno IS NOT NULL AND @datedue > dateadd(DAY,-10,GETDATE())
BEGIN
	 UPDATE PurchaseOrderOutstanding 
	 SET expectedreceiptdate = @datedue ,
		supplierno = @suppliercode,
		quantityonorder = @quantity 
		WHERE itemno = @itemno
		AND stocklocn= @branchno 
		AND purchaseordernumber = @orderNumber

	IF @@ROWCOUNT = 0 

 	--IP - 31/08/11 - RI - #4690 - Below sql replaces the above.
 	 INSERT INTO PurchaseOrderOutstanding (
 		warehousenumber,
 		itemno,
 		stocklocn,
 		supplierno,
 		purchaseordernumber,
 		expectedreceiptdate,
 		quantityonorder,
 		quantityavailable,
 		ItemID
	 )SELECT 
 		/* warehousenumber  */ @warehouse,
 		/* itemno*/ @itemno ,
 		/* stocklocn*/ @branchno,
 		/* supplierno */ @suppliercode,
 		/* purchaseordernumber  */ @orderNumber,
 		/* expectedreceiptdate */ @datedue,
 		/* quantityonorder - smallint */ @quantity ,
 		/* quantityavailable - smallint */ @quantity,
 		/* ItemID - int */ StockInfo.ID 
 	  FROM StockInfo
 	  WHERE itemno =  @itemno
 	  AND RepossessedItem = 0
END
GO 


