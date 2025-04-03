SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_fintransquerysp]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_fintransquerysp]
GO
create procedure dn_fintransquerysp
@transtypeoperand varchar(3), -- '=' or 'set' or 'ALL'
@transtypecode varchar(4), -- if one transtypecode
@valueoperand varchar(2), -- '<' or '>='
@startrunno int, -- startrunno or runno if =
@endrunno int,
@runnooperand varchar(12), -- between or = or >=
@startrefno int, --startrefno or refno if = 
@endrefno int, 
@refnooperand varchar(12), -- between or >= or = or <
@empeeno int, -- employee number
@empeenooperand varchar(4), -- '=' or 'ALL'
@branchno smallint,
@branchnooperand varchar(3), -- '=' OR 'set' or 'ALL'
@accountinbranch smallint, --only restrict if not zero
@datestart datetime, -- datetime for datestart or >= or <
@datefinish datetime, -- datetime
@dateoperand varchar(12), -- 'between' '>=' or '<'
@branchsetname varchar(32), 
@transtypesetname varchar(32),
@employeesetname varchar(32),
@valueonly bit,
@includeothercharges bit,
@Return INTEGER OUTPUT
as
set @return = 0
set nocount on
-- query to retrieve data for 
declare @statement sqltext, @doneone smallint --done one will be used to determine if ' and ' is required
,@nl varchar(32),@idatestart datetime, @nextdatestart datetime

create table #fintrans (acctno char(12),datetrans datetime, transtypecode varchar(3), transvalue money, empeeno int,runno int,transrefno int, source varchar(10), ftnotes varchar(4))

set @nl = '
'
set @doneone = 0
set @statement =' insert into #fintrans (acctno,datetrans,transtypecode,transvalue,empeeno,runno,transrefno,source, ftnotes) ' +
  'SELECT ' +
  'ACCTNO,DATETRANS,TRANSTYPECODE,TRANSVALUE,EMPEENO,runno,transrefno,source,ftnotes ' +
  ' FROM fintrans f ' +
  ' WHERE '

if @runnooperand !='' and @includeothercharges=0
  set @statement = @statement + ' transtypecode NOT IN (''INT'',''ADM'')  ' -- int and admin don't have their runno stampted until 
else
  set @statement = @statement + ' transtypecode = transtypecode ' -- this is so that the rest of the statements all have ands in them

if @transtypeoperand ='='
begin
   set @statement = @statement + ' and transtypecode = ' + '''' + @transtypecode + ''''
end

if @transtypeoperand ='set'
begin
   set @statement = @statement + @nl + ' and exists (select * from setdetails s where '
    + ' s.tname = ' + '''' + 'transtype' + '''' + ' and s.setname = ' + '''' +  @transtypesetname + '''' +
    + ' and s.data = f.transtypecode) '
end

if @valueoperand = '<' or @valueoperand= '>='
begin
        set @statement = @statement + @nl + ' and '
   set @statement =@statement + ' transvalue ' + @valueoperand + ' 0 '
end

if @runnooperand = 'between'
begin
   set @statement = @statement + @nl + ' and '
   set @statement =@statement + ' runno between ' + convert(varchar,@startrunno) + ' and ' + convert(varchar,@endrunno)
--TODO int and admin charges posted in any run don't get set to zero until account settled need to pick up these based on dates
end
else if @runnooperand !=''
begin
   set @statement =@statement + ' and runno ' + @runnooperand + ' ' + convert(varchar,@startrunno)
end

if @refnooperand = 'between'
begin
   set @statement = @statement + @nl + ' and '
   set @statement =@statement + ' transrefno between ' + convert(varchar,@startrefno) + ' and ' + convert(varchar,@endrefno)
end
else if @refnooperand !=''
begin
   set @statement =@statement + ' and transrefno ' + @refnooperand + ' ' + convert(varchar,@startrefno)
end

-- empeenooperand varchar(4) -- '=' or 'set' or 'ALL' - ignore ALL 
if @empeenooperand ='set'
begin
   set @statement = @statement + @nl + ' and '
   set @statement = @statement + '  exists (select * from setdetails s where '
    + ' s.tname = ' + '''' + 'employee' + '''' + ' and s.setname = ' + '''' +  @employeesetname + '''' +
    + ' and convert(smallint,s.data) = f.empeeno) '
end
else
if @empeenooperand='='
begin
   set @statement = @statement + @nl + ' and '
   set @statement =@statement + ' empeeno ='  + convert( varchar,@empeeno)
end

if @branchnooperand ='set'
begin
   set @statement = @statement + @nl + ' and '
   set @statement = @statement + '  exists (select * from setdetails s where '
    + ' s.tname = ' + '''' + 'branch' + '''' + ' and s.setname = ' + '''' +  @branchsetname + '''' +
    + ' and convert(smallint,s.data) = f.branchno) '
end
else
if @branchnooperand ='='
begin
        set @statement = @statement + @nl + ' and '
   set @statement = @statement + ' branchno = ' + convert(varchar,@branchno)
end 

declare @varbranch varchar(6)
if @accountinbranch !=0 --only restrict if not zero
begin
   set @varbranch = convert(varchar,@accountinbranch) + '%'
   set @statement = @statement + @nl + ' and '
   set @statement = @statement + ' acctno like ' + '''' + @varbranch + ''''
end 

if @dateoperand ='between'
begin
   set @statement = @statement + @nl + ' and '
   set @statement = @statement + ' datetrans between ' + '''' + convert(varchar,@datestart) + '''' +
        ' and ' + + '''' + convert(varchar,@datefinish) + ''''
end
else
if @dateoperand ='>=' or  @dateoperand ='<'
begin
   set @statement = @statement + @nl + ' and '
   set @statement = @statement + ' datetrans ' + @dateoperand  + '''' + convert(varchar,@datestart) + '''' 
end

--print @statement
execute sp_executesql @statement   
declare @datepreviousrun datetime,@datecurrentrun datetime
if @runnooperand !='' and @includeothercharges =1 -- now inserting all interest and admin charges which were posted between the summary runs
begin
  select @datepreviousrun = isnull(max(datefinish),'1900-01-01') from interfacecontrol where interface = 'UPDSMRY' AND runno = (@startrunno -1)
  if @runnooperand = 'between'
  begin 
    select @datecurrentrun = isnull(max(datestart),@datepreviousrun) from interfacecontrol where interface = 'UPDSMRY' AND runno = @endrunno
--TODO int and admin charges posted in any run don't get set to zero until account settled need to pick up these based on dates
  end
  else 
  if @runnooperand = '='
  begin
   select @datecurrentrun = isnull(max(datestart),@datepreviousrun) from interfacecontrol where interface = 'UPDSMRY' AND runno = @startrunno
  end
  else
  if @runnooperand = '>='
  begin
   select @datecurrentrun = isnull(max(datestart),@datepreviousrun) from interfacecontrol where interface = 'UPDSMRY'
  end
   
  --Adjust the derived runno based date range if parts of it fall BEFORE our main date range or AFTER our main date range
  if @dateoperand = '>=' or @dateoperand = '=' -- Make use of @datestart as a START date
  begin
    --@datepreviousrun and @datecurrentrun cannot be before our main start date, adjust if necessary
    if @datestart > @datepreviousrun
    begin
      select @datepreviousrun = @datestart
    end
    if @datestart > @datecurrentrun
    begin
      select @datecurrentrun = @datestart
    end 
  end 
  else
  if @dateoperand = '<' -- Make use of @datestart as an END date
  begin
    --@datepreviousrun and @datecurrentrun cannot be after our main end date, adjust if necessary
    --**NOTE: @datestart contains the END date in this case!!!
    if @datestart < @datepreviousrun
    begin
      select @datepreviousrun = @datestart
    end
    if @datestart < @datecurrentrun
    begin
      select @datecurrentrun = @datestart
    end
  end
  else
  if @dateoperand = 'between' -- Make use of @datestart as a START date, and @datefinish as an END date
  begin
    --@datepreviousrun and @datecurrentrun cannot be before our main start date, adjust if necessary
    if @datestart > @datepreviousrun
    begin
      select @datepreviousrun = @datestart
    end
    if @datestart > @datecurrentrun
    begin
      select @datecurrentrun = @datestart
    end
    --@datepreviousrun and @datecurrentrun cannot be after our main end date, adjust if necessary   
    if @datefinish < @datepreviousrun
    begin
      select @datepreviousrun = @datefinish
    end
    if @datefinish < @datecurrentrun
    begin
      select @datecurrentrun = @datefinish
    end
  end
  if @startrunno =0  -- 68898 exclude zero runnos if runno >0
  begin
  insert into #fintrans (acctno,datetrans,transtypecode,transvalue,
			empeeno,runno,transrefno,source )
  		select acctno,datetrans,transtypecode,transvalue,
		empeeno,runno,transrefno,source 
		from fintrans where
  		datetrans between @datepreviousrun and @datecurrentrun 
		AND transtypecode in ('INT','ADM') 
		and runno = 0 --only want runno = 0 as these haven't been settled yet
  end
end

if @valueonly = 1
    select isnull(sum(transvalue),0) as transvalue from #fintrans
else
    select  acctno,datetrans,transtypecode,transvalue,empeeno,runno,transrefno,source, ftnotes from #fintrans -- where transtypecode ='INT'


go
/* code for testing only
declare @return integer
exec dn_fintransquerysp
@transtypeoperand ='set', 
@transtypecode ='',
@valueoperand = '',
@startrefno =0,
@endrefno =0,
@refnooperand ='', -- between or = or > 
@startrunno =20,
@endrunno =21,
@runnooperand ='between', -- between or = or >= 
@empeeno =0, -- employee number
@empeenooperand ='ALL',
@branchno =354,
@branchnooperand ='ALL', -- '=' OR 'set' or 'ALL'
@accountinbranch =720, --only restrict if not zero
@datestart ='1-JAN-2003', -- datetime for datestart or >= or <
@datefinish ='1-JAN-2004', -- datetime
@dateoperand ='', -- 'between' '>=' or '<'
@branchsetname ='', 
@transtypesetname ='Income',
@employeesetname='',
@valueonly =0,
@includeothercharges =0,
@return = 0 
*/
