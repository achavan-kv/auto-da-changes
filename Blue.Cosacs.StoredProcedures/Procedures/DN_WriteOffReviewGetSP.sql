SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_WriteOffReviewGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_WriteOffReviewGetSP]
GO



CREATE PROCEDURE 	dbo.DN_WriteOffReviewGetSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_WriteOffReviewGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : DN_WriteOffReviewGetSP
-- Author       : ?
-- Date         : ?
--
-- This procedure returns accounts to the Write Off Review screen
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 09/09/10  IP  CR1107 - Write Off Review screen Enhancements
-- 01/10/11  IP  #11360 - LW75288 - Accounts with a rejection code were incorrectly being deleted, and therefore
--				 were not being returned.

-- ================================================
			@code varchar(6),
			@branch  varchar(4),
			@excludeAccepted int,
			@limit int,
			@category varchar(12),
			@return int OUTPUT

AS
	if @code = ''
	   set @code ='%'
    SET NOCOUNT ON
	SET 	@return = 0			--initialise return code

	CREATE TABLE #BDWAccts
	(	
		acctno varchar(12),
		Strategy VARCHAR(6),
		balance money,
      	charges money,
       	balexclint money,	
       	agreementtotal money,
       	code varchar (5),
       	empeeno integer,
       	empeenomanual integer,
       	empeename varchar(101),
       	customername varchar(90),
       	custid varchar(60),
		balancebeforerepo money,
		repovalue money,
		ReposDate datetime,				--IP - 30/09/10 - CR1107
		reppercent int,
		balanceafterrepo money,
		paidpcent smallint,
		datelastpaid datetime,
		rejectcode varchar (5),
		claimnumber varchar(10),
		statuscode char(1),
       	manualempeename varchar(101),
       	agrmntvalue MONEY,
       	ProvisionAmount FLOAT,
       	Arrears	money,					--IP - 08/09/10 - CR1107
       	ArrearsExCharges money,			--IP - 08/09/10 - CR1107
       	Instalamount money,				--IP - 08/09/10 - CR1107
       	PaymentAmt money,				--IP - 08/09/10 - CR1107
       	MonthsInArrears float,			--IP - 08/09/10 - CR1107
       	DeliveryDate datetime,			--IP - 08/09/10 - CR1107
       	ServiceChg money,				--IP - 08/09/10 - CR1107
       	SegmentName varchar(32),		--IP - 08/09/10 - CR1107
       	Rebate money,					--IP - 08/09/10 - CR1107
       	BranchNo smallint,				--IP - 09/09/10 - CR1107
       	LastTransDate datetime,			--IP - 09/09/10 - CR1107
	)

	-- Remove account in credit or zero balance	
	DELETE FROM BDWPending
	WHERE EXISTS(SELECT acctno 
		     FROM   Acct
		     WHERE  Acct.acctno = BDWPending.acctno
	             AND    (Acct.outstbal = 0 or Acct.outstbal < 0.01))

	-- RD 29/06/06 Remove account in credit or zero balance	from BDWRejection
	DELETE FROM BDWRejection
	WHERE EXISTS(SELECT acctno 
		     FROM   Acct
		     WHERE  Acct.acctno = BDWRejection.acctno
	             AND    (Acct.outstbal = 0 or Acct.outstbal < 0.01))

	-- 68288 RD 29/06/09 Remove accounts where payment has been since account was flagged as write off pending.
	DELETE FROM BDWPending
	WHERE EXISTS (SELECT * FROM Fintrans f , interfacecontrol i
		      WHERE  BDWPending.acctno = f.acctno
		      AND    i.interface = 'AUTOBDW'
		      AND    f.datetrans > i.datefinish
		      AND    BDWPending.runno = i.runno
		      AND    f.transtypecode IN ('PAY','DDE','DDN','DDR','COR','REF','RET'))

	-- 68288 RD 29/06/09 Remove accounts where payment has been since account was flagged as write off pending.
	DELETE FROM BDWRejection
	WHERE EXISTS (SELECT * FROM Fintrans f 
		      WHERE  f.acctno = BDWRejection.acctno
		      AND    f.datetrans > BDWRejection.rejectdate
		      AND    f.transtypecode IN ('PAY','DDE','DDN','DDR','COR','REF','RET'))


	SET ROWCOUNT  @limit 

	IF(@category = 'BDP' OR @category = 'BDM' or @category = 'TBD')
	BEGIN
		INSERT INTO 	#BDWAccts
		SELECT	acctno,'',0,0,0,0,code,empeeno,empeenomanual,
				'','','',0,0,'',0,0,0,'','','','','',0,0,
				0,0,0,0,0,'',0,'',0,0,''					--IP - 09/09/10 - CR1107
		FROM 	BDWPending
		WHERE 	code = @code
		AND	acctno LIKE @branch
		AND	runno = 0
	END	



	IF @excludeAccepted = 1
	BEGIN
		DELETE FROM #BDWAccts
		WHERE empeeno != 0
	END

	IF(@category = 'BDR' OR @category = 'BDD')
	BEGIN
		INSERT INTO #BDWAccts
		SELECT	acctno,'',0,0,0,0,code,empeeno,'',
				'','','',0,0,'',0,0,0,'',rejectcode,'','','',0,0,
				0,0,0,0,0,'',0,'',0,0,''				--IP - 09/09/10 - CR1107			
		FROM 	BDWRejection
		WHERE 	rejectcode = @code
		AND	acctno LIKE @branch
	END	

	IF(@category = 'ALL' OR @category ='')
	BEGIN
		INSERT INTO 	#BDWAccts
		SELECT	acctno,'',0,0,0,0,code,empeeno,empeenomanual,
				'','','',0,0,'',0,0,0,'','','','','',0,0,
				0,0,0,0,0,'',0,'',0,0,''					--IP - 09/09/10 - CR1107		
		FROM 	BDWPending
		WHERE 	code like @code
		AND	acctno LIKE @branch
		AND	runno = 0
		
		INSERT INTO #BDWAccts
		SELECT	acctno,'',0,0,0,0,code,empeeno,'',
				'','','',0,0,'',0,0,0,'',rejectcode,'','','',0,0,
				0,0,0,0,0,'',0,'',0,0,''				--IP - 09/09/10 - CR1107	
		FROM 	BDWRejection
		WHERE 	rejectcode like @code
		AND	acctno LIKE @branch
    END
	SET ROWCOUNT  0    

	UPDATE	#BDWAccts
	SET	custid = c.custid
	FROM 	custacct c
	WHERE	c.acctno = #BDWAccts.acctno
	AND	c.hldorjnt = 'H'
	
	UPDATE	#BDWAccts
	SET	balance = a.outstbal,
		balexclint = a.outstbal,
		datelastpaid = a.datelastpaid,
		paidpcent = a.paidpcent,
		statuscode = a.currstatus,
		Arrears = a.arrears,								--IP - 08/09/10 - CR1107
		ArrearsExCharges = a.arrears,						--IP - 08/09/10 - CR1107
		MonthsInArrears = round(a.MonthsInArrears,2),		--IP - 08/09/10 - CR1107
		BranchNo = substring(a.acctno, 1, 3)				--IP - 09/09/10 - CR1107
	FROM 	acct a
	WHERE	a.acctno = #BDWAccts.acctno
	
	UPDATE	#BDWAccts
	SET	agreementtotal = a.agrmttotal,
		ServiceChg = a.servicechg						--IP - 08/09/10 - CR1107
	FROM 	agreement a
	WHERE	a.acctno = #BDWAccts.acctno
	
	UPDATE	#BDWAccts
	SET	customername = c.firstname + ' ' + c.name
	FROM 	customer c
	WHERE	c.custid = #BDWAccts.custid
	
	UPDATE	#BDWAccts
	SET	empeename = c.FullName
	FROM 	Admin.[User] c
	WHERE	c.id = #BDWAccts.empeeno

	UPDATE	#BDWAccts
	SET	manualempeename = c.FullName
	FROM 	Admin.[User] c
	WHERE	c.id = #BDWAccts.empeenomanual
	
	UPDATE	#BDWAccts
	SET	claimnumber = c.reference
	FROM 	custcatcode c
	WHERE	c.custid = #BDWAccts.custid
	AND	c.code = 'BPT'

	SELECT	ISNULL(SUM(transvalue), 0) as value,
		f.acctno
	INTO	#charges
	FROM 	fintrans f, #BDWAccts b
	WHERE	f.acctno = b.acctno
	AND	f.transtypecode IN ('INT', 'ADM')
	GROUP BY	f.acctno
	
	UPDATE	#BDWAccts
	SET	balexclint = balance - c.value,
		ArrearsExCharges = Arrears - c.value,				--IP - 08/09/10 - CR1107
		charges = c.value
	FROM 	#charges c
	WHERE	c.acctno = #BDWAccts.acctno

	SELECT	f.acctno, ISNULL(SUM(transvalue), 0) as repossvalue
	INTO	#repo
	FROM 	fintrans f, #BDWAccts b
	WHERE	f.acctno = b.acctno
	AND	f.transtypecode = 'REP'
	GROUP BY	f.acctno
	
	SELECT	f.acctno, ISNULL(SUM(transvalue), 0) as balancebefore
	INTO	#repobefore
	FROM 	fintrans f, #BDWAccts b
	WHERE	f.acctno = b.acctno
	AND	f.transtypecode != 'REP'
	GROUP BY	f.acctno

	SELECT	f.acctno, ISNULL(SUM(transvalue), 0) as balanceafter
	INTO	#repoafter
	FROM 	fintrans f, #BDWAccts b
	WHERE	f.acctno = b.acctno
	AND	f.transtypecode NOT IN ('INT', 'ADM')
	GROUP BY	f.acctno

	UPDATE	#BDWAccts
	SET	agrmntvalue = ISNULL(a.agrmttotal - a.servicechg, 0)
	FROM 	agreement a
	WHERE	a.acctno = #BDWAccts.acctno

	UPDATE	#BDWAccts
	SET	reppercent = abs(ISNULL(repossvalue / agrmntvalue * 100, 0))
	FROM 	#repo r
	WHERE	r.acctno = #BDWAccts.acctno
	AND	agrmntvalue > 0

	UPDATE	#BDWAccts
	SET	repovalue = repossvalue
	FROM 	#repo r
	WHERE	r.acctno = #BDWAccts.acctno

	UPDATE	#BDWAccts
	SET	balancebeforerepo = balancebefore
	FROM 	#repobefore r
	WHERE	r.acctno = #BDWAccts.acctno

	UPDATE	#BDWAccts
	SET	balanceafterrepo = balanceafter
	FROM 	#repoafter r
	WHERE	r.acctno = #BDWAccts.acctno

	UPDATE #BDWAccts
    	SET Strategy = S.strategy FROM CMStrategyAcct S
    	WHERE S.acctno = #BDWAccts.acctno AND dateto IS NULL
    	
    UPDATE #BDWAccts
    SET ProvisionAmount = CASE WHEN provision > 0 THEN provision ELSE 0 END
    FROM View_Provision
    WHERE 	#BDWAccts.acctno = View_Provision.acctno
    
     --IP - 08/09/10 - CR1107 - Update Instalamount
    UPDATE #BDWAccts
    SET	Instalamount = i.instalamount
    FROM instalplan i
    WHERE i.acctno = #BDWAccts.acctno
    
    --IP - 08/09/10 - CR1107 - Update PaymentAmt
    UPDATE #BDWAccts
    set PaymentAmt = (select abs(sum(transvalue)) 
						from fintrans f
						where f.acctno = #BDWAccts.acctno
						and f.transtypecode in ('PAY','DDE','DDN','DDR','COR','REF','RET'))
	
	
	--IP - 08/09/10 - CR1107 - Update DeliveryDate
	UPDATE #BDWAccts
	SET DeliveryDate = (SELECT a.datedel
							from agreement a
							where a.acctno = #BDWAccts.acctno)	
							
	--IP - 08/09/10 - CR1107 - Update the Segment Name
	UPDATE #BDWAccts
	SET SegmentName = (select Segment_Name
						from TM_Segments ts
						where ts.Account_Number = #BDWAccts.acctno)		
						
	--IP - 08/09/10 - CR1107 - Update accounts Rebate
	UPDATE #BDWAccts
	SET Rebate = (select rebate
					from rebates r
					where r.acctno = #BDWAccts.acctno)
					
	--IP - 09/09/10 - CR1107 - Update last transaction date on the account
	UPDATE #BDWAccts
	SET LastTransDate = (select MAX(f.datetrans)
							from fintrans f
							where f.acctno = #BDWAccts.acctno)
	
	--IP - 30/09/10 - CR1107 - Update repossession date for an account
	UPDATE #BDWAccts
	SET ReposDate = (select MAX(f.datetrans)
							 from fintrans f
							 where f.acctno = #BDWAccts.acctno
							 and f.transtypecode = 'REP')
	
	--IP - 01/10/12 - #11360 - LW75288
	-- Removing duplicates first where rejection
	--DELETE FROM #BDWAccts WHERE ISNULL(rejectcode,'') !=''
	--AND EXISTS (SELECT * FROM #bdwaccts b WHERE b.acctno = #BDWAccts.acctno 
	--AND b.rejectcode !='')
	
	-- removing duplicates where in with another code...
	DELETE FROM #BDWAccts WHERE EXISTS 
	(SELECT * FROM  #BDWAccts b WHERE 
	b.acctno = #BDWAccts.acctno 
	AND b.code > #BDWAccts.code)
	
	SELECT	acctno,
		Strategy,
		statuscode,
		custid,
		customername,
		balexclint,
       	charges,
        agreementtotal,
        provisionamount,
        code,
		rejectcode,
		convert(varchar, empeeno) + ' ' + empeename as 'EmployeeName',
		balancebeforerepo,
		repovalue,
		ReposDate,											--IP - 30/09/10 - CR1107
		reppercent,
		balanceafterrepo,
		paidpcent,
		datelastpaid,
		claimnumber,
		convert(varchar, empeenomanual) + ' ' + manualempeename as 'manualname',
		Arrears,											--IP - 08/09/10 - CR1107 
		ArrearsExCharges,									--IP - 08/09/10 - CR1107
		Instalamount,										--IP - 08/09/10 - CR1107
		PaymentAmt,											--IP - 08/09/10 - CR1107
		MonthsInArrears,									--IP - 08/09/10 - CR1107
		DeliveryDate,										--IP - 08/09/10 - CR1107
		ServiceChg,											--IP - 08/09/10 - CR1107
		SegmentName,										--IP - 08/09/10 - CR1107
		Rebate,												--IP - 08/09/10 - CR1107
		BranchNo,											--IP - 09/09/10 - CR1107
		LastTransDate										--IP - 09/09/10 - CR1107
		
	FROM	#BDWAccts

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
	SET ROWCOUNT 0
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

