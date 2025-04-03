
SET QUOTED_IDENTIFIER ON
GO

if exists (select * from sysobjects where name ='CMEod_AssignAccountstoStrategiesSP')
drop procedure CMEod_AssignAccountstoStrategiesSP
go

 
-- ============================================================================================================
-- Version		:002
-- Project      : CoSACS .NET
-- File Name    : CMEod_AssignAccountstoStrategiesSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Assign accounts to Strategies
-- Author       : Alex Ayscough
-- Date         : March 2007
--
-- This procedure will assign accounts into and out of strategies. This will be part of the daily process.
-- The account details screen will list which strategy an account currently is in.
-- The process will load up all accounts within a strategy and then verify each account to see which steps,
-- entry or exit conditions will need to be applied. 
-- If an account is in a worklist it cannot be segmented into a strategy until it is worked and no longer
-- in a strategy.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 30/07/09  jec UAT767 New Strategy Variable tables 
-- 20/08/09  IP  UAT799 Updating Dateto of strategy incorrectly.
-- 09/09/09 jec UAT835 Arrears/Balance can be zero for service strategy
-- 15/09/09 jec UAT859 Arrears/Balance can be zero for REP strategy
-- 01/10/09 ip  UAT855 The dateto was incorrectly being set for an accounts strategies where the dateto had already been set previously. 
--					   Should only update the dateto of the previous strategy to the one that the account has been assigned to.
-- 21/10/09 ip  UAT910 @DateNow was converted to varchar which was truncating the date time, leaving off the time stamp. Changed to use convert(varchar,@datenow, 121) to
--					   include the timestamp. In other places @DateNow was not converted to varchar, therefore causing differences in time.
-- 11/11/09 ip  UAT917 Changed the message to be more of an information message rather than a warning. The incorrect strategy was used in the message.
-- 10/12/09 ip  UAT910 When inserting into the CMWorklistsAcct table, datefrom needed to be inserted using @datenow and not @highPrioWorkListFromDate
-- 15/12/09 ip  UAT937 Service accounts must first be in arrears in order to enter the Service Strategy.
-- 16/12/09 ip  UAT857 Added OR clause for bdwcharges > 0, as written off accounts may just have bdwcharges > 0.
-- 17/12/09 ip  UAT929 Accounts were not entering the DFA Strategy. Resolved duplicate key error when inserting into cmstrategyacct
-- 16/02/12 IP  use @rundate
-- 07/08/19 Zensar  Optimised the stored procedure for performance using With NoLock and replacing * with 1 in Exit clause
-- =====================================================================================================================================
CREATE PROCEDURE [dbo].[CMEod_AssignAccountstoStrategiesSP]
	-- Add the parameters for the stored procedure here
		@rundate DATETIME,								--IP - 13/02/12 
		@return int OUTPUT
as
	
		declare @statement sqltext ,
		@strategy varchar(7),@PreviousStrategy varchar(7),		-- jec 17/02/12
		@counter smallint,@newline varchar(32)
		set @newline =''

		set @counter = 0
		set @PreviousStrategy = ''
		set @statement =''

		-- so load up the qualifying strategies by each strategy and check that working ok....
		declare @Type char(1) ,
		   @Condition varchar(12) ,
		   @Operand varchar(10), --- between >< = 
		   @Operator1 varchar(24), -- value.....
		   @Operator2 varchar(24), -- 
		   @OrClause char(1),  --  can be a/b/c/1/2/3 - used to match orclauses
		   @step smallint,
		   @ActionCode varchar(12),@duplicatestatement sqltext, @insert sqltext,@select sqltext,
		   @StepActiontype char(1), @datenow datetime, @previousor varchar(10),@finalstatement sqltext,
		   @ConditionType char(1)	-- UAT767 jec
	
		declare @Entrystatement sqltext,@Entrystatement2 sqltext,
				@Stepsstatement sqltext,@Stepsstatement2 sqltext,
				@Exitstatement sqltext,@Exitstatement2 sqltext,
				@StrategyVariabletable varchar(30)
		
		set @Previousor =''
		--set @datenow = getdate()
		set @datenow = @rundate								--IP - 13/02/12 - use @rundate

		declare @runno int
		select @runno= max(runno) from interfacecontrol with(NoLock) where interface = 'COLLECTIONS' 
  
		DECLARE AllocateStrategy_cursor CURSOR 
  		FOR SELECT C.Strategy,C.Condition,C.Operand,C.Operator1,C.Operator2,isnull(C.OrClause,''),C.step,C.ActionCode,
  			C.StepActiontype,c.savedtype 
		from CMStrategyCondition C With(NoLock), CMStrategy S With(NoLock),CMCondition d With(NoLock),code o With(NoLock)
		where 
			S.isActive !=0 and
			C.Strategy = S.Strategy --and C.Stepactiontype is not null
		and o.category = 'SS1' 
		AND o.code = s.strategy 
		and c.condition = d.condition 
		--and c.step is null --and c.type ='S'
		and c.condition not like 'hold%'
		--and d.type in ('N','') 
		and c.strategy not in ('NON','SCNON','ARR','ARB') ---Added by Zensar
		--and C.strategy !='NON' -- not allocating to non-arrears strategy
		--and C.strategy !='SCNON' -- not allocating to storecard non-arrears strategy
		--and C.strategy !='ARR' --- Added by Zensar as the Strategy is not in use
		--and C.strategy !='ARB' --- Added by Zensar as the Strategy is not in use
		and C.savedtype ='N'
		order by o.sortorder,c.strategy,c.orclause
		
		OPEN AllocateStrategy_cursor
		FETCH NEXT FROM AllocateStrategy_cursor INTO @Strategy,@Condition,@Operand,@Operator1,@Operator2,
           @OrClause,@step,@ActionCode,@StepActiontype,@ConditionType 
		WHILE (@@fetch_status <> -1)
		BEGIN

			IF (@@fetch_status <> -2)
   			begin         

				-- set table for correct table depending on condition type - UAT767	  
				set @StrategyVariabletable=case
				when @ConditionType ='N' then 'CMStrategyVariablesEntry'
				when @ConditionType ='S' then 'CMStrategyVariablesSteps'
				when @ConditionType ='X' then 'CMStrategyVariablesExit'
			end 
			
			if @previousor !='' and (@orclause ='' or @orclause !=@previousor)
			begin -- there was a previous or which is now terminated so close the bracket and continue with the and conditions.
				set @statement = @statement + ')'
			end

			if @PreviousStrategy !=@Strategy and @counter !=0 -- we have built the sql statement so now we can execute it. 
			BEGIN
				--IP - 11/11/09 - UAT5.2 (917) - Changed the warning message. The incorrect strategy was being used in the message.
				-- check whether there are potential clashes
				--set @duplicatestatement 
				--      = ' insert into interfaceerror (   interface,			runno,			severity,' +			
				--        ' errordate,			errortext) ' +
				--        ' select top 3 ''COLLECTIONS'',' + convert(varchar,@runno) + ',''W'', ' +
				--        ' getdate(), ''Duplicate allocations clash - Example Account ''  + c.acctno + '' already in another strategy. '' + ' +
				--        ' ''Attempted to allocated to strategy ''+ ' + '''' + @strategy + '''' + @statement +
				--        ' and  exists (select * from cmstrategyacct sa where sa.acctno = a.acctno ' +
				--        ' and sa.dateto is null and sa.strategy !=''NON'' ) ' 
            
				set @duplicatestatement 
					= ' insert into interfaceerror (interface,runno,severity,' +			
                    ' errordate,errortext) ' +
                    ' select top 3 ''COLLECTIONS'',' + convert(varchar,@runno) + ',''W'', ' +
					--' getdate(), ''Example Account ''  + c.acctno + '' has already been allocated to another strategy. '' + ' +
                     '''' + convert(varchar,@rundate) + '''' + ', ''Example Account ''  + c.acctno + '' has already been allocated to another strategy. '' + ' +				--IP - 13/02/12 - use @rundate
                    ' ''This account also met the entry conditions for strategy ''+ ' + '''' + @PreviousStrategy + '''' + @statement +
                    ' and  exists (select 1 from cmstrategyacct sa With(NoLock) where sa.acctno = a.acctno ' +
                    ' and sa.dateto is null and sa.strategy !=''NON'' and sa.strategy !=''SCNON'') '			-- jec 17/02/12 storecard non arrears
				execute sp_executesql @duplicatestatement
				
				set @return = @@error
				if @return !=0
				begin
					--print @duplicatestatement
					--Print 'failure in CMEod_AssignAccountstoStrategiesSP '
					return 
				end
				
				set @finalstatement =@insert + @newline + @select + @newline + @statement + 
				'and not exists (select 1 from cmstrategyacct sa With(NoLock) where sa.acctno = a.acctno ' 
				-- don't insert into strategy if already in another arrears strategy. (do if in Non-arrers strategy)
				+ ' and sa.dateto is null and sa.strategy !=''NON'' and sa.strategy !=''SCNON'') '					-- jec 17/02/12 storecard non arrears        
				execute sp_executesql @finalstatement
				
				--print @finalstatement
				set @return = @@error
				
				if @return !=0
				begin
					--print @insert
					--print @select
					--print @finalstatement
					--Print 'failure in CMEod_AssignAccountstoStrategiesSP '
					CLOSE AllocateStrategy_cursor
					dEALLOCATE AllocateStrategy_cursor
					return 
				end
      
				-- update CMStrategyAcct set dateto = @datenow where exists (select * from CMStrategyAcct c where c.acctno =  CMStrategyAcct.acctno
				-- and c.datefrom <=@datenow )
				set @statement =''
				set @finalstatement =''
			end

  
			if @statement =''
			begin  
				--print 'doing insert '
				set @insert ='insert into cmstrategyacct (acctno, Strategy, datefrom, dateto,currentstep, dateincurrentstep) '
				set @select=  ' select a.acctno, ' + '''' + @strategy + '''' + ',' 
				+ '''' + convert(varchar,@datenow, 121) + '''' +  ',' --IP - 20/10/09 - UAT(910) - Changed to use 121 to include timestamp.
				+  'NULL,1,'
				+ '''' + convert(varchar,@datenow, 121) + ''''  --IP - 20/10/09 - UAT(910)
				--set @statement =' from acct a,CMStrategyvariables c where '
				
				set @statement =' from acct a With(NoLock) ,' + @StrategyVariabletable + ' c where '
				+ '((a.arrears >0 and a.outstbal >0) or a.bdwbalance > 0 or a.bdwcharges > 0 or c.strategy =''REP'' or c.strategy =''DFA'')' --09/09/09 jec UAT835  --IP - 15/12/09 - UAT(937) - Removed clause 'or c.strategy =''SER''  -- IP - 16/12/09 - UAT(857) --IP - 17/12/09 - UAT(929) - Include DFA
				+ ' and a.acctno = C.acctno  ' 
				+  ' and c.strategy =' + '''' + @strategy + ''''
			end

  
			if @orclause = @previousOr and @previousor !='' and  @PreviousStrategy =@Strategy  -- we need an OR 
			begin
				set @statement = @statement + ' or ' + @condition +'=''Y'' ' 
			end
         
			if @previousor ='' and @orclause !='' -- new OR clause
			begin
				set @statement = @statement + ' and (' + @condition +'=''Y'' '
			end
         
			if  @orclause ='' 
			begin -- there was a previous or which is now terminated so normal and
				set @statement = @statement + ' and ' + @condition +'=''Y'' '
			end
         
			if @orclause != @previousOr and @previousor !='' and @orclause !='' -- close previous or add annother and and another bracked
			begin
				set @statement = @statement + '  and ( ' + @condition +'=''Y'' ' 
			end
         
			--select @orclause,@previousor
                  
			set @previousOr =@orclause
			set @PreviousStrategy=@Strategy
			set @counter = @counter + 1
		end
		
		FETCH NEXT FROM AllocateStrategy_cursor INTO @Strategy,@Condition,@Operand,@Operator1,@Operator2,
           @OrClause,@step,@ActionCode,@StepActiontype,@ConditionType 

   END
		CLOSE AllocateStrategy_cursor
		DEALLOCATE AllocateStrategy_cursor
 

		-- now do it for last strategy
		if @previousor !='' 
		begin -- there was a previous or which is now terminated so close the bracket and continue with the and conditions.
			set @statement = @statement + ')'
        end







		--   print @statement
		--set @statement = @insert + @newline + @select + @newline + @statement 
		--17/07/09 - IP/JC - correct statement for final condition to include not exist clause.
   
		set @finalstatement =@insert + @newline + @select + @newline + @statement + 
             'and not exists (select 1 from cmstrategyacct sa With(NoLock) where sa.acctno = a.acctno ' 
              -- don't insert into strategy if already in another arrears strategy. (do if in Non-arrers strategy)
              + ' and sa.dateto is null and sa.strategy !=''NON'' and sa.strategy !=''SCNON'' ) '				-- jec 17/02/12 storecard non arrears 
		execute sp_executesql @finalstatement
		-- print @statement
  
  
		--17/07/09 - IP/JC - Setting the dateto of previous strategy for accounts now in a new strategy
		----IP - 20/10/09 - UAT(910) - Moved below update to here as this should be done before sending all accounts of the same customer to high priority strategy.
    
		update CMStrategyAcct set dateto = @datenow
		from CMStrategyAcct sa
		where exists(select 1 from CMStrategyAcct a With(NoLock)
		where sa.acctno = a.acctno 
		and sa.strategy!=a.strategy			-- UAT747 
		--and sa.datefrom != convert(varchar,@datenow) 
		and a.datefrom = @datenow) --IP - 20/08/09 - UAT(799)
		and sa.dateto is null --IP - 01/10/09 - UAT(855) - Reinstated this line.
		and  sa.datefrom != @datenow

    
		-- NM 14/07/2009 -- Sending all the accounts of the same customer to high priority strategy -------------
		select  ca.custid, st.acctno, st.strategy, st.datefrom, st.dateto, st.currentstep, 
		st.dateincurrentstep, Convert(int, c.additional) as StrategyPriority 
		into #TempStrategyAcct
		from cmstrategyacct st With(NoLock)
		inner join custacct ca With(NoLock) on ca.acctno = st.acctno AND ca.hldorjnt = 'H' --IP/JC - 09/12/09 - UAT(910)
		left join code c With(NoLock) on c.category = 'SS1' and c.code = st.strategy
		where st.dateto is NULL and c.additional is not null and IsNumeric(c.additional) = 1
		order by ca.custid,c.additional 

		----------------------------------------------------------------------------------------
		DECLARE CUR1 CURSOR for SELECT custid FROM #TempStrategyAcct group by custid having  count(*) > 1
		OPEN CUR1;

		DECLARE @custid varchar(20), @highPrioAcct CHAR(12),  @highPrioStrategy varchar(6), @highPrioCurrentStep smallint, 
			@highPrioDateInCurStep datetime, @highPrioWorkList varchar(6), @highPrioWorkListFromDate datetime 
	
		FETCH NEXT FROM CUR1 INTO @custid

		WHILE @@FETCH_STATUS = 0   
		BEGIN   
			select top 1 @highPrioAcct = acctno, @highPrioStrategy = strategy, @highPrioCurrentStep = currentstep, @highPrioDateInCurStep = dateincurrentstep
			from #TempStrategyAcct 
			where custid = @custid order by StrategyPriority asc, dateincurrentstep DESC
		
			select top 1 @highPrioWorkList = ISNULL(worklist, ''), @highPrioWorkListFromDate = datefrom
			from CMWorklistsAcct 
			where acctno = @highPrioAcct and strategy = @highPrioStrategy order by datefrom DESC 
		
			--Updating CMStrategyAcct dateto value ----------------------------------------
			update cmstrategyacct set dateto = @datenow
			from cmstrategyacct st With(NoLock)
			inner join #TempStrategyAcct temp on temp.acctno = st.acctno and temp.strategy = st.strategy and temp.datefrom = st.datefrom
			where temp.custid = @custid and st.strategy != @highPrioStrategy
			AND st.dateto IS NULL --IP - 17/12/09 - caused duplicate key error.
			--
		
			--Allocating accounts to high priority strategy -------------------------------
			insert into cmstrategyacct(acctno, strategy, datefrom, dateto, currentstep, dateincurrentstep)
			select temp.acctno, @highPrioStrategy, @datenow, null, @highPrioCurrentStep, @highPrioDateInCurStep
			from #TempStrategyAcct temp 
			where temp.custid = @custid and temp.strategy != @highPrioStrategy
			--



		
			--Adding a record to BailAction table -----------------------------------------
			insert into bailaction (origbr, acctno,allocno, actionno, empeeno, dateadded, code, actionvalue, 
			datedue, amtcommpaidon, notes, addedby)
			select 0, temp.acctno, 0, MAX(IsNull(ba.actionno,0)) + 1, 0, @datenow, 'ALL', 0, 
			'1900-01-01 00:00:00.000', 0, 'Allocated to higher strategy to match other customer account' , 0 
			from #TempStrategyAcct temp
			left join bailaction ba With(NoLock) on ba.acctno = temp.acctno 
			where temp.custid = @custid and temp.strategy != @highPrioStrategy
			group by temp.acctno
			--
			
			--Remove the strategy accounts they have future start date----------------------
			delete cmstrategyacct
			from cmstrategyacct st
			inner join #TempStrategyAcct temp on temp.acctno = st.acctno and temp.strategy = st.strategy and temp.datefrom = st.datefrom
			where st.datefrom > @datenow and temp.custid = @custid and st.strategy != @highPrioStrategy 
			--

			IF @highPrioWorkList != ''
			BEGIN

				--Remove worklist accounts they have newer start date than the new record ----
				delete CMWorklistsAcct 
				from CMWorklistsAcct wl 
				inner join #TempStrategyAcct temp on temp.acctno = wl.acctno and temp.strategy = wl.strategy
				where wl.datefrom > @highPrioWorkListFromDate and temp.custid = @custid and wl.strategy != @highPrioStrategy
				--

				
				--Updating CMWorklistAcct dateto value ---------------------------------------
				update CMWorklistsAcct set dateto = @highPrioWorkListFromDate
				from CMWorklistsAcct wl
				inner join #TempStrategyAcct temp on temp.acctno = wl.acctno and temp.strategy = wl.strategy
				where wl.dateto is NULL and temp.custid = @custid and wl.strategy != @highPrioStrategy
				--

				--insert into CMWorklistsAcct_Investigation(acctno, Worklist, Datefrom, Strategy, Dateto,Place)
				--select temp.acctno, @highPrioWorkList, @datenow, @highPrioStrategy, NULL,1 --IP - 10/12/09 - UAT(910) - Datefrom should be inserted as @datenow, was previously @highPrioWorkListFromDate
				--from #TempStrategyAcct temp 
				--where temp.custid = @custid and temp.strategy != @highPrioStrategy
			 
			


				--Allocating accounts to higher priority worklists ---------------------------
				insert into CMWorklistsAcct(acctno, Worklist, Datefrom, Strategy, Dateto)
				select Distinct temp.acctno, @highPrioWorkList, @datenow, @highPrioStrategy, NULL --IP - 10/12/09 - UAT(910) - Datefrom should be inserted as @datenow, was previously @highPrioWorkListFromDate
				from #TempStrategyAcct temp 
				where temp.custid = @custid and temp.strategy != @highPrioStrategy
			    AND NOT EXISTS (SELECT 1 FROM CMWorklistsAcct W WITH(NoLock) WHERE  w.acctno =temp.acctno and w.strategy = @highPrioStrategy and w.datefrom =@datenow) ---Added by CP on 27/08/2019
				--



			END
				
			FETCH NEXT FROM CUR1 INTO @custid 
		END   

		CLOSE CUR1   
		DEALLOCATE CUR1
	----------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------
    

		set @return = @@error
	 
		if @return !=0
		begin
			Print 'failure in CMEod_AssignAccountstoStrategiesSP '
			print @statement
			return 
		end

GO