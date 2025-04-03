if exists (select * FROM sysobjects where  name ='dn_CustomerMailingBuildLikestring')
drop procedure dn_CustomerMailingBuildLikestring
go
create procedure dn_CustomerMailingBuildLikestring @set varchar(64),@whereclause  varchar(2000) OUT,
@keyfield varchar(12), --acctno or custid
@notclause varchar(3)='', -- will have NOT in
@table varchar(32)= null,
@table2 varchar(32) = null,
@column varchar(32) ,
@columndate varchar(32)=NULL ,
@startdate  datetime =NULL ,
@enddate  datetime =NULL 
as 

if @table is null
    select @table = tname from sets where setname = @set

if @table2 is null
   set @table2 = @table
set @whereclause =@whereclause + ' and ' + @notclause + ' exists (select * from ' + @table
+ ' where ' + @table + '.' + @keyfield + ' = ' 

if @keyfield = 'custid'
    set @whereclause = @whereclause + ' customer.custid ' 
else 
begin
    set @whereclause = @whereclause + ' agreement.acctno '
                      
end

set @whereclause = @whereclause + ' and (' 
-- build up the in clause 
declare @setdata varchar(32),@counter int
set @counter = 0
    DECLARE set_cursor CURSOR 
  	FOR SELECT data
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
                set @whereclause =@whereclause + ' OR '
--            set @whereclause =@whereclause + ''''
            set @whereclause =@whereclause + '(' + @table2 + '.' + @column + ' like ' + '''' + @setdata + '%' + '''' + ')'

       END
       FETCH NEXT FROM set_cursor INTO @setdata
   END
   CLOSE set_cursor
   DEALLOCATE set_cursor

set @whereclause = @whereclause + ')'

if @columndate is not null
begin
    set @whereclause = @whereclause + ' and ' +  @columndate + ' between ' + '''' + convert(varchar,@startdate) + ''''
    + ' and ' + '''' + convert(varchar,@enddate) + ''''
end

set @whereclause = @whereclause + ')'

go
