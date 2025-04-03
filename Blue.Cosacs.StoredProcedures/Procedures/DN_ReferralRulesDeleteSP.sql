SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ReferralRulesDeleteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ReferralRulesDeleteSP]
GO

CREATE PROCEDURE 	dbo.DN_ReferralRulesDeleteSP
			@custid varchar(20),
			@dateprop datetime,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DELETE
	FROM		ReferralRules
	WHERE	custid = @custid
	AND		dateprop = @dateprop

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

