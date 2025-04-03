SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DeleteDeliveryNoteSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DeleteDeliveryNoteSP
END
GO

CREATE PROCEDURE DeleteDeliveryNoteSP
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DeleteDeliveryNoteSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : DeleteDeliveryNoteSP.sql
-- Description	: When a Delivery Note has been deleted from the Delivery Notification screen
--				  delete the delivery note from schedule and deliveryload, add record to scheduleremoval,
--				  and a bailaction record.
-- Author       : Ilyas Parker
-- Date         : 18th February 2009
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 18/11/09  IP  CR929 & 974 - For a Delivery Note that has been deleted from the 
--				 Delivery Notification screen, update the 'Type' column on table 
--				 ScheduleRemoval to 'D'.
-- 31/03/10 jec CR1072 Add Original Buff no to scheduleremoval
-- 20/06/11 ip  CR1212 - RI - #4046 - RI System Changes (Error in Delivery Notification screen).
--------------------------------------------------------------------------------

    -- Parameters
    @stockLocn        INT,
    @buffNo           INT,
    @empeeNo          INT,
    @reason           VARCHAR(80),
    @return           INT  out

AS DECLARE
    -- Local variables
    @acctNo             CHAR(12),
    @actionNo           INT,
	@itemno				VARCHAR(10),
	@contractNo			VARCHAR(20),	
	@parentitemNo		VARCHAR(10),
	@agrmtNo			INT

BEGIN

    -- Pick the first AcctNo for the BailAction record
    SELECT  @acctNo = MIN(AcctNo)
    FROM    Schedule
    WHERE   StockLocn = @stockLocn
    AND     BuffNo = @buffNo
    AND     LoadNo != 0

    -- Save an audit record for the Schedule removal
    INSERT INTO ScheduleRemoval
        (AcctNo, AgrmtNo, ItemNo, StockLocn, Quantity, Price,
         DeliveryArea, BuffNo, LoadNo, DateRemoved, RemovedBy, Type,OrigBuffno, ItemID)		--IP - 20/06/11 - CR1212 - RI - #4046
    SELECT  s.AcctNo, s.AgrmtNo, '', s.StockLocn, s.Quantity, l.Price,
            l.DeliveryArea, s.BuffNo, s.LoadNo, GETDATE(), @empeeNo, 'D',  --IP - 18/11/09 - CR929 & 974 - Audit - Added 'Type'
            s.OrigBuffno, s.ItemID	-- CR1072 jec 31/03/10	--IP - 20/06/11 - CR1212 - RI - #4046
    FROM    Schedule s, LineItem l
    WHERE   s.StockLocn  = @stockLocn
    AND     s.BuffNo     = @buffNo
    AND     s.LoadNo    != 0
    AND     l.AcctNo     = s.Acctno
    AND     l.AgrmtNo    = s.AgrmtNo
    --AND     l.ItemNo     = s.ItemNo
    AND     l.ItemID     = s.ItemID				--IP - 20/06/11 - CR1212 - RI - #4046
    AND     l.StockLocn  = s.StockLocn
    AND     l.Iskit      = 0



	--Delete the schedule records for items on the deleted delivery note.
	DELETE
	FROM Schedule
	WHERE StockLocn = @stockLocn
	AND     BuffNo = @buffNo
	AND     LoadNo != 0


    -- Delete the Delivery Load record
    DELETE FROM DeliveryLoad
    WHERE  BuffBranchNo = @stockLocn
    AND    BuffNo       = @buffNo
    

    IF (ISNULL(@acctNo,'') != '')
    BEGIN
        -- Add the reason as an action on BailAction
        -- First get the last action number for this key value
        SELECT @actionNo = MAX(ActionNo)
        FROM   BailAction
        WHERE  EmpeeNo = @empeeNo
        AND    AcctNo  = @acctNo
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
           (@acctNo,
            0,
            ISNULL(@actionNo,0) + 1,
            @empeeNo,
            GETDATE(),
            'NDEL',
            0,
            GETDATE(),
            0,
            @reason,
            @empeeNo)
    END


    SET @Return = @@ERROR
    RETURN @Return
END

GO
GRANT EXECUTE ON DeleteDeliveryNoteSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End
