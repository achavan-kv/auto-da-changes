if  exists (select * from sysobjects  WHERE name =  'dn_TransportSchedAddsp' )
drop procedure dn_TransportSchedAddsp
go
create procedure dbo.dn_TransportSchedAddsp
@branchno smallint,
@datedel datetime,
@loadno smallint,
@truckid varchar (26),
@printed smallint,
@return integer output
as    
	SET @return =@@error
	
	declare @count int 
	SELECT  @count = count(*)
	  FROM  Transptsched
	 WHERE  branchno = @branchno
	   AND	datedel = @datedel  
	   AND	loadno = @loadno 
		
	IF @count = 0
	BEGIN
		INSERT INTO Transptsched
		(origbr,
		branchno, 
		datedel, 
		loadno,
		truckid,
		printed,
		deliveryslot)
	    
		VALUES (@branchno
		,@branchno
		,@datedel
		,@loadno
		,@truckid
		,@printed
		,1)
	     
		set @return  =@@error
    	END
	ELSE 
		set @return  =@count 
go    