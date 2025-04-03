SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_ChangeReqDelDateSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_ChangeReqDelDateSP
END
GO


CREATE PROCEDURE DN_ChangeReqDelDateSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_ChangeReqDelDateSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Delivery Notification date on LineItem and Schedule
-- Author       : D Richardson
-- Date         : 22 July 2005
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piAcctNo           CHAR(12),
    @piAgrmtNo          INTEGER,
    @piItemId           INT,
    @piStockLocn        SMALLINT,
    @piContractNo       VARCHAR(10),
    @piReqDelDate       DATETIME,
    @piBuffNo			INTEGER,
    @piReason           VARCHAR(80),
    @piEmpeeNo			INTEGER,
    @Return             INTEGER  OUT

AS 
   DECLARE	@AcctNo		CHAR(12),
			@ActionNo	INTEGER

BEGIN

    -- Pick the first AcctNo for the BailAction record
    SELECT  @AcctNo = MIN(AcctNo)
    FROM    Schedule
    WHERE   StockLocn = @piStockLocn
    AND     BuffNo = @piBuffNo
    AND     LoadNo != 0
    
    -- Reset the Load No to zero on the Schedule
    -- and increment the Undelivered Flag to A, B, C ... Z
    UPDATE  Schedule
    SET     LoadNo = 0,
            DatePrinted = null,
            PrintedBy = 0,
            PickListNumber = 0,
            DatePickListPrinted = null,
            DateDelPlan = @piReqDelDate,
            UndeliveredFlag = CASE LTRIM(UndeliveredFlag)
                                  WHEN '' THEN 'A'
                                  WHEN 'Z' THEN 'Z'
                                  ELSE CHAR(ASCII(UndeliveredFlag) + 1)
                              END
    WHERE   AcctNo = @piAcctNo
    AND     AgrmtNo = @piAgrmtNo
    AND     ItemId = @piItemId
    AND     StockLocn = @piStockLocn
    AND     BuffNo = @piBuffNo
    AND     LoadNo != 0
    
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

    -- Update the Line Item
    UPDATE  LineItem
    SET     DateReqDel  = @piReqDelDate
    WHERE   AcctNo    = @piAcctNo
    AND     AgrmtNo   = @piAgrmtNo
    AND     ItemId    = @piItemId
    AND     StockLocn = @piStockLocn
    AND     ContractNo = @piContractNo
    
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
GRANT EXECUTE ON DN_ChangeReqDelDateSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
