SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_ScheduleRedelRepoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_ScheduleRedelRepoSP]
GO

CREATE PROCEDURE	dbo.dn_ScheduleRedelRepoSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : dn_ScheduleRedelRepoSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ??
-- Date         : ??
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 26/05/11  IP  CR1212 - RI - #3636 - Redelivery after Repossession (Standardisation of Courts SKU - New product codes)
-- 12/06/12  IP  #10357 - Warehouse & Deliveries. Do not insert into schedule table for a scheduled Redelivery after repo
-- 14/06/12  IP  #10386 - Warehouse & Deliveries. Use Date selected from Redelivery After Repossession screen to set Lineitem.datereqdel
--				 This is the date used in the booking sent to Warehouse.
-- 15/06/12  IP  #10387 - Warehouse & Deliveries - pass in lineItemId and insert into LineItemBookingSchedule table.
-- 29/01/13 jec  #10686 Authorise Redelivery After Repossession Screen. (#12119) 
-- ================================================
			@origbr  smallint ,-- current branch
			@acctno char(12),-- selected account number
			@datedelplan datetime,-- actually could do with a drop down to select this
			@delorcoll char(1),-- 'R' 
			--@itemno varchar(10),--
			@itemID int,							--IP - 26/05/11 - CR1212 - RI - #3636								
			@stocklocn smallint, --
			@quantity smallint, --
			@retstocklocn smallint, -- original retstocklocn
			--@retitemno varchar(10), --same as original reitemno
			@retItemID int,							--IP - 26/05/11 - CR1212 - RI - #3636					
			@retval money, -- same as original retval
			@buffbranchno smallint, -- current branch
			@buffno integer, --calculated by
			@deliveryarea varchar(8),
			@agrmtno integer,
			@contractno varchar(10),
			@datereqdel datetime,
			--@parentItemNo	VARCHAR(8), --uat363 add parentItemNo
			@parentItemID int,						--IP - 26/05/11 - CR1212 - RI - #3636
			@lineItemId int,						--IP - 15/06/12 - #10387 - Warehouse & Deliveries				
			@return integer  OUTPUT
as

	SET @return = 0

	DECLARE @retitemno varchar(18), @price money,		--#12842
			@repUnitPrice money							--#13829
	SELECT @retitemno = IUPC from StockInfo where ID = @retItemID

	select @price = price from lineitem where id = @lineItemId --#12842

	SELECT @repUnitPrice = isnull(CashPrice,0) from StockPrice where id = @retItemID and branchno = @retstocklocn		--#13829
	
	--IF(@delorcoll != 'S')				-- #10686 jec IP - 12/06/12 - #10357
	--BEGIN
	--	INSERT INTO schedule (	origbr, acctno, agrmtno, datedelplan, delorcoll, itemno,
	--							stocklocn, quantity, retstocklocn, retitemno, retval, vanno,
	--							buffbranchno, buffno, loadno, dateprinted, printedby,
	--							picklistnumber, ItemID, RetItemID,ParentItemID)						--IP - 26/05/11 - CR1212 - RI - #3636
	--	VALUES				 (	@origbr,@acctno, 1, @datedelplan, 'R', '',				--IP - 26/05/11 - CR1212 - RI - #3636 - blank for itemno
	--							@stocklocn, @quantity, @retstocklocn,'', @retval, '',	--IP - 26/05/11 - CR1212 - RI - #3636 - blank for retitemno
	--							@buffbranchno,@buffno, 0, null, 0, 0, @itemID, @retItemID,@parentItemID)	--IP - 26/05/11 - CR1212 - RI - #3636
	--END
	
	--IF(@delorcoll = 'S')				-- #10686 jec 

	--#13829

	BEGIN
		INSERT INTO LineItemBookingSchedule(LineItemID, DelOrColl,RetItemID,RetVal,RetStockLocn, BookingId, Quantity, ItemID, StockLocn, Price, RepUnitPrice)  --#13829
		VALUES (@lineItemId, 'R', @retItemID, @retval,@retstocklocn, 0,@quantity, @itemID, @stocklocn, @price, @repUnitPrice) --#13829	--#12842
	END

	UPDATE	lineitem 
	SET  deliveryprocess = @delorcoll,  
		 deliveryarea = @deliveryarea,
		 notes = 'Redelivery ' + @retitemno, -- IP - 19/02/10 - CR1072 - LW 69770 - Printing Fixes from 4.3 - Merge 
		 --datereqdel = @datereqdel,  
		 datereqdel = case when @delorcoll!= 'S' then @datereqdel else @datedelplan end, --IP - 14/06/12 - #10386
		 delnotebranch = @retstocklocn,
		 qtydiff = 'Y' -- IP - 19/02/10 - CR1072 - LW 69770 - Printing Fixes from 4.3 - Merge
		 --RM: Livewire 71116 update the delivery note branch when a reposessed
		 --line item is redelivered so that the delnote can be printed from the
		 --branch the stock is now located at instead of the original branch
	WHERE	acctno = @acctno
	AND     agrmtno = @agrmtno
	AND		stocklocn = @stocklocn 
	--AND		itemno = @itemno
	AND		ItemID = @itemID					--IP - 26/05/11 - CR1212 - RI - #3636
	AND     contractno = @contractno
	--AND		parentItemNo = @parentItemNo
	AND		ParentItemID = @parentItemID		--IP - 26/05/11 - CR1212 - RI - #3636
go

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
