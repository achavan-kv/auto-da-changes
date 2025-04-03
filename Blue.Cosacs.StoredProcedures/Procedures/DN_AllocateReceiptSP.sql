SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AllocateReceiptSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AllocateReceiptSP]
GO

CREATE PROCEDURE 	dbo.DN_AllocateReceiptSP
			@empeeno int = 0, 
			@branchno int,
			@firstreceiptno int ,
			@lastreceiptno int ,
			@issuedate datetime ,
			@return int OUTPUT

AS

	DECLARE @loopCounter int 
	SET @loopCounter = @firstreceiptno; 
	SET 	@return = 0			--initialise return code
	
	WHILE (@loopCounter <= @lastreceiptno)
	BEGIN
		INSERT INTO tempreceipt (origbr,receiptno,branchno,empeeno,datealloc,dateissued,acctno,amount,datepresent) 
		VALUES ( @branchno,@loopCounter,@branchno,@empeeno,@issuedate,@issuedate,'000000000000',0,null) --KEF changed datepresent to null as was displaying as 01-jan-1900 which is incorrect
		set @loopCounter = @loopCounter + 1
	END 

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
