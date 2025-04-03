SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BailiffTemporaryReceiptEnquirySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BailiffTemporaryReceiptEnquirySP]
GO
CREATE PROCEDURE 	dbo.DN_BailiffTemporaryReceiptEnquirySP
			@empeeno int = 0, 
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT 	tr.origbr,
		tr.receiptno,
		tr.branchno,
		tr.empeeno,
		tr.datealloc,
		tr.dateissued,
		tr.acctno,
		tr.amount,
		tr.datepresent,
		u.FullName AS EmployeeName
	  FROM tempreceipt tr
  	  INNER JOIN Admin.[User] u ON u.id = tr.empeeno
  	  WHERE tr.empeeno = @empeeno
  	  
	   --AND (tr.acctno <> '000000000000'  AND amount > 0 ) 
      AND  tr.acctno not like 'C%' --excluding cancelled

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
