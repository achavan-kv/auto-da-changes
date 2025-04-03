SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierDepositIsReferenceUniqueSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierDepositIsReferenceUniqueSP]
GO

CREATE PROCEDURE 	dbo.DN_CashierDepositIsReferenceUniqueSP
			@reference varchar(50),
			@unique smallint out,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	1 
	FROM		cashierdeposits 
	WHERE	reference = @reference
	and		code <> 'SAF'
	
	IF(@@rowcount = 0)
		SET	@unique = 1
	ELSE
		SET	@unique = 0

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

