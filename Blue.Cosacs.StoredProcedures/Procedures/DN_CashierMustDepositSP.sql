SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierMustDepositSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierMustDepositSP]
GO

CREATE PROCEDURE 	dbo.DN_CashierMustDepositSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_CashierMustDepositSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Cashier Safe Deposits
-- Date         : ??
--
-- This procedure will check whether casier must deposit when logging in.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 28/02/12  jec #9700 Amount for the Paymethods which are not displayed in Cashier deposits screen should be excluded from the amount available to deposit
-- ================================================
	-- Add the parameters for the stored procedure here
			@empeeno int,
			@mustdeposit bit OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE	@outstanding money,
			@sessionStart datetime,
			@datetotalled datetime,
			@hastotalled char(1)

	SELECT	@outstanding = isnull(sum(depositoutstanding),0)
	FROM		cashieroutstanding o 
		INNER JOIN code c on o.paymethod=c.code and c.category='FPM'		-- #9700	
	WHERE	empeeno = @empeeno
		and (LEN(c.additional2)>1 and SUBSTRING(c.additional2,2,1)!='0')	-- #9700

	EXEC DN_CashierSessionStartedSP 		@empeeno = @empeeno,
							@started = @sessionStart OUT,
							@return = @return OUT	

	SELECT	@datetotalled = datelstaudit 
	FROM		courtsperson
	WHERE	UserId = @empeeno

	IF(@sessionStart = '1/1/4000')		/* session has not started yet */
		SET	@hastotalled = 'Y'
	ELSE
		IF(@sessionStart > @datetotalled)
			SET	@hastotalled = 'N'
		ELSE
			SET	@hastotalled = 'Y'

	IF(@hastotalled = 'Y' AND @outstanding != 0)
		SET	@mustdeposit = 1
	ELSE
		SET	@mustdeposit = 0

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End