SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierTotalsBreakdownSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierTotalsBreakdownSaveSP]
GO

CREATE PROCEDURE 	dbo.DN_CashierTotalsBreakdownSaveSP
			@cashiertotalid int,
			@paymethod varchar(4),
			@systemvalue money,
			@uservalue money,
			@depositvalue money,
			@differencevalue money,
			@reason varchar(100),
			@securitisedValue money,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	CashierTotalsBreakdown
	SET		systemtotal = @systemvalue,
			usertotal = @uservalue,
			deposit = @depositvalue,
			difference = @differencevalue,
			reason = @reason,
			securitisedtotal  = @securitisedValue
	WHERE	cashiertotalid = @cashiertotalid
	AND		paymethod = @paymethod

	IF(@@rowcount = 0)
	BEGIN
		INSERT
		INTO		CashierTotalsBreakdown
				(cashiertotalid, paymethod, systemtotal, usertotal, deposit, difference, reason, securitisedtotal)
		VALUES	(@cashiertotalid, @paymethod, @systemvalue, @uservalue, @depositvalue, @differencevalue, @reason, @securitisedValue)
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

