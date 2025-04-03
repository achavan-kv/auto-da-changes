if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_UpdateInvoiceVersionForGRT]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_UpdateInvoiceVersionForGRT] 
GO

/****** Object:  StoredProcedure [dbo].[DN_UpdateInvoiceVersionForGRT]    Script Date: 04-01-2019 11:19:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- CR 2018-13: Invoice CR 05/01/19 Added to save version number in InvoiceDetails table
CREATE  PROCEDURE [dbo].[DN_UpdateInvoiceVersionForGRT]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_UpdateInvoiceVersionForGRT.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Invoice Version Update For GRT
-- Author       : ??
-- Date         : ??
--
-- This procedure will update the Invoice Version
-- 
-- Change Control
-- --------------
-- Date      By			Description
-- ----      --			-----------
-- 16/04/19  Suvidha	Created New SP to Save Version in [InvoiceDetails]

-- ================================================
	-- Add the parameters for the stored procedure here
	@ReturnQuantity int,
	@RetItemNo varchar(18),
	@RetVal float,
	@orig_item_no varchar(18),
	@acctNo varchar(12),
	@agrmtno int,
	@contractNo varchar(10),
	@parentitemID int,
	@lineItemID int,
	@orderValue money,
	@taxAmt float,
	@return int OUTPUT
AS
BEGIN
    Declare @localtaxAmt float
	SET @localtaxAmt = 0
	SET  @return = 0			--initialise return code
	Declare @version smallint,@AgreementInvNoVersion varchar(20),@branchno smallint
	SET NOCOUNT ON;

	SET @AgreementInvNoVersion = NULL
	select @AgreementInvNoVersion = AgreementInvoiceNumber from agreement where acctno = @acctno AND agrmtno= @agrmtno
	select @branchno=branchno from acct where acctno = @acctno
		--Get maximum varsion number for selected account.
	SELECT @version = ISNULL(max(InvoiceVersion),0)  FROM invoiceDetails WHERE acctno =@acctno AND agrmtno= @agrmtno

	IF (@AgreementInvNoVersion IS NULL)
	BEGIN 
		-- Generate new Invoice Number based on Branch Number.
		exec  GenerateInvoiceNumber @branchno, @AgreementInvNoVersion OUTPUT,0
		UPDATE agreement
		SET AgreementInvoiceNumber= @AgreementInvNoVersion
		where acctno = @acctno AND agrmtno= @agrmtno

			UPDATE invoicedetails
			SET AgreementInvNoVersion = @AgreementInvNoVersion
			WHERE itemno = @orig_item_no and acctno = @acctno AND agrmtno= @agrmtno and InvoiceVersion = @version
			and ParentItemID = @parentitemID
	END

	--Update Invoice details table for returned item. 
	--IF(@orig_item_no = 'STAX')
	--BEGIN

			SELECT @localtaxAmt = sum(taxamt) FROM invoicedetails
			WHERE itemno = @orig_item_no and acctno = @acctno AND agrmtno= @agrmtno and InvoiceVersion = @version
	--		--and ParentItemID = @parentitemID and LineItemID = @lineItemID
	--		 --select * from ttt11
	         -- delete from ttt11
			
	--END

	UPDATE invoicedetails
	SET returnquantity = -@ReturnQuantity,
	quantity = -@ReturnQuantity,
	RetItemNo= @RetItemNo,
	RetVal= @RetVal,
	taxAmt=-@taxAmt,
	contractno = @contractNo
	, OrdVal = @orderValue
	WHERE itemno = @orig_item_no and acctno = @acctno AND agrmtno= @agrmtno and InvoiceVersion = @version
	and ParentItemID = @parentitemID and LineItemID = @lineItemID

	SELECT @localtaxAmt = sum(taxamt) FROM invoicedetails
			WHERE acctno = @acctno AND agrmtno= @agrmtno and InvoiceVersion = @version

     

	UPDATE invoicedetails SET RetVal = @localtaxAmt, ordval =  @localtaxAmt , price = @localtaxAmt
	WHERE itemno = 'STAX' and acctno = @acctno AND agrmtno= @agrmtno and InvoiceVersion = @version
	
	
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
END



GO