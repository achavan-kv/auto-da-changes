SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemUpdateQuantitySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemUpdateQuantitySP]
GO


CREATE PROCEDURE 	dbo.DN_LineItemUpdateQuantitySP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemUpdateQuantitySP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Line Item Update Quantity
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
-- 05/02/10  ip  CR1072 - 3.1.12 - add DelNoteBranch to LineItemAudit
-- 13/10/11  jec CR1232 add salesbrno parm
-- ================================================
	-- Add the parameters for the stored procedure here
			@acctNo varchar(12),
			@itemId int,
			@location smallint,
			@newQty float,
			@agreementno int,
			@contractno varchar(10),
			@source varchar(15),
			@parentItemId int,--uat363 rdb add parentItemno

			@user int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE	@stax money
	DECLARE @oldQty float	,@oldSTAXValue money, @newSTAXValue MONEY
	SET	@oldQty = 0

	DECLARE	@ratio float, @oldvalue money, @datechange datetime
	DECLARE @delnotebranch smallint --IP - 05/02/10 - CR1072 - 3.1.12
	
	SET	@ratio = 1
	SET @stax = 0
	SET @oldvalue = 0
	SET	@datechange = GETDATE()
	
	DECLARE @staxId INT
	select @staxId = ID from StockInfo where IUPC = 'STAX'

	SELECT	@oldvalue = ISNULL(ordval , 0)
	FROM	lineitem
	WHERE	acctno = @acctno
	AND		agrmtno = @agreementno
	AND		itemId = @staxId

	/* make sure we have an update lock */
	UPDATE	lineitem
	SET	acctno = @acctno
	WHERE	acctno = @acctno
	AND	agrmtno = @agreementno
	AND	itemId = @itemId
	AND	stocklocn = @location
	AND	contractno = @contractno
	AND parentItemId  = @parentItemId
	
	--IP - 05/02/10 - CR1072 - 3.1.12 - Select the DelNoteBranch from LineItem for the Item
	SELECT @delnotebranch = delnotebranch
	FROM	lineitem
	WHERE	acctno = @acctno
	AND	agrmtno = @agreementno
	AND	itemId = @itemId
	AND	stocklocn = @location
	AND	contractno = @contractno
	AND parentItemId = @parentItemId

	SELECT	@oldQty = quantity 
	FROM	lineitem
	WHERE	acctno = @acctno
	AND	agrmtno = @agreementno
	AND	itemId = @itemId
	AND	stocklocn = @location
	AND	contractno = @contractno
	AND parentItemId  = @parentItemId

	SELECT	@oldSTAXValue = ISNULL(ordval,0)
	FROM	lineitem
	WHERE	acctno = @acctno
	AND		agrmtno = @agreementno
	AND		itemId = @staxId

	IF(@oldQty != 0)
		SET	@ratio = @newQty / @oldQty

	UPDATE	lineitem
	SET	quantity = @newQty,
		ordval = @newQty * price,
		taxamt = taxamt * @ratio
	WHERE	acctno = @acctNo
	AND	agrmtno = @agreementno
	AND	itemId = @itemId
	AND	stocklocn = @location
	AND	contractno = @contractno
	AND parentItemId  = @parentItemId

	SELECT	@stax = ISNULL(sum(taxamt),0)
	FROM	lineitem
	WHERE	acctno = @acctNo
	AND	itemId != @staxId
	AND	agrmtno = @agreementno
	
	UPDATE 	lineitem
	SET	price = @stax,
		ordval = @stax
	WHERE	acctno = @acctNo
	AND	@itemId = @staxId
	AND	agrmtno = @agreementno
	
	SELECT	@newSTAXValue = ISNULL(ordval,0)
	FROM	lineitem
	WHERE	acctno = @acctno
	AND		agrmtno = @agreementno
	AND		ItemId = @staxId
	
	IF ( ISNULL(@oldSTAXValue,0) != ISNULL(@newSTAXValue,0) )
	BEGIN
		/* write an audit record */
		EXEC DN_LineItemAuditUpdateSP 	@acctno = @acctNo,
										@agrmtno = @agreementNo,
										@empeenochange = @user,
										@itemID = @staxId,
										@stocklocn = @location,
										@quantitybefore = 1,
										@quantityafter = 1,
										@valuebefore = @oldvalue,
										@valueafter = @stax,
										@taxamtbefore = 0,
										@taxamtafter = 0,
										@datechange = @datechange,
										@contractno = '',
										@source = @source,
										@parentItemId = 0,			-- jec 21/11/07
										@parentStockLocn = 0,		-- jec 21/11/07
										@delnotebranch  =  @delnotebranch,  --ip - 05/02/10 - CR1072 - 3.1.12
										@salesBrnNo=0,		-- jec CR1232
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

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End