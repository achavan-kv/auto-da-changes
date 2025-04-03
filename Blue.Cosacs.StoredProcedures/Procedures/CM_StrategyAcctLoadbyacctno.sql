
if exists (select * from sysobjects where name = 'CM_StrategyAcctLoadbyAcctno')
drop  procedure CM_StrategyAcctLoadbyAcctno 
go
create procedure CM_StrategyAcctLoadbyAcctno
-- **********************************************************************
-- Title:
-- Developer: Alex Ayscough
-- Date: 2007
-- Purpose:Load up list of strategies - if there are no strategies then
-- assume in non-arrears strategy
-- Changes:
-- 22/05/09 jec UAT541 DateTo incorrectly displayed for NON ARREARS
-- 26/08/09 ip  UAT(819)- added boolean @strategyHasWorklists that checks
-- if the strategy the account currently is in has worklists.
-- 27/08/09 ip UAT(820) - Removed check on @currstatus !='S' as strategy
-- history should be displayed even if the account is settled.
-- Account will display as in deferred strategy if deferred months termstype 
-- 22/04/10 ip UAT(2) - Only display 'NON Non-Arrears' if the account is delivered
-- 12/08/10 jec UAT28 return data in datefrom desc order
-- 24/09/10 jec UAT1005 remove comparison to Getdate & return blank dateto if null
-- 06/01/12 jec #3588 LW73586 - Different date formats
-- 08/03/12 jec #9758 SCNON strategy not updated for storecard accounts
-- ********************************************************************** 
		@acctno char(12) ,
		@strategyHasWorklists bit OUTPUT,
		@return int OUTPUT
AS
    set @return = 0
    declare @currstatus char(1),
			@rowcount INT,
			@strategytext VARCHAR(32)
			
    select @currstatus =currstatus from acct a where a.acctno=@acctno
    select
        c.acctno,
        s.Strategy + ' ' + s.description as Strategy,
        c.datefrom,
        c.dateto,
        c.currentstep,
        c.dateincurrentstep
        into #strategies
        from CMStrategyAcct c,CMStrategy S
        where c.acctno = @acctno
        and C.Strategy = S.strategy
        --and @currstatus!='S'		-- UAT541 --IP UAT(820) - Commented line as should display even for settled accounts.
       
     set  @rowcount=@@rowcount        

    
    if @rowcount=0 and @acctno like '___0%'		-- UAT541
    begin
        if @currstatus !='S' -- if not settled then in non-arrears strategy
        BEGIN
			SET @strategytext ='NON Non-Arrears'
			IF EXISTS (SELECT * FROM acct a ,termstype t 
			WHERE a.acctno= @acctno AND t.termstype= a.termstype 
			AND t.DeferredMonths >0 )
				SET @strategytext ='DFA Deferred Accounts'
            insert into #strategies
            select @acctno,@strategytext,
				case when @strategytext='NON Non-Arrears' then i.datefirst else GETDATE() end,	--jec UAT1005
				null,0,
				case when @strategytext ='NON Non-Arrears'  then i.datefirst else GETDATE() end	--jec UAT1005
            from instalplan i
            where i.acctno = @acctno
            AND i.datefirst > '01/01/1900' --IP - 22/04/10 - UAT(2) UAT5.2
            --AND i.datefirst <= GETDATE()		--jec UAT1005
        end
    end
    
    if @rowcount=0 and @acctno like '___9%'		-- #9758 Storecard accounts					
    begin
        if @currstatus !='S' -- if not settled then in non-arrears strategy
        BEGIN
			SET @strategytext ='SCNON StoreCard Non Arrears'			
            insert into #strategies (acctno, strategy, datefrom, dateto, currentstep, dateincurrentstep)
            select @acctno,@strategytext, ISNULL(s.dateto,GETDATE()),null,0,ISNULL(s.dateto,GETDATE())
            from StoreCardPaymentDetails p LEFT OUTER JOIN  StoreCardStatement s on s.acctno = p.acctno
            where p.acctno = @acctno 
				and (not exists(select * from StoreCardStatement s3 where s3.acctno = @acctno)           
					or (ISNULL(s.dateto,'01/01/1900') > '01/01/1900' 
						AND s.dateto = (select MIN(s2.dateto) from StoreCardStatement s2 where s2.acctno = @acctno) ) )
            
        end
    end
    
    --IP - 26/08/09 - UAT(819)
    if exists(select * from CMStrategyCondition c
			inner join #strategies s on c.Strategy = SUBSTRING(s.strategy, 0, charindex(' ', s.strategy))
			where s.dateto is null
			and c.StepActiontype = 'W'
			and c.SavedType = 'S')
	begin
		set @strategyHasWorklists = 1
	end
	else
	begin
		set @strategyHasWorklists = 0
	end
    -- UAT28 - name columns & return in date order
    select acctno as 'AccountNumber',
        Strategy,
        datefrom as 'Date From',
        dateto as 'Date To',		-- #3588
        --isnull(convert(varchar,dateto,103),' ') as 'Date To',		--UAT1005
        currentstep as 'Current Step',
        dateincurrentstep as 'Date into Current Step' from #strategies
    order by datefrom desc  
    
    
go
--for testing exec CM_StrategyAcctLoadbyAcctno 	@acctno = '780900000341',@strategyHasWorklists =0,	@return =0

-- End End End End End End End End End End End End End End End End End End End End End End End End
