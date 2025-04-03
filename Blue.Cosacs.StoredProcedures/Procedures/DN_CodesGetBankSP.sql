SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CodesGetBankSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CodesGetBankSP]
GO






CREATE PROCEDURE 	dbo.DN_CodesGetBankSP
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	bankcode,
			bankname
	FROM		bank


			

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END





GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

