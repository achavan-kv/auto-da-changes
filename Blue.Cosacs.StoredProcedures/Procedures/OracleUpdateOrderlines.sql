


	IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'OracleUpdateOrderlines')
		DROP PROCEDURE OracleUpdateOrderlines --1000,'N'
	GO 
	CREATE PROCEDURE [dbo].[OracleUpdateOrderlines] @runno INT,@service CHAR(1) ='N' AS 
	declare @acctno char(12),@agrmtno INT,@linenumber INT ,
	@itemno VARCHAR(10),@contractno VARCHAR(10),
	@stocklocn SMALLINT,@prevacctno CHAR(12),@prevagrmtno int , @counter INT ,@neworderno INT ,@currentline INT ,@serialno int
	SET @prevacctno = ''
	SET @prevagrmtno = 0
	SET @linenumber =0
	SET @counter =0 
	SET @serialno  =0

 

	UPDATE LineitemOracleExport SET orderno =(SELECT MAX(x.orderno) 
	FROM lineitemOracleExport x 
	WHERE  x.acctno= lineitemOracleExport.acctno AND x.agrmtno =lineitemOracleExport.agrmtno )
	WHERE  LineitemOracleExport.orderno IS NULL AND lineitemOracleExport.runno= @runno  

	-- previous order with same line number
	update LineitemOracleExport
	set orderlineno=0
	where type='O'
	and exists (SELECT 'x'
	FROM lineitemOracleExport x1
	WHERE  LineitemOracleExport.acctno= x1.acctno AND LineitemOracleExport.agrmtno =x1.agrmtno 
	and LineitemOracleExport.itemno=x1.itemno and LineitemOracleExport.stocklocn=x1.stocklocn
	and LineitemOracleExport.contractno=x1.contractno 
	and isnull(LineitemOracleExport.orderlineno,0)=isnull(x1.orderlineno,0)
	and x1.SerialNo>LineitemOracleExport.SerialNo and x1.type='O')
	and LineitemOracleExport.runno=@runno

 
	
	
	-- deliveries without order

	
	
    INSERT INTO LineitemOracleExport (		acctno,		agrmtno,		itemno,		contractno,		quantity,		stocklocn,
		ordval,		[type],		runno,		buffno,		orderno,		orderlineno	)  	
	select acctno,	agrmtno,	itemno,	contractno,	quantity,	stocklocn,	ordval,	'O',@runno	,0,	orderno	,0
	from  lineitemoracleexport x
WHERE x.runno= @runno  
	and x.quantity>0
	and x.type='D'   -- delivery
and not exists (SELECT 'x'
	FROM lineitemOracleExport x1 
	WHERE  x.acctno= x1.acctno AND x.agrmtno =x1.agrmtno 
	and x.itemno=x1.itemno and x.stocklocn=x1.stocklocn
	and x.contractno=x1.contractno 
	and x1.type='O'
	and x1.runno<=@runno	-- no previous order
	and not exists
	(SELECT 'x'
	FROM lineitemOracleExport x2 
	WHERE  x2.acctno= x1.acctno AND x2.agrmtno =x1.agrmtno 
	and x2.itemno=x1.itemno and x2.stocklocn=x1.stocklocn
	and x2.contractno=x1.contractno  and x1.orderlineno=x2.orderlineno
	and (x2.type='C' or x2.type='D' and x2.quantity<0
	and x2.runno<=@runno)))
	

 
	
	--here we are getting the total number to update. 
	DECLARE @counttoupdate INT ,@highOrderno INT , @exorderno int
	SELECT @counttoupdate = COUNT ( acctno + CONVERT(VARCHAR,agrmtno )) 
	FROM lineitemoracleexport
	WHERE orderNo is NULL
	GROUP BY acctno, agrmtno 
	SET @counttoupdate = @@ROWCOUNT

	UPDATE OracleOrderNo SET orderno = orderno + @counttoupdate
	 
	SELECT @highOrderno=MAX(orderno ) FROM OracleOrderNo


	-- we are going to update the LineitemOracleExport table first before applying this change 
	-- to the lineitem table - better for performance reasons.
	-- first issue is that 


	SET NOCOUNT ON 

	declare acct_cursor CURSOR FOR
	SELECT acctno,agrmtno,itemno,contractno,stocklocn ,isnull(orderlineno,0) 
	FROM LineitemOracleExport L WHERE runno= @runno AND (ISNULL(ORDERno,0)=0 OR ISNULL(orderlineno,0) IN (0,-1,-2) ) 
	AND NOT (TYPE = 'D' AND EXISTS (SELECT 'x' FROM lineitemOracleExport X WHERE l.acctno= x.acctno AND l.itemno= x.itemno 
															   AND l.stocklocn=  x.stocklocn AND l.agrmtno = x.agrmtno
															   AND l.contractno = x.contractno AND RUNNO = @runno
															   AND l.TYPE = 'O')) 
	group by acctno,agrmtno,itemno,contractno,stocklocn ,isnull(orderlineno,0) 
	order by acctno DESC,agrmtno DESC ,itemno ASC


	OPEN acct_cursor 
	FETCH NEXT FROM acct_cursor INTO @acctno,@agrmtno,@itemno ,@contractno,@stocklocn,@currentline
	WHILE @@FETCH_STATUS = 0
	BEGIN

		SET @linenumber = @linenumber + 1
		 
		IF (@prevacctno !=@acctno OR @prevagrmtno != @agrmtno )--AND @prevacctno !=''
		BEGIN

			SELECT @linenumber = ISNULL(MAX(orderlineno) ,1) +1,
			@neworderno= ISNULL(MAX(orderno),0)
			FROM lineitemOracleExport 
			WHERE acctno= @acctno AND agrmtno = @agrmtno
			-- for deliveres after repors/collections we have set a minimum order number which is going to be used to make sure we get a new order number
				
			IF @neworderno =0
			BEGIN
				SET @linenumber = 1
				SET @highOrderno = @highOrderno -1 	
				SET @neworderno = @highOrderno
			END
			ELSE
			  IF @linenumber IS NULL
				SET @linenumber = 1
		END

		IF EXISTS (SELECT 'x' FROM lineitemOracleExport WHERE  orderno = @neworderno 
		AND (acctno != @acctno OR agrmtno !=@agrmtno)  )
		BEGIN -- no duplicates allowed
			UPDATE OracleOrderNo SET orderno = orderno + 1
			SELECT  @exorderno=MAX(orderno) FROM OracleOrderNo
			UPDATE LineitemOracleExport SET orderno =@exorderno,
			orderlineno = @linenumber
			WHERE 
			acctno= @acctno AND agrmtno = @agrmtno
			AND itemno= @itemno AND contractno= @contractno AND stocklocn =@stocklocn	
			AND runno = @runno  AND ISNULL(orderlineno,0) = ISNULL(@currentline,0)
					and ( ISNULL(orderno,0)<1 or  ISNULL(orderlineno,0)<1)
		END 
		ELSE
		BEGIN 

			UPDATE LineitemOracleExport SET orderno =@neworderno,
			orderlineno = @linenumber
			WHERE 
			acctno= @acctno AND agrmtno = @agrmtno
			AND itemno= @itemno AND contractno= @contractno AND stocklocn =@stocklocn	
			AND runno = @runno  AND ISNULL(orderlineno,0) = ISNULL(@currentline,0) 
			and ( ISNULL(orderno,0)<1 or  ISNULL(orderlineno,0)<=0)
			--AND SerialNo = @serialno --was commented out, now put back in... 
		END	
		

			  
			--SELECT @@ROWCOUNT AS numupdated,@currentline AS currentline 
		SET @prevacctno=@acctno
		SET @prevagrmtno=@agrmtno 

		
	FETCH NEXT FROM acct_cursor INTO @acctno,@agrmtno,@itemno ,@contractno,@stocklocn,@currentline 
	END
	CLOSE acct_cursor
	DEALLOCATE acct_cursor

 
	
-- for deliveries update orderlineno to max orderlineno for bookings not returned or cancelled
	UPDATE LineitemOracleExport SET orderlineno =isnull((SELECT case when min(x.orderlineno)=0 then 0 else MAX(x.orderlineno) end 
	FROM lineitemOracleExport x 
	WHERE  x.acctno= lineitemOracleExport.acctno AND x.agrmtno =lineitemOracleExport.agrmtno 
	and x.itemno=LineitemOracleExport.itemno and x.stocklocn=LineitemOracleExport.stocklocn
	and x.contractno=LineitemOracleExport.contractno and x.runno<=@runno and type='O'
	and not exists (SELECT 'x'
	FROM lineitemOracleExport x1 
	WHERE  x.acctno= x1.acctno AND x.agrmtno =x1.agrmtno 
	and x.itemno=x1.itemno and x.stocklocn=x1.stocklocn
	and x.contractno=x1.contractno and x.orderlineno=x1.orderlineno and (x1.type='C' or (x1.type = 'D' and quantity<0)) 
	and x.type='D' and quantity>=0)),0)
	WHERE  lineitemOracleExport.runno= @runno  
	

 



	

	-- returns
	UPDATE LineitemOracleExport SET orderlineno =isnull((SELECT  MIN(x.orderlineno) 
	FROM lineitemOracleExport x 
	WHERE  x.acctno= lineitemOracleExport.acctno AND x.agrmtno =lineitemOracleExport.agrmtno 
	and x.itemno=LineitemOracleExport.itemno and x.stocklocn=LineitemOracleExport.stocklocn
	and x.contractno=LineitemOracleExport.contractno and x.runno<=@runno
	and x.orderlineno!=0
	and not exists (SELECT 'x'
	FROM lineitemOracleExport x1 
	WHERE  x.acctno= x1.acctno AND x.agrmtno =x1.agrmtno 
	and x.itemno=x1.itemno and x.stocklocn=x1.stocklocn
	and x.contractno=x1.contractno and x.orderlineno=x1.orderlineno and (x1.type='C' or (x1.type = 'D' and quantity<=0)) 
	and x.type='D' and quantity<=0)),0)
	WHERE  lineitemOracleExport.runno= @runno 
	and type='D' and quantity<=0



 



	--ensure cancellations have linenumber of last undelivered order
	UPDATE LineitemOracleExport SET orderlineno =isnull((SELECT case when min(x.orderlineno)=0 then 0 else MAX(x.orderlineno) end 
	FROM lineitemOracleExport x 
	WHERE  x.acctno= lineitemOracleExport.acctno AND x.agrmtno =lineitemOracleExport.agrmtno 
	and x.itemno=LineitemOracleExport.itemno and x.stocklocn=LineitemOracleExport.stocklocn
	and x.contractno=LineitemOracleExport.contractno and x.type='O'
	and not exists -- not delivered
	(select 'x' from lineitemOracleExport z 
	WHERE  z.acctno= x.acctno AND z.agrmtno =x.agrmtno 
	and z.itemno=x.itemno and z.stocklocn=x.stocklocn
	and z.orderlineno=x.orderlineno
	and z.contractno=LineitemOracleExport.contractno and z.type='D') ),orderlineno)
	WHERE  lineitemOracleExport.runno= @runno  
	and type='C' 

---- confirm returns have linenumber of last delivery
--	UPDATE LineitemOracleExport SET orderlineno =isnull((SELECT MAX(x.orderlineno) 
--	FROM lineitemOracleExport x 
--	WHERE  x.acctno= lineitemOracleExport.acctno AND x.agrmtno =lineitemOracleExport.agrmtno 
--	and x.itemno=LineitemOracleExport.itemno and x.stocklocn=LineitemOracleExport.stocklocn
--	and x.contractno=LineitemOracleExport.contractno and x.type='D' and quantity>=0),orderlineno)
--	WHERE  lineitemOracleExport.runno= @runno  
--	and type in ('D','R','P') and quantity<=0 

 
	

	GO 