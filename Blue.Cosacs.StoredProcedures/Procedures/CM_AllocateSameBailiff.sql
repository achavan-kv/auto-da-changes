
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE name = 'CM_AllocateSameBailiff'
			AND xtype = 'P')
DROP PROCEDURE CM_AllocateSameBailiff
GO
CREATE PROCEDURE [dbo].[CM_AllocateSameBailiff] @rundate DATETIME						--IP - 13/02/12

-- **********************************************************************
-- Version: 002
-- Title: CM_AllocateSameBailiff.sql
-- Developer: Stephen Chong
-- Date: September 2010
-- Purpose: To allocate same Bailiff to an account if has been successful previously

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 20/10/10 jec UAT86 correct error in code
-- 23/03/11 ip  #3338 - LW73078 - Use empeeno (-114) to indicate auto bailiff allocation. 
--						When assigning accounts to bailiffs with previous success with customers, do not 
--					    assign accounts that have been manually allocated/reallocated in the past two weeks.
-- 13/02/12 ip  Replace GETDATE() with @rundate
-- 08/08/19   Zensar Strategy Job Optimization : Optimised the stored procedure for performance by putting Nolock and Replacing * with 1 in all exist
-- **********************************************************************

AS
BEGIN


	PRINT 'Allocating same Bailiff'
	SET NOCOUNT ON 
	DECLARE @mindate DATETIME, 
			@maxPTP INT
	
	--SET @mindate = (SELECT DATEADD(week,value * -1,GETDATE()) 
	SET @mindate = (SELECT DATEADD(week,value * -1,@rundate)					--IP - 13/02/12 - use @rundate
					FROM CountryMaintenance WITH(NOLOCK)
					WHERE codename = 'BailiffAutoWeeks')
					
	SET @maxPTP = (SELECT VALUE
				   FROM CountryMaintenance WITH(NOLOCK)
				   WHERE codename = 'PTPBailiffAssign' )

	PRINT 'will be getting Bailiffs where successful from ' + CONVERT(VARCHAR,@mindate)
	
	CREATE TABLE #Paywhenallocated --AS TABLE
	(
		acctno CHAR(12),
		empeeno INT,
		id INT IDENTITY(1,1),
		dateallocated datetime,
		allocno int 
	)
	
	create clustered index ixpaywhenallocatedhash on  #Paywhenallocated (acctno)

	DECLARE @unassign AS TABLE
	(
		acctno VARCHAR(12),
		empeeno INT,
		custid VARCHAR(20),
		failedptpcount int
	)
	
	-- Find out all bailiff that cross PTP threshold. These have ptps where datedue > max datetrans... 

	INSERT INTO @unassign ( acctno, empeeno ,custid ,failedptpcount)
	SELECT f.acctno,f.empeeno, c.custid ,COUNT(f.acctno)
	FROM bailactionPTP BAP WITH(NOLOCK),follupalloc f  WITH(NOLOCK) -- only for current allocations
	,custacct c WITH(NOLOCK)
	WHERE BAP.datedue > (SELECT MAX(datetrans)
	                       FROM fintrans f WITH(NOLOCK)

	                       WHERE f.transtypecode  = 'pay'		-- UAT86 jec 20/10/10
	                       AND f.acctno = BAP.acctno
						   )

    AND f.acctno= BAP.acctno AND f.datedealloc IS NULL
    AND f.empeeno= bap.empeeno
    AND c.acctno = f.acctno AND c.hldorjnt = 'H'
	GROUP BY f.acctno, f.Empeeno,c.custid
	

	INSERT INTO @unassign ( acctno, empeeno ,custid ,failedptpcount)
	SELECT f.acctno ,f.empeeno ,c.custid , COUNT(f.acctno) 
	FROM bailactionPTP BAP WITH(NOLOCK),follupalloc f WITH(NOLOCK)
	,custacct c WITH(NOLOCK), acct a WITH(NOLOCK)

	WHERE BAP.datedue > (SELECT  MAX(datetrans)
	                       FROM fintrans f WITH(NOLOCK)

	                       WHERE f.transtypecode  = 'pay'		-- UAT86 jec 20/10/10
	                       AND f.acctno = BAP.acctno
						  )

    AND f.acctno= BAP.acctno 
    AND f.datedealloc >= bap.dateadded  -- these accounts would be deallocated as account goes into Ptp Strategy. 
    AND f.datealloc < bap.dateadded  
    AND f.empeeno= bap.empeeno
    AND a.acctno= f.acctno AND a.outstbal >0 
    AND c.acctno = f.acctno AND c.hldorjnt = 'H'
    AND NOT EXISTS -- exclude where already allocated to another bailiff 
		(SELECT 1 FROM dbo.follupalloc fu WITH(NOLOCK) WHERE fu.acctno= f.acctno
		AND f.empeeno !=fu.empeeno AND fu.datedealloc IS NULL )
	--AND f.datealloc > DATEADD(MONTH,-6, GETDATE()) -- only recently deallocated 	
	AND f.datealloc > DATEADD(MONTH,-6, @rundate) -- only recently deallocated 				--IP - 13/02/12 - use @rundate
	GROUP BY f.acctno, f.Empeeno, c.custid
	
	SELECT custid,SUM(failedPTPcount) AS FailedPTPCount
	INTO #unassigncount FROM @unassign GROUP BY custid 


	
	DELETE u FROM @unassign u
	WHERE NOT EXISTS (SELECT 1 FROM #unassigncount c WITH(NOLOCK)
	WHERE c.custid = u.custid AND c.FailedPTPCount >= @maxptp)
	
		
    PRINT CONVERT(VARCHAR,@@ROWCOUNT) + ' accounts to unassign'
	-- Unassign all acctnos on customer level that cross threshold.
	UPDATE follupalloc
	--SET datedealloc = GETDATE(), empeenodealloc = -114	--IP - 23/03/11 - #3338 - LW73078 - Use empeeno -114
	SET datedealloc = @rundate, empeenodealloc = -114	--IP - 13/02/12 - use @rundate --IP - 23/03/11 - #3338 - LW73078 - Use empeeno -114
	FROM @unassign U
	INNER JOIN custacct CA WITH(NOLOCK) ON U.acctno = CA.acctno
	INNER JOIN custacct CA2 WITH(NOLOCK) ON CA.custid = CA2.custid
	WHERE follupalloc.acctno = CA2.acctno
	AND follupalloc.empeenoalloc = U.empeeno
	AND CA.hldorjnt = 'H' AND ca2.hldorjnt='H'                       
	AND datedealloc IS NULL 
	
	-- Assign all outstanding to the same bailiff with a PTP on the account .

	INSERT INTO #Paywhenallocated (acctno, empeeno, dateallocated,allocno)
	SELECT DISTINCT w.acctno, MAX(BAP.empeeno) AS empeeno, bap.dateadded, MAX(fup.allocno)
	FROM CMWorkListsAcct W WITH(NOLOCK)
	INNER JOIN bailactionPTP BAP WITH(NOLOCK) ON BAP.acctno = w.acctno
	LEFT OUTER JOIN follupalloc FUP WITH(NOLOCK) ON FUP.acctno = w.acctno
	WHERE BAP.dateadded = (SELECT MAX(dateadded)
	                         FROM bailactionPTP BAP2  WITH(NOLOCK)
	                         WHERE BAP2.acctno = w.acctno
							) -- Get latest PTP.


	AND BAP.dateadded > (SELECT MAX(dateadded)
						 FROM bailaction BA  WITH(NOLOCK)
						 WHERE BA.acctno = BAP.acctno AND ba.code !='PTP'
						) -- Latest action on account is PTP
	AND NOT EXISTS (SELECT 1
				    FROM @unassign U
				    WHERE U.acctno = w.acctno)  -- Not unassigned due to PTP threshold.
	AND EXISTS (SELECT 1 
				FROM CMBailiffAllocationRules AR  WITH(NOLOCK)
				WHERE AR.EmpeeNo = EmpeeNo) -- Bailiff or collectors that can be allocated accounts.
	AND (FUP.datedealloc IS NOT NULL 
		 OR (FUP.datedealloc IS NULL AND FUP.empeeno IS NULL)) -- Account not in FUP or is in with a datedealloc.
	AND w.Dateto IS NULL
	AND EXISTS (SELECT 1 
				FROM CMWorkListRights WLR  WITH(NOLOCK)
				WHERE WLR.WorkList = w.Worklist
				AND WLR.EmpeeNo = BAP.Empeeno) -- Check work list and empeeno.
	GROUP BY w.acctno,bap.dateadded
	--PRINT CONVERT(VARCHAR,@@ROWCOUNT) + ' Accounts Assigned to same Bailiff for same customer'
	
-- Assign accounts to bailiffs with previous success with customer. Firstly those with payments whilst they were allocated
	INSERT INTO #Paywhenallocated
	(acctno, empeeno,dateallocated,allocno)
	SELECT DISTINCT w.acctno, MAX(fup.empeeno) AS empeeno,fup.datealloc,max(fup.allocno)
	FROM CMWorkListsAcct W WITH(NOLOCK)
	INNER JOIN custacct ca  WITH(NOLOCK) ON ca.acctno= W.acctno 
	INNER JOIN custacct ca2  WITH(NOLOCK) ON ca.custid = ca2.custid
	INNER JOIN fintrans BA  WITH(NOLOCK) ON ca2.acctno = BA.acctno
	JOIN follupalloc FUP  WITH(NOLOCK) ON FUP.acctno = BA.acctno
	WHERE ca2.hldorjnt  ='H' AND ca.hldorjnt= 'H'
	AND ba.datetrans= (SELECT  MAX(f.datetrans)
					 FROM fintrans f WITH(NOLOCK)
					 WHERE  f.transtypecode  ='pay' 
					 and f.acctno= ba.acctno
					 AND f.datetrans > @mindate
					 AND f.datetrans > fup.datealloc
					 AND (f.datetrans < fup.datedealloc OR fup.datedealloc IS NULL
					 ) )
	AND transvalue < 0 -- credit payment
	AND transtypecode  = 'pay'
	AND w.dateto IS NULL 
	AND NOT EXISTS (SELECT 1
					FROM #Paywhenallocated BA
					WHERE BA.acctno = W.acctno)
	AND NOT EXISTS (SELECT 1
				    FROM @unassign U
				    WHERE U.acctno = w.acctno AND u.empeeno =fup.empeeno)  -- Not unassigned due to PTP threshold.
	AND EXISTS (SELECT 1 
				FROM CMWorkListRights WLR WITH(NOLOCK)
				WHERE WLR.WorkList = w.Worklist
				AND WLR.EmpeeNo = fup.Empeeno)
	AND EXISTS (SELECT 1 
				FROM CMBailiffAllocationRules AR  WITH(NOLOCK)
				WHERE AR.EmpeeNo = EmpeeNo)
	AND NOT EXISTS(select 1 from follupalloc f	 WITH(NOLOCK)			--IP - #3338 - LW73078 - Not allocated manually in the past two weeks
					where f.acctno = fup.acctno
					--and f.datealloc > dateadd(week,-2, getdate())
					and f.datealloc > dateadd(week,-2, @rundate)	--IP - 13/02/12 - use @rundate
					and f.empeenoalloc!=-114)
	GROUP BY w.acctno,fup.datealloc
	--PRINT CONVERT(VARCHAR,@@ROWCOUNT) + ' Accounts Assigned where payment previously made with bailiff'
	
	--SELECT * FROM #Paywhenallocated
-- Next insert those with previous success after PTP - -these would have been deallocated but payment within PTP timeframe
	INSERT INTO #Paywhenallocated
	(acctno, empeeno,dateallocated,allocno)
	SELECT DISTINCT w.acctno , MAX(b.empeeno) AS empeeno,b.dateadded, max(b.allocno)
	FROM CMWorkListsAcct W  WITH(NOLOCK) 
	INNER JOIN custacct ca  WITH(NOLOCK) ON ca.acctno= W.acctno 
	INNER JOIN custacct ca2  WITH(NOLOCK) ON ca.custid = ca2.custid
	INNER JOIN fintrans f  WITH(NOLOCK) ON ca2.acctno = f.acctno
	JOIN Bailaction B ON B.acctno = f.acctno
	WHERE ca2.hldorjnt  ='H' AND ca.hldorjnt='H' 
	AND f.datetrans= (SELECT MAX(fa.datetrans)
					 FROM fintrans fa WITH(NOLOCK)
					 WHERE f.acctno= fa.acctno and
					 fa.transtypecode  ='pay'
					 AND fa.datetrans > @mindate
					 AND fa.datetrans > b.dateadded
					 AND (fa.datetrans < DATEADD(DAY,1,b.datedue)
					 )  )
	AND transvalue < 0 -- credit payment
	AND transtypecode  = 'pay'
	AND w.dateto IS NULL 
	AND NOT EXISTS (SELECT 1
					FROM #Paywhenallocated BA
					WHERE BA.acctno = W.acctno)
	AND NOT EXISTS (SELECT empeeno--exclude those about to be unassigned
							   FROM @unassign U
							   WHERE W.acctno = U.acctno AND u.empeeno=b.empeeno) 
	AND EXISTS (SELECT 1 
				FROM CMWorkListRights WLR WITH(NOLOCK)
				WHERE WLR.WorkList = w.Worklist
				AND WLR.EmpeeNo = B.Empeeno)
	AND EXISTS (SELECT 1 
				FROM CMBailiffAllocationRules AR WITH(NOLOCK)
				WHERE AR.EmpeeNo = EmpeeNo)
	GROUP BY w.acctno, b.dateadded
	--PRINT CONVERT(VARCHAR,@@ROWCOUNT) + ' Accounts Assigned where payment previously made with bailiff after PTP'
	
	--SELECT * FROM #Paywhenallocated
	

	DELETE  
	FROM #Paywhenallocated 
	WHERE EXISTS (SELECT 1 FROM #Paywhenallocated p WITH(NOLOCK)
	WHERE #Paywhenallocated.acctno= p.acctno
	AND p.dateallocated > #Paywhenallocated.dateallocated)
	OR EXISTS (SELECT 1 FROM follupalloc f WITH(NOLOCK)
					WHERE f.acctno=  #Paywhenallocated.acctno AND f.datedealloc IS null)

	
	--PRINT CONVERT(VARCHAR,@@ROWCOUNT) + ' duplicates removed'
	


	--DECLARE @maxalloc INT
	
	--SELECT @maxalloc = ISNULL(MAX(allocno),1) 
	--FROM follupalloc  Allocation number should be per account not for the whole country.
	
	INSERT INTO follupalloc 
	(
		origbr,		acctno,		allocno,
		empeeno,		datealloc,		datedealloc,
		allocarrears,		bailfee,		allocprtflag,
		empeenoalloc,		empeenodealloc, worklist
	) 
	SELECT DISTINCT 0, BA.acctno,ISNULL(MAX(fn.allocno),0) + 1 , 
	CONVERT(VARCHAR,BA.empeeno) , NULL , NULL, 
	a.arrears, 0, 'N', -114, 0, W.Worklist			--IP - 23/03/11 - #3338 - LW73078 - Use empeeno -114
	FROM  #Paywhenallocated AS BA WITH(NOLOCK)
	INNER JOIN CMWorkListsAcct W WITH(NOLOCK) ON BA.acctno = W.acctno
	INNER JOIN acct a WITH(NOLOCK) ON W.acctno = a.acctno
	LEFT JOIN follupalloc fn WITH(NOLOCK) ON a.acctno = fn.acctno
	WHERE Dateto IS NULL 
	AND admin.CheckPermission(ba.empeeno,381) = 1
	GROUP BY BA.acctno, BA.empeeno,BA.id,a.arrears,w.Worklist 
	
	UPDATE dbo.courtsperson SET alloccount = (SELECT COUNT(*) FROM dbo.follupalloc f  WITH(NOLOCK)
												WHERE f.empeeno = courtsperson.userid AND f.datedealloc IS NULL)
		
	Declare @employeeno int,
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
			@remDateTime datetime, 
			@deleteAllReminders bit,
			@callingSource varchar(10),
			@return INT, @allocno INT, 
			@acctno VARCHAR(12)
	--set @dateadded=GetDate()
	set @dateadded=@rundate						--IP - 13/02/12 - use @rundate
	set @code='ALL'
	set @notes=null
	set @datedue=null
	set @actionvalue=0
	set @amtcommpaidon=0
	set @addedby=0
	set @spadateexpiry=null
	set @spareasoncode=null 
	set @spainstal=0
	set @remDateTime=null 
	set @deleteAllReminders=0
	set @callingSource='AutoZoBr'
	
	DECLARE set_cursor CURSOR 
  	FOR 
  	SELECT acctno, empeeno ,allocno 
    FROM #Paywhenallocated
   
   OPEN set_cursor
   FETCH NEXT FROM set_cursor INTO @acctno, @employeeno,@allocno

   WHILE (@@fetch_status <> -1)
   BEGIN
	   IF (@@fetch_status <> -2)
   		BEGIN
   		
			EXEC DN_BailActionSaveSP 
				@acctno = @acctno,
				@spadateexpiry = @spadateexpiry, 
				@allocno = @allocno, 
				@employeeno= @employeeno,
				@dateadded = @dateadded,
				@spareasoncode = @spareasoncode , --  varchar(4)
				@spainstal = @spainstal , --  float
				@remDateTime = @remDateTime , --  datetime
				@deleteAllReminders =@deleteAllReminders, --  bit
				@callingSource = @callingSource,
				@code = @code,
				@actionvalue = @actionvalue,
				@datedue = @datedue,
				@amtcommpaidon = @amtcommpaidon,
				@notes = @notes,
				@addedby = @addedby,@return =@return

	   END
    FETCH NEXT FROM set_cursor
	INTO @acctno, @employeeno, @allocno

   END

   CLOSE set_cursor
   DEALLOCATE set_cursor

   
END

GO
