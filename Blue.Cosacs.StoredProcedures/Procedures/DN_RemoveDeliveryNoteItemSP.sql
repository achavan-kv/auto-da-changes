SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_RemoveDeliveryNoteItemSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_RemoveDeliveryNoteItemSP
END
GO

CREATE PROCEDURE DN_RemoveDeliveryNoteItemSP
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_RemoveDeliveryNoteItemSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Remove a Line Item from a Delivery Note
-- Author       : D Richardson
-- Date         : 4 Aug 2005
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 18/11/09  IP  CR929 & 974 - For an item that has been removed from a Delivery Note 
--				 from the Delivery Notification screen, update the 'Type' column on table 
--				 ScheduleRemoval to 'I'.
-- 31/03/10 jec CR1072 Add Original Buff no to scheduleremoval
--------------------------------------------------------------------------------

    -- Parameters
    @piStockLocn        INTEGER,
    @piBuffNo           INTEGER,
    @piAcctNo           char(12),
    @piAgrmtNo          INTEGER,
    @piItemId           int,
    @piEmpeeNo          INTEGER,
    @piReason           varchar(80),
    @multiple           BIT,        -- 69557
    @Return             INTEGER  OUT

AS DECLARE
    -- Local variables
    @AcctNo             char(12),
    @ActionNo           INTEGER

BEGIN

    -- Get the AcctNo for the BailAction record
    SELECT  @AcctNo = AcctNo
    FROM    Schedule
    WHERE   StockLocn    = @piStockLocn
    AND     BuffNo       = @piBuffNo
    AND     AcctNo       = @piAcctNo
    AND     AgrmtNo      = @piAgrmtNo
    AND     ItemID       = @piItemId

    -- Save an audit record for the Schedule item removal
    INSERT INTO ScheduleRemoval
        (AcctNo, AgrmtNo, ItemNo, ItemID, StockLocn, Quantity, Price,
         DeliveryArea, BuffNo, LoadNo, DateRemoved, RemovedBy, Type,OrigBuffno)
    SELECT  s.AcctNo, s.AgrmtNo, s.ItemNo, s.ItemID, s.StockLocn, s.Quantity, l.Price,
            l.DeliveryArea, s.BuffNo, s.LoadNo, GETDATE(), @piEmpeeNo, 'I',  --IP - 18/11/09 - CR929 & 974 - Audit - Added 'Type'
            s.OrigBuffno	-- CR1072 jec 31/03/10
    FROM    Schedule s, LineItem l
    WHERE   s.StockLocn  = @piStockLocn
    AND     s.BuffNo     = @piBuffNo
    AND     s.AcctNo     = @piAcctNo
    AND     s.AgrmtNo    = @piAgrmtNo
    AND     s.ItemID     = @piItemId
    AND     l.AcctNo     = s.Acctno
    AND     l.AgrmtNo    = s.AgrmtNo
    AND     l.ItemId     = s.ItemId
    AND     l.StockLocn  = s.StockLocn



    -- Reset the Line Item planned delivery date
    DECLARE @canceldelnote varchar(8)
	
	SELECT @canceldelnote = Value FROM CountryMaintenance WHERE [Name] LIKE 'Cancel Delivery Note if failed'

	IF @canceldelnote ='FALSE' -- increment the delivery note 
	BEGIN
		
		UPDATE  LineItem
		SET     DatePlanDel = ''
		WHERE   LineItem.AcctNo     = @piAcctno
		AND     LineItem.AgrmtNo    = @piAgrmtNo
		AND     LineItem.ItemID     = @piItemId
		AND     LineItem.StockLocn  = @piStockLocn


		-- Reset the Load No to zero on the Schedule
		-- and increment the Undelivered Flag to A, B, C ... Z
		UPDATE  Schedule
		SET     LoadNo = 0,
				DatePrinted = null,
				PrintedBy = 0,
				PickListNumber = 0,
				DatePickListPrinted = null,
				DateDelPlan = CONVERT(DATETIME, '1 Jan 1900', 106),
				UndeliveredFlag = CASE LTRIM(UndeliveredFlag)
									  WHEN '' THEN 'A'
									  WHEN 'Z' THEN 'Z'
									  ELSE char(ASCII(UndeliveredFlag) + 1)
								  END
		WHERE   StockLocn    = @piStockLocn
		AND     BuffNo       = @piBuffNo
		AND     AcctNo       = @piAcctNo
		AND     AgrmtNo      = @piAgrmtNo
		AND     ItemID       = @piItemId

     END

	 DECLARE @newbuffno INT
     IF @canceldelnote ='True' -- Mauritius want new buffno generated so don't get duplicate delivery notes
	 BEGIN
	 -- 69557 Multiple items with the same acct no and stock location should be given the same buff no
		IF(@multiple = 1)
	    BEGIN
			SELECT @newbuffno = (SELECT TOP 1 buffno FROM schedule WHERE buffbranchno = @piStockLocn AND acctno = @piacctno ORDER BY buffno DESC)
		END
		ELSE
		BEGIN
			UPDATE branch SET hibuffno = hibuffno + 1 WHERE branchno =@piStockLocn

		    SELECT @newbuffno = hibuffno FROM branch WHERE branchno = @piStockLocn
		END
		
		UPDATE  schedule 
		SET     buffno = @newBuffNo,
				UndeliveredFlag = '',
      			DatePrinted = null,
				PrintedBy = 0,
				PickListNumber = 0,
				DatePickListPrinted = NULL,
				loadno = 0 -- ****** PATCH C UPDATE *****
				
	
		WHERE  AcctNo       = @piacctno
		AND    ItemID       = @piItemId
		AND    StockLocn    = @pistocklocn
		AND    BuffNo       = @pibuffno
     END
     

	-- Delete the Delivery Load record if all line items have been removed
	IF NOT EXISTS (SELECT * FROM Schedule
				   WHERE  StockLocn = @piStockLocn
				   AND    BuffNo    = @piBuffNo
				   AND    LoadNo   != 0)
	BEGIN
		DELETE FROM DeliveryLoad
		WHERE  BuffBranchNo = @piStockLocn
		AND    BuffNo       = @piBuffNo
	END
   

    IF (ISNULL(@AcctNo,'') != '')
    BEGIN
        -- Add the reason as an action on BailAction
        -- First get the last action number for this key value
        SELECT @ActionNo = MAX(ActionNo)
        FROM   BailAction
        WHERE  EmpeeNo = @piEmpeeNo
        AND    AcctNo  = @AcctNo
        AND    AllocNo = 0

        INSERT INTO BailAction
           (acctno,
            allocno,
            actionno,
            empeeno,
            dateadded,
            code,
            actionvalue,
            datedue,
            amtcommpaidon,
            notes,
            addedby)
        VALUES
           (@AcctNo,
            0,
            ISNULL(@ActionNo,0) + 1,
            @piEmpeeNo,
            GETDATE(),
            'NDEL',
            0,
            GETDATE(),
            0,
            @piReason,
            @piEmpeeNo)
    END


    SET @Return = @@ERROR
    RETURN @Return
END

GO

GRANT EXECUTE ON DN_RemoveDeliveryNoteItemSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
