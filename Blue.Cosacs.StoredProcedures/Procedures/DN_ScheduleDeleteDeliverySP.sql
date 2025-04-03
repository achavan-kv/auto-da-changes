SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScheduleDeleteDeliverySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleDeleteDeliverySP]
GO

CREATE PROCEDURE 	dbo.DN_ScheduleDeleteDeliverySP
-- =============================================
-- Author:		??
-- Create date: ??
-- Description:	Schedule Delete Delivery
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 03/03/10  jec CR1072 Malaysia merge
-- 30/03/10  jec UAT43 DHL CANCEL quantity should be negative/Full cancel before Shipment 
-- 13/04/10  ip  UAT87 Insert ScheduleRemoval record when item removed from revise after exported to DHL but before DHL number generated
-- 07/06/11  ip  CR1212 - RI - use itemID
-- 29/05/12  jec #10248 DelQty set to negative
-- =============================================  
			@acctNo varchar(12),
			@agreementNo int,
			--@itemNo varchar(8),
			@itemID int,								--IP - 07/06/11 - CR1212 - RI
			@location smallint,
			@buffBranch smallint,
			@buffNo int,
			@quantity float,
			@empeeno int,
			@qtyRemoved float,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	
	--IP/JC - 02/03/10 - CR1072 - Malaysia 3PL - LW70621 - 4.3 Merge
	declare @pickingdate DATETIME,
			@OrigBuffNo INT
			--@OrigQty INT, @ShipQty int
	select @pickingdate=dhlpickingdate	--, @OrigQty=OrigQty ,@ShipQty= ShipQty
		from schedule where acctno=@acctno 
		--and itemno=@itemno 
		and ItemID = @itemID							--IP - 07/06/11 - CR1212 - RI 
		and stocklocn=@location
	-- get original Buffno
	SELECT @OrigBuffNo=OrigBuffNo 
	FROM schedule 
	    WHERE acctno = @acctNo
		AND		agrmtno = @agreementNo
		--AND		itemno = @itemNo
		AND		ItemID = @itemID						--IP - 07/06/11 - CR1212 - RI 		
		AND		stocklocn = @location		
		AND		buffno = @buffNo

if @pickingdate is null
	--or (@pickingdate is not null and @OrigQty!=@ShipQty)  -- only part shipped
begin
	UPDATE 	lineitem
	SET		delqty = case when delqty - @quantity <0 then 0 else delqty - @quantity end			-- #10248
	WHERE	acctno = @acctNo
	AND		agrmtno = @agreementNo
	--AND		itemno = @itemNo
	AND		ItemID = @itemID							--IP - 07/06/11 - CR1212 - RI 				
	AND		stocklocn = @location

	SET	@return = @@error

	IF (@return = 0) 
		--and @ShipQty=0		-- none shipped
	BEGIN
		INSERT INTO order_removed
		(
			acctno, agrmtno, itemno, stocklocn,
			quantity, vanno, buffbranchno, buffno,
			loadno, dateprinted, empeeno, dateremoved,
			originallocation, type, picklistnumber,
			picklistbranch, delnotebranch, datereqdel,
			transchedno, transchednobranch, ItemID		--IP - 07/06/11 - CR1212 - RI 	
		)
		SELECT	DISTINCT s.acctno, s.agrmtno, '', s.stocklocn,	--IP - 07/06/11 - CR1212 - RI 
				s.quantity, s.vanno, s.buffbranchno, s.buffno,
				s.loadno, s.dateprinted, @empeeno, GETDATE(),
				s.stocklocn, 'R', s.picklistnumber, 
				--s.stocklocn, s.delorcoll, s.picklistnumber, --IP - 03/11/08 - UAT(352) Should not be using s.delorcoll as 'R' refers to 'Removed' and has no relation to delivery type.
				ISNULL(s.picklistbranchnumber, 0), l.delnotebranch,
				l.datereqdel, s.transchedno, s.transchednobranch, s.ItemID	--IP - 07/06/11 - CR1212 - RI 
		FROM	schedule s, lineitem l
		WHERE	s.acctno 	= @acctNo
		AND 	s.agrmtno	= @agreementNo
		--AND 	s.itemno	= @itemNo
		AND		s.ItemID	= @itemID										--IP - 07/06/11 - CR1212 - RI 
		AND 	s.stocklocn = @location
		AND 	s.buffno	= @buffNo
		AND		s.acctno 	= l.acctno
		AND 	s.agrmtno	= l.agrmtno
		--AND 	s.itemno	= l.itemno
		AND		s.ItemID	= l.ItemID										--IP - 07/06/11 - CR1212 - RI 
		AND 	s.stocklocn = l.stocklocn
		AND     s.ParentItemID= l.ParentItemID

	END
	
	SET	@return = @@error

	IF (@return = 0)
	--IP/JC - 02/03/10 - CR1072 - Malaysia 3PL 
	--BEGIN	
	--	DELETE	
	--	FROM	schedule
	--	WHERE	acctno = @acctNo
	--	AND		agrmtno = @agreementNo
	--	AND		itemno = @itemNo
	--	AND		stocklocn = @location
	--	--AND		buffbranchno = @buffBranch
	--	AND		buffno = @buffNo
	--END
	
	-- CR 953 If an item removed from revise agreement before the DHL number has been generated,
	-- but after the item has been exported to DHL, then the schedule record will need to be amended to be negative 
	-- and re-exported so that a cancellation record should go to DHL
	BEGIN
	    IF(SELECT vanno FROM schedule 
	    WHERE acctno = @acctNo
		AND		agrmtno = @agreementNo
		--AND		itemno = @itemNo
		AND		ItemID = @itemID						--IP - 07/06/11 - CR1212 - RI 
		AND		stocklocn = @location
		--AND		buffbranchno = @buffBranch
		AND		buffno = @buffNo) = 'DHL'
		BEGIN
			--if @qtyRemoved=@quantity		-- none shipped
			if (Select DHlDNno FROM schedule		-- none shipped
				WHERE acctno = @acctNo
				AND		agrmtno = @agreementNo
				--AND		itemno = @itemNo
				AND		ItemID = @itemID				--IP - 07/06/11 - CR1212 - RI 
				AND		stocklocn = @location
				AND		buffno = @buffNo) is null
				BEGIN
				
					--IP - 13/04/10 - UAT(87) Insert record into ScheduleRemoval table.
					  INSERT
                        INTO   ScheduleRemoval
                               (
                                      AcctNo,
                                      AgrmtNo,
                                      ItemNo,
                                      StockLocn,
                                      Quantity,
                                      Price,
                                      DeliveryArea,
                                      BuffNo,
                                      LoadNo,
                                      DateRemoved,
                                      RemovedBy,
                                      ItemID								--IP - 07/06/11 - CR1212 - RI 
                               )
                        SELECT distinct s.acctno,
                               s.agrmtno,
                               --s.itemno,
                               '',											--IP - 07/06/11 - CR1212 - RI 
                               s.stocklocn,
                               s.quantity,
                               ordval,
                               '',
                               buffno,
                               loadno,
                               GETDATE(),
                               @empeeno,
                               s.ItemID										--IP - 07/06/11 - CR1212 - RI 
                        FROM   SCHEDULE s
                               JOIN lineitem l
                               ON     l.acctno    = s.acctno
                                  --AND l.itemno    = s.itemno
                                  AND l.ItemID = s.ItemID					--IP - 07/06/11 - CR1212 - RI 
                                  AND l.stocklocn = s.stocklocn
                                  and l.delnotebranch=s.buffbranchno
                        WHERE  s.acctno           = @acctNo
                           --AND s.itemno           = @itemNo
                           AND s.ItemID			  = @itemID					--IP - 07/06/11 - CR1212 - RI 
                           and s.stocklocn		  = @location
                           and s.buffno			  = @buffNo		
		
					UPDATE Schedule
					SET quantity = quantity * -1,vanno = 'CANCEL',delorcoll = 'X'
					WHERE acctno = @acctNo
					AND		agrmtno = @agreementNo
					--AND		itemno = @itemNo
					AND		ItemID	  = @itemID								--IP - 07/06/11 - CR1212 - RI 
					AND		stocklocn = @location
					--AND		buffbranchno = @buffBranch
					AND		buffno = @buffNo	

				end
		    else		-- part shipped - only cancel difference
				BEGIN
						-- reduce quantity on schedule
					UPDATE Schedule
					SET quantity = quantity-@qtyRemoved		--,OrigQty=@OrigQty-@qtyRemoved
					WHERE acctno = @acctNo
					AND		agrmtno = @agreementNo
					--AND		itemno = @itemNo
					AND		ItemID = @itemID								--IP - 07/06/11 - CR1212 - RI 
					AND		stocklocn = @location					
					AND		OrigBuffno = @OrigBuffNo
					and DhlDNNo is null		-- not in transit
					
					insert into schedule (
						origbr,acctno,agrmtno,datedelplan,delorcoll,itemno,stocklocn,quantity,retstocklocn,
						retitemno,retval,vanno,buffbranchno,buffno,loadno,dateprinted,printedby,
						Picklistnumber,undeliveredflag,datePicklistPrinted,picklistbranchnumber,
						contractno,runNo,DHLPickingDate,consignmentNoteNo,deliveryLineNo,DHLDNNo,
						transchedno,transchednobranch,datetranschednoprinted,CreatedBy,DateCreated,
						GRTnotes,OrigBuffNo, ItemID	--,OrigQty,ShipQty,ParentItemId		--IP - 07/06/11 - CR1212 - RI 
					) 
					Select origbr,acctno,agrmtno,datedelplan,'X','',stocklocn,@qtyRemoved*-1,retstocklocn,	-- UAT40 jec
						retitemno,retval,'CANCEL',buffbranchno,@OrigBuffNo,loadno,dateprinted,printedby,
						Picklistnumber,undeliveredflag,null,picklistbranchnumber,
						contractno,runNo,null,consignmentNoteNo,deliveryLineNo,null,
						transchedno,transchednobranch,datetranschednoprinted,CreatedBy,DateCreated,
						GRTnotes,@OrigBuffNo, ItemID --,OrigQty,ShipQty,ParentItemId		--IP - 07/06/11 - CR1212 - RI 
					from schedule 
					where acctno = @acctNo
						AND		agrmtno = @agreementNo
						--AND		itemno = @itemNo
						AND		ItemID   = itemID							--IP - 07/06/11 - CR1212 - RI 
						AND		stocklocn = @location						
						AND		buffno = @buffNo
						
				END
		   -- UAT 12 The export job should not be run automatically
		    --EXEC DeliveryAndReturnsFileExport
		END	
		ELSE
		BEGIN
			DELETE	
		    FROM		schedule
		    WHERE	acctno = @acctNo
		    AND		agrmtno = @agreementNo
		    --AND		itemno = @itemNo
		    AND		ItemID = @itemID										--IP - 07/06/11 - CR1212 - RI 
		    AND		stocklocn = @location
		    --AND		buffbranchno = @buffBranch
		    AND		buffno = @buffNo
		    END
	END
	END
ELSE 
BEGIN
	--if @OrigQty=@ShipQty		--IP/JC - 03/03/10 - CR1072 - Malaysia 3PL
		Begin
		RAISERROR('IN-Transit - unable to delete',16,1)
			SET @return = @@error
			RETURN 
		End
END
	

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO