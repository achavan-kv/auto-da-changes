SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CODdeliveryNoteCalculationSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CODdeliveryNoteCalculationSP]
GO


CREATE procedure DN_CODdeliveryNoteCalculationSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_CODdeliveryNoteCalculationSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Delivery Note Calculation Load
-- Author       : ??
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 18/05/11  jec RI Integration 
-- 22/11/11  jec #8707 UAT64[UAT V6] - Error message printing delivery notes and schedule
-- ================================================
	-- Add the parameters for the stored procedure here
			@acctno char(12),
			@buffno integer,
			@agrmtno integer,
			@totalamountdue money OUT,
			@nonstocktotal money OUT,
			@cod smallint OUT,
			@addtype varchar (2),
			@dateReqDel DateTime,
			@timeReqDel varchar (12),  	
			@locn int,		
			@return integer OUT
as 

   	DECLARE @hasdelivery smallint, 
			@hasschedule smallint,
	    	@amountdue money, 
			@paidamount money,
			@itemno varchar(18),		-- RI
			@itemID int,		-- RI
			@stocklocn smallint,
			@contractno varchar(10),
			@delqty float,
			@agrmttotal money
   
	SET @return = 0
	SET	@totalamountdue = 0
	SET	@nonstocktotal = 0
	SET	@hasschedule = 0
	SET	@hasdelivery = 0
	SET	@paidamount = 0
	SET	@addtype = rtrim(@addtype)
	SET	@agrmttotal = 0

	/* make sure it's a COD account first */
	SELECT	@cod = count(*)
	FROM	agreement 
	WHERE	acctno = @acctno
	AND		agrmtno = @agrmtno
	AND		codflag = 'Y'
	
	SELECT	@agrmttotal = agrmttotal
	FROM	agreement
	WHERE 	acctno = @acctno

	IF(@cod > 0)
	BEGIN	  
		SET NOCOUNT ON

		IF EXISTS (	SELECT * 
					FROM schedule
					WHERE buffno != @buffno
					AND acctno = @acctno 
					AND agrmtno = @agrmtno 
					AND dateprinted is not null )
			SET	@hasschedule = 1

		IF EXISTS (	SELECT * 
					FROM delivery
					WHERE acctno = @acctno 
					AND agrmtno = @agrmtno )
			SET	@hasdelivery = 1	   	
		
		SELECT	*
		INTO 	#lineitems
		FROM	lineitem 
		WHERE	acctno = @acctno
		AND	agrmtno = @agrmtno
		AND stocklocn = @locn
		AND	dateReqDel = @dateReqDel 
		AND	timeReqDel = @timeReqDel
		
		UPDATE	#lineitems
		SET		delqty = 0
		-- Udate Itemno to IUPC		-- RI Jec 18/05/11
		UPDATE	#lineitems
		SET		ItemNo = s.IUPC
		From #lineitems l INNER JOIN StockInfo s on s.ID=l.ItemID

		SELECT	@stocklocn = stocklocn 
		FROM	#lineitems
	   
		DECLARE	itemscursor
		CURSOR	FOR
		SELECT	li.itemno,			-- #8707			
				stocklocn,
				contractno,
				ItemID				-- RI
		FROM	#lineitems li INNER JOIN StockInfo s on li.ItemID=s.ID		-- RI
		
		OPEN	itemscursor
	
		FETCH NEXT FROM itemscursor
		INTO @itemno, @stocklocn, @contractno, @ItemID			-- RI

		WHILE (@@fetch_status = 0)
		BEGIN
			SELECT	@delqty = ISNULL(SUM(quantity), 0)
			FROM	delivery
			WHERE	acctno = @acctno
			AND		agrmtno = @agrmtno
			--AND		itemno = @itemno
			AND		itemID = @itemID			-- RI
			AND		stocklocn = @stocklocn
			AND		contractno = @contractno

			/*SELECT	@delqty = @delqty + ISNULL(SUM(quantity),0)
			FROM	schedule
			WHERE	acctno = @acctno
			AND		agrmtno = @agrmtno
			AND		itemno = @itemno
			AND		stocklocn = @stocklocn*/

			UPDATE	#lineitems
			SET		delqty = @delqty
			WHERE	acctno = @acctno 
			AND		agrmtno = @agrmtno
			--AND		itemno = @itemno
			AND		itemID = @itemID			-- RI 
			AND		stocklocn = @stocklocn
			AND		contractno = @contractno

			FETCH NEXT FROM itemscursor
			INTO @itemno, @stocklocn, @contractno, @ItemID			-- RI		
		END

		CLOSE itemscursor
		DEALLOCATE itemscursor

		/* if there have not been any deliveries on this account then we need to include 
		** all orphaned non-stocks in the non-stock total. Otherwise it will just be 
		** non-stocks who's parent will be delivered on this delivery note 
		*/
		
		IF( @hasschedule = 0 AND @hasdelivery = 0 )
		BEGIN
			SELECT 	@nonstocktotal = isnull(sum((L.quantity - L.delqty)*L.price),0) 
	   		--FROM 	lineitem L INNER JOIN stockitem S ON S.itemno = L.itemno AND S.stocklocn = L.stocklocn
	   		FROM 	lineitem L INNER JOIN stockitem S ON S.itemID = L.itemID AND S.stocklocn = L.stocklocn		-- RI
	   		WHERE 	L.acctno =@acctno		/* we're trying to get non-stocks which either have */
	   	    AND	 	L.agrmtno =@agrmtno 	/* no parent or non-stocks who's parent will be */
	   	    AND 	S.itemtype != 'S'		/* delivered on this delivery note */
			--AND		L.parentitemno = '' 
        		
			/* SELECT	@nonstocktotal = @nonstocktotal + isnull(sum((L.quantity - L.delqty)*L.price),0) 
   			** FROM 	#lineitems L INNER JOIN stockitem S
			**				         ON	S.itemno = L.itemno
			**						 AND	S.stocklocn = L.stocklocn 
			**						 LEFT OUTER JOIN #lineitems L2
			**						 ON	L.parentitemno = L2.itemno 
			**						 AND	L2.parentlocation = L2.stocklocn
   			** WHERE 	L.acctno =@acctno		we're trying to get non-stocks which either have 
   			** AND	 	L.agrmtno =@agrmtno 	no parent or non-stocks who's parent will be
   			** AND 		S.itemtype != 'S'		delivered on this delivery note 
			** AND		L2.datereqdel = @datereqdel
			** AND		L2.timereqdel = @timereqdel
			** and		l2.stocklocn =@stocklocn
			*/
        
			/*SELECT 	@paidamount = isnull(sum(transvalue),0) 
			FROM 	fintrans
			WHERE	acctno = @acctno 
			AND		transtypecode in ('PAY','COR','REF','RET','SCX','REB','XFR','DDN','DDR','DDE')*/
		END
		
		SELECT 	@paidamount = isnull(sum(transvalue),0) 
		FROM 	fintrans
		WHERE	acctno = @acctno 
		AND		transtypecode in ('PAY','COR','REF','RET','SCX','REB','XFR','DDN','DDR','DDE')
		
	   	SELECT 	@amountdue =isnull (sum((L.quantity - L.delqty)*L.price),0)
	    --FROM	#lineitems L INNER JOIN stockitem S ON S.itemno = L.itemno AND S.stocklocn = L.stocklocn
	    FROM	#lineitems L INNER JOIN stockitem S ON S.itemID = L.itemID AND S.stocklocn = L.stocklocn	-- RI
		WHERE 	L.acctno = @acctno 
		AND	 	L.agrmtno = @agrmtno                                     
        AND	 	L.dateReqDel = @dateReqDel
		AND		L.timeReqDel = @timeReqDel
		AND 	L.deliveryaddress = @addtype 
		AND 	L.iskit = 0 					/* exclude kit line items */
		AND		S.itemtype = 'S'
		
		IF( @hasschedule = 0 AND @hasdelivery = 0 )
			SET @totalamountdue =@amountdue + @nonstocktotal + @paidamount
		ELSE	
			SET @totalamountdue = @amountdue
		
		IF(@agrmttotal = abs(@paidamount))
			SET @totalamountdue = 0
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End