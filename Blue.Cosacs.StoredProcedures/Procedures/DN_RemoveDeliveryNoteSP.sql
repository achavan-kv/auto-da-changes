SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_RemoveDeliveryNoteSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_RemoveDeliveryNoteSP
END
GO


CREATE PROCEDURE DN_RemoveDeliveryNoteSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_RemoveDeliveryNoteSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Remove a Delivery Note
-- Author       : D Richardson
-- Date         : 22 July 2005
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 18/11/09  IP  CR929 & 974 - For a Delivery Note that has been removed from the 
--				 Delivery Notification screen, update the 'Type' column on table 
--				 ScheduleRemoval to 'R'.
--------------------------------------------------------------------------------

    -- Parameters
    @piStockLocn        INTEGER,
    @piBuffNo           INTEGER,
    @piEmpeeNo          INTEGER,
    @piReason           VARCHAR(80),
    @Return             INTEGER  OUT

AS DECLARE
    -- Local variables
    @AcctNo             CHAR(12),
    @ActionNo           INTEGER

BEGIN

    -- Pick the first AcctNo for the BailAction record
    SELECT  @AcctNo = MIN(AcctNo)
    FROM    Schedule
    WHERE   StockLocn = @piStockLocn
    AND     BuffNo = @piBuffNo
    AND     LoadNo != 0

    -- Save an audit record for the Schedule removal
    INSERT INTO ScheduleRemoval
        (AcctNo, AgrmtNo, ItemNo, ItemID, StockLocn, Quantity, Price,
         DeliveryArea, BuffNo, LoadNo, DateRemoved, RemovedBy, Type)
    SELECT  s.AcctNo, s.AgrmtNo, s.ItemNo, s.ItemID, s.StockLocn, s.Quantity, l.Price,
            l.DeliveryArea, s.BuffNo, s.LoadNo, GETDATE(), @piEmpeeNo, 'R'  --IP - 18/11/09 - CR929 & 974 - Audit - Added 'Type'
    FROM    Schedule s, LineItem l
    WHERE   s.StockLocn  = @piStockLocn
    AND     s.BuffNo     = @piBuffNo
    AND     s.LoadNo    != 0
    AND     l.AcctNo     = s.Acctno
    AND     l.AgrmtNo    = s.AgrmtNo
    AND     l.ItemID     = s.ItemID
    AND     l.StockLocn  = s.StockLocn
    AND     l.Iskit      = 0

    -- Reset the Line Item planned delivery date
    UPDATE  LineItem
    SET     DatePlanDel = ''
    FROM    Schedule s
    WHERE   s.StockLocn         = @piStockLocn
    AND     s.BuffNo            = @piBuffNo
    AND     s.LoadNo           != 0
    AND     LineItem.AcctNo     = s.Acctno
    AND     LineItem.AgrmtNo    = s.AgrmtNo
    AND     LineItem.ItemID     = s.ItemID
    AND     LineItem.StockLocn  = s.StockLocn
    AND     LineItem.Iskit      = 0


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
                                  ELSE CHAR(ASCII(UndeliveredFlag) + 1)
                              END
    WHERE   StockLocn = @piStockLocn
    AND     BuffNo = @piBuffNo
    AND     LoadNo != 0    -- Don't increment the flag for items already removed


    -- Delete the Delivery Load record
    DELETE FROM DeliveryLoad
    WHERE  BuffBranchNo = @piStockLocn
    AND    BuffNo       = @piBuffNo
    

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
GRANT EXECUTE ON DN_RemoveDeliveryNoteSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
