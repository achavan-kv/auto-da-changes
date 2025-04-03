SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FintransGetAccountsAddedToSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FintransGetAccountsAddedToSP]
GO

CREATE PROCEDURE 	dbo.DN_FintransGetAccountsAddedToSP
			@acctno varchar(12),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	acctno
	FROM		fintransAddtos
	WHERE	acctnocheq = @acctno
	AND		acctno != @acctno
	GROUP BY	acctno
	HAVING	sum(transvalue) != 0

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

