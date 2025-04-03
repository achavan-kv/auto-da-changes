SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemUpdateItemLocationSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemUpdateItemLocationSP]
GO

CREATE PROCEDURE 	dbo.DN_LineItemUpdateItemLocationSP
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemUpdateItemLocationSP.PRC
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
-- 25/11/11  jec #8720 Unable to Change Stock Location for Gifts
-- 07/06/12  ip  #10229 - Warehouse & Deliveries - included Express
-- 07/08/12  ip  #10789 - Added SalesBrnNo when inserting into Lineitem. Previously caused null 
--				 exception when generating BookingSubmit message to Warehouse.
--------------------------------------------------------------------------------
			@acctno varchar(12),
			@agreementno int,
			@itemId int,
			@parentItemID int,
			@stocklocn smallint,
			@datereqdel datetime,
			@timereqdel varchar(12),
			@dnbranch smallint,
			@notes varchar(200),
			@deliveryaddress char(2),
			@contractno varchar(10),
			@deliveryarea varchar(8),
			@deliveryprocess char(1),
			@damaged char(1),
			@buffno int,
			@newbuffno int,
			@origlocation smallint,
			@empeeno int,
			@assembly char(1),
			@express char(1),					--IP - 07/06/12 - #10229
			@return int OUTPUT
AS

	SET 	@return = 0			--initialise return code

	
  exec DN_LineItemDeleteAllSP @acctno = @acctno, @agreementno = @agreementno, @return = @return
   
  update l
  set   quantity = a.quantity,
	ordval = a.ordval,
	taxamt = a.taxamt
 from lineitem l
 inner join lineitem_amend a 
 on a.acctno = l.acctno
	and a.agrmtno = l.agrmtno
	and a.acctno = @acctno
    and a.ItemID = l.ItemID
    and a.stocklocn = l.stocklocn
    and a.contractno = l.contractno
    AND a.ParentItemID = l.ParentItemID			-- #8720

	IF (@newbuffno != 0 AND @return = 0)
	BEGIN
		INSERT INTO order_removed
		(
			acctno, agrmtno, itemno, itemId, stocklocn,
			quantity, vanno, buffbranchno, buffno,
			loadno, dateprinted, empeeno, dateremoved,
			originallocation, confirmedby, dateconfirmed, type, picklistnumber,
			picklistbranch, delnotebranch, datereqdel,
			transchedno, transchednobranch
		)
		SELECT	s.acctno, s.agrmtno, '', s.ItemID, @stocklocn,
				s.quantity, s.vanno, s.buffbranchno, s.buffno,
				s.loadno, s.dateprinted, @empeeno, GETDATE(),
				@origlocation, @empeeno, null, 'C', s.picklistnumber,
				ISNULL(s.picklistbranchnumber, 0), l.delnotebranch,
				l.datereqdel, s.transchedno, s.transchednobranch
		FROM	schedule s, lineitem l
		WHERE	s.acctno 	= @acctNo
		AND 	s.agrmtno	= @agreementNo
		AND 	s.itemId	= @itemId
		AND 	s.stocklocn = @origlocation
		AND 	buffno		= @buffno
		AND		s.acctno 	= l.acctno
		AND 	s.agrmtno	= l.agrmtno
		AND 	s.itemId	= l.itemId
		AND 	s.stocklocn = l.stocklocn
		and l.quantity!=0			-- #8720
	END		

	 IF @stocklocn = @origlocation
	BEGIN
		UPDATE 	lineitem
		SET		stocklocn =	@stockLocn,		
				datereqdel = @datereqdel,		
				timereqdel = @timereqdel,		
				delnotebranch =	@dnbranch,		
				notes =	@notes,		
				deliveryaddress	= @deliveryaddress,
				deliveryarea  = @deliveryarea,
				deliveryprocess = @deliveryprocess,
				damaged = @damaged,
				assemblyrequired = @assembly,
				express = @express						--IP - 07/06/12 - #10229 
		WHERE	acctno = @acctno
		AND		agrmtno = @agreementno
		AND		stocklocn = @origlocation
		AND		itemId = @itemId
		AND		contractno = @contractno
		AND		parentitemid = @parentItemID 
	END	
	ELSE   
 BEGIN
	 
	 DECLARE @newOrderLineno int
	 
	 SELECT @newOrderLineno = MAX(orderlineno) +1
	 FROM lineitem
	 WHERE acctno = @acctno
		AND  agrmtno = @agreementno
		
	 
	 SELECT * INTO #temp 
	 FROM lineitem
	 WHERE stocklocn = @origlocation
		AND itemId = @itemId
		AND acctno = @acctno
		AND agrmtno = @agreementno
		AND contractno = @contractno
		AND parentItemID = @parentItemID
		and quantity!=0			-- #8720
		
	SET @return = @@error   
  
	IF (@return = 0)  
	BEGIN  
	 
		IF EXISTS
		(
			SELECT 'x' FROM lineitem
			WHERE stocklocn = @stocklocn
				AND itemId = @itemId
				AND acctno = @acctno
				AND  agrmtno = @agreementno
				AND contractno = @contractno
				AND ParentItemID = @parentItemID
		)
		BEGIN		
			UPDATE  l  
			SET  quantity = l.quantity + t.quantity, 
				 delqty = l.delqty + t.quantity,
				 ordval = (l.quantity + t.quantity) * l.price,
				 taxamt = isnull(nullif((l.taxamt/isnull(nullif(l.ordval, 0), 1)), 0), s.taxrate/100) * (l.quantity + t.quantity) * l.price,
				 timereqdel = @timereqdel,     -- #10230 
				 notes =	@notes,		
				 deliveryaddress	= @deliveryaddress,
				 deliveryarea  = @deliveryarea,
				 deliveryprocess = @deliveryprocess,
				 damaged = @damaged,
				 assemblyrequired = @assembly,
				 express = @express			   --IP - 07/06/12 - #10229
			FROM lineitem l 
			INNER JOIN #temp t 
				ON t.acctno = l.acctno 
				AND l.itemId = t.itemId
				AND l.agrmtno = t.agrmtno
			INNER JOIN stockitem s 
				on l.itemId = s.itemId
				and l.stocklocn = s.stocklocn
			WHERE l.acctno = @acctno  
				AND  l.agrmtno = @agreementno  
				AND  l.stocklocn = @stocklocn  
				AND  l.itemId = @itemId
				AND  l.contractno = @contractno  
				AND l.ParentItemID = @parentItemID
		 END 
		 ELSE
		 BEGIN
		 
		INSERT INTO lineitem
			([origbr] ,[acctno],[agrmtno],[itemno],[itemId],[itemsupptext],[quantity]		--IP - 27/05/11 - CR1212 - RI
           ,[delqty],[stocklocn],[price],[ordval],[datereqdel]
           ,[timereqdel],[dateplandel],[delnotebranch],[qtydiff],[itemtype]
           ,[notes],[taxamt],[isKit],[deliveryaddress]
           ,[parentitemId],[parentlocation] ,[contractno],[expectedreturndate]
           ,[deliveryprocess],[deliveryarea],[DeliveryPrinted]
           ,[assemblyrequired],[damaged],[OrderNo],[Orderlineno],[PrintOrder],[taxrate],[SalesBrnNo],[Express])	--#10789 - added SalesBrnNo --IP - 07/06/12 - #10229
			SELECT origbr,acctno,agrmtno,itemno,itemId,itemsupptext,quantity,				--IP - 27/05/11 - CR1212 - RI
				delqty,@stocklocn,price,ordval,@datereqdel,
				@timereqdel,dateplandel,@dnbranch,qtydiff,itemtype,
				notes,taxamt,isKit,@deliveryaddress,
				parentitemId,parentlocation,contractno,expectedreturndate,
				@deliveryprocess,@deliveryarea,DeliveryPrinted,
				@assembly,@damaged,OrderNo,@newOrderLineno, 0, taxrate, salesbrnno, @express			--IP - 07/06/12 - #10229
			FROM #temp
		 END
		 
		 UPDATE lineitem
		 SET quantity = 0,
			ordval = 0,
			delqty = 0,
			taxamt = 0
		 WHERE stocklocn = @origlocation
				AND itemId = @itemId
				AND acctno = @acctno
				AND  agrmtno = @agreementno
				AND contractno = @contractno
				AND ParentItemID = @parentitemid
		 
		SET @return = @@error   
	 END
	  
 END
	
	SET	@return = @@error	
	
	IF (@return = 0)
	BEGIN
		UPDATE	lineitem
		SET		--stocklocn = @stocklocn,
				delnotebranch = @dnbranch,
				parentlocation = @stocklocn
		WHERE	acctno = @acctno
		AND		parentitemId = @itemId
		AND		parentlocation = @origlocation
		--69312 If the child item is a discount then to ensure that the parentlocation field is changed do not base this query on a contract number
		--AND		contractno != ''
		AND 	quantity != 0
	END
	
	SET	@return = @@error	

	IF (@newbuffno != 0 AND @return = 0)
	BEGIN
		UPDATE	Schedule
		SET		stocklocn = @stocklocn,
				buffno = @newbuffno,
				buffbranchno = @dnbranch,
				loadno = 0,
				picklistnumber = 0,
				picklistbranchnumber = 0,
				transchedno = 0,
				transchednobranch = 0,
				dateprinted = null,
				printedby = 0
		WHERE	acctno 		= @acctNo
		AND 	agrmtno		= @agreementNo
		AND 	itemId		= @itemId
		AND 	stocklocn 	= @origlocation
		AND 	buffno		= @buffno
	END	
	
	
 --UPDATE LINEITEM AUDIT
 
 declare @oldquantity float, @oldvalue money,@oldtaxamt float
 ,@source char(15), @taxAmount float, 
 @parentStockLocn smallint, @datechange datetime,
 @quantity float,@price money,@orderValue money, @salesBrnNo smallint 
 
 set @source = 'CHANGEORDER'
 set @datechange = GETDATE()

 SELECT @price = price, @parentStockLocn = parentlocation, 
		@parentItemId = parentitemId, @orderValue = ordval,
		@quantity = quantity, @taxAmount = taxamt, @salesBrnNo = SalesBrnNo 
 FROM lineitem
 WHERE stocklocn = @stocklocn
	AND itemId = @itemId
	AND acctno = @acctno
	AND  agrmtno = @agreementno
	AND contractno = @contractno 

 EXEC DN_LineItemGetOldQtySP 	@acctno = @acctNo,
					@itemId = @itemId,
					@stocklocn = @stockLocn,
					@contractno = @contractNo,
					@agreementno = @agreementNo,
					@parentitemid = @parentitemid,
					@quantity = @oldquantity OUT,
					@return = @return OUT
					
	SET	@oldquantity = isnull(@oldquantity,0)
	 
	

	/* retrieve the old value */
	EXEC DN_LineItemGetOldValueSP 	@acctno = @acctNo,
					@itemId = @itemId,
					@stocklocn = @stockLocn,
					@contractno = @contractNo,
					@agreementno = @agreementNo,
					@parentitemid = @parentitemid,
					@value = @oldvalue OUT,
					@taxamt = @oldtaxamt OUT,  -- 67977 RD
					@return = @return OUT
					
	SET	@oldvalue = isnull(@oldvalue,0)
	SET @oldtaxamt = isnull(@oldtaxamt,0)   -- 67977 RD 

	IF( (@quantity != @oldquantity) OR
	    (@orderValue != @oldvalue) OR 
	    (@taxAmount != @oldtaxamt) )   -- 67977 RD
	    
	BEGIN
		/* write an audit record */
		EXEC DN_LineItemAuditUpdateSP 	@acctno = @acctNo,
						@agrmtno = @agreementNo,
						@empeenochange = @empeeno,
						@itemId = @itemId,
						@stocklocn = @stockLocn,
						@quantitybefore = @oldquantity,
						@quantityafter = @quantity,
						@valuebefore = @oldvalue,
						@valueafter = @orderValue,
						@taxamtbefore = @oldtaxamt,   
						@taxamtafter = @taxAmount,    
						@datechange = @datechange,
						@contractno = @contractNo,
						@source = @source,
						@parentItemId = @parentItemId,			
						@parentStockLocn = @parentStockLocn,	
						@delnotebranch = @dnbranch,
						@salesBrnNo = @salesBrnNo,
						@return = @return OUT	
	END 
	
---- OLD stock audit

 
 IF @stocklocn != @origlocation
 BEGIN  
	 
	 
	SELECT @price = price, @parentStockLocn = parentlocation, 
		@parentItemId = parentitemId, @orderValue = ordval,
		@quantity = quantity, @taxAmount = taxamt
	 FROM lineitem
	 WHERE stocklocn = @origlocation
					AND itemId = @itemId
					AND acctno = @acctno
					AND  agrmtno = @agreementno
					AND contractno = @contractno 
					AND ParentItemID = @parentitemid 
	 
	 EXEC DN_LineItemGetOldQtySP 	@acctno = @acctNo,
						@itemId = @itemId,
						@stocklocn = @origlocation,
						@contractno = @contractNo,
						@agreementno = @agreementNo,
						@parentitemid = @parentitemid,
						@quantity = @oldquantity OUT,
						@return = @return OUT
						
		SET	@oldquantity = isnull(@oldquantity,0)
		 
		/* retrieve the old value */
		EXEC DN_LineItemGetOldValueSP 	@acctno = @acctNo,
						@itemId = @itemId,
						@stocklocn = @origlocation,
						@contractno = @contractNo,
						@agreementno = @agreementNo,
						@parentitemid = @parentitemid,
						@value = @oldvalue OUT,
						@taxamt = @oldtaxamt OUT,  -- 67977 RD
						@return = @return OUT
						
		SET	@oldvalue = isnull(@oldvalue,0)
		SET @oldtaxamt = isnull(@oldtaxamt,0)   -- 67977 RD 

		IF( (@quantity != @oldquantity) OR
			(@orderValue != @oldvalue) OR 
			(@taxAmount != @oldtaxamt) )   -- 67977 RD
		    
		BEGIN
			/* write an audit record */
			EXEC DN_LineItemAuditUpdateSP 	@acctno = @acctNo,
							@agrmtno = @agreementNo,
							@empeenochange = @empeeno,
							@itemId = @itemId,
							@stocklocn = @origlocation,
							@quantitybefore = @oldquantity,
							@quantityafter = @quantity,
							@valuebefore = @oldvalue,
							@valueafter = @orderValue,
							@taxamtbefore = @oldtaxamt,   
							@taxamtafter = @taxAmount,    
							@datechange = @datechange,
							@contractno = @contractNo,
							@source = @source,
							@parentItemId = @parentItemId,			
							@parentStockLocn = @parentStockLocn,
							@delnotebranch = @dnbranch,	
							@salesBrnNo = @salesBrnNo,
							@return = @return OUT	
		END 
END   

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
