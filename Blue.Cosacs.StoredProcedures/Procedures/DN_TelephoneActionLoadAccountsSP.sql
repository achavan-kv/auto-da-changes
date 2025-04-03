SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TelephoneActionLoadAccountsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TelephoneActionLoadAccountsSP]
GO
CREATE PROCEDURE 	[dbo].[DN_TelephoneActionLoadAccountsSP]
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_TelephoneActionLoadAccountsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Retrieve details for Telephone Action screen.
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/11/09 jec  UAT781 Last Action is displaying incorrect action.
-- 15/12/09 jec  UAT936 Select for required worklist
-- 06/01/10 IP   UAT951 Apply sort order to the accounts returned.
-- 02/02/10 IP   UAT971 Incorrect data returned in columns e.g. DateFrom displayed value for DateAdded
-- 28/04/10 IP - UAT983 Return CreditBlocked. Inform the user that the Customers Credit is blocked when processing an SPA (Extended Term).
-- 07/07/10 AA - UAT1041 Performance improvement
-- 26/08/10 jec  CR1084 return Referral Reason 
-- 14/09/10 jec  CR1084 return all Referral Reasons 
-- 02/06/11 IP   #3743 - LW73641 - Check customer is not locked by another user when returning an account.
-- 07/03/12 IP   #9744 - cater for Store Card accounts. Store Card accounts were not being returned.
-- 02/04/13 IP   #12860 - UAT 12631 - Error when un-ticking View Top 500 - Removed select distinct. No duplicates are being returned.
-- 17/04/13 IP   #18860 - UAT 12631 - Speeded up retrieval of data by removing aggregate which was not required and using update to update dateadded for action
--------------------------------------------------------------------------------
    -- Parameters
			@user INT,
			@viewTop500 BIT, --IP - 12/11/09 - UAT5.2 (882)
			@worklist VARCHAR(10),		-- UAT936 
			@return int OUTPUT

AS

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET 	@return = 0			--initialise return code

    -- 67945 Added default value 0 for instalamount and DueDay
    CREATE TABLE #tname
    (
		worklist varchar(10),
		acctno char(12), 
		accttype char(1), 
		dateacctopen datetime, 
		agrmttotal money, 
		instalamount MONEY NOT NULL DEFAULT 0,
		DueDay SMALLINT NOT NULL DEFAULT 0,
		datelastpaid datetime, 
		outstbal money, 
		arrears money, 
		IntAdm money, 
		MonthsInArrears money, 
		highststatus char(1), 
		currstatus char(1), 
		title varchar(25), 
		name varchar(60), 
		firstname varchar(30), 
		custaddress1 varchar (50), --NM & IP - 29/12/08 - CR976 - Need to display the Customers address
		custaddress2 varchar(50),
		custaddress3 varchar(50),
		cuspocode varchar(10),
		custid varchar(20), 
		datestatchge datetime, 
		hometel varchar(30), 
		homedialcode varchar(8),
		worktel varchar(30),
		workdialcode varchar(8),
		mobileno VARCHAR(30), 
		mobiledialcode varchar(8),
		ext  varchar(6), 
		DateFrom DATETIME,
		DateAlloc DATETIME, 
		ActionCode VARCHAR(80) NOT NULL DEFAULT '',
		DateAdded  DATETIME    NOT NULL DEFAULT '',
		paidpcent SMALLINT,
		ReminderDateTime DATETIME,
		CreditBlocked BIT, --IP - 28/04/10 - UAT(983) UAT5.2 
		Provision FLOAT 
    )

	CREATE CLUSTERED INDEX ixsdfsdf_acctno ON #tname(acctno,custid) 
	
	CREATE INDEX ixsdfsdf_custid34 ON #tname(custid) 
	
	--IP - 05/06/09 - Credit Collection Walkthrough Changes - if an action has been performed on an 
	--account then this will only appear in the worklist once the number of days set in the Country Parameter have passed
	--since the action was performed. It will always appear for a supervisor.
	--
	DECLARE @DaysSinceAction int,
			@Supervisor bit

	SELECT @DaysSinceAction = VALUE FROM countrymaintenance WHERE codename = 'NoOfDaysSinceAction'

	SET @Supervisor = Admin.CheckPermission(@User,326)
		
	DECLARE @Top500 VARCHAR(10) --IP - 12/11/09 - UAT5.2 (882)
	
	IF(@viewTop500 = 1)
	BEGIN
		SET @Top500 = 'TOP 500'
	END
	ELSE
	BEGIN
		--SET @Top500 = 'DISTINCT'
		SET @Top500 = ''
	END
	
	-- here we are loading up the accounts in a #temp table... 
	CREATE TABLE #acctstoload  (acctno CHAR(12) NOT NULL primary key,dateadded DATETIME )
	
	DECLARE @statement sqltext 
	BEGIN 
		SET @statement =  ' insert into #acctstoload (acctno,dateadded) '
		--+  ' select TOP 500 A.acctno ,MAX(b.dateadded) ' +  --IP - 07/10/09 - Changed to select top 500
		 --+  ' select ' + @Top500 +  ' A.acctno ,MAX(isnull(b.dateadded, ''1/1/1900'')) ' +  --IP - 07/10/09 - Changed to select top 500 --IP - 12/11/09 UAT5.2 (882) - Added @Top500 --IP - 02/02/10 - UAT(971)
		+  ' select ' + @Top500 +  ' A.acctno ,' + 'cast( '''' as datetime) as dateadded' + --#12860 --IP - 07/10/09 - Changed to select top 500 --IP - 12/11/09 UAT5.2 (882) - Added @Top500 --IP - 02/02/10 - UAT(971)
          '  FROM ACCT A,instalplan ip , Courtsperson c ,CMWorkListRights WLR ' + 
			'	INNER JOIN CMWorkListsAcct WLA ON  WLA.Worklist = WLR.WorkList	' + 
			'	INNER JOIN CustAcct ca ON WLA.acctno = ca.acctno ' +						--IP - 02/06/11 - #3743 - LW73641
             '   LEFT JOIN [bailactionMaxAction] b ON  [b].[acctno] = WLA.[acctno] ' +
             '   LEFT OUTER JOIN View_ProvisionPercent PP ON PP.acctno = WLA.acctno ' + 
             ' WHERE 	WLR.EmpeeNo = ' + CONVERT(VARCHAR,@user) +
		        + '    and 	A.Acctno = WLA.acctno '
		        + ' AND WLA.Dateto IS NULL '
		        + ' AND ip.acctno= a.acctno and ip.agrmtno= 1 AND (ip.instalamount >=1 or A.accttype = ''T'') ' --IP - 07/03/12 - #9744 - cater for Store Card accounts
		        + ' AND ca.hldorjnt = ' + '''H'''										--IP - 02/06/11 - #3743 - LW73641
			    --+ ' AND WLA.[acctno] NOT IN (SELECT [acctno] FROM [AccountLocking] WHERE CurrentAction = ''T'' AND lockedby !=' + CONVERT(VARCHAR,@user) + ' ) '  --IP - 02/06/11 - #3743 - LW73641
			    + ' AND ca.Custid NOT IN (SELECT [CustID] FROM [CustomerLocking] WHERE isnull(CurrentAction,''T'') = ''T'' AND LockedBy!=' + CONVERT(VARCHAR,@user) + ' ) ' --IP - 02/06/11 - #3743 - LW73641
			    -- UAT936 jec 15/12/09 select for required worklist or worklist =blank
			    + ' and (wla.worklist= ' + '''' + CAST(@worklist as VARCHAR(10)) + ''''+ ' or ' + '''' + CAST(@worklist as VARCHAR(10)) + '''' + ' ='''') '
				+ ' AND NOT EXISTS(select * from bailactionMaxAction b ' + -- do not load up if a positive action has taken place recently stored in this new table.
								' where b.acctno = wla.acctno ' +
                                ' and b.dateadded > dateadd(day, -' + CONVERT(VARCHAR,@DaysSinceAction) + ',getdate()) ' +
								' and ' + CONVERT(VARCHAR,@Supervisor) + '= 0
								) GROUP BY a.acctno '  -- but do load up accounts if this is a supervisor i.e 1 will not= 0. 
								
		-- need to implement sort order dynamically.... Can do on the following   Arrears, DatelastPaid,OutstandingBalance,Worklist,MonthsInarrears
		declare @sortorder varchar(360), @sortcolumn VARCHAR(32),@counter INT ,@asdesc VARCHAR(10),@groupby VARCHAR(360),@sort INT 
		SET @counter = 1 SET @sortorder =' ORDER BY ' SET @groupby = ''
		declare sort_cursor CURSOR FAST_FORWARD READ_ONLY FOR
		SELECT  S.SortColumnName,s.AscDesc,Min(s.sortorder) AS sortorder
		FROM courtsperson C
		INNER JOIN Admin.UserRole ur ON c.EmpeeNo = ur.UserId
		CROSS JOIN CMWorkListSortOrder S 
		GROUP BY   S.SortColumnName,s.AscDesc
		order by sortorder 
		OPEN sort_cursor
		
		
		
		FETCH NEXT FROM sort_cursor INTO @sortcolumn,@asdesc,@sort
		WHILE @@FETCH_STATUS = 0
		BEGIN
			
			--IP - 06/01/2010 - UAT(951) - Added alias's. If changes are made to the alias's here, please ensure they are made below (in the final select).
			IF @sortcolumn = 'Arrears'
				SET @sortcolumn = 'a.arrears'
				
			IF @sortcolumn = 'ProvisionPercent'
				SET @sortcolumn = 'pp.provision'
				
			IF @sortcolumn ='OutstandingBalance'
				SET @sortcolumn = 'a.outstbal '
			
			IF @sortcolumn = 'MonthsInarrears'	
				SET @sortcolumn = 'a.arrears/ip.instalamount'
				
			IF @sortcolumn = 'WorkList'
				SET @sortcolumn = 'WLR.' + @sortcolumn
	
			IF @counter >1 
			BEGIN
				SET @sortorder= @sortorder + ', '

			END

				SET @groupby = @groupby + ',' 				
				SET @groupby = @groupby + @sortcolumn
				SET @sortorder = @sortorder + @sortcolumn + ' ' + @asdesc 

			SET @counter = @counter + 1
		FETCH NEXT FROM sort_cursor INTO @sortcolumn,@asdesc,@sort


		END

		CLOSE sort_cursor
		DEALLOCATE sort_cursor
		IF @counter >1 -- only do if some sorting was returned... 
			SET @statement = @statement + @groupby + @sortorder 

		EXEC sp_executesql @statement
		--PRINT @statement

		--#12860
		update #acctstoload
		set dateadded = isnull(b.dateadded, '1/1/1900')
		from [bailactionMaxAction] b
		where #acctstoload.acctno = b.acctno
	
  --SELECT * FROM INFORMATION_SCHEMA.columns WHERE column_name = 'sortorder'
		--SELECT * FROM CMWorkListSortOrder
	END 
	BEGIN
			--IP - 02/02/10 - UAT(971)
	    	INSERT INTO #tname
		    SELECT	 WLA.Worklist, A.AcctNo, A.AcctType, A.DateAcctOpen, A.AgrmtTotal,
		    	0, 0, A.DateLastPaid,isnull(A.OutstBal,0), 
			isnull(A.Arrears,0),0,0,A.HighstStatus, A.CurrStatus, 
			'','', '',
			'', '', '','', '', convert(datetime, null), 
			'', '', '','','','', '',WLA.[Datefrom],'01/01/1900', '',t.dateadded,
			paidpcent , '01/01/1900', 0,0 --IP - 28/04/10 - UAT(983) - Return CreditBlocked 
            FROM ACCT A, #acctstoload t
				INNER JOIN CMWorkListsAcct WLA ON  WLA.acctno= t.acctno 
                WHERE a.acctno = t.acctno AND wla.Dateto IS NULL 
			UPDATE #tname SET custid  = ca.custid
			FROM custacct ca WHERE ca.acctno= #tname.acctno AND ca.hldorjnt= 'H'
			
			UPDATE t SET title = CU.Title,
			NAME = CU.NAME,
			firstname = CU.Firstname,
			CreditBlocked = CU.CreditBlocked	--IP - 28/04/10 - UAT(983) UAT5.2 		
			FROM #tname t, customer cu 
			WHERE t.custid = cu.custid 

    END
		
	UPDATE #tname  
    SET Provision = vp.provision
    FROM view_provision vp
    WHERE #tname.acctno = vp.acctno

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
	DECLARE @tname6 TABLE (custid VARCHAR(20) NOT NULL PRIMARY KEY,
	addtype VARCHAR(3) NOT NULL)
	
	INSERT INTO @tname6 (
		custid,
		addtype
	) 
	select	c.custid, 
		min(c.addtype) 
	--into	#tname6 
	from 	custaddress c, #tname t
	where 	c.custid = t.custid
	and 	c.addtype != ''
	and 	c.custid != '' 
	AND		c.datemoved IS NULL 
	group by c.custid
	
	IF (@@error != 0)
	BEGIN
		drop TABLE #tname;
		drop TABLE #tname2;
	END
	
	update	#tname
	set 	HomeTel =  telno, homedialcode = rtrim(dialcode) 
	from 	custtel c
	where 	#tname.custid = c.custid
	and	tellocn = 'H'
	and 	datediscon is null

	IF (@@error != 0)
	BEGIN
		drop TABLE #tname;
		drop TABLE #tname2; 
	END

	update #tname
	set WorkTel = telno, workdialcode = rtrim(dialcode),
	      Ext = isnull(extnno,' ')	
	from custtel c
	where 	#tname.custid = c.custid
	and	tellocn = 'W'
	and 	datediscon is null

    IF (@@error != 0)
	BEGIN
		drop TABLE #tname;
		drop TABLE #tname2;
 
	END

	update #tname
	set mobileno = telno, mobiledialcode = rtrim(dialcode)
	from custtel c
	where 	#tname.custid = c.custid
	and	tellocn = 'M'
	and 	datediscon is NULL

	IF (@@error != 0)
	BEGIN
		drop TABLE #tname;
		drop TABLE #tname2;
 
	END

    -- Add columns for the last Bailiff Action added-- don't need as already populated'
    /*UPDATE #tname
    SET    DateAdded = ISNULL((SELECT MAX(ba.DateAdded)
                               FROM   BailAction ba
                               WHERE  ba.AcctNo = #tname.AcctNo),'')*/

    -- Use MAX(Code) just in case DateAdded can duplicate
    UPDATE #tname
    SET    ActionCode = ISNULL((SELECT MAX(ba.RecentCode + ' : ' + c.CodeDescript)
                                FROM   bailactionMaxAction ba, Code c
                                WHERE  ba.AcctNo = #tname.AcctNo
                                --AND    ba.DateAdded = #tname.DateAdded
                                AND    c.Category = 'FUA'		-- UAT781 jec 16/11/09
                                AND    c.Code = ba.RecentCode),'')
                         
    
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

	--Update the Customers addresses
	UPDATE #tname
	SET custaddress1 = CAD.cusaddr1,
		custaddress2 = CAD.cusaddr2,
		custaddress3 = CAD.cusaddr3,
		cuspocode = CAD.cuspocode
	FROM #tname t INNER JOIN  Custaddress CAD 
	ON CAD.custid = t.custid
	AND CAD.addtype = 'H'
	AND CAD.datemoved is null
    
     --IP - 02/02/10 - UAT(971) Update DateAlloc
    UPDATE #tname
    SET DateAlloc = ISNULL((SELECT f.datealloc FROM follupalloc f
						WHERE f.acctno = t.acctno
						AND f.datedealloc IS NULL
						AND f.datealloc IS NOT NULL), '01/01/1900')
	FROM #tname t

	--Now need to update #tname with ReminderDateTime
	declare reminder_cursor cursor for  
			select AcctNo 
			from #tname
			for update of ReminderDateTime

	open reminder_cursor
	declare @AcctNo varchar(12)
	fetch next from reminder_cursor into @AcctNo

	while @@fetch_status = 0 
	begin

		update #tname
		set ReminderDateTime = isnull((select Max(CMR.ReminderDateTime) from CMReminder CMR
							   where CMR.AcctNo = @AcctNo 
									 and CMR.ReminderDateTime < getDate() and CMR.Status = 'N'), '1/1/1900')

		where current of reminder_cursor

		fetch next from reminder_cursor into @AcctNo

	end

	close reminder_cursor
	deallocate reminder_cursor

	---------------------------------------------------
	
	--IP - 06/01/2010 - UAT(951)
	--IP - 28/04/10 - UAT(983) UAT5.2 - Return CreditBlocked
	-- 26/08/10 jec  CR1084 return Referral Reason (join on acctno and custid as there are duplicate proposals with diff custid's' 
	SET @statement = 'select
		T.Worklist,	
		T.AcctNo,
		T.AcctType,
        T.DateFrom,
        T.DateAlloc,
		T.DateAcctOpen,
		T.AgrmtTotal,
        T.instalamount,
        T.DueDay,
		T.OutstBal,
		T.Arrears,
		T.MonthsInArrears,
		T.DateLastPaid,
		T.HighstStatus,
		T.CurrStatus,
		T.Title,
		T.Name,
		T.Firstname,
		T.Custaddress1 as Address1,
		T.Custaddress2 as Address2,
		T.Custaddress3 as Address3,
		T.Cuspocode as AddressPC, 
		T.Custid,
		T.HomeTel,
		T.HomeDialCode,
		T.WorkTel,
		T.WorkDialCode,
        T.mobileno,
		T.MobileDialCode,
		T.Ext,
        T.ActionCode,
        T.DateAdded,
        T.paidpcent,
		T.ReminderDateTime,
		A.BranchNo,
		T.CreditBlocked,
		case when p.reason='''' then '''' else isnull(p.reason + '':'' + c.codedescript,'''') end as ReferralReason,
		case when p.reason2='''' then '''' else isnull(p.reason2 + '':'' + c2.codedescript,'''') end as ReferralReason2,
		case when p.reason3='''' then '''' else isnull(p.reason3 + '':'' + c3.codedescript,'''') end as ReferralReason3,
		case when p.reason4='''' then '''' else isnull(p.reason4 + '':'' + c4.codedescript,'''') end as ReferralReason4,
		case when p.reason5='''' then '''' else isnull(p.reason5 + '':'' + c5.codedescript,'''') end as ReferralReason5,
		case when p.reason6='''' then '''' else isnull(p.reason6 + '':'' + c6.codedescript,'''') end as ReferralReason6,
		p.dateprop,
		CASE WHEN T.Provision < 0 THEN 0 ELSE T.Provision END AS ProvisionAmount
	from	#tname T 
	inner join Acct A on A.AcctNo = T.AcctNo
	inner join proposal p on p.AcctNo = T.AcctNo and p.custid=T.custid
	left outer join code c on p.reason=c.code and c.category=''SN1''
	left outer join code c2 on p.reason2=c2.code and c2.category=''SN1''
	left outer join code c3 on p.reason3=c3.code and c3.category=''SN1''
	left outer join code c4 on p.reason4=c4.code and c4.category=''SN1''
	left outer join code c5 on p.reason5=c5.code and c5.category=''SN1''
	left outer join code c6 on p.reason6=c6.code and c6.category=''SN1''	
	order by T.ReminderDateTime desc'
	
	--IP - 06/01/2010 - UAT(951) - If changes are made to the alias's above, please change the below.
	IF @counter >1 -- only do if some sorting was returned... 
	BEGIN
		SET @sortorder = REPLACE(@sortorder, 'a.', 'T.')
		SET @sortorder = REPLACE(@sortorder, 'ip.', 'T.')
		SET @sortorder = REPLACE(@sortorder, 'WLR.', 'T.')
		SET @sortorder = REPLACE(@sortorder, 'PP.', 'T.')
		SET @statement  =@statement + REPLACE(@sortorder, 'ORDER BY ', ',')
		
	END
	
	--PRINT @statement
	EXEC sp_executesql @statement 
	
	--NM & IP - 23/12/2008 - Select the worklists
	--from the list of accounts
	--as this will be used to populate the worklist drop down on the Telephone Callers screen.
	-- UAT936 jec 16/12/09 return worklist requested at top of list if Supervisor
	if @Supervisor=1 and @worklist!=''
	BEGIN
		declare @worklisttab TABLE (worklist VARCHAR(80),id INT IDENTITY primary KEY )
		insert into @worklisttab 
		select c.worklist + ': ' + c.Description
		from cmworklistrights r, CMWorkList c
		where empeeno = @user and @worklist=r.WorkList
		AND c.WorkList = r.WorkList
		
		
		insert into @worklisttab 
		select c.worklist + ': ' + c.Description
		from cmworklistrights r, CMWorkList c
		where empeeno = @user and @worklist!=c.worklist
		AND c.WorkList = r.WorkList
		select worklist  from @worklisttab
	END
	else
	BEGIN
		select c.worklist  + ': ' + c.Description AS Worklist
		from cmworklistrights r, CMWorkList c
		where empeeno = @user AND c.WorkList = r.WorkList
	END

	drop TABLE #tname
	drop TABLE #tname2
 
GO 

-- End End End End End End End End End End End End End End End End End End End End End End End End
