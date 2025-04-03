SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CourtsPersonGetOutstandingPaymentsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CourtsPersonGetOutstandingPaymentsSP]
GO

CREATE PROCEDURE 	dbo.DN_CourtsPersonGetOutstandingPaymentsSP
			@branchno smallint,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	

	SELECT 	DISTINCT
			u.id,
			u.FullName  AS EmployeeName
	FROM Admin.[User] u 
	INNER JOIN fintrans_new_income FT ON ft.empeeno = u.id
	AND		FT.branchno = @branchno


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

