
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_ScheduleUpdateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleUpdateSP]
GO


CREATE PROCEDURE dbo.DN_ScheduleUpdateSP
-- =============================================
-- Author:		??
-- Create date: ??
-- Description:	Schedule Update
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 17/02/10  jec CR1072 Malaysia merge
-- 09/03/10  jec set Origuffno = Buffno
-- 21/05/10 UAT160 Remove merged 4.3 code
-- 27/04/11 jec CR1212 RI Integration - Use ItemID instead of ItemNo
-- 18/06/12 jec #10411 GRT screen should generate 'collect' booking
-- 27/06/12 ip  #10519 Schedule record was not being inserted for an immediate collection.
-- 29/06/12 ip  #10522 - discount was not being inserted into Schedule table.
-- 06/08/12 ip  #10789 - prevent inserting schedule record for stockitems, only non stocks
-- =============================================
    @origbr smallint,
    @acctno varchar(12),
    @agrmtno int,
    @datedelplan datetime,
    @delorcol char(1),
    --@itemno varchar(8),
    @itemId	INT,			-- RI
	@ParentItemId INT,
    @stocklocn smallint,
    @quantity float,
    --@retitemno varchar(8),
    @retitemId	INT,			-- RI
    @retstocklocn smallint,
    @retval float,
    @buffno int,
    @buffbranchno smallint,
    @vanno varchar(8),
    @loadNo smallint,
    @printedBy int,
    @changedBy int,
    @contractno varchar(10),
    @undeliveredflag char(1),
    @createdby INT, -- IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
    @dateCreated DATETIME, -- IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
    @GRTnotes VARCHAR(200),	-- IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
    @return int OUTPUT

AS DECLARE

    @CurRetStockLocn    SMALLINT,
    @CurQuantity        FLOAT,
    @DeliveryProcess    CHAR(1),
    @LineItemId			int,
    @ItemType			CHAR(1) --#10789

BEGIN

    SET @return = 0

	--IP - 29/06/12 - #10522 - no longer required
	--select @DeliveryProcess=deliveryprocess,@LineItemId=id 
	--from lineitem where acctno=@acctno and agrmtno=@agrmtno and stocklocn=@stocklocn and itemid=@itemId
	
	--#10789
	select @ItemType = ItemType from lineitem where acctno=@acctno and agrmtno=@agrmtno and stocklocn=@stocklocn and itemid=@itemId
	
    -- Audit Collection Note changes to StockLocn or Quantity
    IF (@DelOrCol = 'C')
    BEGIN
        -- First select filters on BuffNo
        SELECT  @CurRetStockLocn = RetStockLocn,
                @CurQuantity     = Quantity
        FROM    Schedule
        WHERE   AcctNo      = @acctNo
        AND     AgrmtNo     = @agrmtno
        --AND     ItemNo      = @itemno
        AND     ItemID      = @itemID			-- RI
        AND     StockLocn   = @stocklocn
        AND     BuffNo      = @buffNo
        AND		Contractno	= @contractno

        IF (@@ROWCOUNT = 0)
        BEGIN
            -- Second select uses zero BuffNo
            SELECT @CurRetStockLocn = RetStockLocn,
                   @CurQuantity     = Quantity
            FROM   Schedule
            WHERE   AcctNo      = @acctNo
            AND     AgrmtNo     = @agrmtno
            --AND     ItemNo      = @itemno
            AND     ItemID      = @itemID			-- RI
            AND     StockLocn   = @stocklocn
            AND     BuffNo      = 0
			AND		Contractno	= @contractno

            IF (@@ROWCOUNT = 0)
            BEGIN
                SET @CurRetStockLocn = @retstocklocn
                SET @CurQuantity     = @quantity
            END
        END

        IF (   @CurRetStockLocn != @retstocklocn
            OR @CurQuantity     != @quantity)
        BEGIN
            -- Audit the change
            EXEC DN_ScheduleCollectionAuditSP @AcctNo, @AgrmtNo, @ItemID, @CurRetStockLocn, @RetStockLocn, @CurQuantity, @Quantity, @ChangedBy, @return  -- RI
        END
    END

    -- Need to set a return value if the schedule record was already
    -- there so we know whether or not to update the lineitem.delqty

    -- First update filters on BuffNo
    UPDATE  Schedule
    SET     OrigBr          = @origbr,
            DelOrColl       = @delorcol,
            Quantity        = @quantity,
            RetStockLocn    = @retstocklocn,
            --RetItemNo       = @retItemNo,
            RetItemID       = @retItemID,			-- RI
            RetVal          = @retVal,
            DateDelPlan     = @datedelplan,
            VanNo           = @vanno,
            LoadNo          = @loadNo,
            PrintedBy       = @printedby,
            DatePrinted     = null,
            Contractno		= @contractno,
            UnDeliveredFlag = @undeliveredflag            
    WHERE   AcctNo          = @acctNo
    AND     AgrmtNo         = @agrmtno
    --AND     ItemNo          = @itemno
    AND     ItemID          = @itemID			-- RI
    AND     StockLocn       = @stocklocn
    --AND     BuffBranchNo    = @buffbranchno
    AND     BuffNo          = @buffNo
    AND		Contractno		= @contractno

    IF(@@rowcount = 0)
    BEGIN

        -- Second update uses zero BuffNo
        UPDATE  Schedule
        SET     OrigBr          = @origbr,
                DelOrColl       = @delorcol,
                Quantity        = @quantity,
                RetStockLocn    = @retstocklocn,
                --RetItemNo       = @retItemNo,
                RetItemID       = @retItemID,			-- RI
                RetVal          = @retVal,
                DateDelPlan     = @datedelplan,
                VanNo           = @vanno,
                LoadNo          = @loadNo,
                BuffBranchNo    = @buffbranchno,
                BuffNo          = @buffNo,
                PrintedBy       = @printedby,
                DatePrinted     = null,
				Contractno		= @contractno				
        WHERE   AcctNo          = @acctNo
        AND     AgrmtNo         = @agrmtno
        --AND     ItemNo          = @itemno
        AND     ItemID          = @itemID			-- RI
        AND     StockLocn       = @stocklocn
        --AND   BuffBranchNo      = 0
        AND     BuffNo          = 0
		AND		Contractno		= @contractno

        IF(@@rowcount = 0)
        BEGIN
			--if (@DeliveryProcess!='S' and @delorcol!='C')
			--if (@DeliveryProcess!='S')					--IP - 29/06/12 - #10522 - not required	--IP - 27/06/12 - #10519
			--Begin
			if(@ItemType = 'N')
			Begin
				INSERT INTO Schedule
				(
					OrigBr,
					AcctNo,
					AgrmtNo,
					datedelplan,
					DelOrColl,
					ItemNo,
					Quantity,
					RetStockLocn,
					RetItemNo,
					RetVal,
					BuffBranchNo,
					BuffNo,
					StockLocn,
					VanNo,
					LoadNo,
					PrintedBy,
					DatePrinted,
					Contractno,
					UnDeliveredFlag,
					CreatedBy,
					DateCreated,
					GRTnotes,
					OrigBuffNo,		-- jec - 09/03/10 - Malaysia 3PL
					ItemID,
					RetItemID,			-- RI                 
					ParentitemId
					
				)
				--VALUES --IP - 18/02/10 - CR1072 - LW 70913 - General Fixes from 4.3 - Merge - Commented out
				--(
				select
					@origbr,
					@acctNo,
					@agrmtno,
					@datedelplan,
					@delorcol,
					--@itemNo,
					'',				-- RI set ItemNo blank
					@quantity,
					@retstocklocn,
					--@retItemNo,
					'',				-- RI set RetItemNo blank
					@retVal,
					@buffbranchno,
					@buffNo,
					@stocklocn,
					@vanno,
					@loadNo,
					@printedby,
					null,
					@contractno,
					@undeliveredflag,
					@createdBy,	 -- CR1048 jec
					@dateCreated, -- CR1048 jec
					@GRTnotes, -- CR1048 jec
					@buffno,		-- jec - 09/03/10 - Malaysia 3PL                
					@ItemID,
					@RetItemID,			-- RI
					@ParentItemID
				--where  not (@delorcol = 'C' and (@itemno like '19%' or @itemno like 'XW%')) --IP - 18/02/10 - CR1072 - LW 70913 - General Fixes from 4.3 - Merge
				--) 
	            
				--sl fix schedule to collect warranty in lineitem --IP - 18/02/10 - CR1072 - LW 70913 - General Fixes from 4.3 - Merge
				--if @delorcol = 'C' and (@itemno like '19%' or @itemno like 'XW%') and @quantity<0  
				--begin  
				--	update lineitem  
				--	 set quantity=0  
				--	 where acctno=@acctno  
				--	 and agrmtno=@agrmtno  
				--	 and itemno=@itemno  
				--	 and stocklocn=@stocklocn  
				--	 and contractno=@contractno  
			 --   end  

				SET @return = 1
	            
            End
            
   --         else
   --         -- #10411  Insert into LineItemBookingSchedule for Warehouse if Delprocess is scheduled and collection
   --         BEGIN
			--	INSERT INTO LineItemBookingSchedule(LineItemID, DelOrColl,RetItemID,RetVal,RetStockLocn,BookingId,Quantity)
			--	SELECT @LineItemId, @delorcol, @retitemId, @retval, @retstocklocn, 0,@quantity				
				
			--END
        END
    END

    IF @@ERROR != 0 SET @return = @@ERROR
   

END
GO

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End