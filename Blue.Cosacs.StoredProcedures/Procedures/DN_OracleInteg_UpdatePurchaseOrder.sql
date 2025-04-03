SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_OracleInteg_UpdatePurchaseOrder]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_OracleInteg_UpdatePurchaseOrder]
GO

-- =============================================
-- Author:		Mohamed Nasmi
-- Create date: 03/03/2009
-- Description:	To update PurchaseOrderOutstanding table from Oracle inbound CSV files
-- =============================================

CREATE PROCEDURE [dbo].[DN_OracleInteg_UpdatePurchaseOrder] 
	@itemno varchar(10),
	@stocklocn smallint,
	@supplierno varchar(12),
	@purchaseordernumber varchar(12),
	@expectedreceiptdate datetime,
	@quantityonorder smallint,
	@quantityavailable smallint,	
	@return INT OUTPUT

AS
	-- SET NOCOUNT ON added to marginally improve performance 
	SET NOCOUNT ON;
    SET @return = 0
	
	DECLARE @warehousenumber varchar(3), @currentQtyAvailable as smallint, @currentQtyOnOrder as smallint
	
	SELECT @warehousenumber = warehouseno FROM branch
	WHERE branchno = @stocklocn
	
	SELECT @currentQtyAvailable = quantityavailable,
		   @currentQtyOnOrder =  quantityonorder
	FROM PurchaseOrderOutstanding
	WHERE itemno = @itemno and stocklocn = @stocklocn and purchaseordernumber = @purchaseordernumber
	
	IF @currentQtyAvailable is not NULL and @currentQtyOnOrder is not NULL 
	BEGIN
	   SET @quantityavailable = @quantityonorder - ( @currentQtyOnOrder - @currentQtyAvailable)
	END
	 
	------------------------------------------------------
	
	UPDATE PurchaseOrderOutstanding
	SET 
		warehousenumber = @warehousenumber,
		supplierno = @supplierno,	
		expectedreceiptdate = @expectedreceiptdate,
		quantityonorder = @quantityonorder,
		quantityavailable = @quantityavailable
	WHERE 
	itemno = @itemno and stocklocn = @stocklocn and purchaseordernumber = @purchaseordernumber

	IF @@ROWCOUNT = 0
	
    INSERT INTO PurchaseOrderOutstanding
    ( 
		warehousenumber, itemno, stocklocn,
		supplierno, purchaseordernumber, expectedreceiptdate,
		quantityonorder, quantityavailable		
	)
	VALUES
	( 
		@warehousenumber, @itemno, @stocklocn,
		@supplierno, @purchaseordernumber, @expectedreceiptdate,
		@quantityonorder, @quantityavailable
	) 

	SET @return = @@ERROR

	RETURN @return
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO