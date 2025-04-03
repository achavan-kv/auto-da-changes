SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CodesGetAddressTypesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CodesGetAddressTypesSP]
GO






CREATE PROCEDURE 	dbo.DN_CodesGetAddressTypesSP
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	DISTINCT
			code,
			codedescript
	FROM		code
	WHERE	(category = 'CT1'	
	OR		category = 'CA1')
	AND		statusflag = 'L'


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END





GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

