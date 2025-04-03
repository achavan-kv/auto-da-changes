SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from sysobjects where name = 'dbremovedeliverydates')
drop procedure dbremovedeliverydates
go
-- AA 14 Feb 06 No longer using branch at a time as I think sql server's transaction log
-- is big enough to handle this.
-- AA 23 Jun 06 Changing completely to improve performance
CREATE procedure dbremovedeliverydates @return int OUT
 --@branchno smallint removing branch number from arrears calculation call
as
begin
 set @return = 0
declare
/* AA - 11/04/02  excluding m  type  accounts from process*/
    @varbranch varchar(6) , /*no longer doing it a branch at a time */
    @status integer , 
    @globdelpcent float,
    @count int, @runno integer
    set nocount on
    -- we are interested in accounts which have had goods returned in the last month
    select @runno = isnull(min(runno),999999) from interfacecontrol 
    where interface = 'updsmry' and datestart> dateadd (month, -1, getdate())
    /* globdelpcent is the percentage delivered at which the delivery
    date is set different for each country usually about 50 or 75% */
    SELECT @globdelpcent =globdelpcent from country
/*    if @branchno > 0 
        set @varbranch = convert(varchar,@branchno)
    else
        set @varbranch =N''        

    set @varbranch = @varbranch + '0%'
*/
    declare @deliveryamounts table
    (acctno char(12) not null primary key,
     balance money,
     deltotal money,
     agrmttotal money)

    insert into @deliveryamounts
    (acctno,    balance,    deltotal,    agrmttotal)
    --here we are checking accounts which have had collections in the last month
    select a.acctno,a.outstbal,0, a.agrmttotal 
    from acct a, agreement g
    where a.acctno = g.acctno and
    g.datedel > '1-jan-1990' and
    a.outstbal >0 and
    a.currstatus !=N'S' and
    A.accttype  not in ('M','S')  and -- exclude addto + special accounts
    exists  ( select acctno  from 
	      fintrans f where f.transtypecode in ('DEL','GRT','ADD','CLD') and transvalue < 0		-- #10138
              and a.acctno = f.acctno and (runno >@runno or runno = 0)) 
    and g.agrmtno = 1

    set @status = @@error

    if @status = 0
    BEGIN    
        select sum(f.transvalue) as transvalue, f.acctno 
        into #ftdel
        from fintrans f, @deliveryamounts d
        where f.transtypecode in ('DEL','GRT','ADD','CLD') and			-- #10138
        d.acctno = f.acctno
        group by f.acctno
    
        set @status = @@error
    END

    if @status = 0 
    BEGIN
         create  clustered index ixftdelam on  #ftdel (acctno)
        update d
        set deltotal = f.transvalue
        from #ftdel f, @deliveryamounts d
        where d.acctno = f.acctno 

        set @status = @@error
    END

    delete from @deliveryamounts where agrmttotal =0;
     
    if @status = 0
    BEGIN
 
        update agreement set datedel =NULL
        where exists 
        (select acctno from @deliveryamounts d
        where @globdelpcent > (d.deltotal/d.agrmttotal)* 100  and d.acctno= agreement.acctno)

        update instalplan set datelast =NULL
        where exists
        (select acctno from @deliveryamounts d
        where @globdelpcent > (d.deltotal/d.agrmttotal) * 100 and d.acctno= instalplan.acctno)

        SELECT @status =@@error, @count = @@rowcount
        if @status =0 
        BEGIN
            update acct set arrears = 0 where acctno in
            (select acctno from @deliveryamounts
            where @globdelpcent > (deltotal/agrmttotal) * 100)
            SELECT @status =@@error, @count = @@rowcount
        END
    END
    set @return = @status
    return @return
end
go

