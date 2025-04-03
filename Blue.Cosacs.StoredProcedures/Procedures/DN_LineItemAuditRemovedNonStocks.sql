IF EXISTS(SELECT * FROM sysobjects WHERE NAME = 'DN_LineItemAuditRemovedNonStocks')
DROP PROCEDURE dbo.DN_LineItemAuditRemovedNonStocks
GO 
CREATE PROCEDURE dbo.DN_LineItemAuditRemovedNonStocks 	@acctNo char(12), --IP -12/03/08 Livewire: (69281) Changed from varchar
			@agrmtno int, @empeenochange INT,@return int OUTPUT
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemAuditRemovedNonStocks.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ??
-- Date         : ??
--
-- This procedure will update the LineItemAudit table.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 24/05/11  IP  CR1212 - RI - #3651 - Sales Branch required
-- ================================================
AS  
SET @return = 0 
/* This procedure will put records into the lineitem audit ,table for non stock items which have been removed from a sale
-- these are not left in the XML array so were not being saved to the lineitemaudit table */
DECLARE 	@origbr smallint,
			@itemNo varchar(8), --IP  12/03/08 Livewire: (69281) changed from varchar
			@itemId INT,
			@itemSuppText varchar(76),			
			@quantity float,			
			@stockLocn smallint,
			@price money,			
			@orderValue money,			
			@dateReqDel datetime,
			@timeReqDel varchar(12),			
			@datePlanDel datetime,			
			@delNoteBranch smallint,
			@qtyDiff char(1),			
			@itemType varchar(1),			
			@hasString smallint,
			@notes varchar(200),			
			@taxAmount float,
			@parentItemNo varchar(8), 	
			@parentItemId INT,
			@parentStockLocn smallint,		
			@isKit smallint,			
			@deliveryAddress char(2),
			@ordbuffno int,
			@contractNo varchar(10), 
			@expectedreturndate datetime,	
			@countrycode char(1), 
			@deliveryarea varchar(8),
			@deliveryprocess char(1),	
			@damaged char(1),
			@assembly char(1),	
			@source char(15),
			@salesBrnNo smallint			--IP - 24/05/11 - CR1212 - RI - #3651
SET @source = 'Revise'

declare @branchno smallint,	@datetrans DateTime, 
	@delivered_quantity smallint,@scheduled_qty smallint,
    @category smallint, @custid varchar (50),
	@oldquantity float, @oldvalue money,@oldtaxamt float,  -- 67977 RD
	@datechange datetime, @auditdataperiod smallint

SET @datechange = GETDATE()
declare acct_cursor CURSOR FAST_FORWARD READ_ONLY FOR
SELECT itemno,stocklocn,quantity,contractno,taxamt,parentitemno,ParentLocation, l.ItemID, l.ParentItemID, l.SalesBrnNo	--IP - 24/05/11 - CR1212 - RI - #3651
FROM lineitem l WHERE acctno = @acctno AND agrmtno = @agrmtno AND quantity= 0 
AND EXISTS (SELECT * FROM lineitem_amend x WHERE l.acctno= x.acctno AND l.ItemID = x.ItemID 
												 AND l.stocklocn=  x.stocklocn AND l.agrmtno = x.agrmtno
												 AND l.contractno = x.contractno AND x.quantity <>0)
OPEN acct_cursor
FETCH NEXT FROM acct_cursor 
	INTO @itemno,@stocklocn, @quantity,@contractno , @taxamount ,@parentitemno,@parentStockLocn, @itemId, @parentItemId, @salesBrnNo --IP - 24/05/11 - CR1212 - RI - #3651
WHILE @@FETCH_STATUS = 0
BEGIN



	/* retrieve the old quantity */
	EXEC DN_LineItemGetOldQtySP 	@acctno = @acctNo,
					@itemId = @itemId,
					@stocklocn = @stockLocn,
					@contractno = @contractNo,
					@agreementno = @agrmtno,
					@parentitemid = @parentitemid,
					@quantity = @oldquantity OUT,
					@return = @return OUT
	SET	@oldquantity = isnull(@oldquantity,0)

	/* retrieve the old value */
	EXEC DN_LineItemGetOldValueSP 	@acctno = @acctNo,
					@itemId = @itemId,
					@stocklocn = @stockLocn,
					@contractno = @contractNo,
					@agreementno  = @agrmtno,
					@parentitemid = @parentitemid,
					@value = @oldvalue OUT,
					@taxamt = @oldtaxamt OUT,  -- 67977 RD
					@return = @return OUT
					
	SET	@oldvalue = isnull(@oldvalue,0)
	SET @oldtaxamt = isnull(@oldtaxamt,0)   -- 67977 RD 

	IF( (@quantity != @oldquantity) OR
	    (@orderValue != @oldvalue) OR 
	    (@taxAmount != @oldtaxamt) )   -- 67977 RD
	    
	BEGIN
		/* write an audit record */
		EXEC DN_LineItemAuditUpdateSP 	@acctno = @acctNo,
						@agrmtno = @agrmtno,
						@empeenochange = @empeenochange,
						@itemId = @itemId,
						@stocklocn = @stockLocn,
						@quantitybefore = @oldquantity,
						@quantityafter = @quantity,
						@valuebefore = @oldvalue,
						@valueafter = @orderValue,
						@taxamtbefore = @oldtaxamt,   -- 67977 RD
						@taxamtafter = @taxAmount,    -- 67977 RD
						@datechange = @datechange,
						@contractno = @contractNo,
						@source = @source,
						@parentItemId = @parentItemId,			-- jec 21/11/07
						@parentStockLocn = @parentStockLocn,	-- jec 21/11/07
						@delNoteBranch = @delNoteBranch,		-- ip - 05/02/10 - CR1072 - 3.1.12
						@salesBrnNo = @salesBrnNo,				-- ip - 24/05/11 - CR1212 - RI - #3651
						@return = @return OUT	
	END



FETCH NEXT FROM acct_cursor 
	INTO @itemno,@stocklocn, @quantity,@contractno , @taxamount ,@parentitemno,@parentStockLocn, @itemId, @parentItemId, @salesBrnNo --ip - 24/05/11 - CR1212 - RI - #3651


END

CLOSE acct_cursor
DEALLOCATE acct_cursor

	/* delete expired audit records */
	SELECT 	@auditdataperiod = isnull(Convert(smallint, Value),3)
	FROM	CountryMaintenance
	WHERE	codename = 'auditdataperiod'
	AND		CountryCode = @countrycode

	DELETE 
	FROM 	LineItemAudit 
	WHERE 	datechange < dateadd (month,-@auditdataperiod, getdate())
	AND 	acctno = @acctNo AND agrmtno= @agrmtno 

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO 
