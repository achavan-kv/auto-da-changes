SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ReferralRuleWriteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ReferralRuleWriteSP]
GO

CREATE PROCEDURE 	dbo.DN_ReferralRuleWriteSP
			@custid varchar(20),
			@dateprop datetime,
			@code varchar(10),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	ReferralRules
	SET		referralcode = @code
	WHERE	custid = @custid
	AND		dateprop = @dateprop
	AND		referralcode = @code

	IF(@@rowcount = 0)
	BEGIN
		INSERT
		INTO		ReferralRules
				(custid, dateprop, referralcode)
		VALUES	(@custid, @dateprop, @code)
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

