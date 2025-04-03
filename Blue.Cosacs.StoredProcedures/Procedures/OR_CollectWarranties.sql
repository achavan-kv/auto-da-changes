SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[OR_CollectWarranties]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[OR_CollectWarranties]
GO

CREATE PROCEDURE OR_CollectWarranties
AS
RETURN 0
/* RD 07/11/05 FR67692 Disabled this procedure
	DECLARE @retThreeYrElec varchar(4),
		@retFiveYrElec varchar(4),
		@retThreeYrFurn varchar(4),
		@retFiveYrFurn varchar(4),
		@acctno varchar(12), 
		@warrantyno varchar(8), 
		@contractno varchar(10), 
		@stocklocn smallint,
	     	@price money, 
		@agrmtno int, 
		@returncode varchar(8),
		@refno int,
		@buffno int,
		@datedel datetime,
		@branchno smallint
	
	SET @retThreeYrElec = '1980'
	SET @retFiveYrElec = '1986'
	SET @retThreeYrFurn = '1985'
	SET @retFiveYrFurn = '1987'
	
	SELECT	@branchno = hobranchno
	FROM 	country
	
	SELECT  d.acctno, d.itemno, d.stocklocn, d.contractno, l.notes,l.agrmtno
	INTO	#collections
	FROM    delivery d, lineitem l
	WHERE   delorcoll = 'C'
	AND	d.acctno = l.acctno
	AND	d.itemno = l.itemno
   and d.agrmtno = l.agrmtno
	AND	d.stocklocn = l.stocklocn
	AND	d.contractno = l.contractno
	AND	runno = 0
	and d.acctno not like '___5%'

	SELECT	l.acctno,
		l.itemno as warrantyno,
		l.contractno,
		l.stocklocn,
		l.price,
		l.agrmtno,
		CONVERT(varchar(6), '') as returncode,
		CONVERT(int, 0) AS yearvalue,
		CONVERT(int, 0) AS monthvalue,
		CONVERT(int, 0) AS dayvalue,
		CONVERT(int, 0) AS elapsedMonths,
		CONVERT (smalldatetime, null) as yearchange,
		CONVERT (smalldatetime, null) as monthchange,
		GETDATE() as datedel,
		c.notes
	INTO 	#warranties
	FROM	lineitem l, stockitem s, #collections c
	WHERE	l.acctno = c.acctno
   AND   l.agrmtno = c.agrmtno
	AND	l.parentitemno = c.itemno
	AND	l.parentlocation = c.stocklocn
	AND	l.contractno != ''
	AND 	l.quantity != 0
	AND	l.itemno = s.itemno
	AND	l.stocklocn = s.stocklocn
	AND	s.category IN(12,82)	
	
	DELETE
	FROM	#warranties
	WHERE EXISTS( SELECT 1
		      FROM #collections
		      WHERE #warranties.acctno = #collections.acctno
		      AND #warranties.warrantyno = #collections.itemno
		      AND #warranties.stocklocn = #collections.stocklocn
		      AND #warranties.contractno = #collections.contractno)
	
	UPDATE	#warranties
	SET	datedel = d.datedel
	FROM	delivery d
	WHERE 	#warranties.acctno = d.acctno
	AND 	#warranties.warrantyno = d.itemno
	AND 	#warranties.stocklocn = d.stocklocn
	AND 	#warranties.contractno = d.contractno
	AND	#warranties.agrmtno = d.agrmtno
	 
	UPDATE	#warranties
	SET	yearvalue = DATEDIFF(year, datedel, getdate())
	
	UPDATE	#warranties
	SET	yearchange = DATEADD(year, yearvalue, datedel)
	
	UPDATE	#warranties
	SET	monthvalue = DATEDIFF(month, yearchange, getdate())
	
	UPDATE	#warranties
	SET	monthchange = DATEADD(month, monthvalue, yearchange)
	
	UPDATE	#warranties
	SET	dayvalue = DATEDIFF(day, monthchange, getdate())
	
	UPDATE	#warranties
	SET	monthvalue = monthvalue - 1
	WHERE	dayvalue < 0
	
	UPDATE	#warranties
	SET	yearvalue = yearvalue - 1,
		monthvalue = monthvalue + 12
	WHERE	monthvalue < 0
	
	UPDATE	#warranties
	SET	elapsedMonths = yearvalue * 12 + monthvalue
	
	UPDATE	#warranties
	SET	returncode = @retThreeYrElec + convert(varchar(2), elapsedMonths)
	WHERE	warrantyno like '19___3%'
	
	UPDATE	#warranties
	SET	returncode = @retFiveYrElec + convert(varchar(2), elapsedMonths)
	WHERE	warrantyno like '19___5%'
	
	UPDATE	#warranties
	SET	returncode = @retThreeYrFurn + convert(varchar(2), elapsedMonths)
	WHERE	warrantyno like 'XW___3%'
	
	UPDATE	#warranties
	SET	returncode = @retFiveYrFurn + convert(varchar(2), elapsedMonths)
	WHERE	warrantyno like 'XW___5%'
	
	UPDATE	#warranties
	SET	returncode = @retThreeYrElec + '36',
		price = 0
	WHERE	warrantyno like '19___3%'
	AND	notes like 'Replacement%'
	
	UPDATE	#warranties
	SET	returncode = @retFiveYrElec + '60',
		price = 0
	WHERE	warrantyno like '19___5%'
	AND	notes like 'Replacement%'
	
	UPDATE	#warranties
	SET	returncode = @retThreeYrFurn + '36',
		price = 0
	WHERE	warrantyno like 'XW___3%'
	AND	notes like 'Replacement%'
	
	UPDATE	#warranties
	SET	returncode = @retFiveYrFurn + '60',
		price = 0
	WHERE	warrantyno like 'XW___5%'
	AND	notes like 'Replacement%'
	
	DECLARE delivery_cursor CURSOR FOR 
	SELECT	acctno,
		warrantyno,
		contractno,
		stocklocn,
		price,
		agrmtno,
		returncode
	FROM	#warranties	 
	
	OPEN delivery_cursor
	
	FETCH NEXT FROM delivery_cursor 
	INTO @acctno, @warrantyno, @contractno, @stocklocn,
	     @price, @agrmtno, @returncode
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXECUTE @refno = nexttransrefno @branchno = @branchno, @numrefs = 1
		EXECUTE @buffno = nextbuffno  @branchno = @branchno
	
		SET @price = -1 * @price
		
		SET @datedel = CONVERT(SMALLDATETIME, CONVERT(VARCHAR(10), getdate(), 105), 105)

		INSERT	
		INTO	delivery
			(origbr, acctno, agrmtno, datedel, delorcoll, itemno, 
			stocklocn, quantity, retitemno, retstocklocn, retval, 
			buffno, buffbranchno, datetrans, branchno, transrefno,
			transvalue, runno, contractno)
		VALUES	(0, @acctno, @agrmtno, @datedel, 'C', @warrantyno, 
			@stocklocn, -1, @returncode, @stocklocn, 0, 
			@buffno, @branchno, getdate(), @branchno, @refno,
			@price, 0, @contractno)
		
		FETCH NEXT FROM delivery_cursor 
		INTO @acctno, @warrantyno, @contractno, @stocklocn,
		     @price, @agrmtno, @returncode
	END
	CLOSE delivery_cursor
	DEALLOCATE delivery_cursor
	
	DECLARE fintrans_cursor CURSOR FOR 
	SELECT	acctno,
			price
	FROM	#warranties	 
	WHERE	price != 0
	
	OPEN fintrans_cursor
	
	FETCH NEXT FROM fintrans_cursor 
	INTO @acctno, @price
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXECUTE @refno = nexttransrefno @branchno = @branchno, @numrefs = 1
	
		SET @price = -1 * @price
		
		INSERT INTO fintrans(branchno, acctno, transrefno, datetrans, transtypecode, empeeno,
                             transupdated, transprinted, transvalue, runno, source)
       	VALUES(@branchno, @acctno, @refno, getdate(), 'GRT', 99999, 'N', 'N',
           		@price, 0, 'COSACS');
		
		FETCH NEXT FROM fintrans_cursor 
		INTO @acctno, @price
	END
	CLOSE fintrans_cursor
	DEALLOCATE fintrans_cursor

*/

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
