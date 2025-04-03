SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ReallocateReceiptSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ReallocateReceiptSP]
GO

CREATE PROCEDURE 	dbo.DN_ReallocateReceiptSP
			@empeeno int = 0, 
			@branchno int,
			@firstreceiptno int ,
			@lastreceiptno int ,
			@return int OUTPUT

AS
 
	SET 	@return = 0			--initialise return code
	
	UPDATE  tempreceipt 
	   SET  branchno =  @branchno,
		empeeno = @empeeno,
		datealloc = GETDATE() 
	 WHERE  receiptno >= @firstreceiptno 
	   AND  receiptno <= @lastreceiptno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
