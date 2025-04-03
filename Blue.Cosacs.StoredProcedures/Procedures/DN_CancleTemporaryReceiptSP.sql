SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CancleTemporaryReceiptSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CancleTemporaryReceiptSP]
GO
CREATE PROCEDURE 	dbo.DN_CancleTemporaryReceiptSP
			@receiptno int = 0 ,
			@empeeno int = 0,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
   /*refcode although stored as varchar 3 is in fact varchar 2-but for some reason on the import a
line character is stored as the third digit*/

	UPDATE tempreceipt 
	   SET acctno = 'C' + CAST(@empeeno as varchar),
	       datepresent = GETDATE()
	 WHERE receiptno = @receiptno
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

