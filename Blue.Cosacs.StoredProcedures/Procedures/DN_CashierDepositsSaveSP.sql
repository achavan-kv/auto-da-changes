SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

-- UAT issue 113 prevent saving of null cashierdeposit id - comes up with error after.

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierDepositsSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierDepositsSaveSP]
GO

CREATE PROCEDURE 	dbo.DN_CashierDepositsSaveSP
			@datedeposit datetime,
			@transtypecode varchar(4),
			@runno int,
			@empeeno int,
			@empeenoentered int,
			@cashiertotalid int,
			@empeenovoided int,
			@voided char(1),
			@datevoided datetime,
			@value money,
			@branch smallint,
			@reference varchar(30),
			@paymethod varchar(4),
			@includeincashiertotals smallint,
			@isCashierFloat smallint,
			@return int OUTPUT

AS

    DECLARE @sessionStart datetime
   if @cashiertotalid is null
      set @cashiertotalid=0

	SET 	@return = 0			--initialise return code

	UPDATE	cashierdeposits
	SET		runno = @runno,
			empeenoentered = @empeenoentered,
			cashiertotalid = @cashiertotalid,
			empeenovoided = @empeenovoided,
			voided = @voided,
			datevoided = @datevoided,
			depositvalue = @value,
			branchno = @branch,
			reference = @reference,
			includeincashiertotals = @includeincashiertotals,
			isFloat = @isCashierFloat
	WHERE	datedeposit = @datedeposit
	AND		code = @transtypecode
	AND		empeeno = @empeeno
	AND		paymethod = @paymethod
	AND     IsFloat = @isCashierFloat

	IF(@@rowcount = 0)
	BEGIN
		INSERT 
		INTO		cashierdeposits
				(datedeposit, code, runno, empeeno,
				empeenoentered, cashiertotalid, empeenovoided, 
				voided, datevoided, depositvalue, branchno, reference, paymethod,
				includeincashiertotals, isFloat)
		VALUES	(@datedeposit, @transtypecode, @runno, @empeeno,
				@empeenoentered, @cashiertotalid, @empeenovoided, 
				@voided, @datevoided, @value, @branch, @reference, @paymethod,
				@includeincashiertotals, @isCashierFloat)
	END


	EXEC DN_CashierSessionStartedSP @empeeno = @empeeno,
	                                @started = @sessionStart OUT,
                                    @return = @return OUT	

    IF (@sessionStart = '1/1/4000')
    BEGIN
        -- Session has not started yet
        -- so make sure these deposits are linked to the last totals
        UPDATE CashierDeposits
        SET    CashierTotalId = isnull((SELECT MAX(id)
                                 FROM   CashierTotals
                                 WHERE  EmpeeNo = @EmpeeNo),0)
        WHERE  CashierTotalId = 0
        AND    EmpeeNo = @EmpeeNo
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

