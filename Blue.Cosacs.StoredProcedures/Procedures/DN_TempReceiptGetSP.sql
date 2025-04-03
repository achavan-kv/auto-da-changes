SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TempReceiptGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TempReceiptGetSP]
GO

CREATE PROCEDURE 	dbo.DN_TempReceiptGetSP
			@receiptNo int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	ReceiptNo,
			BranchNo,
			empeeno,
			DateAlloc,
			DateIssued,
			AcctNo,
			Amount,
			DatePresent
	FROM		tempreceipt
	WHERE	ReceiptNo = @receiptNo
	order by DateAlloc	--UAT 35

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

