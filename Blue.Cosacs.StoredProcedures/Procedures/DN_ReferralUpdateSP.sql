SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ReferralUpdateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ReferralUpdateSP]
GO




CREATE PROCEDURE 	dbo.DN_ReferralUpdateSP
			@origbr smallint,
			@custid varchar(20),
			@dateprop smalldatetime,
			@reflresult char(1),
			@empeeno int,
			@datereferral datetime,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	IF @dateprop IS NULL SET @DateProp = CONVERT(DATETIME,'1-Jan-1900',105)

	UPDATE	referral
	SET		origbr = @origbr,
			custid = @custid,
			dateprop = @dateprop,
			reflresult = @reflresult,
			empeeno = @empeeno,
			datereferral = @datereferral
	WHERE	custid = @custid
	AND		dateprop = @dateprop

	IF(@@rowcount = 0)
	BEGIN
		INSERT 
		INTO	referral
			(origbr, custid, dateprop,
			reflresult, empeeno, datereferral)
		VALUES
			(@origbr, @custid, @dateprop,
			@reflresult, @empeeno, @datereferral)
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

