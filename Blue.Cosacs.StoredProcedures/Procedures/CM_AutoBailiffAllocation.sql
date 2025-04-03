
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_AutoBailiffAllocation]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_AutoBailiffAllocation]
GO
-- ============================================================================================
-- Version:     002
-- Author:		Ilyas Parker
-- Create date: 13/05/2009
-- Description:	Procedure that determines share of accounts to allocate for each bailiff/collectors based on their level and worklist
--              calls CM_AllocateZoneBranch
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 09/07/2009 Jec Cred.Coll walkthrough changes - get correct "last run date"	
-- 29/09/09 IP - UAT(892) if this is the first time Credit Collections is running, then need to get the last run date as 
--			7 days from todays date.	
-- 05/01/2010 IP UAT(955) - Include accounts for re-allocation.		
-- 19/01/2010 IP UAT(955) - If the 'Reallocate' checkbox has been checked for a Bailiff/Collector for a branch/zone then these accounts
--							will be deallocated and accounts from this branch/zone will not be allocated.
-- 13/02/2012 IP Replace GETDATE() with @rundate
-- 08/08/2019 Zensar Optimised the stored procedure for performance by putting Nolock and replacing * with 1 in all exist
-- 09/08/2019 Zensar Added Flag Based on Country Parameter to do Bailiff Allocation Based on Zones
--12/08/2019 Zensar Added check on NumberofAccountsinZoneorBranch before calling CM_AllocateZoneBranch
-- ============================================================================================
CREATE PROCEDURE [dbo].[CM_AutoBailiffAllocation] 
        @Worklist varchar(10),
	    @level VARCHAR(2), -- '1', '2a','3a','3b','4','5'
	    @rundate DATETIME,									--IP - 13/02/12 
		@return	int	OUTPUT
				
as
begin

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	set nocount on;

    set @return = 0    --initialise return code
	
	declare @lastRunDate datetime,
			@rowCount int,
			@loop int,
			@Empeeno int,
			@AllocationOrder int,
			@BranchOrZone varchar(10),
			@AllocationRank int,
			@NumberOfStaff int,
			@NumberofAccountsinZoneorBranch int,
			@TotalAvailable INT,
			@Reallocate INT --IP - 19/01/10 - UAT(955)
			

	--Last 'Datestart' for Credit Collections.
		--select @lastRunDate = max(datestart) from interfacecontrol where interface = 'COLLECTIONS'
	-- select last successful run not current run		jec 09/07/2009
	--select @lastRunDate = max(datestart) from interfacecontrol where interface = 'COLLECTIONS'
	--and runno<(select max(runno) from interfacecontrol where interface = 'COLLECTIONS' and result='P')
	--IP - 29/09/09 - UAT(892)
	--select @lastRunDate = isnull(max(datestart), dateadd(day, -7, getdate())) from interfacecontrol where interface = 'COLLECTIONS'
	select @lastRunDate = isnull(max(datestart), dateadd(day, -7, @rundate)) from interfacecontrol where interface = 'COLLECTIONS'			--IP - 13/02/12 - use @rundate
	and runno<(select max(runno) from interfacecontrol where interface = 'COLLECTIONS' )and result='P'

	
	CREATE TABLE #AllocationZoneBranch
	(
		Empeeno bigint, 
		AllocationRank smallint,
		BranchOrZone varchar(10),
		AllocationOrder varchar(10),
		NumberOfStaff bigint,
		NumberOfAccounts bigint,
		TotalAvailable bigint,
		Worklist varchar(15), 
		TotalNoofAvailableAcctsforCollector bigint,
		SlotsAvailable smallint,
		reallocate smallint
	)

	--Start Zensar(SH)
	DECLARE @ZoneBasedAllocation char(1)
	select @ZoneBasedAllocation = convert(char(1), Value) from countrymaintenance where codename = 'ZoneBasedAllocation'
	if(@ZoneBasedAllocation = 'Y')
	BEGIN		
			--Select employee to allocate accounts to based on them having rights to the worklist and based on the rank passed in.
			INSERT INTO #AllocationZoneBranch
			select cmr.empeeno as Empeeno, 
			   cp.allocationrank as AllocationRank,
			   cmr.branchorzone as BranchOrZone,  
			   cmr.allocationorder as AllocationOrder, 
			   0 as NumberOfStaff, 
			   0 as NumberOfAccounts,
			   sum(cp.maxaccounts - cp.alloccount) as TotalAvailable,  -- total available to be allocated 
			   @worklist as Worklist,
			   0 AS TotalNoofAvailableAcctsforCollector ,
			   CONVERT (INT,0) AS SlotsAvailable,
			   CASE WHEN cmr.reallocate = 1 THEN 0 ELSE 1 END AS reallocate --IP - 19/01/10 - UAT(955)
		--into #AllocationZoneBranch 
		from CMBailiffAllocationRules cmr WITH (NOLOCK)--Added for Strategy Job Optimization by Zensar(Suvidha) 
		inner join cmworklistrights cmw WITH (NOLOCK) on cmr.empeeno = cmw.empeeno and cmw.worklist = @worklist
		inner join courtsperson cp WITH (NOLOCK) on cmw.empeeno = cp.UserId
			 --and cmw.worklisT  ='WCO1'
		WHERE  cp.allocationrank = left(@level, 1) --only for Bailiff/Collectors for the level we are passing in.
		and cp.Locked = 0 And cmr.IsZone = 1 --Zensar(SH) Added this condition to do Bailiff Allocation Based Only on Zones
		group by cmr.empeeno, cmr.allocationorder, cmr.branchorzone, cp.allocationrank,  cmr.reallocate --IP - 19/01/10 - UAT(955)
		order by cp.allocationrank, cmr.allocationorder
	END
	ELSE
	BEGIN
		--Select employee to allocate accounts to based on them having rights to the worklist and based on the rank passed in.
		INSERT INTO #AllocationZoneBranch
		select cmr.empeeno as Empeeno, 
			   cp.allocationrank as AllocationRank,
			   cmr.branchorzone as BranchOrZone,  
			   cmr.allocationorder as AllocationOrder, 
			   0 as NumberOfStaff, 
			   0 as NumberOfAccounts,
			   sum(cp.maxaccounts - cp.alloccount) as TotalAvailable,  -- total available to be allocated 
			   @worklist as Worklist,
			   0 AS TotalNoofAvailableAcctsforCollector ,
			   CONVERT (INT,0) AS SlotsAvailable,
			   CASE WHEN cmr.reallocate = 1 THEN 0 ELSE 1 END AS reallocate --IP - 19/01/10 - UAT(955)
		--into #AllocationZoneBranch 
		from CMBailiffAllocationRules cmr WITH (NOLOCK)--Added for Strategy Job Optimization by Zensar(Suvidha) 
		inner join cmworklistrights cmw WITH (NOLOCK) on cmr.empeeno = cmw.empeeno and cmw.worklist = @worklist
		inner join courtsperson cp WITH (NOLOCK) on cmw.empeeno = cp.UserId
			 --and cmw.worklisT  ='WCO1'
		WHERE  cp.allocationrank = left(@level, 1) --only for Bailiff/Collectors for the level we are passing in.
		and cp.Locked = 0
		group by cmr.empeeno, cmr.allocationorder, cmr.branchorzone, cp.allocationrank,  cmr.reallocate --IP - 19/01/10 - UAT(955)
		order by cp.allocationrank, cmr.allocationorder
		
	END
	--End Zensar(SH)










	--Depending on the level passed in the SlotsAvailable will either be upto the employees minimum or maximum accounts that can be allocated.
	UPDATE a
	SET SlotsAvailable = CASE WHEN RIGHT(@level,1) = 'a' THEN 
	b.minaccounts -b.alloccount  
	ELSE 
	b.maxaccounts -b.alloccount   
	end
	FROM #AllocationZoneBranch a WITH (NOLOCK), courtsperson b WITH (NOLOCK) --Added for Strategy Job Optimization by Zensar(Suvidha) 

	WHERE b.empeeno= a.Empeeno
--	SELECT MinAccounts,MaxAccounts,alloccount FROM courtsperson

	alter table #AllocationZoneBranch add AllocationID integer identity(1,1) not null
    
    --- TotalNoofAvailableAcctsforCollector is total number available accounts based on number of zones /branches collector given
	UPDATE #AllocationZoneBranch
	SET TotalNoofAvailableAcctsforCollector = ISNULL ((SELECT COUNT(c.acctno) FROM CMWorklistsAcct c WITH (NOLOCK)--Added for Strategy Job Optimization by Zensar(Suvidha) 
	JOIN custacct ca WITH(NOLOCK) ON ca.acctno= c.acctno --Added for Strategy Job Optimization by Zensar(Suvidha) 

	JOIN custaddress cad WITH(NOLOCK) ON ca.custid = cad.custid AND cad.zone = #AllocationZoneBranch.BranchOrZone AND cad.addtype = 'H' AND cad.datemoved IS NULL--Added for Strategy Job Optimization by Zensar(Suvidha)  
	JOIN CMStrategy s WITH(NOLOCK) ON s.Strategy= c.Strategy--Added for Strategy Job Optimization by Zensar(Suvidha) 

	JOIN code cd WITH (NOLOCK) ON cd.code = s.Strategy AND cd.category='ss1'--Added for Strategy Job Optimization by Zensar(Suvidha) 
	WHERE  ca.hldorjnt = 'H' 
	AND ( c.datefrom >@lastRunDate OR NOT EXISTS (SELECT 1 FROM follupalloc f WITH (NOLOCK) WHERE f.acctno = ca.acctno AND f.datedealloc IS NULL) ) --Added for Strategy Job Optimization by Zensar(Suvidha) 
	AND ISNULL(cd.reference,'0') >'0'),0) 
	
	-- we have done the zones now do the branch
	UPDATE #AllocationZoneBranch
	SET TotalNoofAvailableAcctsforCollector = TotalNoofAvailableAcctsforCollector  + ISNULL ((SELECT COUNT(c.acctno) FROM CMWorklistsAcct c WITH (NOLOCK) --Added for Strategy Job Optimization by Zensar(Suvidha)
	JOIN CMStrategy s WITH (NOLOCK) ON s.Strategy= c.Strategy--Added for Strategy Job Optimization by Zensar(Suvidha)

	JOIN code cd WITH (NOLOCK) ON cd.code = s.Strategy AND cd.category='ss1'--Added for Strategy Job Optimization by Zensar(Suvidha)
	WHERE  left(c.acctno, 3) = #AllocationZoneBranch.branchorzone AND c.Datefrom > @lastRunDate 
	AND ISNULL(cd.reference,'0') >'0' ),0)
	
	
	--Update 'NumberOfStaff' column - based on the branchorzone (number of employees the accounts
	--will be divided amongst for each branch/zone based on the level (rank) and where they have not reached their maximum number of accounts
	--to be allocated.
	update #AllocationZoneBranch
	set NumberOfStaff = ISNULL((select count(a1.empeeno)
					 from #AllocationZoneBranch a1, courtsperson cp WITH (NOLOCK)--Added for Strategy Job Optimization by Zensar(Suvidha)
					 where a1.branchorzone = a2.branchorzone
					 and a1.empeeno = cp.empeeno
					 and cp.allocationrank = convert(smallint, left(@level,1))
					 group by branchorzone),0)
	from #AllocationZoneBranch a2, courtsperson cp WITH (NOLOCK)--Added for Strategy Job Optimization by Zensar(Suvidha)
	WHERE cp.empeeno = a2.Empeeno 
	AND cp.allocationrank = convert(smallint, left(@level,1))  -- reinstating as should share based on allocation rank
	AND cp.alloccount < cp.MaxAccounts 
    --- Todo fix for min accounts 


	-- 

	---WHERE NumberOfStaff >0 
	/*update #AllocationZoneBranch
	set NumberOfStaff = ISNULL((select count(cp.empeeno)
					 from courtsperson cp 
					 WHERE cp.empeeno = a2.empeeno 
					 AND cp.alloccount < cp.MaxAccounts),0)
	from #AllocationZoneBranch a2
	*/
	

--	--Update 'NumberOfAccounts' column  - based on the branchorzone (accounts available for allocation within the
--	--branch/zone)
--  number of accounts in zone/branch 
	update azb  
	set NumberOfAccounts = (select count(distinct cmw.acctno)
						from cmworklistsacct cmw WITH (NOLOCK) inner join custacct ca WITH (NOLOCK)--Added for Strategy Job Optimization by Zensar(Suvidha)

						on cmw.acctno = ca.acctno and ca.hldorjnt = 'H' 
						inner join custaddress c WITH (NOLOCK)--Added for Strategy Job Optimization by Zensar(Suvidha) 

						on ca.custid = c.custid 
						and c.addtype = 'H' AND c.datemoved IS NULL 
						where (c.zone = azb.branchorzone 
						OR left(cmw.acctno, 3) = azb.branchorzone )
						AND NOT EXISTS (SELECT 1 FROM follupalloc f WITH(NOLOCK) WHERE f.acctno= cmw.acctno AND f.datedealloc IS NULL)--Added for Strategy Job Optimization by Zensar(Suvidha)
						and cmw.worklist = @Worklist
						--and cmw.datefrom >= @lastRunDate
						and (cmw.datefrom >= @lastRunDate OR cmw.dateto IS NULL) --IP - 05/01/2010 - UAT(955)
						)
	from #AllocationZoneBranch azb
	--select the rowcount
	
	set @rowCount = @@rowcount
	
	declare allocation_cursor CURSOR FOR
    SELECT empeeno, 
		   allocationrank,	
		   branchorzone,
		   allocationorder,	
		   totalavailable,
		   reallocate --IP - 19/01/10 - UAT(955)
	FROM #AllocationZoneBranch 
	ORDER BY allocationrank, allocationorder,TotalNoofAvailableAcctsforCollector ASC -- want to do minimum 
	OPEN allocation_cursor
	FETCH NEXT FROM allocation_cursor INTO @empeeno,@AllocationRank, @BranchOrZone, @AllocationOrder,@TotalAvailable, @Reallocate --IP - 19/01/10 - UAT(955)
	WHILE @@FETCH_STATUS = 0
	BEGIN
					
		IF RIGHT(@level,1) = 'a' -- then first pass doing minimum for 2a or 3a
		BEGIN
			SELECT @TotalAvailable =ISNULL(MinAccounts -alloccount,0)
			FROM courtsperson WHERE Userid = @empeeno 
		END
		ELSE --allocating maximum number of accounts
		BEGIN
			SELECT @TotalAvailable =ISNULL(MaxAccounts -alloccount,0)
			FROM courtsperson WHERE UserId = @empeeno 
		END

		--Print totalavailable
		 --select @totalavailable as available,@level as level

       SELECT @NumberofAccountsinZoneorBranch= NumberOfAccounts ,
       @NumberOfStaff=NumberOfStaff 
       FROM #AllocationZoneBranch 
       WHERE BranchOrZone= @BranchOrZone AND Worklist = @Worklist
		-- so allocate accounts out sharing based on the zone and allocation rank and number of accounts

		-- select 'check1 ',* from ##AcctsToAllocate

		if (@totalavailable >0 And @NumberofAccountsinZoneorBranch > 0)   ------- @NumberofAccountsinZoneorBranch check added by Zensar

		BEGIN
			--select ' allocating for employee ' + CONVERT(VARCHAR,@Empeeno) + ' rank:' + CONVERT(VARCHAR,@AllocationRank)
   --          + ' Order' + CONVERT(VARCHAR,@allocationorder) + ' NumberofAcctsinZoneB:' + CONVERT(VARCHAR,@NumberofAccountsinZoneorBranch)
   --          + ' total slots available ' + CONVERT(VARCHAR,@TotalAvailable) + ' 
			--	worklists:' + @Worklist
				
				EXEC CM_AllocateZoneBranch @Empeeno=@empeeno , 
								   @AllocationRank=  @AllocationRank,
								   @BranchOrZone = @BranchOrZone,  
								   @AllocationOrder=@AllocationOrder, 
								   @NumberOfStaff=@NumberOfStaff OUT  , 
								   @NumberofAccountsinZoneorBranch=@NumberofAccountsinZoneorBranch OUT ,
								   @TotalAvailable=@TotalAvailable OUT,
 								   @Worklist= @Worklist,
 								   @lastRunDate =@lastRunDate ,
 								   @Reallocate = @Reallocate, --IP - 19/01/10 - UAT(955)
 								   @return =@return 
             
             
             -- removing so remaining accounts can be shared correctly 
             DELETE FROM #AllocationZoneBranch 
             WHERE empeeno= @empeeno      
             AND AllocationRank =  @AllocationRank
             AND BranchOrZone = @BranchOrZone
             AND AllocationOrder=@AllocationOrder
             AND Worklist= @Worklist
             /*cmr.empeeno as Empeeno, 
		   cp.allocationrank as AllocationRank,
		   cmr.branchorzone as BranchOrZone,  
		   cmr.allocationorder as AllocationOrder, 
           0 as NumberOfStaff, 
	       0 as NumberOfAccounts,
	       sum(cp.maxaccounts - cp.alloccount) as TotalAvailable,  -- total available to be allocated 
	       @worklist as Worklist,
	       0 AS TotalNoofAvailableAcctsforCollector ,
	       CONVERT (INT,0) AS SlotsAvailable
		*/
--			UPDATE #AllocationZoneBranch
--			SET NumberOfAccounts = @NumberofAccountsinZoneorBranch,
--			NumberOfStaff = @NumberOfStaff
--			WHERE BranchOrZone= @BranchOrZone AND Worklist = @Worklist

			UPDATE #AllocationZoneBranch
			SET NumberOfStaff = @NumberOfStaff
			WHERE BranchOrZone= @BranchOrZone AND Worklist = @Worklist

			--Update the NumberOfAccounts to the correct numbers available. 
			
			update #AllocationZoneBranch
			set NumberOfAccounts = n.Numtoallocate
						from #NumberofAccounts n
						where (n.zone = #AllocationZoneBranch.branchorzone 
						OR n.Branch = #AllocationZoneBranch.branchorzone )
						AND n.worklist = @Worklist
						AND n.worklist = #AllocationZoneBranch.worklist
					   
			
			--update #AllocationZoneBranch
			--set NumberOfAccounts = (select count(distinct cmw.acctno)
			--			from cmworklistsacct cmw inner join custacct ca
			--			on cmw.acctno = ca.acctno
			--			and ca.hldorjnt = 'H' 
			--			and cmw.worklist = @Worklist
			--			--and cmw.datefrom >= @lastRunDate
			--			and (cmw.datefrom >= @lastRunDate OR cmw.dateto IS NULL) --IP - 05/01/2010 - UAT(955)
			--			inner join custaddress c 
			--			on ca.custid = c.custid 
			--			and c.addtype = 'H' AND c.datemoved IS NULL 
			--			where (c.zone = a1.branchorzone 
			--			OR left(cmw.acctno, 3) = a1.branchorzone )
			--			AND NOT EXISTS (SELECT * FROM follupalloc f WHERE f.acctno= cmw.acctno AND f.datedealloc IS NULL))
			--from #AllocationZoneBranch a1
			
	   end 	
	FETCH NEXT FROM allocation_cursor INTO @empeeno,@AllocationRank, @BranchOrZone, @AllocationOrder,@TotalAvailable, @Reallocate --IP - 19/01/10 - UAT(955)

	END

	CLOSE allocation_cursor
	DEALLOCATE allocation_cursor



    if (@@error != 0)
    begin
        set @return = @@error
    end
end

GO

