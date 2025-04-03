SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AddToCalcSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AddToCalcSP]
GO

CREATE PROCEDURE 	dbo.DN_AddToCalcSP
			@acctno varchar(12),
			@rebate money, 
			@value money OUT,
			@return int OUTPUT

AS
/*	
	this is a wrapper procedure to call the DBAddToCalc procedure. 
	A wrapper is required because of the standard format of .net procedures
	requiring an output parameter called @return
*/

	SET 	@return = 0			--initialise return code

	EXEC	@return  = dbaddtocalc
		@acctno = @acctno,
		@value = @value OUT,
		@rebate = @rebate

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

