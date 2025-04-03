SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LetterWritePotentialSpendSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LetterWritePotentialSpendSP]
GO



CREATE PROCEDURE 	dbo.DN_LetterWritePotentialSpendSP
			@origbr smallint,
			@acctno varchar(12),
			@dateacctlttr datetime,
			@datedue datetime,
			@lettercode varchar(10),
			@addtovalue money,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE @count int,
		@months int
    
	SELECT	@months = value
	FROM	countrymaintenance
	WHERE	codename = 'creditlettermonths'

	SELECT	@count = COUNT(*)
	FROM	letter
	WHERE	acctno = @acctno
	AND	lettercode = @lettercode
	AND	GETDATE() < DATEADD(MONTH, @months, dateacctlttr)

	IF(@count = 0)
	BEGIN
		INSERT 
		INTO	letter	
			(runno, acctno, dateacctlttr,  --IP - 26/05/09 - Changed 'Origbr' to 'Runno' as column now called 'Runno'.
			datedue, lettercode, addtovalue)
		VALUES	(0, @acctno, @dateacctlttr, 
			 @datedue, @lettercode, @addtovalue)
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO