SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemUpdateTaxAmountSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemUpdateTaxAmountSP]
GO

CREATE PROCEDURE 	dbo.DN_LineItemUpdateTaxAmountSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemUpdateTaxAmountSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Line Item Update Tax Amount
-- Author       : ??
-- Date         : ??
--
-- This procedure will update the LineItem and LineItemAudit tables
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 21/11/07  jec UAT114 add ParentItemno and ParentLocation to LineItemAudit
-- 18/05/11  IP/NM CR1212 - #3627 - use itemID
-- 19/06/11  IP - CR1212 - RI - #3949 - SalesBrnNo was not being passed into procedure to update lineitemaudit table.
-- ================================================
	-- Add the parameters for the stored procedure here
			@acctno varchar(12),
			--@itemno varchar(10),
			@itemID int,					--IP/NM - 18/05/11 -CR1212 - #3627
			@branchno smallint,
			@taxamount money,
			@agrmtno int,
			@source varchar(16), --IP - CR929 & 974 - Deliveries - changed from (15) to (16)
			@empeenochange int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	
	DECLARE	@stax money, @oldSTAXValue money, 
			@newSTAXValue MONEY, @dateChange datetime

	SET @dateChange = GETDATE()

	UPDATE	lineitem
	SET		taxamt = @taxamount 
	WHERE	acctno = @acctno
	--AND		itemno = @itemno
	AND     ItemID = @itemID			--IP/NM - 18/05/11 -CR1212 - #3627
	AND		stocklocn = @branchno

	DECLARE @staxID int
	select @staxID = ID from StockInfo where IUPC = 'STAX'	--IP/NM - 18/05/11 -CR1212 - #3627
	
	SELECT	@stax = ISNULL(SUM(taxamt),0)
	FROM		lineitem
	WHERE	acctno = @acctno
	--AND		itemno != 'STAX'
	AND ItemID != @staxID				--IP/NM - 18/05/11 -CR1212 - #3627

	SELECT	@oldSTAXValue = ISNULL(ordval,0)
	FROM	lineitem
	WHERE	acctno = @acctno
	--AND		itemno = 'STAX'
	AND		ItemID = @staxID

	UPDATE 	lineitem
	SET		price = @stax,
			ordval = @stax
	WHERE	acctno = @acctno
	--AND		itemno = 'STAX'				--IP/NM - 18/05/11 -CR1212 - #3627
	AND		ItemID = @staxID

	SELECT	@newSTAXValue = ISNULL(ordval,0)
	FROM	lineitem
	WHERE	acctno = @acctno
	--AND		itemno = 'STAX'
	AND		ItemID = @staxID			--IP/NM - 18/05/11 -CR1212 - #3627
	
	IF ( ISNULL(@oldSTAXValue,0) != ISNULL(@newSTAXValue,0) )
	BEGIN
		--DECLARE @staxId INT
		--select @staxId = ID from StockInfo where IUPC = 'STAX'
		/* write an audit record */
		EXEC DN_LineItemAuditUpdateSP 	@acctno = @acctno, @agrmtno = @agrmtno,
										@empeenochange = @empeenochange, 
										@itemID = @staxId,
										@stocklocn = @branchno, @quantitybefore = 1,
										@quantityafter = 1, @valuebefore = @oldSTAXValue,
										@valueafter = @stax, @taxamtbefore = 0,
										@taxamtafter = 0, @datechange = @dateChange,
										@contractno = '', @source = @source,
										@parentitemID = 0,			-- jec 21/11/07
										@parentStockLocn = ' ',		-- jec 21/11/07
                                        @delNoteBranch = 0,
                                        @salesBrnNo = 0,				--IP - 18/06/11 - CR1212 - RI - #3949 
										@return = @return OUT	
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO