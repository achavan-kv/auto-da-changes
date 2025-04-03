SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierTotalsGetUnexportedSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierTotalsGetUnexportedSP]
GO

CREATE PROCEDURE 	dbo.DN_CashierTotalsGetUnexportedSP
			@branchno smallint,
			@total money OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	CT.id,
			CT.dateto,
			u.branchno,
			CT.empeeno,
			u.FullName as EmployeeName,
			CT.usertotal,
			CT.systemtotal,
			CT.difference,
			CT.deposittotal as deposit
	FROM		cashiertotals CT 
	INNER JOIN Admin.[User] u ON CT.empeeno = u.Id
	WHERE	CT.branchno = @branchno
	AND		CT.runno = 0

	SELECT	@total = sum(usertotal)
	FROM		cashiertotals
	WHERE	branchno = @branchno
	AND		runno = 0

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

