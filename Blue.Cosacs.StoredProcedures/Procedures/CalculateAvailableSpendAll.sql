if  exists (select * from sysobjects  where name =  'CalculateAvailableSpendAll' )
drop procedure CalculateAvailableSpendAll
go
create procedure CalculateAvailableSpendAll 
as 
declare @custid varchar(20),@RFcreditlimit money,@available money,@counter int
set @counter = 0
set nocount on

DECLARE customer_cursor CURSOR 
  	FOR SELECT custid ,RFcreditlimit
   from customer where rfcreditlimit >0 and creditblocked =0
    
   OPEN customer_cursor
   FETCH NEXT FROM customer_cursor INTO @custid,@RFcreditlimit

   WHILE (@@fetch_status <> -1)
   BEGIN
	   IF (@@fetch_status <> -2)
   	begin

			EXEC DN_CustomerGetRFLimitSP
            @custid = @custid,
            @AcctList ='',
            @limit =@RFcreditlimit ,
            @available=@available  OUT,
            @return = 0
       if @available !=0 and @available is not null
			update customer set availablespend =@available where custid =@custid
         and  availablespend !=@available
			set @counter = @counter +1
         if @counter%1000 = 0
             print convert (varchar ,@counter) + ' available spend done'
   	
	   END
      FETCH NEXT FROM customer_cursor INTO @custid,@RFcreditlimit

   END

   CLOSE customer_cursor
   DEALLOCATE customer_cursor




go
