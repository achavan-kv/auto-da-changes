SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TransTypeGetDepositTypesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TransTypeGetDepositTypesSP]
GO

CREATE PROCEDURE 	dbo.DN_TransTypeGetDepositTypesSP
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	transtypecode as code,
			description as codedescript,
			isdeposit,
			referencemandatory,
			referenceunique
	FROM		transtype 
	WHERE	isdeposit != 0

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

