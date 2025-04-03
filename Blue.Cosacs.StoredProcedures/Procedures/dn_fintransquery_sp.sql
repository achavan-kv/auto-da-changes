SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_fintransquery_sp]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_fintransquery_sp]
GO
create procedure dn_fintransquery_sp
@transtypeoperand varchar(3), -- '=' or 'set' or 'ALL'
@transtypecode varchar(4), -- if one transtypecode
@valueoperand varchar(2), -- '<' or '>='
@startrunno int, -- startrunno or runno if =
@endrunno int,
@runnooperand varchar(12), -- between or = or > 
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
@valueonly bit
as


-- query to retrieve data for 
declare @statement sqltext, @doneone smallint --done one will be used to determine if ' and ' is required
,@nl varchar(32),@idatestart datetime, @nextdatestart datetime
-- code to ensure datestart has 00:00:00 time and datefinish has 23:59:00 time
-- livewire 68186   jec
declare @hh int,@mm int,@ss int

set @hh=datepart(hour,@datestart)
set @mm=datepart(minute,@datestart)
set @datestart=dateadd(hour,0-@hh,@datestart)
set @datestart=dateadd(minute,0-@mm,@datestart)

set @hh=datepart(hour,@datefinish)
set @mm=datepart(minute,@datefinish)
set @datefinish=dateadd(hour,23-@hh,@datefinish)
set @datefinish=dateadd(minute,59-@mm,@datefinish)

create table #fintrans (acctno char(12),datetrans datetime, transtypecode varchar(3), transvalue money, empeeno int,runno int, source varchar(10))

set @nl = '
'
set @doneone = 0
set @statement =' insert into #fintrans (acctno,datetrans,transtypecode,transvalue,empeeno,runno,source) ' +
  'SELECT ' +
  'ACCTNO,DATETRANS,TRANSTYPECODE,TRANSVALUE,EMPEENO,runno,source ' +
  ' FROM FINTRANS f ' +
  ' WHERE ' 
  
if @transtypeoperand ='='
begin
   set @doneone = 1
   set @statement = @statement + ' transtypecode = ' + '''' + @transtypecode + ''''
end

if @transtypeoperand ='set'
begin
   set @doneone = 1
   set @statement = @statement + '  exists (select * from setdetails s where '
    + ' s.tname = ' + '''' + 'transtype' + '''' + ' and s.setname = ' + '''' +  @transtypesetname + '''' +
    + ' and s.data = f.transtypecode) '
end
if @valueoperand = '<' or @valueoperand= '>='
begin
   if @doneone = 0 
	set @doneone = 1
   else
        set @statement = @statement + @nl + ' and '
   set @statement =@statement + ' transvalue ' + @valueoperand + ' 0 '
end



if @runnooperand = 'between'
begin
  if @doneone = 0 
	set @doneone = 1
   else
        set @statement = @statement + @nl + ' and '
   set @statement =@statement + ' runno between ' + convert(varchar,@startrunno) + ' and ' + convert(varchar,@endrunno)
--TODO int and admin charges posted in any run don't get set to zero until account settled need to pick up these based on dates

end
else 
begin
   set @statement =@statement + ' runno ' + @runnooperand + ' ' + @startrunno
end

-- empeenooperand varchar(4) -- '=' or 'ALL' - ignore ALL 
if @empeenooperand='='
begin
  if @doneone = 0 
	set @doneone = 1
   else
        set @statement = @statement + @nl + ' and '
   set @statement =@statement + ' empeeno ='  + convert( varchar,@empeeno)
end

if @branchnooperand ='set'
begin
   if @doneone = 0 
	set @doneone = 1
   else
        set @statement = @statement + @nl + ' and '
   set @statement = @statement + '  exists (select * from setdetails s where '
    + ' s.tname = ' + '''' + 'branch' + '''' + ' and s.setname = ' + '''' +  @branchnooperand + '''' +
    + ' and convert(smallint,s.data) = f.branchno) '
end
else
if @branchnooperand ='='
begin
   if @doneone = 0 
	set @doneone = 1
   else
        set @statement = @statement + @nl + ' and '
   set @statement = @statement + ' branchno = ' + convert(varchar,@branchno)
end 

declare @varbranch varchar(6)
if @accountinbranch !=0 --only restrict if not zero
begin
   set @varbranch = convert(varchar,@accountinbranch) + '%'
   if @doneone = 0 
	set @doneone = 1
   else
        set @statement = @statement + @nl + ' and '
   set @statement = @statement + ' acctno like ' + '''' + @varbranch + ''''
end 

if @dateoperand ='between'
begin
   if @doneone = 0 
	set @doneone = 1
   else
        set @statement = @statement + @nl + ' and '
   set @statement = @statement + ' datetrans between ' + '''' + convert(varchar,@datestart) + '''' +
        ' and ' + + '''' + convert(varchar,@datefinish) + ''''
end
else
if @dateoperand ='>=' or  @dateoperand ='<'
begin
   if @doneone = 0 
	set @doneone = 1
   else
        set @statement = @statement + @nl + ' and '
   set @statement = @statement + ' datetrans ' + @dateoperand  + '''' + convert(varchar,@datestart) + '''' 
end

print @statement
execute sp_executesql @statement   
if @valueonly = 1
    select sum(transvalue) as transvalue from #fintrans
else
    select  acctno,datetrans,transtypecode,transvalue,empeeno,runno,source from #fintrans
GO
/* code for testing only
exec dn_fintransquery_sp
@transtypeoperand ='set', 
@transtypecode ='PAY',
@valueoperand = '',
@startrunno =425,
@endrunno =427,
@runnooperand ='between', -- between or = or > 
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
@valueonly =0 */
go
