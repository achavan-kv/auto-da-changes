SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CodesGetCustomerSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CodesGetCustomerSP]
GO






CREATE PROCEDURE 	dbo.DN_CodesGetCustomerSP
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	code as code,
			codedescript as description
	FROM		code
	WHERE	category in ('CC1','CC2')

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END





GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

