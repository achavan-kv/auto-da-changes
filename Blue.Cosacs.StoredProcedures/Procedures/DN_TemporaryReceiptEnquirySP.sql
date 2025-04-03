SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TemporaryReceiptEnquirySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TemporaryReceiptEnquirySP]
GO
CREATE PROCEDURE 	dbo.DN_TemporaryReceiptEnquirySP
			@empeeno int = 0, 
			@firstreceipt int = 0 ,
			@lastreceipt int = 0 ,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
   /*refcode although stored as varchar 3 is in fact varchar 2-but for some reason on the import a
line character is stored as the third digit*/

	IF(@empeeno != 0)
		BEGIN
			SELECT 	tr.origbr,
				tr.receiptno,
				tr.branchno,
				tr.empeeno,
				tr.datealloc,
				tr.dateissued,
				tr.acctno,
				tr.amount,
				tr.datepresent,
				emp.empeename AS EmployeeName
			 FROM tempreceipt tr, courtsperson emp
			 WHERE tr.empeeno = @empeeno
			 AND tr.empeeno = emp.empeeno 
			 
		END
	ELSE 
		BEGIN
			SELECT tr.origbr,
				tr.receiptno,
				tr.branchno,
				tr.empeeno,
				tr.datealloc,
				tr.dateissued,
				tr.acctno,
				tr.amount,
				tr.datepresent,
				emp.empeename AS EmployeeName
			  FROM tempreceipt tr, courtsperson emp
			 WHERE receiptno between @firstreceipt and @lastreceipt
			   AND tr.empeeno = emp.empeeno
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
