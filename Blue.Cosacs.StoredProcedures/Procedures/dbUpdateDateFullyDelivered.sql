if exists (select * from sysobjects where name = 'dbUpdateDateFullyDelivered')
    drop procedure dbUpdateDateFullyDelivered
go
create procedure dbUpdateDateFullyDelivered @acctno char(12)
as
declare @status integer, @datedel datetime, @datetrans datetime,@transvalue money,@total money,@agrmttotal money
set @total = 0
select  @agrmttotal= agrmttotal from agreement where acctno = @acctno
-- loop through delivery records to set earliest fully delivered date
set nocount on
DECLARE delivery_cursor CURSOR 
  	FOR SELECT acctno , datetrans, transvalue
   from fintrans
   where Transtypecode In ('DEL', 'GRT','ADD') and acctno =@acctno
   order by datetrans
   OPEN delivery_cursor
   FETCH NEXT FROM delivery_cursor INTO @acctno,@datetrans,@transvalue

   WHILE (@@fetch_status <> -1)
   BEGIN
	   IF (@@fetch_status <> -2)
   	begin
          set @total=@total + @transvalue          
          if @total >=@agrmttotal
          begin
              update agreement set datefullydelivered=@datetrans where acctno =@acctno
              break
          end
	   END
      FETCH NEXT FROM delivery_cursor INTO @acctno,@datetrans,@transvalue

   END

   CLOSE delivery_cursor
   DEALLOCATE delivery_cursor	

go