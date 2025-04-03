SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go


if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dbgenerateaccts]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dbgenerateaccts]
GO

CREATE PROCEDURE dbgenerateaccts
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : dbgenerateaccts.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Generate Bad Debt Write-off accounts
-- Author       : ?
-- Date         : ?
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------------------------------------------
--				68961 removing commit to prevent end of day error
-- 29/06/07 jec 69034 reversing 68532 change and reverse 68961
-- 15/09/09 ip  UAT5.2 (858) - Insert accounts that qualify for WriteOff into the 'WOF' strategy.
-- 08/09/10 IP  CR1107 - Write Off Review screen Enhancements (Manual Rules) for Malaysia
-- 14/12/11 jec #8871 Strategies EOD failed - accounts in multiple strategies.
-- 12/04/12 IP  #9905 Insert Store Card accounts into the 'SCWOF' strategy.
-- ==============================================================================================================
    -- Parameters
        @return int OUTPUT

AS
    set nocount on
    SET 	@return = 0			--initialise return code
	DECLARE	@status integer, 
			@query_text varchar (1000),
			@interface varchar(10),
			@months smallint,
			@repmonths smallint,
			@scmths smallint,
			@runno int,
			@dorun varchar(1),
			@dodefault varchar(1),
			@numprocessed int,
			@countrycode char(1),
			@mthsSinceDelRepos smallint,			--IP - 07/09/10 - CR1107 - months since delivery
			@mthsSinceLastPayRepo smallint,			--IP - 07/09/10 - CR1107 - months since last pay for repossession
			@mthsSinceLastRepo smallint,			--IP - 07/09/10 - CR1107 - months since last repossession
			@repoPcentProv smallint,				--IP - 07/09/10 - CR1107 - provision percent for repossessed account
			@mthsSinceLastPayNonRepo smallint,		--IP - 07/09/10 - CR1107 - months since last pay for non-repossession
			@nonRepoArrLevel smallint,				--IP - 07/09/10 - CR1107 - arrears level (months in arrears) for non-repossession
			@nonRepoPcentProv smallint,				--IP - 07/09/10 - CR1107 - provision percent for non-repossessed account
			@AutoRulesEnabled bit,					--IP - 08/09/10 - CR1107 - Are Auto Rules enabled
			@ManualRulesEnabled bit					--IP - 08/09/10 - CR1107 - Are Manual Rules enabled
						

	--SET @interface = 'BDWINITIAL'
	SET @interface = 'GENBDW'
	SET @status = 0
	SET @runno=0

/* CR781 - code not required
	BEGIN TRAN

	SELECT	@dorun = donextrun,
			@dodefault = dodefault
	FROM		eodcontrol
	WHERE	interface = @interface

     	IF @dorun = 'Y'

     	BEGIN

		SET @query_text =N'create interface control record'
		EXEC DN_InterfaceAddSP	@interface = @interface,
						@runno = @runno OUTPUT,
						@return = @status OUTPUT
--	END

	COMMIT TRAN
*/
    -- Get run number
    set @runno=(Select max(runno) from interfacecontrol where interface=@interface)
    
    If @runno = 0
        Begin
            SET @query_text = convert(varchar,@status) + ' ' + 'RunNo Error'
            raiserror (@query_text,1,1)

	    END
            
	BEGIN TRAN
       -- removing automatically created rejection reasons as inserting later
	   delete from bdwrejection where REJECTcode in ('SPA','S','F','LINK')
		-- removing pending records which have not already been rejected.
	   delete from bdwpending where not exists (select * from bdwrejection b 
						    where b.acctno = bdwpending.acctno)

	SELECT	@countrycode = countrycode,
		@months = mthsarrears,
		@repmonths = mthsreposs,
		@scmths = mthsstatus
	FROM 	country
	
	--IP - 08/09/10 - CR1107 
	select @AutoRulesEnabled = Value from CountryMaintenance where CodeName = 'enableAutoRules'
	select @ManualRulesEnabled = Value from CountryMaintenance where CodeName = 'enableManualRules'
	
	--IP - 08/09/10 - CR1107 - Manual Rules that apply to Repossessed accounts
	select @mthsSinceDelRepos = value from CountryMaintenance where CodeName = 'mthsSinceDelRepos'
	select @mthsSinceLastPayRepo = value from CountryMaintenance where CodeName = 'mthsSinceLastPayRepo'
	select @mthsSinceLastRepo = value from CountryMaintenance where CodeName = 'mthsSinceLastRepo'
	select @repoPcentProv = value from CountryMaintenance where CodeName = 'repoPcentProv'
	
	--IP - 08/09/10 - CR1107 - Manual Rules that apply to Non-Repossessed accounts
	select @mthsSinceLastPayNonRepo = value from CountryMaintenance where CodeName = 'mthsSinceLastPayNonRepo'
	select @nonRepoArrLevel = value from CountryMaintenance where CodeName = 'nonRepoArrLevel'
	select @nonRepoPcentProv = value from CountryMaintenance where CodeName = 'nonRepoPcentProv'

   	declare @Tallymanlink  varchar (6), @tallymanserverdb varchar(128),@statement sqltext
	
	select @Tallymanlink= value from countrymaintenance where [name] ='Link to Tallyman'


	CREATE TABLE #tmpaccts(	acctno varchar(12) not null,
				code varchar(5) not null,
				reject int,
				rejectcode varchar(5),
				custid varchar(20) not null,
				accttype char(1) not null											--IP - 12/04/12 - 	#9905
				primary key (acctno,code))

   	SET @query_text =N'Stage 1 - Process non-moving accounts'

	-- 68184 RD 11/05/06 Modified datediff-month query to DATEDIFF(day, datetrans, getdate())/30.4375
   	INSERT INTO 	#tmpaccts
	SELECT 	DISTINCT 
		acctno,
		'NMV',
		0,
		'',
		'',
		accttype																	--IP - 12/04/12 - 	#9905
   	FROM	acct
   	WHERE  	arrears > 0
   	AND	outstbal > 0
   	AND	currstatus NOT IN ('S', '7', '8')
   	AND   	accttype NOT IN ('C', 'S', 'L')
   	AND	NOT EXISTS( SELECT 	acctno
			    FROM 	fintrans
			    WHERE 	fintrans.acctno = acct.acctno
			    AND 	DATEDIFF(day, datetrans, getdate())/30.4375 <= @months
			    AND 	transtypecode NOT IN ('INT', 'ADM'))
	AND @AutoRulesEnabled = 1 --IP - 08/09/10 - CR1107
	
   	SET @status = @@error

      create index ix_tem_tmpaccts_custid on #tmpaccts(custid)	

   	IF @status = 0  --         AND @dorun = 'Y' 
   	BEGIN
        	SET 	@query_text =N'Stage 2 - Process reposessed accounts'

		INSERT INTO 	#tmpaccts
		SELECT	DISTINCT 
			acct.acctno,
			'REP',
			0,
			'',
			'',
			accttype									--IP - 12/04/12 - 	#9905
		FROM	acct, acctcode
		WHERE	acct.acctno = acctcode.acctno
		-- AND 	acct.outstbal > 0 //NM & AA commented CR976 - Repossession
		AND	acct.currstatus NOT IN ('S', '7', '8')
		AND   	acct.accttype NOT IN ('C', 'S', 'L')
		AND	acctcode.code = 'FREP'
		AND	NOT EXISTS (SELECT acctno
			    	    FROM   #tmpaccts
			    	    WHERE  acct.acctno = #tmpaccts.acctno)
		AND		NOT EXISTS(SELECT 	acctno
					   FROM 	fintrans
					   WHERE	fintrans.acctno = acct.acctno
					   AND 		DATEDIFF(day, datetrans, getdate())/30.4375 <= @repmonths
					   AND 		transtypecode NOT IN ('INT', 'ADM'))
		AND @AutoRulesEnabled = 1 --IP - 08/09/10 - CR1107
		
     		SET @status =@@error
	END
	
     	IF @status = 0 -- AND @dorun = 'Y' 
     	BEGIN
        	SET 	@query_text =N'Stage 3 - Process accounts flagged as bankrupt'

		INSERT INTO 	#tmpaccts
		SELECT	acct.acctno,
			'BPT',
			0,
			'',
			'',
			accttype									--IP - 12/04/12 - 	#9905
		FROM	acct, custacct, custcatcode
		WHERE	acct.acctno = custacct.acctno
		AND	custacct.custid = custcatcode.custid
		AND	custacct.hldorjnt = 'H'
		AND	custcatcode.code = 'BPT'
		AND	acct.currstatus NOT IN ('S', '7', '8')
		AND   	acct.accttype NOT IN ('C', 'S', 'L')
		AND	 NOT EXISTS (SELECT	acctno
				     FROM 	#tmpaccts
				     WHERE 	acct.acctno = #tmpaccts.acctno)
		AND @AutoRulesEnabled = 1	--IP - 08/09/10 - CR1107
		
     		SET @status =@@error
	END

  	IF @status = 0     --  AND @dorun = 'Y' 
        and (@Tallymanlink ='True' or @Tallymanlink ='true')
     	BEGIN
		select @tallymanserverdb=value  from countrymaintenance where [name] = 'Tallyman Server Database'
		
		truncate table Tallymanbdw
        	
		SET 	@query_text =N'Stage 3a - Process accounts flagged in Tallyman'
		
		SET @statement =' INSERT INTO 	Tallymanbdw 	(acctno ,   	code ,		reject , rejectcode ,custid) ' +
				' SELECT a.acctno,convert(varchar,s.Segment_ID), 0,'''',s.CustomerIC_Number ' +
			        'FROM acct a, ' + @tallymanserverdb +  '.dbo.TM_Segments s,  code c ' +
               			' where c.category = ''TBD'' AND convert(integer,c.code) = s.segment_Id and s.Account_Number = a.acctno ' +
			        ' and not exists  (select * from BDWPending where a.acctno = BDWPending.acctno and BDWPending.runno =0 ' +
				' and a.currstatus !=''S'' and a.outstbal >0) '
         print @statement

         execute sp_executesql @statement
			set @status = @@error
			if @status = 0
			begin
				INSERT INTO #tmpaccts(acctno ,   code ,	reject , rejectcode ,custid, accttype)			--IP - 12/04/12 - 	#9905
				select acctno ,   	code ,	reject , rejectcode ,custid, '' from Tallymanbdw			--IP - 12/04/12 - 	#9905
				set @status = @@error
			end
        END		
	
	
     	IF @status = 0  -- AND @dorun = 'Y' 
     	BEGIN
        	SET 	@query_text =N'Stage 4 - Process accounts in status SC 6-8 for x months'
		
		SELECT	s.acctno, max(s.datestatchge) as 'datestatchge'
		INTO	#status
		FROM 	status s, acct a
		WHERE	a.acctno = s.acctno
		AND 	a.outstbal > 0 
		AND	a.currstatus = '6'
		AND 	s.statuscode = '6'
		AND   	a.accttype NOT IN ('C', 'S', 'L')
		AND		@AutoRulesEnabled = 1 --IP - 08/09/10 - CR1107
		GROUP BY s.acctno	
		HAVING 	DATEDIFF(day, max(s.datestatchge), getdate())/30.4375 >= @scmths

		INSERT INTO 	#tmpaccts
		SELECT	acctno,
			'SC',
			0,
			'',
			'',
			''						--IP - 12/04/12 - 	#9905
		FROM	#status
		WHERE	 NOT EXISTS (	SELECT acctno
				  	FROM #tmpaccts
				  	WHERE #status.acctno = #tmpaccts.acctno)
	     	SET @status =@@error
	END
	
	--IP - 07/09/10 - CR1107 - Write Off Review screen Enhancements
	IF @status = 0
	BEGIN
			SET 	@query_text =N'Stage 5a - Process MRA SC6 Manual rules applied - Repossessed accounts'
		
		INSERT INTO #tmpaccts	
		SELECT a.acctno,
		   'MRA',
		   0,
		   '',
		   '',
		   accttype									--IP - 12/04/12 - 	#9905
		FROM acct a
		inner join View_provisionpercent p on a.acctno = p.acctno
		left join TM_Segments ts on a.acctno = ts.Account_Number
		WHERE a.currstatus = '6'
		AND a.accttype not in ('C', 'S', 'L')
		AND a.arrears > 0
		and a.outstbal > 0
		and p.provision >= @repoPcentProv
		AND EXISTS(select * from fintrans f
					where f.acctno = a.acctno
					and f.transtypecode = 'REP')
		AND EXISTS (select max(f.datetrans) from fintrans f
						where f.acctno = a.acctno
						and f.transtypecode = 'DEL'
						having DATEDIFF (day,max(f.datetrans), GETDATE())/30 >=@mthsSinceDelRepos)
		AND EXISTS	(select MAX(f.datetrans) from fintrans f
						where f.acctno = a.acctno
						and f.transtypecode = 'REP'
						having datediff(day, MAX(f.datetrans), GETDATE())/30 >= @mthsSinceLastRepo)
		AND EXISTS (select MAX(f.datetrans) from fintrans f
						where f.acctno = a.acctno
						and f.transtypecode = 'PAY'
						having datediff(day,MAX(f.datetrans), getdate())/30 >= @mthsSinceLastPayRepo)
		AND NOT EXISTS(select * from #tmpaccts
						where #tmpaccts.acctno = a.acctno)
		and ts.Segment_Name not in ('Arrangement Awaiting Payment', 'Insurance New', 'Insurance Update', 'New Service Due', 'Pending BRSS Actions',
									 'Second Legal Review', 'Service Actions Day 28 >', 'Service Not Actioned', 'Goods Returned', 'Part Repo Decision (DCA)')
		and ts.Segment_Name in ('Full Repo Done', 'Full Repo Done (DCA)')
		and @ManualRulesEnabled = 1
		
		SET @status =@@error
		
	END
	
	IF @status = 0
	BEGIN
			SET 	@query_text =N'Stage 5b - Process MRA SC6 Manual rules applied - Non-Repossessed accounts'
	
		INSERT INTO #tmpaccts	
		SELECT a.acctno,
		   'MRA',
		   0,
		   '',
		   '',
		   accttype								--IP - 12/04/12 - 	#9905
		FROM acct a
		inner join View_provisionpercent p on a.acctno = p.acctno
		left join TM_Segments ts on a.acctno = ts.Account_Number
		WHERE a.currstatus = '6'
		AND a.accttype not in ('C', 'S', 'L')
		AND a.arrears > 0
		and a.outstbal > 0
		and p.provision >= @nonRepoPcentProv
		and a.MonthsInArrears >= @nonRepoArrLevel
		AND NOT EXISTS(select * from fintrans f
					where f.acctno = a.acctno
					and f.transtypecode = 'REP')
		AND EXISTS (select MAX(f.datetrans) from fintrans f
						where f.acctno = a.acctno
						and f.transtypecode = 'PAY'
						having datediff(day,MAX(f.datetrans), getdate())/30 >= @mthsSinceLastPayNonRepo)
		AND NOT EXISTS(select * from #tmpaccts
						where #tmpaccts.acctno = a.acctno)
		and ts.Segment_Name not in ('Arrangement Awaiting Payment', 'Insurance New', 'Insurance Update', 'New Service Due', 'Pending BRSS Actions',
								 'Second Legal Review', 'Service Actions Day 28 >', 'Service Not Actioned', 'Goods Returned')
		and @ManualRulesEnabled = 1
		
		SET @status =@@error
		
	END

	IF @status = 0 --  AND @dorun = 'Y' 
     	BEGIN
        	SET 	@query_text =N'Stage 6 - Process accounts linked to accounts not in SC 6'
		
		UPDATE 	#tmpaccts
		SET	custid = custacct.custid
		FROM	custacct
		WHERE	custacct.acctno = #tmpaccts.acctno
		AND	custacct.HldOrJnt = 'H'

		UPDATE	#tmpaccts
		SET	reject = 1,
			rejectcode = 'LINK'
		WHERE	EXISTS(SELECT a.acctno
			       FROM  CustAcct ca, Acct a
			       WHERE ca.acctno = a.acctno		
			       AND   ca.HldOrJnt = 'H'
			       AND   ca.CustId	= #tmpaccts.CustId
			       AND   a.currstatus NOT IN( '6', 'S')
			       AND NOT EXISTS(SELECT *
					      FROM #tmpaccts
					      WHERE #tmpaccts.acctno = a.acctno))
	     	SET @status =@@error
	END
	
	IF @status = 0  -- AND @dorun = 'Y' 
     	BEGIN
        	SET 	@query_text =N'Remove accounts that have been manually declined'
		DELETE
		FROM #tmpaccts
		WHERE EXISTS(SELECT b.acctno
			     FROM	code c, bdwrejection b
			     WHERE #tmpaccts.acctno = b.acctno
			     AND b.rejectcode = c.code
			     AND c.category = 'BDD')
	END

     	IF @status = 0  -- AND @dorun = 'Y' 
     	BEGIN
        	SET 	@query_text =N'Stage 7 - Process accounts with SPA flag'
		UPDATE 	#tmpaccts
		SET	reject = 1,
			rejectcode = 'SPA'
		WHERE 	EXISTS(SELECT a.acctno
			       FROM   acct a, spa s
			       WHERE  #tmpaccts.acctno = a.acctno
			       AND    a.acctno = s.acctno
			       AND dateadd(month,1,isnull(s.dateexpiry, '1/1/1900')) > getdate())
	     	SET @status =@@error
	END
	
     	IF @status = 0  -- AND @dorun = 'Y' 
     	BEGIN
        	SET 	@query_text =N'Stage 8 - Calculate interest/admin charges'
		SELECT	f.acctno,
			SUM(f.transvalue) as value
		INTO	#charges
		FROM 	#tmpaccts t, fintrans f
		WHERE	t.acctno = f.acctno
		AND	transtypecode IN ('INT', 'ADM')
		GROUP BY f.acctno
	     	SET @status =@@error
	END

	create clustered index ix_temp_charges_acctno on #charges(acctno)
	
     	IF @status = 0  -- AND @dorun = 'Y' 
     	BEGIN
        	SET 	@query_text =N'Stage 9 - process accounts where corrected balance <= instalment'
	
		UPDATE 	#tmpaccts
		SET	reject = 1,
			rejectcode = 'BALC'
		FROM	#charges c, acct a
		WHERE	#tmpaccts.acctno = a.acctno
		AND	#tmpaccts.acctno = c.acctno
		AND	a.outstbal - c.value <= (	SELECT TOP 1 instalamount
							FROM instalplan
							WHERE acctno = #tmpaccts.acctno
							ORDER BY datefirst DESC)
	     	SET @status =@@error
	END
	
     	IF @status = 0  -- AND @dorun = 'Y' 
     	BEGIN
        	SET 	@query_text =N'Stage 10 - Process accounts in SC 9'

		UPDATE 	#tmpaccts
		SET	reject = 1,
			rejectcode = 'STAFF'
		WHERE	EXISTS(SELECT acctno
			       FROM   acct a
			       WHERE  #tmpaccts.acctno = a.acctno	 
			       AND  currstatus = '9')
	     	SET @status =@@error
	END
	
     	IF @status = 0  -- AND @dorun = 'Y' 
     	BEGIN
        	SET 	@query_text =N'Stage 11 - Process frozen accounts'
		UPDATE 	#tmpaccts
		SET	reject = 1,
			rejectcode = 'F'
		WHERE 	EXISTS(SELECT a.acctno
			       FROM   acct a, acctcode c
			       WHERE  a.acctno = #tmpaccts.acctno
			       AND    a.acctno = c.acctno
			       AND    c.code = 'F' and (datedeleted is null or dateadd(month,1,datedeleted) >getdate()) )
	     	SET @status =@@error
	END

-- select top 10 * from acctcode

     	IF @status = 0  -- AND @dorun = 'Y' 
     	BEGIN
        	SET 	@query_text =N'Stage 12 - Process suspended accounts'
		UPDATE 	#tmpaccts
		SET	reject = 1,
			rejectcode = 'S'
		WHERE	EXISTS(SELECT a.acctno
			       FROM   acct a, acctcode c
			       WHERE  a.acctno = #tmpaccts.acctno
			       AND    a.acctno = c.acctno
			       AND    c.code = 'S' and (datedeleted is null or dateadd(month,1,datedeleted) <getdate()))
	     	SET @status =@@error
	END

     	IF @status = 0 -- AND @dorun = 'Y' 
            AND @countrycode = 'S' 
     	BEGIN
        	SET 	@query_text =N'Stage 13 - Process accounts with no claim number'

		UPDATE 	#tmpaccts
		SET	reject = 1,
			rejectcode = 'BANK'
		FROM	acct, custacct, custcatcode
		WHERE	#tmpaccts.acctno = acct.acctno
		AND	acct.acctno = custacct.acctno
		AND	custacct.custid = custcatcode.custid
		AND	custacct.hldorjnt = 'H'
		AND	#tmpaccts.code = 'BPT'
		AND	custcatcode.reference = ''

     		SET @status =@@error
	END	

     	IF @status = 0  -- AND @dorun = 'Y' 
     	BEGIN

        	SET 	@query_text =N'Process accounts with low status code'

		UPDATE 	#tmpaccts
		SET	reject = 1,
			rejectcode = 'LSC'
		FROM	acct
		WHERE	#tmpaccts.acctno = acct.acctno
		AND	acct.currstatus IN ('1', '2', '3', '4')

	     	SET @status =@@error
	END
   -- removing from bdwpending and bdwrejection
   	IF @status = 0  -- AND @dorun = 'Y' 
   	BEGIN
        	SET 	@query_text =N'Stage 14 - Insert processed accounts into BDWPending, BDWReject'
        	
        if(@Tallymanlink !='True' or @Tallymanlink !='true') --IP - 09/09/10
        begin
		 --IP - 15/09/09 - UAT5.2 (858)
		 --Insert accounts into the 'WOF' strategy for those that are inserted into the BDWPending table (qualify for WriteOff).
		   DECLARE @dateNow DATETIME
		   SET @dateNow = GETDATE()
			
		   INSERT INTO CMStrategyAcct 
           (acctno,strategy,datefrom,dateto,currentstep,dateincurrentstep)
           SELECT 
           acctno,
           --'WOF',
           case when accttype !='T' then 'WOF' else 'SCWOF' end,							--IP - 12/04/12 - #9905
           @datenow,null,0,@datenow
		   FROM	#tmpaccts
		   WHERE reject = 0
		   AND NOT EXISTS (select * from bdwpending b where b.acctno = #tmpaccts.acctno)
		   --AND EXISTS (SELECT * FROM cmstrategy c WHERE c.strategy = 'WOF' AND c.isactive = 1)
		   AND EXISTS (SELECT * FROM cmstrategy c WHERE ((c.strategy = 'WOF' and #tmpaccts.accttype !='T')				--IP - 12/04/12 - #9905
							or (c.strategy = 'SCWOF' and #tmpaccts.accttype = 'T')) AND c.isactive = 1)
		   --and not exists(select * from CMStrategyAcct ca where ca.acctno = #tmpaccts.acctno and dateto is null)		-- #8871
		   and not exists(select * from CMStrategyAcct ca where ca.acctno = #tmpaccts.acctno and ca.strategy in ('WOF', 'SCWOF') and dateto is null)		--IP - 12/04/12 - #9905
		   --GROUP BY acctno
		   GROUP BY acctno, accttype																					--IP - 12/04/12 - #9905
		   
		   --Update the dateto if the account was in a previous strategy.
		   UPDATE CMStrategyAcct
		   SET dateto = @datenow
		   FROM CMStrategyAcct a
		   WHERE EXISTS (SELECT * FROM CMStrategyAcct a1
							WHERE a1.acctno = a.acctno 
							AND a1.datefrom = @datenow
							--AND a1.strategy = 'WOF'
							AND a1.strategy in ('WOF', 'SCWOF')								--IP - 12/04/12 - #9905						
						)
		   AND a.dateto IS null
		   --AND a.strategy != 'WOF'
		   AND a.strategy not in ('WOF', 'SCWOF')											--IP - 12/04/12 - #9905	
		 
		   --Update the cmworklistsacct table dateto so account exits current worklist.
		   UPDATE CMWorklistsAcct
		   SET Dateto = @datenow
		   FROM CMWorklistsAcct cmw
		   WHERE EXISTS (SELECT * FROM CMStrategyAcct a1
							WHERE a1.acctno = cmw.acctno
							--AND a1.strategy = 'WOF'
							AND a1.strategy in ('WOF', 'SCWOF')								--IP - 12/04/12 - #9905	
							AND a1.datefrom = @datenow)
		   AND cmw.dateto IS null
		 end

		INSERT INTO 	BDWPending (acctno,empeeno,code,runno,empeenomanual)
		SELECT	acctno,
			0,
			Max (code),
			0,
			0
		FROM	#tmpaccts
		WHERE	reject = 0
		AND 	NOT EXISTS (select * from bdwpending b where b.acctno = #tmpaccts.acctno)
		GROUP BY acctno
		

		
		INSERT INTO 	BDWRejection (acctno,empeeno,code,rejectcode,balance,rejectdate)
		SELECT	t.acctno,
			0,
			max (t.code),
			Max (t.rejectcode),
			Max (a.outstbal),
			getdate()
		FROM	#tmpaccts t, acct a
		WHERE	t.acctno = a.acctno
		AND	reject = 1
		AND 	NOT EXISTS (select * from bdwrejection b where b.acctno = t.acctno)
		GROUP BY t.acctno

	       SET @status =@@error

	END
	
   	IF @status = 0  -- AND @dorun = 'Y'
   	BEGIN
        	SET 	@query_text =N'Stage 15a - Prevent previously accepted accounts from be loaded again'

		DELETE FROM	BDWRejection
		WHERE EXISTS(SELECT	acctno
			     FROM	BDWPending
			     WHERE	BDWRejection.acctno = BDWPending.acctno
			     AND	BDWPending.empeeno != 0)
	END

   	IF @status = 0  -- AND @dorun = 'Y'
     	BEGIN
        	SET 	@query_text =N'Stage 16 - Record values for processed accounts'
		SELECT	@numprocessed = COUNT(*)
		FROM	#tmpaccts

		EXEC DN_InterfaceValueAddSP	@interface = @interface,
						@runno = @runno,
						@counttype1 = 'PROCESSED',
						@counttype2 = '',
						@branchno = 0,

						@accttype = '',
						@countvalue = @numprocessed,
						@value = 0,
						@return = @status output
     		SET @status =@@error
	END
-- 69034 jec 29/06/07 reversing 68532 change
    -- removing historic rejections which are no longer valid
--   	IF @status = 0  
--    BEGIN
--		delete from BDWRejection where not exists (select * from BDWPending WHERE BDWRejection.acctno = BDWPending.acctno)
--    		SET @status =@@error
--	END

   	IF @status = 0  --  AND @dorun = 'Y'
     	BEGIN
        	SET 	@query_text =N'Stage 17 - Set date finish for interface run'

		EXEC DN_InterfaceSetDateFinishSP	@interface = @interface,
							@runno = @runno,
							@return = @status output
      		SET @status =@@error
	END

	

/* CR781 - code not required
  	IF @status = 0  -- AND @dorun = 'Y'
  	BEGIN
        	SET 	@query_text =N'Stage 16 - Set result for interface run'
		EXEC DN_InterfaceSetResultSP	@result = 'P',
						@interface = @interface,
						@runno = @runno,
						@return = @status output

	END
*/
/* Jec CR781
	UPDATE	eodcontrol 
	SET		donextrun = dodefault
	WHERE	interface = @interface
*/
	IF @status != 0
	BEGIN
	   	ROLLBACK TRAN
		SET @query_text = convert(varchar,@status) + ' ' + @query_text
/*  JEC CR781 
   		INSERT INTO Interfaceerror(	interface,
						runno,
						errordate,
						severity,
						errortext)   
		   VALUES(	@interface,
				0,
				getdate(),
				'E',
				@query_text)   

		EXEC DN_InterfaceSetResultSP	@result = 'F',
						@interface = @interface,
						@runno = @runno,
						@return = @status output
*/
-- pass error back. .net will handle pass/fail etc
	     raiserror (@query_text,1,1)

	END

	else
-- re-instated (reverse 68961) jec 02/07/07
		COMMIT TRAN

--	RETURN @status

	IF @@error != 0
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS Off

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End