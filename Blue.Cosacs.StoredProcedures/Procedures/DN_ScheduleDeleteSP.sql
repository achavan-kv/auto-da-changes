SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScheduleDeleteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleDeleteSP]
GO


CREATE PROCEDURE 	dbo.DN_ScheduleDeleteSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_ScheduleDeleteSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ??
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 17/05/11 ip  RI Integration changes - CR1212 - #3627 - Changed join between lineitem and schedule table to now use ItemID 
--				 rather than itemno
-- ================================================
			@acctNo varchar(12),
			@agreementNo int,
			--@itemNo varchar(8),
			@itemID int,				--IP - 17/05/11 - CR1212 - #3627
			@location smallint,
			@buffBranch int,
			@buffNo int,
			@replacement int,
			@return int OUTPUT

AS DECLARE

    @origLocn smallint

	SET 	@return = 0			--initialise return code

	DECLARE @lineItemID int, @bookingID int

	select @lineItemID = ID from lineitem where acctno = @acctNo
	AND 		agrmtno		= @agreementNo
	AND 		ItemID		= @itemID				
	AND 		stocklocn 	= @location

	select top 1 @bookingID = BookingId from LineItemBookingSchedule where LineItemID = @lineItemID and DelOrColl = 'C' order by BookingId desc
	DELETE FROM Schedule
	WHERE		acctno 		= @acctNo
	AND 		agrmtno		= @agreementNo
	--AND 		itemno		= @itemNo
	AND 		ItemID		= @itemID				--IP - 17/05/11 - CR1212 - #3627
	AND 		stocklocn 	= @location
	--AND 		buffbranchno	= @buffBranch 
	AND 		buffno		= @buffno

	

    IF (@replacement = 1)
    BEGIN
		-- #14313 - Only update for cancel collection note
		UPDATE LineItemBookingSchedule
		SET Quantity = 0 where LineItemID = @lineItemID and DelOrColl = 'C'
		and Quantity != 0
		and BookingId = @bookingID

		-- Also remove an 'Exchange' record if there is one
		DELETE FROM Exchange
		WHERE	acctno 		= @acctNo
		AND 	agrmtno		= @agreementNo
		--AND 	itemno		= @itemNo
		AND 	ItemID		= @itemID				--IP - 17/05/11 - CR1212 - #3627
		AND 	stocklocn 	= @location
		AND 	buffno		= @buffno
	
		-- Also remove an identical replacement if there is one
		--DELETE FROM Schedule
		--WHERE   Schedule.acctno    = @acctNo
		--AND     Schedule.agrmtno   = @agreementNo
		----AND     Schedule.itemno    = @itemNo
		--AND     Schedule.ItemID    = @itemID		--IP - 17/05/11 - CR1212 - #3627
		--AND     Schedule.stocklocn = @location
		--AND     Schedule.delorcoll = 'D'

		UPDATE LineItemBookingSchedule
		SET Quantity = 0 where LineItemID = @lineItemID and DelOrColl = 'D'
		and Quantity != 0
		and BookingId > @bookingID
		
		-- If replacement del not has been removed, need to re-set 
		-- qtydiif to ensure the account does not appear in the 
		-- Print Delivery Notes screen.
		IF (@@ROWCOUNT > 0)
		BEGIN
			UPDATE LineItem
			SET    qtydiff = 'N',
			       notes = ''
			WHERE  acctNo     = @acctNo
			AND    agrmtNo    = @agreementNo
			--AND    itemNo     = @itemNo
			AND    ItemID     = @itemID				--IP - 17/05/11 - CR1212 - #3627
			AND    stockLocn  = @location
		END
	
		-- Also check if the removed schedule record was for a
		-- replacement to a different stock location. If so, then
		-- we need to set the lineitem back to the original delivery location.
		SELECT TOP 1 @origLocn = d.stockLocn
		FROM   LineItem l, Delivery d
		WHERE  l.acctNo     = @acctNo
		AND    l.agrmtNo    = @agreementNo
		--AND    l.itemNo     = @itemNo
		AND    l.ItemID     = @itemID					--IP - 17/05/11 - CR1212 - #3627
		AND    l.stockLocn  = @location
		AND    d.acctNo     = l.acctNo
		AND    d.agrmtNo    = l.agrmtNo
		--AND    d.itemNo     = l.itemNo
		AND    d.ItemID     = l.ItemID					--IP - 17/05/11 - CR1212 - #3627
		AND    d.stockLocn != l.stockLocn
		AND NOT EXISTS (SELECT 1 FROM LineItem
						WHERE  acctNo    = d.acctNo
						AND    agrmtNo   = d.agrmtNo
						--AND    itemNo    = d.itemNo
						AND    ItemID    = d.ItemID		--IP - 17/05/11 - CR1212 - #3627
						AND    stockLocn = d.stockLocn)
		AND NOT EXISTS (SELECT 1 FROM Schedule
						WHERE  acctNo    = l.acctNo
						AND    agrmtNo   = l.agrmtNo
						--AND    itemNo    = l.itemNo
						AND    ItemID    = l.ItemID		--IP - 17/05/11 - CR1212 - #3627
						AND    stockLocn = l.stockLocn)
		                
		IF (@@ROWCOUNT = 1)
		BEGIN
			UPDATE LineItem
			SET    StockLocn  = @origLocn
			WHERE  acctNo     = @acctNo
			AND    agrmtNo    = @agreementNo
			--AND    itemNo     = @itemNo
			AND    ItemID     = @itemID					--IP - 17/05/11 - CR1212 - #3627
			AND    stockLocn  = @location
		END
    END


	SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


