SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierTotalsSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierTotalsSaveSP]
GO

CREATE PROCEDURE 	dbo.DN_CashierTotalsSaveSP
			@datefrom datetime,
			@dateto datetime,
			@empeeno int,
			@runno int,
			@empeenoauth int,
			@usertotal money,
			@systemtotal money,
			@difference money,
			@deposit money,
			@branchno smallint,
			@identity int OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
  SET 	@datefrom  =convert (datetime,convert (varchar(20),@datefrom,120))
  SET 	@dateto  =convert (datetime,convert (varchar(20),@dateto,120))
	UPDATE	CashierTotals
	SET		runno	=	@runno,
			empeenoauth = @empeenoauth,
			usertotal = @usertotal,
			systemtotal = @systemtotal,
			difference = @difference,
			deposittotal = @deposit,
			branchno = @branchno
	WHERE	datefrom = @datefrom
	AND		dateto = @dateto
	AND		empeeno = @empeeno

	IF(@@rowcount=0)
	BEGIN
		INSERT
		INTO		CashierTotals
				(datefrom, dateto, empeeno, runno, 
				empeenoauth, usertotal, systemtotal, 
				difference, deposittotal, branchno)
		VALUES	(@datefrom, @dateto, @empeeno, @runno,
				@empeenoauth, @usertotal, @systemtotal,
				@difference, @deposit, @branchno)
	END

	-- DSR 21/4/05 The update above will not set @@identity
	-- SET  @identity = @@identity
	-- Instead select the id from the table
	SELECT @identity = id
	FROM   CashierTotals
	WHERE  datefrom = @datefrom
	AND    dateto   = @dateto
	AND    empeeno  = @empeeno

    -- DSR 3/5/05 Do not update all deposits in this date range because the
    -- final deposits for the last set of totals were done after their totals
	UPDATE	CashierDeposits
	SET		cashiertotalid = @identity
	WHERE	empeeno = @empeeno
	AND		datedeposit BETWEEN @datefrom AND @dateto
	AND     cashiertotalid = 0

	UPDATE	courtsperson
	SET		datelstaudit = @dateto
	WHERE	userid = @empeeno
		

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

