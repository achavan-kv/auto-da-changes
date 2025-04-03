SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TermsTypeVariableRateSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TermsTypeVariableRateSaveSP]
GO
CREATE PROCEDURE 	dbo.DN_TermsTypeVariableRateSaveSP
			@termstype varchar(4),
			@intratefrom datetime,
			@intrateto datetime,
			@frommonth int,
			@tomonth int,
			@rate float,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	INSERT 
	INTO		TermsTypeVariableRates
			(termstype, intratefrom, intrateto, frommonth, tomonth, rate)
	VALUES	(@termstype, @intratefrom, @intrateto, @frommonth, @tomonth, @rate)

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

