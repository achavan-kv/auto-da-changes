/*
    Modified by John Croft
    Date:    17 May 2006
            AA 14 Jun 2006
        @whereclause  varchar(2000)
        @totals = 'N'
*/

if exists (select * FROM sysobjects where  name ='dn_CustomerMailingBuildstring')
drop procedure dn_CustomerMailingBuildstring
go
create procedure dn_CustomerMailingBuildstring @set varchar(64),@whereclause  sqltext OUT,
@keyfield varchar(12), --acctno or custid
@notclause varchar(3) ='', -- will have NOT in
@table varchar(32) =null,
@table2 varchar(32) =null,
@column varchar(32) ,
@columndate varchar(32) =NULL ,
@startdate  datetime =NULL ,
@enddate  datetime =NULL ,
@totals char(1) =''
as
if @whereclause is null
   set @whereclause =' ' 
set @whereclause = @whereclause + ' '
--select @set as setname
if @table is null
    select @table = tname from sets where setname = @set
declare @intorstring varchar(2)
select @intorstring = isnull(columntype,'') from sets where setname =@set

-- if @totals has been set then doing a direct load from delivery/lineitem
-- otherwise if @totals ='' then doing a subselect using exists

if @table2 is null
   set @table2 = @table
if @totals = 'Q' or @totals = 'V'  -- if we are getting from delivery table then not doing sub select query
begin
    if @table ='delivery' and @notclause =' ' 
        set @whereclause =@whereclause -- + ' and '
    else  -- do subselect
       begin
           set @whereclause =@whereclause + ' and ' + @notclause + ' exists (select * from ' + @table

       end
end
--print 'buildstring @totals.... ' + @totals
if @totals ='N' -- no totals
   begin
    set @whereclause =@whereclause + ' and ' + @notclause + ' exists (select * from ' + @table

   end

if @table ='lineitem' or @table = 'delivery' and @totals ='N' --(@totals = 'Q' or @totals = 'V')
    set @whereclause = @whereclause + ' ,stockitem'


if @totals = 'N' -- no totals
    set @whereclause =@whereclause + ' where ' + @table + '.' + @keyfield + ' = ' 

if @keyfield = 'custid' --and @table != 'custcatcode'    -- AA 27/06/06
    set @whereclause = @whereclause + ' cu.custid ' 
else 
begin
  if @totals = 'N'
    set @whereclause = @whereclause + ' agreement.acctno  ' 
  if (@table ='lineitem' or @table = 'delivery') and @totals = 'N'
  begin
     set @whereclause = @whereclause +
     ' and stockitem.itemno = ' +@table + '.itemno and ' +
     ' stockitem.stocklocn = ' + @table + '.stocklocn  ' +
     ' and ' + @table + '.agrmtno = agreement.agrmtno '
  end
                     
end
   --print 'here' + @whereclause
set @whereclause = @whereclause + ' and ' + @table2 + '.' + @column + ' in( '
-- build up the in clause 
declare @setdata varchar(32),@counter int
set @counter = 0
--select * from setdetails
    DECLARE set_cursor CURSOR 
  	FOR SELECT isnull(data,'')
        from setdetails
	 where setname = @set

   OPEN set_cursor
   FETCH NEXT FROM set_cursor INTO @setdata

   WHILE (@@fetch_status <> -1)
   BEGIN
       IF (@@fetch_status <> -2)
       BEGIN
            set @counter = @counter + 1

            if @counter > 1
                set @whereclause =@whereclause + ','
            if @intorstring = 'V' -- for varchar
                set @whereclause =@whereclause + ''''

            set @whereclause =@whereclause + @setdata
            if @intorstring = 'V' -- for varchar
                set @whereclause =@whereclause + ''''

            IF @COUNTER >1000 --wont have more than 1000 items
               break    

       END
       FETCH NEXT FROM set_cursor INTO @setdata
   END
   CLOSE set_cursor
   DEALLOCATE set_cursor
-- end the in clause + exits clause


set @whereclause = @whereclause + ')'

if @columndate is not null
begin
    set @whereclause = @whereclause + ' and ' + @table + '.' + @columndate + ' between ' + '''' + convert(varchar,@startdate) + ''''
    + ' and ' + '''' + convert(varchar,@enddate) + ''''
--print '@Columndate.... ' + @columndate + 'Table... ' +  @table 
end
--if not (@table ='delivery' and @notclause ='')
if @totals = 'N'    -- no totals
    set @whereclause = @whereclause + ')'

--print 'buildstring.(where)... ' + @whereclause
--select 'buildstring.(where)... ', @whereclause as bld@where

go
