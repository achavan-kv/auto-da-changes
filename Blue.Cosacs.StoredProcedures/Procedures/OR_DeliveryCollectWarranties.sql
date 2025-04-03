SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[OR_DeliveryCollectWarranties]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[OR_DeliveryCollectWarranties]
GO


CREATE PROCEDURE 	dbo.OR_DeliveryCollectWarranties
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : OR_DeliveryCollectWarranties.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        :
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 22/06/11  IP  CR1212 - RI - #3987 - RI Integration System Changes - join on ItemID
--------------------------------------------------------------------------------
					@empeeno int

AS

	DECLARE @buffno int, @refno int, @acctno varchar(12), @agrmtno int,
		@itemno varchar(18),												--IP - 22/06/11 - CR1212 - RI - #3987
		@itemID int,														--IP - 22/06/11 - CR1212 - RI - #3987
		@retitemno varchar(8), @stocklocn smallint,
		@retlocn smallint, @quantity float, @branchno smallint,
		@transvalue money, @contractno varchar(10), @datedel datetime,
		@return int	, @parentItemID int --@parentitemno varchar(8)			--IP - 22/06/11 - CR1212 - RI - #3987

	DECLARE delivery_cursor CURSOR FOR 
	--SELECT	s.acctno, l.agrmtno, s.itemno, s.stocklocn, s.quantity, 
	--	s.retitemno, s.retstocklocn, s.retval, s.contractno,l.parentitemno
	SELECT	s.acctno, l.agrmtno, s.ItemID, s.stocklocn, s.quantity,			--IP - 22/06/11 - CR1212 - RI - #3987
		s.retitemno, s.retstocklocn, s.retval, s.contractno,l.ParentItemID
	--FROM	schedule s, lineitem l, stockitem st, delivery d
	FROM	schedule s, lineitem l, stockinfo si, stockquantity sq, delivery d
	WHERE	s.acctno = l.acctno
	--AND	s.itemno = l.itemno
	AND	s.ItemID = l.ItemID
	AND	s.stocklocn = l.stocklocn
	AND	s.contractno = l.contractno
	AND	s.acctno = d.acctno
	--AND	l.parentitemno = d.itemno
	AND	l.ParentItemID = d.ItemID											--IP - 22/06/11 - CR1212 - RI - #3987
	AND	l.parentlocation = d.stocklocn
	--AND	s.itemno = st.itemno
	AND	s.ItemID = si.ID													--IP - 22/06/11 - CR1212 - RI - #3987
	AND si.ID = sq.ID														--IP - 22/06/11 - CR1212 - RI - #3987
	--AND	s.stocklocn = st.stocklocn
	AND	s.stocklocn = sq.stocklocn											--IP - 22/06/11 - CR1212 - RI - #3987									
	AND	s.quantity < 0
	AND	s.delorcoll = 'C'
	AND	d.runno = 0
	AND	d.quantity < 0
	--AND	st.category IN(12,82)
	AND	si.category IN(select distinct reference from code where category = 'WAR') --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties
	AND	si.itemtype != 'S'
	
	OPEN delivery_cursor
	
	FETCH NEXT FROM delivery_cursor 
	--INTO	@acctno, @agrmtno, @itemno, @stocklocn, @quantity,
	--	@retitemno, @retlocn, @transvalue, @contractno,@parentitemno
	INTO	@acctno, @agrmtno, @itemID, @stocklocn, @quantity,
	@retitemno, @retlocn, @transvalue, @contractno,@parentItemID
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @datedel = CONVERT(SMALLDATETIME, CONVERT(VARCHAR(10), GETDATE(), 105), 105)
		SET @branchno = CONVERT(INTEGER,LEFT(@acctno,3))	
		SET @itemno = (select iupc from stockinfo where id = @itemID)		--IP - 22/06/11 - CR1212 - RI - #3987	
		
		EXECUTE @refno = nexttransrefno @branchno = @branchno, @numrefs= 1
		EXECUTE @buffno = nextbuffno @branchno = @branchno
	
		INSERT	
		INTO	delivery
			(origbr, acctno, agrmtno, datedel, delorcoll, itemno, 
			stocklocn, quantity, retitemno, retstocklocn, retval, 
			buffno, buffbranchno, datetrans, branchno, transrefno,
			transvalue, runno, contractno, notifiedby,ftnotes, ItemID)		--IP - 22/06/11 - CR1212 - RI - #3987	
		VALUES	(0, @acctno, @agrmtno, @datedel, 'C', @itemno, 
			@stocklocn, @quantity, @retitemno, @retlocn, -@transvalue, 
			@buffno, @branchno, GETDATE(), @branchno, @refno,
			-@transvalue, 0, @contractno,@empeeno,'WRNC', @itemID)			--IP - 22/06/11 - CR1212 - RI - #3987	
/*
		INSERT INTO fintrans(branchno, acctno, transrefno, datetrans, transtypecode, empeeno,
	                             transupdated, transprinted, transvalue, runno, source, paymethod, ftnotes)
        	VALUES(@branchno, @acctno, @refno, GETDATE(), 'GRT', @empeeno, 'N', 'N',
	               -@transvalue, 0, 'COSACS', 0, 'WRNC');
*/	
		--EXEC DN_LineItemUpdateQuantitySP @acctno=@acctNo,@itemno=@itemNo,@location=@stocklocn,@newQty=0,
		--@agreementno=@agrmtno,@contractno=@contractno,@source='fact',@parentitemno=@parentitemno,@user=99999,@return=@return OUT 
		
		EXEC DN_LineItemUpdateQuantitySP @acctno=@acctNo,@itemID=@itemID,@location=@stocklocn,@newQty=0,
		@agreementno=@agrmtno,@contractno=@contractno,@source='fact',@parentItemID=@parentItemID,@user=99999,@return=@return OUT	--IP - 22/06/11 - CR1212 - RI - #3987
	    
	    DELETE 
		FROM	schedule
	    WHERE   acctno = @acctno
	    AND     agrmtno = @agrmtno
	    --AND     itemno = @itemno
	    AND     ItemID = @itemID										--IP - 22/06/11 - CR1212 - RI - #3987			
	    AND     stocklocn = @stocklocn
	    AND     contractno = @contractno
	    
	    UPDATE	agreement
	    SET		agrmttotal = agrmttotal + -@transvalue,
				cashprice = cashprice + -@transvalue
	    WHERE   acctno = @acctno
	    AND     agrmtno = @agrmtno
	    
	    UPDATE	acct
	    SET		agrmttotal	= a.agrmttotal
	    FROM	agreement a
	    WHERE   acct.acctno = @acctno
	    AND		acct.acctno = a.acctno

		FETCH NEXT FROM delivery_cursor 
		--INTO	@acctno, @agrmtno, @itemno, @stocklocn, @quantity,
		--	@retitemno, @retlocn, @transvalue, @contractno,@parentitemno
		 INTO	@acctno, @agrmtno, @itemID, @stocklocn, @quantity,
		 @retitemno, @retlocn, @transvalue, @contractno,@parentItemID

	END
	CLOSE delivery_cursor
	DEALLOCATE delivery_cursor

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
