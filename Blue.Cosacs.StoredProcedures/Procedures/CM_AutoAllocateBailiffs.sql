
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_AutoAllocateBailiffs]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_AutoAllocateBailiffs]
GO


-- ============================================================================================
--Version:		002
-- Author:		Ilyas Parker
-- Create date: 13/05/2009
-- Description:	Procedure that loads up workslists and accounts
-- for automatic allocation calls the CM_AutoBailiffAllocation	
-- routine. Called by CMEod_Strategies
-- 13/11/2009 AA - deallocating where reallocate set to 1 for Zone Allocation
-- 20/01/2010 IP - UAT(955) - Accounts were previously incorrectly deallocated for a branch. 
--				   If the 'Reallocate' checkbox has been checked for a Bailiff/Collector for a branch/zone then these accounts
--				   will be deallocated and accounts from this branch/zone will not be allocated.
-- 23/04/10   IP - UAT(1008) - UAT5.2 Internal - Settled accounts currently allocated should be de-allocated.
-- 16/07/10  jec - UAT967 Account not being deallocated when transfered to new worklist
-- 26/07/10   IP - UAT(967) - Accounts were not being re-allocated to the same Bailiff when worklist changed. Needed to include accounts when selecting count
--							of accounts to be deallocated into the @AcctsToDeallocate table. (Code needed to be changed for fix made on 16/07/10)
-- 21/12/10 jec - LW73080 Do not auto deallocate Settled accounts with BDWbalance/BDWcharges!=0 
-- 23/03/11 IP  - #3338 - LW73078 - Use empeeno (-114) for Auto Bailiff Allocation (Merged from 5.4.3)
-- 14/09/11 IP  - #3726 - LW73405 - Deallocate accounts that are in strategies that cannot have accounts allocated, or accounts that do not exist in a strategy
-- 13/02/12 IP  Replace GETDATE() with @rundate
-- 19/03/12 IP  - #9804 - Include Store Card Strategies SCCOL and SCBAI to allocate accounts that are in these strategies.
-- 23/03/12 IP  - #9804 - Previously strategies that can have accounts allocated were hardcoded as (BAI, COL). Changed to look at Category SS1 Reference column
--						  which should be set to 1 if users wish to have accounts allocated for these strategies.
-- 0808/19 Zensar  Optimised the stored procedure for performance by  replacing * with 1 in all exist checks, 
--			max with order by and used NoLock hints.
-- ============================================================================================
CREATE PROCEDURE [dbo].[CM_AutoAllocateBailiffs] 

				@rundate DATETIME,									--IP - 13/02/12 
				@return	int	OUTPUT
as
begin
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	set nocount on;

    set @return = 0    --initialise return code

	declare @worklist varchar(10),
			@rowCount int,
			@AcctsToDealloc int, --IP - 20/01/10 - UAT(955)
			@loop INT,
			@testing SMALLINT 
			
	SET @testing = 1
	
	--Select worklists for the 'Bailiff' and 'Collector' strategies
	--Pass the worklists into procedure 'CM_AutoBailiffAllocation' 
	--Auto allocate accounts in these worklists
	CREATE TABLE #levels (LEVEL VARCHAR(2), Orderby INT )
	INSERT INTO #levels VALUES ('1', 1 )-- 'LEVEL 1 Bailiffs/Collectors GET max first
	INSERT INTO #levels VALUES ('2a',2) -- LEVEL 2 Bailiffs/Collectors GET minimum 
	INSERT INTO #levels VALUES ('3a',3)-- LEVEL 3 Bailiffs/Collectors GET minimum 
	INSERT INTO #levels VALUES ('2b',4)-- LEVEL 2 Bailiffs/Collectors GET max
	INSERT INTO #levels VALUES ('3b',5)-- LEVEL 3 Bailiffs/Collectors GET max
	INSERT INTO #levels VALUES ('4',6)-- LEVEL 4 Bailiffs/Collectors GET max
	INSERT INTO #levels VALUES ('5',7)-- LEVEL 5 Bailiffs/Collectors GET max

	
	if not exists (SELECT * FROM tempdb.sys.objects 
	WHERE NAME LIKE '%##AcctsToAllocate%')
	CREATE TABLE ##AcctsToAllocate
	(
		acctno char(12),
		worklist varchar(10),
		allocated bit, 
		AllocID integer identity 
	)
	--alter table ##AcctsToAllocate add AllocID integer identity(1,1)  not null
	truncate table ##AcctsToAllocate

	create clustered index ix_hashacctsallocate on  ##AcctsToAllocate(acctno)
	--IP - 19/01/10 - UAT(955) - Table to hold the accounts that are to be deallocated.
	DECLARE @AcctsToDeallocate TABLE
	(
		acctno VARCHAR(12),
		empeeno int
	)
	
	select cw.worklist,l.[level],l.Orderby
	into #worklists 
	from #levels l, cmstrategy cs WITH(NOLOCK) inner join cmstrategycondition csc WITH(NOLOCK)--Added for Strategy Job Optimization by Zensar(Suvidha)
					 on cs.strategy = csc.strategy 
					 inner join cmworklist cw WITH(NOLOCK)--Added for Strategy Job Optimization by Zensar(Suvidha)
					 on cw.worklist = substring(csc.actioncode, 0, charindex(' ',csc.actioncode))
					 inner join code c WITH(NOLOCK) on c.code = cs.strategy-- Added for Strategy Job Optimization by Zensar(Suvidha)					--IP - 23/03/12 - #9808
	where c.category = 'SS1'													--IP - 23/03/12 - #9808
	and c.reference > 0															--IP - 23/03/12 - #9808	-- only select strategies that can have accounts allocated.
	--where cs.strategy in ('COL', 'BAI')
    --where cs.strategy in ('COL', 'BAI', 'SCCOL', 'SCBAI')						--IP - 19/03/12 - #9804 - Include Store Card strategies			
	--and cw.worklist = 'WCO1' --testing only
	order by cw.worklist,l.Orderby 
	--select the rowcount
	set @rowCount = @@rowcount

	--Start Zensar(SH) Commented the following print since it is not used
	/* IF @testing = 1
		PRINT 'Number of Worklists '+ CONVERT(VARCHAR,@rowCount) */
	--End Zensar(SH) Commented the following print since it is not used
		
	------------------------------------------------------------------------------------------------------------------------------
	--IP - 20/01/10 - Select accounts that are to be deallocated into a temporary table. This will help in determining the number of
	--accounts that are being deallocated by employee which will help in updating the Courtsperson.alloccount by the number of accounts
	--being deallocated.
	
	--INSERT INTO @AcctsToDeallocate
	--SELECT DISTINCT f.acctno, f.empeeno
	--FROM follupalloc f
	--WHERE datedealloc IS NULL  
	--AND NOT EXISTS(SELECT * FROM  #worklists w 
	--				WHERE w.WorkList != ISNULL(f.worklist, '') )
	----AND w.worklist !=ISNULL(f.worklist, '') 

	--IP - 26/07/10 - UAT(967) UAT5.2	
	INSERT INTO @AcctsToDeallocate
	SELECT DISTINCT f.acctno, f.empeeno
	-- UAT967 jec 16/07/10 -- select correct accounts (previous code didn't select any)
	FROM follupalloc f WITH(NOLOCK) INNER JOIN CMWorklistsAcct wa WITH(NOLOCK) on f.WorkList=wa.Worklist and f.acctno=wa.acctno--Added for Strategy Job Optimization by Zensar(Suvidha)			
	WHERE datedealloc IS NULL 
		and ISNULL(wa.dateto,'1900-01-01')>datealloc
		AND EXISTS(SELECT 1 FROM  #worklists w WITH(NOLOCK) WHERE w.WorkList = wa.Worklist )--Added for Strategy Job Optimization by Zensar(Suvidha)
	
	INSERT INTO @AcctsToDeallocate
	SELECT f.acctno, f.empeeno
	FROM follupalloc f WITH(NOLOCK)
	WHERE f.datedealloc IS NULL AND EXISTS (SELECT 1 FROM CMBailiffAllocationRules C WITH(NOLOCK) WHERE c.reallocate = 1 AND c.empeeno= f.empeeno --Added for Strategy Job Optimization by Zensar(Suvidha)
	AND c.IsZone = 0 AND f.acctno LIKE c.branchorzone + '%' ) 
	

	INSERT INTO @AcctsToDeallocate
	SELECT f.acctno, f.empeeno  
	FROM follupalloc f WITH(NOLOCK)
	WHERE f.datedealloc IS NULL AND EXISTS (
	SELECT 1 FROM CMBailiffAllocationRules r WITH(NOLOCK) JOIN  --Added for Strategy Job Optimization by Zensar(Suvidha)
	custaddress ca WITH(NOLOCK) ON ca.zone = r.BranchorZone
	JOIN custacct c WITH(NOLOCK) ON ca.custid = c.custid
	WHERE r.reallocate = 1 AND r.empeeno= f.empeeno 
	 AND c.hldorjnt = 'H' AND f.acctno = c.acctno 
	 AND ca.addtype= 'H' -- home address 
	 AND ca.datemoved IS NULL -- current address
	 AND r.IsZone = 1 ) 

	--IP - 23/04/10 - UAT(1008) UAT5.2 Internal
	INSERT INTO @AcctsToDeallocate
	SELECT f.acctno, f.empeeno
	FROM follupalloc f WITH(NOLOCK) INNER JOIN acct a WITH(NOLOCK)--Added for Strategy Job Optimization by Zensar(Suvidha)
	ON f.acctno = a.acctno
	WHERE f.datedealloc IS NULL 
	AND a.currstatus = 'S'
	and bdwbalance=0 and bdwcharges=0		--LW73080 21/12/10
	
	--IP - 14/09/11 - #3726 - LW73405 - Deallocate accounts that are in strategies that cannot have accounts allocated, or accounts that do not exist in a strategy
	INSERT INTO @AcctsToDeallocate
	SELECT f.acctno, f.empeeno
	FROM follupalloc f WITH(NOLOCK) LEFT JOIN cmstrategyacct cma WITH(NOLOCK) on f.acctno = cma.acctno--Added for Strategy Job Optimization by Zensar(Suvidha)
	LEFT JOIN code c WITH(NOLOCK) on cma.strategy = c.code and c.category = 'SS1'--Added for Strategy Job Optimization by Zensar(Suvidha)
	WHERE f.datedealloc is NULL
	AND (cma.dateto is null and c.reference!=1
	OR NOT EXISTS(select 1 from cmstrategyacct cma2 WITH(NOLOCK)--Added for Strategy Job Optimization by Zensar(Suvidha)
					where cma2.acctno = f.acctno)) 
	AND NOT EXISTS (select 1 from @AcctsToDeallocate a  --Added for Strategy Job Optimization by Zensar(Suvidha)
						where a.acctno = f.acctno)
	
	SELECT @AcctsToDealloc = (SELECT COUNT(a.acctno) FROM @AcctsToDeallocate a)--Changed for Strategy Job Optimization by Zensar(Suvidha)

	---------------------------------------------------------------------------------------------------------------------------------------

	-- update the follupalloc table to set datedellocated where in different worklist
	--UPDATE f SET datedealloc = GETDATE(),
	UPDATE f SET datedealloc = @rundate,															--IP - 13/02/12 - use @rundate
				 empeenodealloc = '-114' --IP - 23/03/11 - #3338 - LW73078 - Use empeeno -114
	-- UAT967 jec 16/07/10 -- select correct accounts (previous code didn't select any)
	FROM follupalloc f WITH(NOLOCK) INNER JOIN CMWorklistsAcct wa WITH(NOLOCK) on f.WorkList=wa.Worklist and f.acctno=wa.acctno					
	WHERE datedealloc IS NULL 
		and ISNULL(wa.dateto,'1900-01-01')>datealloc
		AND EXISTS(SELECT 1 FROM  #worklists w WHERE w.WorkList = wa.Worklist )--replacred * with 1for Strategy Job Optimization by Zensar(Suvidha)
	 
	--FROM #worklists w ,follupalloc f
	--FROM follupalloc f
	--WHERE datedealloc IS NULL  
	--AND NOT EXISTS(SELECT * FROM  #worklists w 
	--				WHERE w.WorkList != ISNULL(f.worklist, '') ) --IP - 20/01/10 - UAT(955) - Previously was deallocating the incorrect accounts
	--AND w.worklist !=ISNULL(f.worklist, '') --IP 18/09/09 - UAT5.2 UAT(871) - Added isnull check

	-- deallocate accounts where reallocate set to 1 for branches
	UPDATE f 
	--SET datedealloc = GETDATE(),
	SET datedealloc = @rundate,																				--IP - 13/02/12 - use @rundate
	    empeenodealloc = '-114'		--IP - 23/03/11 - #3338 - LW73078 - Use empeeno -114
	FROM follupalloc f WITH(NOLOCK)--Added for Strategy Job Optimization by Zensar(Suvidha) 
	WHERE f.datedealloc IS NULL AND EXISTS (SELECT 1 FROM CMBailiffAllocationRules C WITH(NOLOCK) WHERE c.reallocate = 1 AND c.empeeno= f.empeeno --Added for Strategy Job Optimization by Zensar(Suvidha)
	 AND c.IsZone = 0 AND f.acctno LIKE c.branchorzone + '%' )  -- should be for branch only...  

-- now deallocate accounts where reallocate set to 1 for zones
	UPDATE f 
	--SET datedealloc = GETDATE(),
	SET datedealloc = @rundate,			--IP - 13/02/12 - use @rundate
		empeenodealloc = '-114'		--IP - 23/03/11 - #3338 - LW73078 - Use empeeno -114
	FROM follupalloc f WITH(NOLOCK)--Added for Strategy Job Optimization by Zensar(Suvidha) 
	WHERE f.datedealloc IS NULL AND EXISTS (
	SELECT 1 FROM CMBailiffAllocationRules r WITH(NOLOCK) JOIN --Added for Strategy Job Optimization by Zensar(Suvidha) 
	custaddress ca ON ca.zone = r.BranchorZone
	JOIN custacct c WITH(NOLOCK) ON ca.custid = c.custid--Added for Strategy Job Optimization by Zensar(Suvidha) 
	WHERE r.reallocate = 1 AND r.empeeno= f.empeeno 
	 AND c.hldorjnt = 'H' AND f.acctno = c.acctno 
	 AND ca.addtype= 'H' -- home address 
	 AND ca.datemoved IS NULL -- current address
	 AND r.IsZone = 1 ) 
	 
-- now deallocate accounts which are settled.
--IP - 23/04/10 - UAT(1008) UAT5.2 Internal
	UPDATE f
	--SET datedealloc = GETDATE(),
	SET datedealloc = @rundate,			--IP - 13/02/12 - use @rundate
		empeenodealloc = '-114'		--IP - 23/03/11 - #3338 - LW73078 - Use empeeno -114
	FROM follupalloc f WITH(NOLOCK) INNER JOIN acct a WITH(NOLOCK)--Added for Strategy Job Optimization by Zensar(Suvidha)
	ON f.acctno = a.acctno
	WHERE f.datedealloc IS NULL
	AND a.currstatus = 'S'
	and bdwbalance=0 and bdwcharges=0		--LW73080 19/01/11
	
	--IP - 14/09/11 - #3726 - LW73405 - Deallocate accounts that are in strategies that cannot have accounts allocated, or accounts that do not exist in a strategy
	UPDATE f
	--SET datedealloc = GETDATE(),
	SET datedealloc = @rundate,			--IP - 13/02/12 - use @rundate
		empeenodealloc = '-114'		
	FROM follupalloc f WITH(NOLOCK) LEFT JOIN cmstrategyacct cma WITH(NOLOCK) on f.acctno = cma.acctno--Added for Strategy Job Optimization by Zensar(Suvidha)
	LEFT JOIN code c WITH(NOLOCK) on cma.strategy = c.code and c.category = 'SS1'--Added for Strategy Job Optimization by Zensar(Suvidha)
	WHERE f.datedealloc is NULL
	AND (cma.dateto is null and c.reference!=1
	OR NOT EXISTS(select 1 from cmstrategyacct cma2 WITH(NOLOCK)--Added for Strategy Job Optimization by Zensar(Suvidha)
					where cma2.acctno = f.acctno)) 

	--IP - 20/01/10 - UAT(955) - Update the Courtsperson.alloccount by subtracting the number of accounts that have been deallocated for the employee.
	IF(@AcctsToDealloc >0)
	BEGIN										
		UPDATE c SET alloccount = alloccount - (SELECT COUNT(a.acctno) FROM @AcctsToDeallocate a
													WHERE a.empeeno = c.UserId)
		FROM courtsperson c
	END

	IF EXISTS (SELECT 1 FROM CountryMaintenance--Added for Strategy Job Optimization by Zensar(Suvidha)
			   WHERE CodeName = 'BailiffAssign'
			   AND value = 'true')
	BEGIN
		EXEC CM_AllocateSameBailiff @rundate = @rundate				--IP - 13/02/12 - use @rundate
	END													

	alter table #worklists add WorklistID integer identity(1,1)  not null
    DECLARE @level VARCHAR(2)
	set @loop = 1


	
	DECLARE @lastRunDate DATETIME 
	
	--select @lastRunDate = isnull(max(datestart), dateadd(day, -7, getdate())) from interfacecontrol where interface = 'COLLECTIONS'
	select @lastRunDate = isnull(max(datestart), dateadd(day, -7, @rundate)) from interfacecontrol where interface = 'COLLECTIONS'		--IP - 13/02/12 - use @rundate
	and runno<(select max(runno) from interfacecontrol where interface = 'COLLECTIONS')and result='P'

	 
	
		select count( cmw.acctno) AS Numtoallocate,cmw.Worklist,c.zone , left(cmw.acctno, 3) AS Branch 
		INTO  #NumberofAccounts
						from cmworklistsacct cmw WITH(NOLOCK) inner join custacct ca WITH(NOLOCK)--Added for Strategy Job Optimization by Zensar(Suvidha)
						on cmw.acctno = ca.acctno
						and ca.hldorjnt = 'H' 
						and (cmw.datefrom >= @lastRunDate OR cmw.dateto IS NULL) --IP - 05/01/2010 - UAT(955)
						inner join custaddress c WITH(NOLOCK) --Added for Strategy Job Optimization by Zensar(Suvidha)
						on ca.custid = c.custid 
						and c.addtype = 'H' AND c.datemoved IS NULL 
						where NOT EXISTS (SELECT 1 FROM follupalloc f WITH(NOLOCK) WHERE f.acctno= cmw.acctno AND f.datedealloc IS NULL)--Added for Strategy Job Optimization by Zensar(Suvidha)
						GROUP BY cmw.worklist ,c.zone ,left(cmw.acctno, 3) 
			
						--SELECT * FROM #NumberofAccounts 
	
--update #AllocationZoneBranch
--			set NumberOfAccounts = n.Numtoallocate
--						from #NumberofAccounts n
--						where n.zone = a1.branchorzone 
--						OR n.Branch = a1.branchorzone 
					

	while @loop <= @rowCount
	begin
		select @worklist = worklist,
		@level = LEVEL 
		from #worklists where worklistid = @loop
		EXEC CM_AutoBailiffAllocation @worklist, @level,@rundate, 0				--IP - 13/02/12 - use @rundate
		--select @worklist 
	set @loop = @loop + 1
	end

 
    if (@@error != 0)
    begin
        set @return = @@error
    end
end

GO
