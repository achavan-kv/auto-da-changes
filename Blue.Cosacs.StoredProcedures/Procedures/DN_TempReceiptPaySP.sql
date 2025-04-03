SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TempReceiptPaySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TempReceiptPaySP]
GO


CREATE PROCEDURE 	dbo.DN_TempReceiptPaySP
			@acctno varchar(12),
			@receiptno int,
			@payment money,
			@date datetime,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	tempreceipt
	SET		amount = @payment,
			datepresent = @date,
			acctno = @acctno
	WHERE	receiptno = @receiptno

	IF(@@rowcount = 0)
	BEGIN
		RAISERROR('Temp Receipt %d does not exisit', 16, 1, @receiptno)
		SET @return = -1	
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

