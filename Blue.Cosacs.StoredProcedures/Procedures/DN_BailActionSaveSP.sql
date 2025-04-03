SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
if exists (select * FROM dbo.sysobjects WHERE id = object_id('[dbo].[DN_BailActionSaveSP]') AND OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BailActionSaveSP]
GO
-- PC 11/01/2007 Livewire	68694 chanaged this to look for exact date when picking up fee from fintrans as commission could be put thought twice if two transactions within 2 minutes
-- AA 07/02/2006 Livewire  - bailiff commission not saving when Fee taken.
SET QUOTED_IDENTIFIER ON
GO

-- ============================================================================================
-- Version		: 002
-- Project      : CoSACS .NET
-- File Name    : DN_BailActionSaveSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save details to Bailaction
-- Author       : ??
-- Date         : ??
--
-- This procedure will save the details to the bailaction table.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/01/2009 Jec CR976 Remove write to SPA. Now writtten in CM_SPAWriteArrangementScheduleSP
-- 02/09/09   IP  5.2 UAT(823) - Blacklist Action (BLC)
-- 21/10/09   IP  5.2 UAT(914) - No Further Action (NFA)
-- 08/08/19   Zensar(SH) Strategy Job Optimization : Optimised the stored procedure for performance by putting Nolock and Replacing * with 1 in all exist
-- 09/08/19  Zensar(Dipti) Strategy Job Optimization : SPA functionality is no longer used by cosacs.
-- ================================================================================================

CREATE PROCEDURE 	[dbo].[DN_BailActionSaveSP]
	-- Add the parameters for the stored procedure here
			@acctno varchar(12),
			@employeeno int,
			@dateadded datetime,
			@code varchar(4),
			@notes varchar(700),
			@datedue datetime,
			@actionvalue float,
			@amtcommpaidon float,
			@addedby int,
			@spadateexpiry datetime,
			@spareasoncode varchar(4),
			@spainstal float,
			@remDateTime datetime,  -- NM & IP To save call reminder information
			@deleteAllReminders bit,
			@callingSource varchar(10), -- NM & IP To specify which screen is calling this SP; Only required for TelephoneActions
			@allocno INT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
    SET NOCOUNT ON -- 
	DECLARE @actionno smallint
	DECLARE @BALANCE money
	SET	@actionno = 1

	IF @allocno = 0 
		SELECT	@allocno = allocno
		FROM	follupalloc 
		WHERE	datedealloc is null
		AND	acctno = @acctno

	SELECT	@actionno = isnull(MAX(actionno) + 1,1)     -- DSR 29/5/03 added MAX()     -- PN 30/5/2003 - added isnull()
	FROM	bailaction
	WHERE	acctno = @acctno
	AND	allocno = isnull(@allocno, 0)
	AND	empeeno = @employeeno


	-- 67910 RD 2003/2006 Added empee name in notes
	DECLARE @NameandNotes varchar(720)

	INSERT INTO bailaction
		(origbr, acctno, allocno, actionno, empeeno, 
		dateadded, code, notes, actionvalue, datedue, amtcommpaidon, addedby)
	SELECT 0, @acctno, isnull(@allocno, 0), @actionno, @employeeno, 
		@dateadded, @code,fullname + ' - '+  @notes  , @actionvalue, @datedue, @amtcommpaidon, @addedby
	FROM Admin.[User] WITH(NOLOCK)--Zensar(SH)
	WHERE id = @addedby

   -- AA heat call   
   declare  @bcommissionequalsfee smallint,@debitaccount smallint,@currstatus char(1), 
            @branchno smallint,@ChequeColln char(1),@feevalue money, @transrefno integer,@paymethod int
   
   IF @code ='PAY'
   begin
  	  IF not exists (select 1 FROM BailiffCommn --Zensar(SH)
  		               WHERE   empeeno = @employeeno AND acctno =@acctno AND 
  		               datetrans =@dateadded )
     begin -- check whether should have been fee posted.
		  declare @datestatchge datetime
	      SELECT @currstatus =currstatus FROM acct WITH(NOLOCK) WHERE acctno = @acctno
	      if @currstatus in ('S','1') -- check previous status for this account as status has been moved in the payment screen
	      begin
				select top 1 datestatchge=  datestatchge from [status] WITH(NOLOCK) where acctno = @acctno and statuscode not in ('1','S') Order by datestatchge desc--Zensar(SH)
				select @currstatus = statuscode from status WITH(NOLOCK) where datestatchge =@datestatchge and acctno = @acctno--Zensar(SH)
	      end
	      SELECT @transrefno = isnull (transrefno,0),
	      @paymethod = isnull (paymethod,0) FROM fintrans WITH(NOLOCK) WHERE acctno =@acctno AND datetrans =@dateadded--Zensar(SH)
	      
	      SELECT @feevalue = isnull (transvalue,0)
	      FROM fintrans WITH(NOLOCK) WHERE acctno =@acctno AND 
	      datetrans = @dateadded -- 68694 chanage this to look for exact date as commission could be put thought twice if two transactions within 2 minutes
			--datetrans BETWEEN dateadd (minute, -2,@dateadded ) AND  dateadd (minute, 2,@dateadded ) --fee will have been posted around this time
	      AND transtypecode ='FEE'
	      IF @paymethod = 2 -- payment method -2 = cheque payment
	          set @ChequeColln='Y'
	      else
	          set @ChequeColln='N'
         SELECT @debitaccount =debitaccount FROM bailcommnbas WITH(NOLOCK) WHERE empeeno =@employeeno AND statuscode =@currstatus
			AND collecttype='P'
		   SELECT @bcommissionequalsfee = bcommissionequalsfee FROM country 
	      IF @debitaccount = 1 AND @bcommissionequalsfee= 1  AND @feevalue != 0
			begin
          INSERT INTO BailiffCommn
              (EmpeeNo,
               TransRefNo,
               AcctNo,
               DateTrans,
               TransValue,
               ChequeColln,
               Status)
           VALUES
              (@employeeno,
               @TransRefNo,
               @AcctNo,
               @dateadded,
               @feevalue,
               @ChequeColln,
               'H')
         end;
     end
  end

     -- Commented By Zensar on Date:08/08/2019 for Strategy Optimization Job -- Start

--	IF (@code = 'SPA')
--	BEGIN
----  CR976 jec 16/01/09
----	    INSERT
----	      INTO spa
----	      (origbr, acctno, allocno, actionno, empeeno,
----	      dateadded, code, spainstal, dateexpiry, empeenospa)
----	    VALUES
----	      (0, @acctno, isnull(@allocno, 0), @actionno, @employeeno,
----	      @dateadded, @spareasoncode, @spainstal, @spadateexpiry, @addedby)

--	   IF EXISTS (SELECT 1 FROM bdwpending WITH(NOLOCK) WHERE acctno =@acctno)--Zensar(SH)
--   	BEGIN
--  		SELECT @balance = outstbal FROM acct WITH(NOLOCK) WHERE acctno =@acctno
  
--      INSERT INTO bdwrejection
--      (acctno, empeeno, code, rejectcode, balance, rejectdate)
--      SELECT 
--      P.acctno, 0,P.CODE,'SPA',@balance, getdate()
--      FROM bdwpending P WITH(NOLOCK),SPA S WITH(NOLOCK) WHERE S.acctno =@acctno
--      AND S.ACCTNO = P.ACCTNO
--      AND S.dateexpiry >dateadd(month, -1,getdate())
   
--	   IF (@@error != 0)
--    	BEGIN
--		  SET @return = @@error
--    	END

--   END

--	END

	  -- Commented By Zensar on Date:08/08/2019 for Strategy Optimization Job -- End




	-- NM & IP
	-- This is to update the status of the CMReminder. if @deleteAllReminders = 1 it will delete all entries for that account
	-- @deleteAllReminders = 0 it will then delete only the past Reminders set by that employee for that account
	if(@callingSource = 'TELACTION') -- If this is called from telephone action screen
	BEGIN
		UPDATE CMReminder
		SET status = 'A'
		where acctno = @acctno and 
		(
		(@deleteAllReminders = 0 and reminderDateTime < getdate() and empeeno = @employeeno) 
			or
		@deleteAllReminders = 1 
		)
	END
	
	
	IF (@code = 'REM' or @code = 'PREM')
	BEGIN
	    INSERT
	      INTO CMReminder
		(acctno,allocno, actionno, empeeno,
		code, reminderDateTime, comment, dateadded, status) 
	    VALUES
	      (@acctno, isnull(@allocno, 0), @actionno, @employeeno,
	      @code, @remDateTime,@notes, @dateadded, 'N')
	END
	
	declare @custid varchar(20)
	
	--Select the main holder customer of the account.
	select @custid = ca.custid from custacct ca WITH(NOLOCK)--Zensar(SH)
						 where ca.acctno = @acctno
						 and ca.hldorjnt ='H'

	--NM & IP - 07/01/09 - CR976 - Extra Telephone Actions - (RFC - Blacklist Customer)
	--If 'RFC' has been selected then add the code 'R' (Refuse further credit) into the 'CustCatCode' table.
	--IP - 28/08/09 - UAT(823) - Code for blacklist is 'BLC'
	--IP - 02/09/09 - Executing proc to add the customer code as this will ensure that either the existing one is updated if there is one
	--or a new code inserted.
	--IF (@code = 'RFC')
	IF (@code = 'BLC')
	BEGIN
		Declare @custCodeDate datetime
		set @custCodeDate = getdate()
		
		--INSERT INTO custcatcode (origbr, custid, datecoded, code, datedeleted, empeenocode, reference)
		--VALUES(null, @custid, getdate(), 'R', null, @employeeno, '')
		EXEC DN_CustomerCodeAddSP  @custid = @custid, @code = 'R', @date = @custCodeDate, @codedby = @employeeno, @reference = '',@return = @return out
	END
	
	--NM & IP - 07/01/09 - CR976 - Extra Telephone Actions - (TRC - Trace Details)
	--If 'TRC' has been selected then add the code 'T' (Trace) into the 'CustCatCode' table.
	IF(@code = 'TRC')
	BEGIN	
		
		INSERT INTO custcatcode (origbr, custid, datecoded, code, datedeleted, empeenocode, reference)
		VALUES(null, @custid, getdate(), 'T', null, @employeeno, '')
	END
	-- jec 02/06/09 - Trace resolved - update TRC datedeleted
	IF(@code = 'TRR')
	BEGIN	
		
		Update custcatcode
			set datedeleted = getdate()
		Where custid=@custid and code='T'
	END
	
	--IP - 21/10/09 - UAT(914) - If the No Further Action processed on the account
	--and the account is in the PWO strategy, then update to exit the account from the strategy and worklist.
	IF(@code = 'NFA')
	BEGIN
	
		IF EXISTS(SELECT 1 FROM cmstrategyacct  WITH(NOLOCK) --Zensar(SH)
					WHERE acctno = @acctno
					AND strategy = 'PWO'
					AND dateto IS NULL)
		BEGIN
				UPDATE cmstrategyacct
				SET dateto = GETDATE()
				WHERE acctno = @acctno
				AND strategy = 'PWO'
				AND dateto IS NULL
				
				UPDATE cmworklistsacct
				SET dateto = GETDATE()
				WHERE acctno = @acctno
				AND strategy = 'PWO'
				AND dateto IS NULL

				
		END
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
