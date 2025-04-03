SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DDMandateGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DDMandateGetSP]
GO


CREATE PROCEDURE dbo.DN_DDMandateGetSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDMandateGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get a Giro Mandate by Mandate Id or by Account No
-- Author       : D Richardson
-- Date         : 11 Aug 2003
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piMandateId        INTEGER,
    @piAcctNo           CHAR(12),
    
    @Return             INTEGER  OUTPUT

AS DECLARE
    -- Local variables
    @SQLStr             VARCHAR(1800),
    @RowCount           INT

BEGIN

    SET NOCOUNT ON
    SET QUOTED_IDENTIFIER ON
    SET @Return = 0

    /* Get the mandate record */
    SET @SQLStr =
        ' SELECT ' +
        '   ISNULL(man.mandateId,0)          AS mandateId, ' +
        '   ISNULL(man.acctNo,'''')          AS acctNo, ' +
        '   ISNULL(man.approvalDate,'''')    AS approvalDate, ' +
        '   ISNULL(man.bankAcctName,'''')    AS bankAccountName, ' +
        '   ISNULL(man.bankAcctNo,'''')      AS bankAccountNo, ' +
        '   ISNULL(man.bankBranchNo,'''')    AS bankBranchNo, ' +
        '   ISNULL(man.bankCode,'''')        AS bankCode, ' +
        '   ISNULL(man.cancelReason,'''')    AS cancelReason, ' +
        '   ISNULL(man.changedBy,'''')       AS changedBy, ' +
        '   ISNULL(man.comment,'''')         AS comment, ' +
        '   ISNULL(man.dueDayId,0)           AS dueDayId, ' +
        '   ISNULL(man.endDate,'''')         AS endDate, ' +
        '   ISNULL(man.noFees,0)             AS noFees,  ' +
        '   ISNULL(man.receiveDate,'''')     AS receiveDate, ' +
        '   ISNULL(man.rejectCount,0)        AS rejectCount, ' +
        '   ISNULL(man.returnDate,'''')      AS returnDate, ' +
        '   ISNULL(man.startDate,'''')       AS startDate, ' +
        '   ISNULL(man.status,'''')          AS status, ' +
        '   ISNULL(man.submitDate,'''')      AS submitDate, ' +
        '   ISNULL(customer.Title,'''') + '' '' ' +
            ' + ISNULL(customer.Name,'''') + '', '' ' +
            ' + ISNULL(customer.Firstname,'''') AS customerName, ' +
        '   ISNULL(inp.instalamount,0)       AS ''instal Amount'' ' +
        ' FROM  CustAcct, Customer, DDMandate man ' +
        ' LEFT OUTER JOIN instalplan inp ' +
        ' ON    inp.AcctNo = man.AcctNo ' +
        ' AND   inp.AgrmtNo = 1 ' +
        ' WHERE CustAcct.AcctNo = man.AcctNo ' +
        ' AND   CustAcct.HldOrJnt = ''H'' ' +
        ' AND   Customer.CustId = CustAcct.CustId ';


    IF @piMandateId != 0
    BEGIN
        SET @SQLStr = @SQLStr + ' AND man.MandateId = ' + CONVERT(VARCHAR,@piMandateId);
    END
    ELSE
    BEGIN
        /* There could be mandate history records for the account, so
        ** find the latest Mandate Id for the Account No.
        */
        SET @SQLStr = @SQLStr + 
            ' AND man.MandateId = ' +
                 ' (SELECT MAX(MandateId) ' +
                 '  FROM   DDMandate ' +
                 '  WHERE  AcctNo = ''' + @piAcctNo + ''')';
    END;

    EXECUTE (@SQLStr)

    -- Do not use two SET assignments which will clear the second value
    SELECT @Return = @@ERROR, @RowCount = @@ROWCOUNT


    IF (@Return = 0 AND @RowCount = 0 AND @piMandateId = 0)
    BEGIN
        /* Just get the customer name and any instalment amount */
        SELECT
              0         AS mandateId,
              @piAcctNo AS acctNo,
              NULL      AS approvalDate,
              ''        AS bankAccountName,
              ''        AS bankAccountNo,
              ''        AS bankBranchNo,
              ''        AS bankCode,
              ''        AS cancelReason,
              0         AS changedBy,
              ''        AS comment,
              0         AS dueDayId,
              NULL      AS endDate,
              0         AS noFees,
              NULL      AS receiveDate,
              0         AS rejectCount,
              NULL      AS returnDate,
              NULL      AS startDate,
              ''        AS status,
              NULL      AS submitDate,
              ISNULL(customer.Title,'') + ' ' +
                ISNULL(customer.Name,'') + ' ' +
                ISNULL(customer.Firstname,'') AS customerName,
              ISNULL(inp.instalamount,0) AS 'instal Amount'
        FROM  CustAcct, Customer
        LEFT OUTER JOIN instalplan inp
        ON    inp.AcctNo        = @piAcctNo
        AND   inp.AgrmtNo       = 1
        WHERE CustAcct.AcctNo   = @piAcctNo
        AND   CustAcct.HldOrJnt = 'H'
        AND   Customer.CustId   = CustAcct.CustId;

        SET @Return = @@ERROR
    END;


    return @Return
END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

