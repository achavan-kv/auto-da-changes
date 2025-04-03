SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LetterWriteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LetterWriteSP]
GO



CREATE PROCEDURE 	dbo.DN_LetterWriteSP
			@origbr smallint,
			@acctno varchar(12),
			@dateacctlttr datetime,
			@datedue datetime,
			@lettercode varchar(10),
			@addtovalue money,
            @excelGen char,    -- CR633 jec 20/06/06
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	INSERT 
	INTO		letter							
			(runno, acctno, dateacctlttr,	--IP - 26/05/09 - Changed 'Origbr' to 'Runno' as column now called 'Runno'.
			datedue, lettercode, addtovalue, excelGen)
	VALUES	(0, @acctno, @dateacctlttr, 
			@datedue, @lettercode, @addtovalue, @excelGen)

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

