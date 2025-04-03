

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_AllocateZoneBranch]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_AllocateZoneBranch]
GO

-- ============================================================================================
-- Version:		002
-- Author:		Ilyas Parker
-- Create date: 14/05/2009
-- Description:	Inserts into the follupalloc table (allocation records) for bailiffs and collectors
--              called by CM_Auto3fAllocation. Also deallocates these accounts
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 11/06/2009 Jec Cred.Coll walkthrough changes - add "ALL" code for allocated accounts 
-- 09/12/2009 IP/JC Cater for divide by zero error	
-- 05/01/2010 IP UAT(955) - Include accounts for re-allocation.	
-- 19/01/2010 IP UAT(955) - If the 'Reallocate' checkbox has been checked for a Bailiff/Collector for a branch/zone then these accounts
--							will be deallocated and accounts from this branch/zone will not be allocated.	
-- 23/03/11   IP #3338 - LW73078 - Use empeeno (-114) for Auto Bailiff Allocation (Merged from 5.4.3)
-- 08/08/19   Zensar Strategy Job Optimization : Optimised the stored procedure for performance by putting Nolock and replacing * with 1 in all exist

-- ============================================================================================
CREATE PROCEDURE [dbo].[CM_AllocateZoneBranch] 
	            @Empeeno int,
				@AllocationRank int,
				@BranchOrZone varchar(10),
				@AllocationOrder int,
				@NumberOfStaff INT OUT,
				@NumberOfAccountsinZoneorBranch INT OUT , -- for allocation
				@TotalAvailable INT OUT ,  -- for this employee
				@Worklist varchar(10),
				@lastRunDate DATETIME,
				@reallocate INT, --IP - 19/01/10 - UAT(955)
				@return	int	OUTPUT
as
begin
	SET NOCOUNT ON 
    -- Need to calculate number of accounts to allocate - share it between the baliffs in this zone...     
    -- Question is if we allocate minimum    
    -- Allocate all the accounts in this zone but risk is some bailiffs wont get fair share    
    -- Allocate based on Bailiffs who only have one zone... 
    
    --
    DECLARE @testing SMALLINT, @newline VARCHAR(32),
			@BailActionRows int,@insertCount int
    -- leave this as is
    SET @newline = '
'
    
    SET @testing = 0
 
	IF @testing = 1
		select 'CHECK AllocateZoneBranch: ',* from ##AcctsToAllocate
	
	DECLARE @rowcount INT , @NumberToAllocate INT, @date datetime  ,@totalslots FLOAT ,@slotsForthisEmpeeno INT 
--	set @date = getdate()

	--IP/JC - 09/12/09 - UAT(934) - Cater for divide by zero error
	SELECT @numbertoAllocate = CASE WHEN @numberofStaff > 0 THEN @NumberOfAccountsinZoneorBranch/@numberofStaff ELSE 0 END
	--WHERE @numberofStaff >0 
	
	SELECT @totalslots = SUM(slotsavailable)  FROM #AllocationZoneBranch WHERE branchorzone  = @BranchOrZone 	
	
	SELECT @slotsForthisEmpeeno = slotsavailable  FROM #AllocationZoneBranch WHERE empeeno= @empeeno 
	
	--IP/JC - 09/12/09 - UAT(934) - Cater for divide by zero error
	SELECT @numbertoAllocate = CASE WHEN @totalslots!=0 THEN (@slotsForthisEmpeeno * 1.00/ @totalslots *1.00)
																 * @NumberOfAccountsinZoneorBranch * 1.0 * @reallocate --IP - 19/01/10 - UAT(955) - If Reallocate multiply by 0
								ELSE 0 END
	--WHERE @numberofStaff >0 
	
	-- CHECK should number of staff include those with less accounts available.... 
	--IF @numbertoallocate > 0 AND  @NumberOfAccountsinZoneorBranch >0 AND @testing = 1
	--BEGIN
	--	SELECT @TotalAvailable AS available, @numberofStaff AS NumberofStaff, @numbertoAllocate AS numbergoingtoallocate, @slotsForthisEmpeeno AS slits
	--	,alloccount,maxaccounts,@NumberOfAccountsinZoneorBranch AS totnum,@totalslots AS slots FROM courtsperson WHERE empeeno = @empeeno
	
	--	SELECT * FROM #AllocationZoneBranch	
	--END
		
	--RETURN 
	IF  @numbertoAllocate > @TotalAvailable -- don't exceed individuals availability 
	   SET  @numbertoAllocate  = @TotalAvailable
	
	SELECT @numbertoAllocate as Number, @EMPEENO as empee, @worklist as worklist,@branchorzone as borz
   
	IF @numbertoAllocate <=0 
	BEGIN
	    RETURN 	
	    print ' No accounts TO allocate' + CONVERT(VARCHAR,@empeeno ) 
	
	END
	  
    --- Number of accounts to allocate
    -- less than total available for bailiff
    
    --  New parameter which would say do allocate accounts
    --- Total  

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	set nocount on;

    set @return = 0    --initialise return code

---------------	--IP - Deallocating accounts first before allocating----------------------------
	TRUNCATE table ##AcctsToAllocate
	DECLARE @statement1 sqltext, @loop int, @acctno varchar(12), @follupallocWorklist varchar(10)

	--Select the accounts into a temporary table.
	--For each account check if the current worklist is different to the previous worklist when allocated to a Bailiff/Collector
	--If the worklist is different, then deallocate the account from follupalloc.
	SET @statement1 ='INSERT INTO ##AcctsToAllocate (acctno, worklist, allocated)' + @newline +
					 ' SELECT  TOP ' + CONVERT(VARCHAR,@numbertoAllocate) +
					 ' cmw.acctno,cmw.worklist,1 ' +
					 ' FROM CMWorkListsAcct cmw WITH(NOLOCK)  ' +  @newline +--Zensar(SH)
					 --' LEFT JOIN Follupalloc f ON cmw.acctno = f.acctno AND f.datedealloc IS NULL' +	 @newline +
					 ' JOIN Custacct ca WITH(NOLOCK) ON cmw.acctno = ca.acctno AND ca.hldorjnt = ''H''' + @newline +--Zensar(SH)
					 ' JOIN Custaddress cad WITH(NOLOCK) ON ca.custid = cad.custid' + @newline +--Zensar(SH)
					 ' AND cad.addtype = ''H''' + @newline +
					 ' AND cad.datemoved IS NULL' + @newline +
					 ' AND (CAD.zone =' + '''' + @BranchOrZone  + '''' +  @newline +
					 ' OR left(cmw.acctno, 3) = ' + '''' + @branchorzone + '''' +  ')' +  @newline +
					 ' AND cmw.worklist =' + '''' + @Worklist + '''' +  @newline +
					 --' AND cmw.datefrom >' + '''' + CONVERT(VARCHAR,@lastRunDate) + '''' + @newline + --IP - 05/01/2010 - UAT(955) - Commented out line
					 ' AND cmw.dateto IS NULL ' +  @newline +
					 ' AND NOT EXISTS (SELECT 1 FROM follupalloc f WITH(NOLOCK) WHERE f.acctno= cmw.acctno AND f.datedealloc IS NULL) ' + --' and f.worklist = cmw.worklist) ' --AA don't need this as reallocated earlier--Zensar(SH)
					 ' AND NOT EXISTS (SELECT 1 FROM BailliffUnsuccessful f  WITH(NOLOCK) WHERE f.acctno= cmw.acctno ' +--Zensar(SH)
					 ' and f.empeeno = ' + convert(varchar,@empeeno) + ')' +  --' and f.worklist = cmw.worklist) ' --AA don't need this as reallocated earlier
					 ' GROUP BY cmw.acctno,cmw.worklist,cad.cuspocode,cad.cusaddr3, cad.cusaddr2,cad.cusaddr1 ' +
					 ' order by cad.cuspocode,cad.cusaddr3, cad.cusaddr2,cad.cusaddr1' 					 
--	print @statement1
	EXECUTE sp_executesql @statement1 
	SELECT @return = @@ERROR ,  @rowcount = @@ROWCOUNT

--	alter table ##AcctsToAllocate add AllocID integer identity(1,1)  not null
    IF @testing = 1 
		select COUNT(*) AS Actualnumto , CONVERT(VARCHAR,@numbertoAllocate) AS 'proposednumber' from ##AcctsToAllocate --order by acctno
--	PRINT '#AcctsToAllocate'
--	select * from #AcctsToAllocate

	/*set @loop = 1
	while @loop <= @rowCount
	begin
		select @acctno = acctno,
			   @follupallocWorklist = worklist 
		from ##AcctsToAllocate where AllocID = @loop
	
	--Deallocate the account from the follupalloc table if the account is now in a different worklist.
		IF(@follupallocWorklist <> @worklist 
		and @follupallocWorklist IS NOT NULL) -- should we be checking this - what about migration?? Need to do manual deallocation script
		BEGIN
			UPDATE follupalloc 
			SET datedealloc = getdate(),
				empeenodealloc = 99999
			WHERE acctno = @acctno
			AND datedealloc IS NULL	
		END
		set @loop = @loop + 1
	end
	Don't need this as deallocating earlier
	*/
	
-------------------------------------------------------------------------------------------------
  DECLARE @statement2 sqltext 
  -- save accounts that will be inserted into follupaloc table
	create table #BailAction (ID int identity, acctno varchar(12), allocno INT )
  	--SET @statement2 = 'INSERT INTO BailAction (acctno) ' + @newline +
  	SET @statement2 = 'INSERT INTO #BailAction (acctno,allocno ) ' + @newline + --IP - 25/08/09 - UAT(813) 
  	'SELECT ' +
	--' a.acctno,	CONVERT(VARCHAR,@empeeno)' +  @newline +
	 ' a.acctno,ISNULL(MAX(f.allocno) +1,1) ' + @newline +	--IP - 25/08/09 - UAT(813)
	 ' FROM CMWorkListsAcct W WITH(NOLOCK) ' + @newline +--Zensar(SH)
	 ' JOIN acct a WITH(NOLOCK) ON a.acctno= W.acctno ' + @newline +--Zensar(SH)
	 ' JOIN custacct ca WITH(NOLOCK) on ca.acctno= a.acctno ' + @newline +--Zensar(SH)
	 ' JOIN custaddress cad WITH(NOLOCK) on cad.custid = ca.custid ' + @newline +--Zensar(SH)
	 ' LEFT JOIN follupalloc f WITH(NOLOCK) ON f.acctno = a.acctno ' + @newline +--Zensar(SH)
	 ' JOIN ##AcctsToAllocate at ON at.acctno = w.acctno ' + @newline +
	 ' WHERE ca.hldorjnt  =''H'' and cad.addtype =''H'' and cad.datemoved is NULL   ' + @newline +
	 ' AND (CAD.zone =' + '''' + @BranchOrZone  + '''' +  @newline +
	 ' OR left(a.acctno, 3) = ' + '''' + @branchorzone + '''' +  ')' +  @newline +
	 ' AND W.worklist =' + '''' + @Worklist + ''''  	 +  @newline +
	 --' AND w.datefrom >' + '''' + CONVERT(VARCHAR,@lastRunDate) + '''' + @newline + --IP - 05/01/2010 - UAT(955) - Commented out line.
	 ' AND w.dateto IS NULL ' +  @newline +
	 ' AND NOT EXISTS (SELECT 1 FROM follupalloc f WITH(NOLOCK) WHERE f.acctno= w.acctno AND f.datedealloc IS NULL) ' + @newline + --Zensar(SH)
	  ' AND NOT EXISTS (SELECT 1 FROM BailliffUnsuccessful f WITH(NOLOCK) WHERE f.acctno= w.acctno ' + --Zensar(SH)
	 ' and f.empeeno = ' + convert(varchar,@empeeno) + ')' +  --' and f.worklist = cmw.worklist) ' --AA don't need this as reallocated earlier
	 ' GROUP BY A.acctno,a.arrears,cad.cuspocode,cad.cusaddr3, cad.cusaddr2,cad.cusaddr1 ' +  @newline +
	 ' order by cad.cuspocode,cad.cusaddr3, cad.cusaddr2,cad.cusaddr1' 

	 EXECUTE sp_executesql @statement2 
	 set @BailActionRows=@@ROWCOUNT

	-- insert accounts into follupaloc
	 
	SET @statement2 = 'INSERT INTO follupalloc (
		origbr,		acctno,		allocno,
		empeeno,		datealloc,		datedealloc,
		allocarrears,		bailfee,		allocprtflag,
		empeenoalloc,		empeenodealloc, worklist
	)  ' + @newline +
	'SELECT ' +
	' 0, a.acctno,ISNULL(MAX(f.allocno) +1,1) , ' +
	CONVERT(VARCHAR,@empeeno) + ', NULL , NULL, ' +
	' a.arrears, 0, ''N'', ' +
	' -114, 0, ' + ''''+@Worklist+'''' +  @newline +	--IP - 23/03/11 - #3338 - LW73078 - Use empeeno -114 
	 ' FROM CMWorkListsAcct W WITH(NOLOCK) ' + @newline + --Zensar(SH)
	 ' JOIN acct a WITH(NOLOCK) ON a.acctno= W.acctno ' + @newline + --Zensar(SH)
	 ' JOIN custacct ca WITH(NOLOCK) on ca.acctno= a.acctno ' + @newline + --Zensar(SH)
	 ' JOIN custaddress cad WITH(NOLOCK) on cad.custid = ca.custid ' + @newline + --Zensar(SH)
	 ' LEFT JOIN follupalloc f WITH(NOLOCK) ON f.acctno = a.acctno ' + @newline + --Zensar(SH)
	 ' JOIN ##AcctsToAllocate at ON at.acctno = w.acctno ' + @newline +
	 ' WHERE ca.hldorjnt  =''H'' and cad.addtype =''H'' and cad.datemoved is NULL   ' + @newline +
	 ' AND (CAD.zone =' + '''' + @BranchOrZone  + '''' +  @newline +
	 ' OR left(a.acctno, 3) = ' + '''' + @branchorzone + '''' +  ')' +  @newline +
	 ' AND W.worklist =' + '''' + @Worklist + ''''  	 +  @newline +
	 --' AND w.datefrom >' + '''' + CONVERT(VARCHAR,@lastRunDate) + '''' + @newline + --IP - 05/01/2010 - UAT(955) - Commented out line.
	 ' AND w.dateto IS NULL ' +  @newline +
	 ' AND NOT EXISTS (SELECT 1 FROM follupalloc f WITH(NOLOCK) WHERE f.acctno= w.acctno AND f.datedealloc IS NULL) ' + @newline + --Zensar(SH)
	 ' AND NOT EXISTS (SELECT 1 FROM BailliffUnsuccessful f WITH(NOLOCK) WHERE f.acctno= a.acctno ' + --Zensar(SH)
	 ' and f.empeeno = ' + convert(varchar,@empeeno) + ')' +  --' and f.worklist = cmw.worklist) ' --AA don't need this as reallocated earlier
	 ' GROUP BY A.acctno,a.arrears,cad.cuspocode,cad.cusaddr3, cad.cusaddr2,cad.cusaddr1 ' +  @newline +
	 ' order by cad.cuspocode,cad.cusaddr3, cad.cusaddr2,cad.cusaddr1' 
	 --PRINT @statement2 --Testing only

	 --PRINT 'AllocationRank '
	 --PRINT @AllocationRank
	 --PRINT 'BranchOrZone'
	 --PRINT @BranchOrZone 
	 --PRINT 'AllocationOrder'
	 --PRINT @AllocationOrder 
	 --PRINT 'NumberOfAccountsinZoneorBranch'
	 --PRINT @NumberOfAccountsinZoneorBranch 
	 --PRINT 'TotalAvailable'
	 --PRINT @TotalAvailable 

	 EXECUTE sp_executesql @statement2 
	 SELECT @return = @@ERROR ,  @rowcount = @@ROWCOUNT
	 
	 IF @rowcount >0 
	 
			update #NumberofAccounts 
			set Numtoallocate = Numtoallocate - @rowcount 
						where zone = @BranchOrZone  
						OR Branch = @BranchOrZone  
						AND worklist = @Worklist 
			
	 
	--Now insert allocated "ALL" code into bailaction for accounts allocated	
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
			@callingSource varchar(10), @allocno INT 
	set @dateadded=GetDate()
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
	
	set @insertCount=0
	while @insertCount<@BailActionRows
	BEGIN
		SET @insertCount=@insertCount+1
		--Select acctno from #BailAction where ID=@insertCount
		--IP - 25/08/09 - UAT(813) - @acctno and @employeeno were not being set.
		select @acctno = acctno, @allocno = allocno  from #BailAction where ID=@insertCount
		select @employeeno = @Empeeno
		
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
	
	
	
--------------------------------------------------------------------------------------------------
--original
--    DECLARE @statement sqltext 
--	SET @statement = 'INSERT INTO follupalloc (
--		origbr,		acctno,		allocno,
--		empeeno,		datealloc,		datedealloc,
--		allocarrears,		bailfee,		allocprtflag,
--		empeenoalloc,		empeenodealloc, worklist
--	) 
--	SELECT 
--	TOP ' + CONVERT(VARCHAR,@numbertoAllocate)
--	+ ' 0, a.acctno,ISNULL(MAX(f.allocno) +1,1) , ' +
--	CONVERT(VARCHAR,@empeeno) + ', NULL , NULL, ' +
--	' a.arrears, 0, ''N'', ' +
--	' 99999, 0, ' + ''''+@Worklist+'''' + 
--	 ' FROM CMWorkListsAcct W ' +
--	 ' JOIN acct a ON a.acctno= W.acctno ' +
--	 ' JOIN custacct ca on ca.acctno= a.acctno ' +
--	 ' JOIN  custaddress cad on cad.custid = ca.custid ' +
--	 ' LEFT JOIN follupalloc f ON f.acctno = a.acctno ' +
--	 ' WHERE ca.hldorjnt  =''H'' and cad.addtype =''H'' and cad.datemoved is NULL   ' +
--	 ' AND (CAD.zone =' + '''' + @BranchOrZone  + '''' + 
--	 ' OR substring(a.acctno, 0, 4) = ' + '''' + @branchorzone + '''' +  ')' + 
--	 ' AND W.worklist =' + '''' + @Worklist + ''''  	 + 
--	 ' AND w.datefrom >' + '''' + CONVERT(VARCHAR,@lastRunDate) + '''' +
--	 ' AND w.dateto IS NULL ' + 
--	 ' AND NOT EXISTS (SELECT * FROM follupalloc f WHERE f.acctno= w.acctno AND f.datedealloc IS NULL) ' +
--	 ' GROUP BY A.acctno,a.arrears,cad.cusaddr3, cad.cusaddr2 ' + 
--	 ' order by cad.cusaddr3, cad.cusaddr2' 
--	 PRINT @statement --Testing only
		
--	 PRINT 'AllocationRank '
--	 PRINT @AllocationRank
--	 PRINT 'BranchOrZone'
--	 PRINT @BranchOrZone 
--	 PRINT 'AllocationOrder'
--	 PRINT @AllocationOrder 
--	 PRINT 'NumberOfAccountsinZoneorBranch'
--	 PRINT @NumberOfAccountsinZoneorBranch 
--	 PRINT 'TotalAvailable'
--	 PRINT @TotalAvailable 
--
--	EXECUTE sp_executesql @statement 
--	SELECT @return = @@ERROR ,  @rowcount = @@ROWCOUNT
----------------------------------------------------------------------------------------------------------------------	
	UPDATE courtsperson SET alloccount = ISNULL((SELECT COUNT(f.empeeno) FROM follupalloc f with(NoLock)  WHERE datedealloc IS NULL AND f.empeeno= @empeeno),0)--Zensar(SH)
	WHERE UserId =@empeeno 
	
	SET @TotalAvailable=@TotalAvailable - @rowcount 
	
	--IP - No longer need this
	--SET @NumberOfAccountsinZoneorBranch =@NumberOfAccountsinZoneorBranch - @rowcount 
	
	--Commented Below by Zensar(SH)
	--SELECT @rowcount AS ROWCOUNT22  --Zensar(SH)
	
	SET @numberofStaff =@numberofStaff -1 
	
    if (@@error != 0)
    begin
        set @return = @@error
    end
end

GO
