if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_UpdateInvoiceVersion]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_UpdateInvoiceVersion] 
GO

/****** Object:  StoredProcedure [dbo].[DN_UpdateInvoiceVersion]    Script Date: 04-01-2019 11:19:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- CR 2018-13: Invoice CR 05/01/19 Added to save version number in InvoiceDetails table
CREATE PROCEDURE [dbo].[DN_UpdateInvoiceVersion]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_UpdateInvoiceVersion.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Invoice Version Update
-- Author       : ??
-- Date         : ??
--
-- This procedure will update the Invoice Version
-- 
-- Change Control
-- --------------
-- Date      By			Description
-- ----      --			-----------
-- 04/01/19  Raj Kishore	Created New SP to Save Version in [InvoiceDetails]

-- ================================================
	-- Add the parameters for the stored procedure here
			@acctNo varchar(14),
			@agrmtno int,
			@return int OUTPUT

AS
	SET  @return = 0			--initialise return code
	Declare @version smallint
    DECLARE @AgreementInvNoVersion varchar(20),@branchno smallint

	SET @AgreementInvNoVersion = NULL
	--Declare @agrmtno nvarchar(20),@version nvarchar(10)
	if (@agrmtno = '1')-- In case of Web POS do not add versions in Invoice detail table.
	BEGIN

		----Check if previous records are present in table.
		--IF EXISTS (SELECT * FROM invoiceDetails WHERE acctno = @acctno AND agrmtno = @agrmtno)
		--BEGIN	
		--	-- check if there is already record present with same datedel.		   
						
		--END
		SELECT @version = ISNULL(max(InvoiceVersion),0) + 1  FROM invoiceDetails WHERE acctno =@acctno AND agrmtno= @agrmtno

		IF @version > 99
		BEGIN 
			SET @version = 99
		END
	END
	ELSE
	BEGIN
		SET @version = 1 
	END

	select @AgreementInvNoVersion = AgreementInvoiceNumber from agreement where acctno = @acctno AND agrmtno= @agrmtno
	select @branchno=branchno from acct where acctno = @acctno
	IF (@AgreementInvNoVersion IS NULL)
	BEGIN 
		-- Generate new Invoice Number based on Branch Number.
		exec  GenerateInvoiceNumber @branchno, @AgreementInvNoVersion OUTPUT,0
		UPDATE agreement
		SET AgreementInvoiceNumber= @AgreementInvNoVersion
		where acctno = @acctno AND agrmtno= @agrmtno
	END


	

	--Update [dbo].[InvoiceDetails] set [InvoiceVersion] = @version where acctno =@acctno AND agrmtno= @agrmtno and [InvoiceVersion] = 0
	INSERT INTO [dbo].[InvoiceDetails]
	([acctno],[agrmtno],[InvoiceVersion],[datedel],[itemno],[stocklocn],[quantity],[branchno],Price,taxamt,ItemID,ParentItemID,AgreementInvNoVersion, LineItemID, contractno, OrdVal)

	SELECT acctno, agrmtno,@version,GetDate(),itemno,stocklocn,quantity,@branchno,price,taxamt,ItemID,ParentItemID,@AgreementInvNoVersion, ID
	, contractno, OrdVal
	FROM lineitem
	where acctno = @acctno AND agrmtno= @agrmtno

IF @version > 99
BEGIN 
SET @version = 99
END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
		
GO