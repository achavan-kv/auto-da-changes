
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalClearSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalClearSP]
GO

CREATE PROCEDURE 	dbo.DN_ProposalClearSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_ProposalClearSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Clear Proposal  
-- Author       : ??
-- Date         : ??
--
-- This procedure will Clear the Proposal for Delivery
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 02/03/10 jec CR1072 Malaysia v4 merge
-- 16/05/11 ip  RI Integration changes - CR1212 - #3627 - Changed joins between schedule and lineitem to use ItemID rather than ItemNo
-- 19/09/11 ip  Sprint 6.1.2 - #3478 - LW71346 - When processing a GRT Exchange, revising the account adding the new item and attempting to 
--				delivery authorise, the item was not being inserted into schedule. This was due to previous changes made to this procedure to cater for
--				issues #3615 (lw73601) and #4261. The updates to lineitem.delqty were incorrectly moved to the bottom of the procedure and are required
--				at the beginning for the select into #schedule to work correctly.
-- 25/05/12 jec #10178 Delivery Authorisation should send message to Warehousing
-- 08/06/12 ip  #10319 - Changed to return lineitem.quantity as QtyBooked for new bookings to be posted for scheduled bookings.
-- 13/06/12 ip  #10385 - Returning the current BookingId (which will become the PreviousBookingId) as a new id is created for a new booking.
--						 This is required when updating the lineitembookingfailures table.
-- 15/06/12 ip  #10387 - Insert into LineItemBookingSchedule for scheduled deliveries.
-- 18/06/12 jec #10441 - GRT screen should generate 'collect' booking
-- 21/06/12 jec #10488 - Return actual Delivery quantity
-- 02/08/12 ip  #10789 - Interface Immediate deliveries to Warehouse.
-- 13/09/13 ip  #14644 - Increasing the Qty of an item should send a seperate booking leaving the previous bookings intact.
-- ================================================
	-- Add the parameters for the stored procedure here
			@acctno varchar(12),
			@empeeno int,
			@source varchar(10), --IP - 03/02/10 - CR1072 - 3.1.9 - The Delivery Authorisation source either Manual or Auto
			@return int OUTPUT

AS
	SET NOCOUNT ON
	SET 	@return = 0			--initialise return code

	DECLARE	@branchno smallint   
	DECLARE @distinctbuffs smallint
	DECLARE	@hibuffno int
    DECLARE	@dateplandel datetime
    DECLARE	@timereqdel varchar(12)
    DECLARE	@deliveryarea varchar (16)
    DECLARE	@deliveryaddress varchar (4)
	DECLARE	@Pdateplandel datetime
	DECLARE	@Pdeliveryarea varchar (16)
	DECLARE	@Pdeliveryaddress varchar (4)
	DECLARE	@counter smallint
	DECLARE	@suppliercode varchar (18)
	DECLARE	@psuppliercode varchar (18)
    DECLARE	@ptimereqdel varchar(12)
	DECLARE	@stocklocn smallint
	DECLARE	@countrycode char(1)
	DECLARE	@pstocklocn smallint
	DECLARE	@deliveryprocess char(1)
	DECLARE	@pdeliveryprocess char(1)
	DECLARE	@newbuffno int
	DECLARE	@delnotebranch smallint
	DECLARE	@pdelnotebranch smallint
	DECLARE @dateAuth smalldatetime --IP - 03/02/10 - CR1072 - 3.1.9 -- AA UAT 103 changing to small datetime
	 
	SET @dateAuth = getdate()
	

	IF @source = 'AUTO'
	BEGIN
		IF EXISTS (
					SELECT *
					FROM instantcreditflag
					WHERE acctno = @acctno
					and ( datecleared is null
						or datecleared = '1900-01-01'
						or datecleared = '')
				)OR
			EXISTS (
				select * 
				from proposal
				where acctno = @acctno
					and propresult != 'A'
				)
		BEGIN
		
				DELETE 
				FROM	AccountLocking
   				WHERE	acctno = @acctno
				AND		lockedby = @empeeno
				
				return -- if has uncleared instantflags we don't want to Auto DA
		
		END
		
	END
	
	
	IF(@acctno like '___5%')
	BEGIN
		IF EXISTS (	SELECT * 
					FROM custacct 
					WHERE acctno = @acctno 
					AND custid like 'paid%')
		 BEGIN
		 		DELETE 
				FROM	AccountLocking
   				WHERE	acctno = @acctno
				AND		lockedby = @empeeno
				
				return -- if cash and go don't want schedules!
		END
	END

	UPDATE	agreement
	SET		holdprop = 'N',
			empeenoauth = @empeeno,
			dateauth = @dateAuth --IP - 03/02/10 - CR1072 - 3.1.9
	WHERE	acctno = @acctno
	
	--IP - 03/02/10 - CR1072 - 3.1.9
	IF NOT EXISTS (SELECT * FROM delauthoriseAudit WHERE acctno= @acctno AND dateauthorised  = @dateauth)
	BEGIN
		INSERT INTO DelAuthoriseAudit(Acctno, Empeeno, DateAuthorised, Source)
		VALUES (@acctno, @empeeno, @dateAuth, @source)

		IF (@@error != 0)
		BEGIN
			SET @return = @@error
		END
	END

	delete from delauthorise
	where acctno = @acctno
	
	--IP - 19/09/11 - #3478 - Re-instated the below two updates previously moved to the bottom of the procedure. 
	--These updates to delqty are required for the following select into #schedule. 
	UPDATE	lineitem 
	SET		delqty = ISNULL((SELECT	sum(quantity) 
							 FROM	delivery d
   							 WHERE  d.acctno = lineitem.acctno 
							 --AND    d.itemno = lineitem.itemno 
							 AND    d.ItemID = lineitem.ItemID		--IP - 16/05/11 - CR1212 - #3627
							 AND	d.ParentItemID = lineitem.ParentItemID
							 AND    d.stocklocn = lineitem.stocklocn 
							 AND    d.contractno = lineitem.contractno),0)
						
	WHERE	lineitem.acctno = @acctno
		--and Deliveryprocess!='S'		 -- #10178 only for non scheduled items
   
	UPDATE	lineitem 
	SET		delqty = delqty + ISNULL((SELECT  sum(quantity) 
									  FROM    schedule d
									  WHERE	  d.acctno = lineitem.acctno 
									  --AND     d.itemno = lineitem.itemno 
									  AND	  d.ItemID = lineitem.ItemID	--IP - 16/05/11 - CR1212 - #3627
									  AND     d.parentitemid = lineitem.parentitemid
									  AND     d.stocklocn = lineitem.stocklocn
									  AND d.delorcoll !='X'),0)
	WHERE	lineitem.acctno = @acctno
	
	-- #10385 Update lineitem for Warehouse scheduled
	UPDATE	lineitem 
	SET		delqty = delqty + ISNULL((SELECT  sum(abs(ls.quantity))		-- #17695	dont reduce for collection not collected ??		
									  FROM    LineItemBooking lb LEFT OUTER JOIN LineItemBookingSchedule ls on lb.id=ls.bookingId
									  WHERE	  lb.LineItemID=lineitem.ID
										--and lb.id=(select MAX(lb1.id) from LineItemBooking lb1 where lb1.LineItemID=lineitem.ID)		--#14644
									  ),0)
	WHERE	lineitem.acctno = @acctno
   
	-- we are now going to insert outstanding lineitems into the schedule table for this account
	-- we could do this row at a time for using a cursor as it will not be much for a cursor
	-- but probably easier to use a temporary table in any case as do not  have to declare variables

	--SELECT	acctno, agrmtno, lineitem.itemno, datereqdel as dateplandel,  
	SELECT  acctno, agrmtno, lineitem.ItemID,lineitem.ParentItemID, datereqdel as dateplandel,			--IP - 16/05/11 - CR1212 - #3627
			lineitem.stocklocn, --lineitem.quantity-(delqty - ISNULL(lb.Quantity,0)) as quantity,
			case when lineitem.deliveryprocess='I' then lineitem.quantity			-- 10488
									--when lineitem.deliveryprocess='S' then lineitem.quantity-delqty 
									when lineitem.deliveryprocess='S' then lineitem.quantity		-- #10605
									end as quantity, 
			lineitem.stocklocn as buffbranchno,
   			convert (int, 0) as buffno, lineitem.deliveryarea,deliveryaddress,
   			lineitem.delnotebranch,  lineitem.deliveryprocess, stockitem.suppliercode,
   			lineitem.timereqdel, lineitem.ID as LineItemId,		--IP - 15/06/12 - #10387
			lineitem.Price		--#12842
	INTO	#schedule
	from lineitem	--LEFT OUTER JOIN LineItemBooking lb on lineitem.ID=lb.LineItemID
		--LEFT OUTER JOIN LineItemBookingFailures lf on lf.OriginalBookingID=lb.ID
		INNER JOIN stockitem
			--ON lineitem.itemno = stockitem.itemno and lineitem.stocklocn = stockitem.stocklocn
			ON lineitem.ItemID = stockitem.ItemID and lineitem.stocklocn = stockitem.stocklocn	--IP - 16/05/11 - CR1212 - #3627
		INNER JOIN branch
			ON branch.branchno = stockitem.stocklocn 
		INNER JOIN branch printWarehouse
			ON lineitem.delnotebranch = printWarehouse.branchno 
   where 
   lineitem.acctno =@acctno
	-- and lineitem.qtydiff ='Y'			-- #17695	removed ??
	and stockitem.itemtype !='N'
   --and lineitem.deliveryprocess !=''
   --and lineitem.deliveryprocess ='I'		-- #10178	 Immediate delivery only				--IP - 15/06/12 - #10387
   and (lineitem.quantity-delqty >0 or delqty>lineitem.quantity)			-- #17695
   and lineitem.iskit = 0
   AND (branch.dotnetforwarehouse='Y' OR printWarehouse.dotnetforwarehouse = 'Y' )
  -- and (lb.id=(select MAX(lb1.id) from LineItemBooking lb1 where lb1.LineItemID=lineitem.ID)
		--or not exists(select * from LineItemBooking lb1 where lb1.LineItemID=lineitem.ID) )
   
/*
	cr Malaysia replacing this with new code so any items from warehouses will get scheduled	
	FROM	lineitem, stockitem, branch
	WHERE	lineitem.itemno = stockitem.itemno 
	AND		branch.branchno = stockitem.stocklocn and branch.dotnetforwarehouse='Y'
	AND		lineitem.stocklocn = stockitem.stocklocn
	AND		lineitem.acctno =@acctno and lineitem.qtydiff ='Y' and stockitem.itemtype !='N'
	AND		lineitem.deliveryprocess !=''
	AND		quantity-delqty >0
	AND		lineitem.iskit = 0 -- RD 24/08/05 Added to ensure that Kit Item are not printed on the delivery along with components
*/

	-- New buff no assignment 
        
  SELECT ID= IDENTITY(INT,1,1), dateplandel,  
                 deliveryarea,  
                 deliveryaddress,  
                 delnotebranch,  
                 suppliercode,  
                 deliveryprocess, CONVERT(INT,0) AS buff 
                 INTO #newbuff
                 FROM #schedule
          GROUP BY dateplandel,  
                 deliveryarea,  
                 deliveryaddress,  
                 delnotebranch,  
                 suppliercode,  
                 deliveryprocess  
        
        UPDATE #newbuff
        SET buff = (SELECT CASE WHEN 1=1 THEN (SELECT COUNT(*) FROM #newbuff a 
											  WHERE a.delnotebranch = #newbuff.delnotebranch 
											  AND a.ID < #newbuff.ID)+1 END + branch.hibuffno)
		
        FROM branch
        WHERE branchno =  #newbuff.delnotebranch
        
        UPDATE #schedule
        SET buffno = buff
        FROM #newbuff a
        WHERE a.dateplandel = #schedule.dateplandel  
        AND a.deliveryarea = #schedule.deliveryarea  
        AND a.deliveryaddress = #schedule.deliveryaddress
        AND a.delnotebranch = #schedule.delnotebranch
        AND a.suppliercode = #schedule.suppliercode 
        AND a.deliveryprocess = #schedule.deliveryprocess 
        
    
        -- Code below caused many errors replaced with above.
	SET @counter = 1
	DECLARE schedule_cursor CURSOR 
	FOR SELECT	dateplandel, deliveryarea, deliveryaddress, stocklocn,
				suppliercode, deliveryprocess, delnotebranch, timereqdel
		FROM	#schedule
		ORDER BY dateplandel, deliveryarea, deliveryaddress, deliveryprocess, 
				 stocklocn,suppliercode,delnotebranch
 
	OPEN schedule_cursor
	FETCH NEXT FROM schedule_cursor 
	INTO @dateplandel, @deliveryarea, @deliveryaddress, @stocklocn, 
	     @suppliercode, @deliveryprocess, @delnotebranch, @timereqdel
 
	WHILE (@@fetch_status <> -1)
	BEGIN
		IF (@@fetch_status <> -2)
		BEGIN
			IF(@counter  > 1)
			BEGIN 
				IF(@dateplandel != @pdateplandel OR
				   @deliveryarea != @pdeliveryarea OR
				   @deliveryaddress != @pdeliveryaddress OR
				   @deliveryprocess != @pdeliveryprocess OR
				   @stocklocn != @pstocklocn OR
				   @delnotebranch != @pdelnotebranch OR
				   @timereqdel != @ptimereqdel OR
				   (@stocklocn = 999 
					AND @countrycode = 'S' 
					AND @suppliercode != @psuppliercode)
				  )
					
				BEGIN
					UPDATE	branch 
					SET		hibuffno = hibuffno + 1 
					WHERE	branchno = @stocklocn
                       
					SELECT	@newbuffno = hibuffno 
					FROM	branch 
					WHERE	branchno = @stocklocn
				END			 			
					
				SET @pdeliveryarea = @deliveryarea
				SET @pdateplandel = @dateplandel
				SET @pdeliveryaddress = @deliveryaddress
				SET @pstocklocn = @stocklocn
				SET @psuppliercode = @suppliercode
				SET @pdelnotebranch = @delnotebranch
				SET @pdeliveryprocess = @deliveryprocess 
				SET @ptimereqdel = @timereqdel

				UPDATE	#schedule 
				SET		buffno = @newbuffno 
				WHERE	dateplandel = @dateplandel 
				AND		deliveryarea = @deliveryarea 
				AND		deliveryaddress = @deliveryaddress
	 			AND		stocklocn = @stocklocn 
				AND		suppliercode = @suppliercode
				AND		deliveryprocess = @deliveryprocess
				AND		delnotebranch = @delnotebranch
				AND		timereqdel = @timereqdel
			END
			ELSE
				UPDATE	branch 
				SET		hibuffno = hibuffno + 1 
				WHERE	branchno = @stocklocn
                   
				SELECT	@newbuffno = hibuffno 
				FROM	branch 
				WHERE	branchno = @stocklocn		 			
				
				SET @pdeliveryarea = @deliveryarea
				SET @pdateplandel = @dateplandel
				SET @pdeliveryaddress = @deliveryaddress
				SET @pstocklocn = @stocklocn
				SET @psuppliercode = @suppliercode
				SET @pdelnotebranch = @delnotebranch
				SET @pdeliveryprocess = @deliveryprocess 
				SET @ptimereqdel = @timereqdel

				UPDATE	#schedule 
				SET		buffno =@newbuffno 
				WHERE	dateplandel = @dateplandel 
				AND		deliveryarea = @deliveryarea 
				AND		deliveryaddress = @deliveryaddress
 		 		AND		stocklocn = @stocklocn 
				AND		suppliercode = @suppliercode
				AND		deliveryprocess = @deliveryprocess
				AND		delnotebranch = @delnotebranch
				AND		timereqdel = @timereqdel

				SET @counter = @counter + 1
			END
         
		FETCH NEXT FROM schedule_cursor INTO @dateplandel, @deliveryarea, @deliveryaddress,
											  @stocklocn,@suppliercode,@deliveryprocess,
											  @delnotebranch, @timereqdel
	END

	CLOSE schedule_cursor
	DEALLOCATE schedule_cursor
	--END
	
	--IP - 02/08/12 - #10789
    --INSERT INTO schedule (origbr, acctno, agrmtno, datedelplan, 
    --                     delorcoll, itemno, stocklocn, quantity, 
    --                     retstocklocn, retitemno, retval, vanno, 
    --                     buffbranchno, buffno, loadno, dateprinted, 
    --                     printedby, Picklistnumber, undeliveredflag,  
    --                     datePicklistprinted,	--OrigQty,ShipQty)	--02/03/10 jec/ip CR1072
    --                     --OrigBuffNo)		--04/03/10 jec/ip CR1072
    --                     OrigBuffNo, ItemID,ParentItemID)	--IP - 16/05/11 - CR1212 - #3627 - Added ItemID
    --SELECT	stocklocn, acctno, agrmtno,dateplandel,
    --        --'D', itemno, stocklocn, quantity,       
    --        'D', '', stocklocn, quantity,        --IP - 16/05/11 - CR1212 - #3627
    --        null, null, null, null,
    --        delnotebranch,buffno, 0, null,
    --        0,0,'', null,	--quantity,0		--02/03/10 jec/ip CR1072
    --        --buffno		--04/03/10 jec/ip CR1072
    --        buffno, ItemID,ParentItemID	--IP - 16/05/11 - CR1212 - #3627 - Added ItemID
    --FROM	#schedule 
    --WHERE   deliveryprocess = 'I'			--IP -15/06/12 - #10387
    
    
	select s.acctno,s.itemid,s.ParentItemID,s.stocklocn,0 as DeliveredQty, SUM(ISNULL(ls.Quantity,0)) as ScheduledQty	--#14644 - Added ScheduledQty
	into #Deliveries
	from #schedule s 
	--#14644 - Doing update for DeliveredQty below
	--LEFT Outer JOIN Delivery d on d.acctno = s.acctno					-- #10488
	--							 AND    d.ItemID = s.ItemID		
	--							 AND	d.ParentItemID = s.ParentItemID
	--							 AND    d.stocklocn = s.stocklocn
					 LEFT Outer JOIN LineItemBookingSchedule ls on ls.LineItemId = s.LineItemId	--#14644
	group by s.acctno,s.itemid,s.ParentItemID,s.stocklocn
    
    --#14644 - Update DeliveredQty on #Deliveries
	update #Deliveries
	set DeliveredQty = isnull((select sum(d.quantity)
						 from Delivery d
						 where d.acctno = #Deliveries.acctno
						 and d.ItemID = #Deliveries.ItemID	
						 and d.ParentItemID = #Deliveries.ParentItemID
						 and d.stocklocn = #Deliveries.stocklocn),0)
	--#14644 - New LineItemBookingSchedule record will be inserted for increased quantities. Previous record remains intact.
    -- #10605 Update any existing schedule qty to zero as new booking schedule created	
 --   UPDATE LineItemBookingSchedule
	--	set quantity=0
	--from #schedule s INNER JOIN LineItemBookingSchedule l on l.LineItemID=s.LineItemID	
    --IP - 15/06/12 - #10387

	--#14644 - Everytime Quantity increased, a new booking is created. Therefore the quantity of the new LineItemBookingSchedule should subtract that already delivered and scheduled.
    INSERT INTO LineItemBookingSchedule(LineItemID, DelOrColl,RetItemID,RetVal,RetStockLocn,BookingId,Quantity, ItemID, StockLocn, Price)
    SELECT s.LineItemId, 'D', 0, 0, 0, 0,(Quantity - d.DeliveredQty) - ScheduledQty, s.itemid, s.stocklocn, s.price --#14644 - subtract ScheduledQty	--#12842
    FROM #schedule s INNER JOIN #Deliveries d on d.acctno = s.acctno		-- #10605			
								 AND    d.ItemID = s.ItemID		
								 AND	d.ParentItemID = s.ParentItemID
								 AND    d.stocklocn = s.stocklocn
	--WHERE s.deliveryprocess = 'S'	 --IP - 02/08/12 - #10789

	--IP - 19/09/11 - #3478	- This update has been re-instated to where it was previously. It was moved to here incorrectly for a previous fix.
	
    
    -- #10178 Delivery Authorisation should send message to Warehousing    
    --Insert into LineitemBooking (LineItemID,Quantity)
    --select l.id,l.Quantity-l.delqty as QtyBooked
    
    select l.id,l.Quantity as QtyBooked, l.delqty, isnull(Max(lb.ID),0) as PreviousBookingId,isnull(Max(lf.ID),0) as FailureId, --IP - 13/06/12 - #10385 --IP - 08/06/12 - #10319
					ISNULL(lb.Quantity,0) as OrigBookQty,ISNULL(lf.Quantity,0) as FailedQty, 
					SUM(d.DeliveredQty) as DeliveredQty, SUM(d.ScheduledQty) as ScheduledQty	--#14644		 --SUM(ISNULL(ls.Quantity,0)) as ScheduleQty 				-- #10605 # 10488 #10385
    from LineItem l INNER JOIN stockitem s ON l.ItemID = s.ItemID and l.stocklocn = s.stocklocn	
							INNER JOIN branch b ON b.branchno = s.stocklocn 
							INNER JOIN branch printWarehouse ON l.delnotebranch = printWarehouse.branchno 
							LEFT JOIN LineItemBooking lb ON l.ID = lb.LineItemID	--IP - 13/06/12 - #10385
							LEFT JOIN LineItemBookingFailures lf ON lb.ID = lf.OriginalBookingId
							--LEFT JOIN LineItemBookingSchedule ls ON ls.BookingID = lb.ID			--#14644 -- #10441
							LEFT OUTER JOIN #Deliveries d on d.acctno = l.acctno					-- #10488
									 AND    d.ItemID = l.ItemID		
									 AND	d.ParentItemID = l.ParentItemID
									 AND    d.stocklocn = l.stocklocn 
    where l.acctno=@acctno 
    --and qtydiff ='Y' -- #10441
    and s.itemtype !='N'   
    --and deliveryprocess ='S'	--IP - 02/08/12 - #10789	-- #10178	 Schedule delivery only
    --and l.quantity-(delqty + ISNULL(ls.Quantity,0)) >0				-- #10441
    --and l.quantity-(delqty - ISNULL(lb.Quantity,0)) >0
    and (l.quantity-d.DeliveredQty) - d.scheduledQty >0		--#14644		-- #10605	
    and iskit = 0
    AND (b.dotnetforwarehouse='Y' OR printWarehouse.dotnetforwarehouse = 'Y' )
    and (lb.id=(select MAX(lb1.id) from LineItemBooking lb1 where lb1.LineItemID=l.ID)	-- #10385
		or not exists(select * from LineItemBooking lb1 where lb1.LineItemID=l.ID) )
    GROUP BY l.id,l.Quantity, l.delqty,lb.Quantity,lf.Quantity			-- #10385

    
    -- #10178 Set delqty to authorised qty (as per scheduling)
    UPDATE	lineitem 
			SET		delqty = quantity	
	WHERE	  acctno=@acctno 
		and qtydiff ='Y' and itemtype !='N'   
		--and deliveryprocess ='S'		--#10789 -- #10178	 Schedule delivery only
		and quantity-delqty >0
		and iskit = 0
     

    -- RD 21/09/05 Livewire issue 67595 Account not loaded in Print Delivery Screen dues open locks
	DELETE 
	FROM	AccountLocking
   	WHERE	acctno = @acctno
	AND		lockedby = @empeeno

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End
