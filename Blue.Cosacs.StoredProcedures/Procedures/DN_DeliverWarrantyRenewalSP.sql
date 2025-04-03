SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliverWarrantyRenewalSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliverWarrantyRenewalSP]
GO

CREATE PROCEDURE DN_DeliverWarrantyRenewalSP
		         @empeeno int,
		         @return int OUTPUT
		         
			
AS

SET @return = 0

	DECLARE	@acctno varchar(12), 
		@agrmtno int, 
		@itemId INT,
		@stocklocn smallint, 
		@ordval money, 
		@contractno varchar(10),
		@branchno int,
		@buffno int,
		@refno int,
		@timestamp datetime,
		@itemIUPC as varchar(18)				--IP - 16/06/11 - CR1212 - RI - #3961
	
	SELECT	w.acctno, 
		ag.agrmtno, 
		l.ItemID, 
		l.stocklocn,
		l.ordval, 
		l.contractno
	INTO	#renewals
	FROM	WarrantyRenewalPurchase w, agreement ag, acct a, lineitem l
	WHERE	w.acctno = a.acctno
	AND	w.acctno = l.acctno
	AND	w.acctno = ag.acctno
	AND	w.datedelivered IS NULL
	AND	ag.holdprop = 'N'
	AND NOT EXISTS(	SELECT 	1
			FROM	cancellation c
			WHERE	w.acctno = c.acctno)
	
	SELECT	DISTINCT(r.acctno)
	INTO	#delaccts
	FROM	#renewals r, lineitem l
	INNER JOIN dbo.StockInfo SI ON l.ItemID = SI.ID
	WHERE	r.acctno = l.acctno
	AND	r.agrmtno = l.agrmtno
	AND	r.ItemID = l.ItemID
	AND	r.stocklocn = l.stocklocn
	AND	r.contractno = l.contractno
	AND	SI.IUPC NOT IN('DT', 'SD')
	AND l.datereqdel > CONVERT(SMALLDATETIME, CONVERT(VARCHAR(10), GETDATE(), 105), 105)

	DELETE FROM #renewals
	WHERE EXISTS(SELECT 1
		     FROM #delaccts d
		     WHERE #renewals.acctno = d.acctno)
	
	DECLARE delivery_cursor CURSOR FOR 
	SELECT	acctno,
		agrmtno,
		ItemID,
		stocklocn,
		ordval,
		contractno	
	FROM	#renewals	 

	OPEN delivery_cursor
	
	FETCH NEXT FROM delivery_cursor 
	INTO @acctno, @agrmtno, @itemId, @stocklocn, @ordval, @contractno
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @timestamp = GETDATE()
		SET @branchno = CONVERT(INTEGER,LEFT(@acctno,3))
		SET @itemIUPC = (select IUPC from stockinfo where id = @itemId)	--IP - 16/06/11 - CR1212 - RI - #3691
	
		EXECUTE @refno = nexttransrefno @branchno = @branchno, @numrefs= 1
		EXECUTE @buffno = nextbuffno @branchno = @branchno
	
		INSERT	
		INTO	delivery
			(origbr, acctno, agrmtno, datedel, delorcoll, itemno, ItemID, 
			stocklocn, quantity, retitemno, retstocklocn, retval, 
			buffno, buffbranchno, datetrans, branchno, transrefno,
			transvalue, runno, contractno, notifiedby)
		VALUES	(0, @acctno, @agrmtno, @timestamp, 'D', @itemIUPC, @itemId,		--IP - 16/06/11 - CR1212 - RI - #3691
			@stocklocn, 1, '', 0, 0, 
			@buffno, @branchno, @timestamp, @branchno, @refno,
			@ordval, 0, @contractno, @empeeno)
	
		UPDATE	WarrantyRenewalPurchase
		SET	datedelivered = @timestamp
		WHERE	acctno = @acctno
	
		FETCH NEXT FROM delivery_cursor 
		INTO @acctno, @agrmtno, @itemId, @stocklocn, @ordval, @contractno
	END
	CLOSE delivery_cursor
	DEALLOCATE delivery_cursor
	
	UPDATE	acct 
	SET		outstbal =
	 	 (SELECT  ISNULL(sum(transvalue),0)
		  FROM	  fintrans f, #renewals r
	      WHERE	  f.acctno = r.acctno
	      AND	  r.acctno = acct.acctno
	      AND	  f.acctno = acct.acctno)
   where exists (select * from  #renewals r where r.acctno = acct.acctno)
   
   IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
