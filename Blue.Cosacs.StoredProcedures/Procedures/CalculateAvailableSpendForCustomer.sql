if  exists (select * from sysobjects  where name =  'CalculateAvailableSpendForCustomer' )
drop procedure CalculateAvailableSpendForCustomer
go

-- ============================================================================================
-- Author:		Ilyas Parker
-- Create date: 25/11/10
-- Description:	Procedure that re-calculates the available spend for a customer
-- Change Control
-- --------------

-- ============================================================================================
create PROCEDURE CalculateAvailableSpendForCustomer 
			@custid varchar(20)
        
as 

set nocount on

declare @available decimal
	
if exists(select * from customer
			where custid = @custid
			and ((rfcreditlimit > 0  AND creditblocked = 0) OR ISNULL(AvailableSpend,0) <>0 )
			 )
	BEGIN 
			EXEC DN_CustomerGetRFLimitSP
            @custid = @custid,
            @AcctList ='',
            @limit = 0 ,
            @available=@available  OUT,
            @return = 0
  
		IF @available <0
			SET @available =0
	
		SELECT @available
		
		DECLARE @storeAvailable MONEY 
			
		SELECT @storeAvailable= StoreCardAvailable FROM customer WHERE custid = @custid 
        IF @storeAvailable > @available
			SET @storeAvailable = @available 
        
       if @available is not null
		BEGIN
			
			update customer set availablespend =@available 
			, StoreCardAvailable = @storeAvailable
			where custid =@custid
         and  availablespend !=@available
        END 
  
  
   END
			
GO