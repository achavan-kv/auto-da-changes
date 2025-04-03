if exists (select * from sysobjects where name ='dn_fincoedtransactions')
drop procedure dn_fincoedtransactions 
go
create procedure dn_fincoedtransactions
@datefrom datetime, @dateto datetime, @transtypeset varchar(64), @return int out
 as
SET NOCOUNT ON
declare @statement SQLtext,@counter integer,@transtype varchar(5)
create table #finco (acctno char(12), transvalue money, transtypecode varchar(3),datetrans datetime,fincoed char(1))
set @statement = 'insert into #finco select acctno,transvalue, transtypecode,datetrans,''N'' ' +
' from fintrans where datetrans between ' + '''' + convert(varchar,@datefrom) + '''' + ' and ' +  '''' 
+ convert(varchar,@dateto) + '''' 

if @transtypeset !='' 
begin 
 set @statement = @statement +
 ' and transtypecode in ('
set @return = 0
set @counter = 1
DECLARE set_cursor CURSOR 
  	FOR SELECT data 
   from setdetails
   where setname =@transtypeset and tname ='transtype'
   OPEN set_cursor
   FETCH NEXT FROM set_cursor INTO @transtype

   WHILE (@@fetch_status <> -1)
   BEGIN
	   IF (@@fetch_status <> -2)
   	begin
		if @counter > 1
			set @statement = @statement + ','

		set @statement =@statement + '''' + @transtype + ''''
           --select @mincontract,@minacctno,@maxcontract,@maxacctno
   	
		set @counter = @counter + 1
	   END
      FETCH NEXT FROM set_cursor
 INTO @transtype

   END

   CLOSE set_cursor
   DEALLOCATE set_cursor
 set @statement =@statement + ')'
end
exec sp_executesql @statement
--print @statement
update #finco set fincoed = 'Y' from sec_account s where s.acctno= #finco.acctno
and datesecuritised <datetrans

select acctno as AccountNo,transvalue as Value, transtypecode as Code,datetrans AS TransactionDate,FinCoed from #finco
set @return =@@error
return @return

go

