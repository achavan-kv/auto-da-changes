SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TelephoneActionLoadStrategyAccountsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TelephoneActionLoadStrategyAccountsSP]
GO

--exec DN_TelephoneActionLoadStrategyAccountsSP '8015','720%','0','%',0

CREATE PROCEDURE 	dbo.DN_TelephoneActionLoadStrategyAccountsSP
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_TelephoneActionLoadStrategyAccountsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Retrieve details for Bailiff Review screen.
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 06/09/10 jec  CR1084 return Proposal date 
-- 08/03/11 ip   Sprint 5.11 #3272 - LW73288 - Incorrect code selected for action. Should use category 'FUA'. 
--------------------------------------------------------------------------------
    -- Parameters
			@empeeno int,
			@branchOrAcctFilter varchar(12),
			@acctnosearch smallint,
			@strategy VARCHAR(3),
			@return int OUTPUT

AS

	SET NOCOUNT ON
	SET 	@return = 0			--initialise return code

    -- 67945 Added default value 0 for instalamount and DueDay
    CREATE TABLE #tname
    (
	acctno varchar(12), 
	accttype char(1), 
	dateacctopen datetime, 
	agrmttotal money, 
	instalamount MONEY NOT NULL DEFAULT 0,
	DueDay SMALLINT NOT NULL DEFAULT 0,
	datelastpaid datetime, 
	termstype varchar(2),
	outstbal money, 
	arrears money, 
	IntAdm money, 
	MonthsInArrears money, 
	highststatus char(1), 
	currstatus char(1), 
	title varchar(25), 
	name varchar(60), 
	firstname varchar(30), 
	custid varchar(20), 
	datestatchge datetime, 
	dateacctlttr datetime, 
	lettercode varchar(10), 
	hometel varchar(30), 
	worktel varchar(30),
    mobileno VARCHAR(30), 
	ext  varchar(6), 
	DateAlloc DATETIME, 
	empeeno int, 
	empeetype char(3), --IP - 22/05/08 - Credit Collections - Need to cater for (3) character Employee Types.
	empeename varchar(101),
	description varchar(40),
    ActionCode VARCHAR(80) NOT NULL DEFAULT '',
    DateAdded  DATETIME    NOT NULL DEFAULT '',
    paidpcent SMALLINT,
    BankOrder  CHAR(1)     NOT NULL DEFAULT '',
    cashLoan BIT,                                --CR906 JH 11/09/2007
    CreditBlocked BIT,							 --IP - 28/04/10 - UAT(983) UAT5.2
	ProvisionAmount FLOAT
    )

	-- 67945 RD 08/02/2006 Missing Cash account from Bailiff Review and Telephone Action screen 
	-- due to link to instalplan
	-- setting value to zero for columns instalamount, datefirst and revmoed link to instalplan table
	IF(@acctnosearch = 1)
	BEGIN
	    	insert into #tname (
	    	acctno,	accttype, dateacctopen,agrmttotal,
	    	instalamount,DueDay,datelastpaid,termstype,
	    	outstbal,arrears,IntAdm,MonthsInArrears,
	    	highststatus,currstatus,title,name,
	    	firstname,custid,datestatchge,dateacctlttr,
	    	lettercode,hometel,worktel,mobileno,
	    	ext,DateAlloc,empeeno,empeetype,
	    	empeename,description,ActionCode,DateAdded,
	    	paidpcent,BankOrder,cashLoan,CreditBlocked,ProvisionAmount)
	select	DISTINCT A.AcctNo, A.AcctType, A.DateAcctOpen, A.AgrmtTotal,
		    	0, 0, A.DateLastPaid, A.termstype,isnull(A.OutstBal,0), 
			isnull(A.Arrears,0),0, 0,A.HighstStatus, A.CurrStatus, 
			isnull(CU.Title,''),isnull(CU.name,''), isnull(CU.Firstname,''),
			CU.custid, convert(datetime, null), convert(datetime, null), '',
			'', '', '','',ISNULL(F.DateAlloc,''), F.empeeno, '', 
			CP.empeename, '', '', '',paidpcent, ''	,isLoan AS cashLoan, CU.CreditBlocked,0.0 --IP - 28/04/10 - UAT(983) UAT5.2 - Added CreditBlocked
		from 	ACCT A, CUSTOMER CU, CUSTACCT CA, FOLLUPALLOC F, COURTSPERSON CP,  CODE C1, TermsTypeTable T, CMStrategyAcct SA
		where 	A.acctno LIKE @branchOrAcctFilter
		and 	(F.datedealloc = '' or F.datedealloc is null)
		and 	(datealloc != '' and datealloc is not null)
		and 	A.Acctno = F.acctno
		and 	CA.Acctno = F.acctno
		and 	CA.hldorjnt = 'H'
		and 	CU.custid = CA.custid
        AND A.termstype = T.termstype
		and	F.empeeno = CP.empeeno
		AND SA.acctno = A.acctno
        AND Strategy LIKE @strategy
		AND SA.dateto IS NULL --IP - 01/06/09 - Credit Collection Walkthrough change - account should be in the selected strategy.
		AND	SA.strategy = c1.code 
		AND	c1.reference > 0 
		And	C1.CATEGORY = 'SS1' 
    END
	ELSE
	BEGIN
			--IP - 01/06/09 - Credit Collection Walkthrough change - the account should be in a strategy 
			--which can have its accounts allocated to Bailiff / Collectors.
	    	insert into #tname (
	    	acctno,	accttype, dateacctopen,agrmttotal,
	    	instalamount,DueDay,datelastpaid,termstype,
	    	outstbal,arrears,IntAdm,MonthsInArrears,
	    	highststatus,currstatus,title,name,
	    	firstname,custid,datestatchge,dateacctlttr,
	    	lettercode,hometel,worktel,mobileno,
	    	ext,DateAlloc,empeeno,empeetype,
	    	empeename,description,ActionCode,DateAdded,
	    	paidpcent,BankOrder,cashLoan,CreditBlocked,ProvisionAmount)
		select	DISTINCT A.AcctNo, A.AcctType, A.DateAcctOpen, A.AgrmtTotal,
		    	0, 0, A.DateLastPaid,A.termstype,isnull(A.OutstBal,0), 
			isnull(A.Arrears,0),0, 0,A.HighstStatus, A.CurrStatus, 
			isnull(CU.Title,''),isnull(CU.name,''), isnull(CU.Firstname,''),
			CU.custid, convert(datetime, null), convert(datetime, null), '',
			'', '', '','', ISNULL(F.DateAlloc,''), 0 , '', '', '', '', '',paidpcent, ''	,isLoan AS cashLoan, CU.CreditBlocked,0 -- IP - 28/04/10 - UAT(983) UAT5.2 - Added CreditBlocked
		from 	ACCT A, CUSTOMER CU, CUSTACCT CA, FOLLUPALLOC F, TermsTypeTable T, CMStrategyAcct SA, Code c
		-- UAT 111 - the filter is now only used above when the user enters an acct number
		--where 	A.acctno like @branchOrAcctFilter
		where	F.empeeno = @empeeno
		and 	(F.datedealloc = '' or F.datedealloc is null)
		and 	(datealloc != '' and datealloc is not null)
		and 	A.Acctno = F.acctno
		and 	CA.Acctno = F.acctno
		and 	CA.hldorjnt = 'H'
		and 	CU.custid = CA.custid
        AND     A.termstype = T.termstype
        AND     SA.acctno = A.acctno
        AND     Strategy LIKE @strategy
		AND     SA.dateto IS NULL 
		AND		SA.strategy = c.code 
		AND		c.reference > 0 
		And		C.CATEGORY = 'SS1' 
	END	
	
	UPDATE #tname
	SET ProvisionAmount = Provision
	FROM View_Provision
	WHERE #tname.acctno = View_Provision.acctno


	-- 67945 RD 08/02/06 Now updating value for InstalAmount
	UPDATE	#tname
	SET	InstalAmount = ISNULL(I.InstalAmount,0),
		DueDay = ISNULL(DAY(I.DateFirst),0)
	FROM	instalplan i
	WHERE	#tname.acctno = i.acctno

	select	s.acctno, 
		max(s.datestatchge) as datestatchge
	into 	#tname2 
	from 	status s, #tname  t
	where 	t.acctno = s.acctno
	group by s.acctno
	
	update	#tname
	set 	datestatchge = #tname2.datestatchge
	from 	#tname2
	where 	#tname.acctno = #tname2.acctno
	
	IF (@@error != 0)
	BEGIN
		drop TABLE #tname
		drop TABLE #tname2
	END
	
	select	l.acctno, 
		max(l.dateacctlttr) as dateacctlttr
	into	#tname3
	from 	letter l, #tname t
	where 	t.acctno = l.acctno
	group by l.acctno
	
	select	l.acctno, 
		l.dateacctlttr, 
		count(l.dateacctlttr) col3
	into	#tname4
	from 	letter l, #tname t1, #tname3 t2
	where 	t1.acctno = l.acctno 
	and 	t1.acctno = t2.acctno
	and 	t2.dateacctlttr = l.dateacctlttr
	group by l.acctno, l.dateacctlttr
	having count(l.acctno) > 1
	
	IF (@@error != 0)
	BEGIN
		drop TABLE #tname
		drop TABLE #tname2
		drop TABLE #tname3
	END
	
	select	l.acctno, 
		l.dateacctlttr, 
		max(l.lettercode) as lettercode
	into	#tname5
	from 	letter l, #tname4 t
	where 	t.acctno = l.acctno
	and 	t.dateacctlttr = l.dateacctlttr
	group by l.acctno, l.dateacctlttr
	
	IF (@@error != 0)
	BEGIN
		drop TABLE #tname
		drop TABLE #tname2
		drop TABLE #tname3
		drop TABLE #tname4
	END
	
	update	#tname
	set 	lettercode = letter.lettercode,
		dateacctlttr = #tname5.dateacctlttr
	from 	letter, #tname5
	where 	#tname.acctno = letter.acctno
	and 	#tname.acctno = #tname5.acctno
	and 	#tname5.dateacctlttr = letter.dateacctlttr
	and 	#tname5.lettercode = letter.lettercode
	
	IF (@@error != 0)
	BEGIN
		drop TABLE #tname
		drop TABLE #tname2
		drop TABLE #tname3
		drop TABLE #tname4
	END
	
	update	#tname
	set 	lettercode   = letter.lettercode,
		dateacctlttr = #tname3.dateacctlttr
	from 	letter , #tname3
	where 	#tname.acctno = letter.acctno
	and 	#tname.acctno = #tname3.acctno
	and 	#tname3.dateacctlttr = letter.dateacctlttr
	and 	#tname.dateacctlttr = ''
	
	IF (@@error != 0)
	BEGIN
		drop TABLE #tname
		drop TABLE #tname2
		drop TABLE #tname3
		drop TABLE #tname4
		drop TABLE #tname5
	END
	
	select	c.custid, 
		min(c.addtype) as addtype
	into	#tname6 
	from 	custaddress c, #tname t
	where 	c.custid = t.custid
	and 	c.addtype != ''
	and 	c.addtype is not null
	and 	c.custid != ''
	and 	c.custid is not null
	group by c.custid
	
	IF (@@error != 0)
	BEGIN
		drop TABLE #tname;
		drop TABLE #tname2;
		drop TABLE #tname3;
		drop TABLE #tname4;
		drop TABLE #tname5;
	END
	
	update	#tname
	set 	HomeTel = rtrim(dialcode) +  telno
	from 	custtel c
	where 	#tname.custid = c.custid
	and	tellocn = 'H'
	and 	datediscon is null

	IF (@@error != 0)
	BEGIN
		drop TABLE #tname;
		drop TABLE #tname2;
		drop TABLE #tname3;
		drop TABLE #tname4;
		drop TABLE #tname5;
		drop TABLE #tname6;
	END

	update #tname
	set WorkTel = rtrim(dialcode) + telno,
	      Ext = isnull(extnno,' ')	
	from custtel c
	where 	#tname.custid = c.custid
	and	tellocn = 'W'
	and 	datediscon is null

	IF (@@error != 0)
	BEGIN
		drop TABLE #tname;
		drop TABLE #tname2;
		drop TABLE #tname3;
		drop TABLE #tname4;
		drop TABLE #tname5;
		drop TABLE #tname6;
	END

    --CR852 Include mobile number JH 07/06/2007
    update #tname
	set mobileno = rtrim(dialcode) + telno
	from custtel c
	where 	#tname.custid = c.custid
	and	tellocn = 'M'
	and 	datediscon is NULL

    IF (@@error != 0)
	BEGIN
		drop TABLE #tname;
		drop TABLE #tname2;
		drop TABLE #tname3;
		drop TABLE #tname4;
		drop TABLE #tname5;
		drop TABLE #tname6;
	END

    -- Add columns for the last Bailiff Action added
    UPDATE #tname
    SET    DateAdded = ISNULL((SELECT MAX(ba.DateAdded)
                               FROM   BailAction ba
                               WHERE  ba.AcctNo = #tname.AcctNo),'')

    -- Use MAX(Code) just in case DateAdded can duplicate
    UPDATE #tname
    SET    ActionCode = ISNULL((SELECT MAX(ba.Code + ' : ' + c.CodeDescript)
                                FROM   BailAction ba, Code c
                                WHERE  ba.AcctNo = #tname.AcctNo
                                AND    ba.DateAdded = #tname.DateAdded
                                --AND    c.Category = 'FUP'
                                AND    c.Category = 'FUA' --IP - 08/03/11 - #3272 - LW73288
                                AND    c.Code = ba.Code),'')
                         
    -- Add a column for the Bankers Order indicator
    UPDATE #tname
    SET    BankOrder = 'B'
    WHERE  EXISTS (SELECT ac.Code FROM AcctCode ac
                   WHERE  ac.AcctNo = #tname.AcctNo
                   AND    ac.Code = 'B'
                   AND    ac.DateDeleted IS NULL)
                   
    -- Calc the months into arrears excluding Interest and Admin charges
    UPDATE #tname
    SET    IntAdm = isnull(Acct.Outstbal, 0) - isnull(Acct.AS400Bal, 0)
	FROM #tname LEFT OUTER JOIN 
	Acct ON #tname.AcctNo = Acct.AcctNo 
/*ISNULL((SELECT SUM(f.TransValue)
                            FROM   FinTrans f
                            WHERE  f.AcctNo = #tname.AcctNo
                            AND    f.TransTypeCode IN ('INT','ADM')),0) */

    -- Round DOWN
    UPDATE #tname
    SET    MonthsInArrears = ROUND((#tname.Arrears - #tname.IntAdm) / i.InstalAmount,1)
    FROM   InstalPlan i
    WHERE  i.AcctNo = #tname.AcctNo
    AND    #tname.Arrears > 0
    AND    #tname.Arrears > #tname.IntAdm
    AND    i.InstalAmount > 0
    

	select	t.AcctNo,		--CR1084 
		AcctType,
		TermsType,
        	DateAlloc,
		DateAcctOpen,
		AgrmtTotal,
        	instalamount,
        	DueDay,
		OutstBal,
		Arrears,
		MonthsInArrears,
		DateLastPaid,
		HighstStatus,
		CurrStatus,
		Title,
		Name,
		Firstname,
		t.Custid,			--CR1084 
		HomeTel,
		WorkTel,
        mobileno,
		Ext,
		EmpeeNo,
		EmpeeType,
		EmpeeName as EmployeeName,
		Description,
        ActionCode,
        DateAdded,
        paidpcent,
        BankOrder,
		cashLoan,
		CreditBlocked,		--IP - 28/04/10 - UAT(983) UAT5.2
		ProvisionAmount,
		p.dateprop			--CR1084 
	from	#tname t
			inner join proposal p on p.AcctNo = T.AcctNo		--CR1084 
	ORDER BY DateAdded
	
	drop TABLE #tname
	drop TABLE #tname2
	drop TABLE #tname3
	drop TABLE #tname4
	drop TABLE #tname5
	drop TABLE #tname6
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End