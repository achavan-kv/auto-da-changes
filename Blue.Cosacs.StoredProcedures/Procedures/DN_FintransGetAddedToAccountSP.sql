SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FintransGetAddedToAccountSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FintransGetAddedToAccountSP]
GO


CREATE PROCEDURE 	dbo.DN_FintransGetAddedToAccountSP
			@acctno varchar(12),
			@addToValue money,
			@addedTo varchar(12) OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

    -- DSR 10 June 2004
    -- Not using @addToValue (was passed in as zero anyway) due to
    -- possibly more than one ADD transaction with the same value.
    -- So get the latest negative ADD transaction (ADDCR)
	SELECT	TOP 1 @addedTo = chequeno
	FROM	fintrans
	WHERE	acctno = @acctno
	AND		transtypecode = 'ADD'
	AND		transvalue < 0
	ORDER BY DateTrans DESC

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

