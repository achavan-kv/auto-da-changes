
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_RFCardPrintListSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_RFCardPrintListSP
END
GO


CREATE PROCEDURE DN_RFCardPrintListSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_RFCardPrintListSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : List the customer details for the RF card print CSV
-- Author       : D Richardson
-- Date         : 19 May 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piPrivilege			SMALLINT,
    @return                 INTEGER       OUTPUT

AS  DECLARE
    -- Local variables
    @privilegeCheck         VARCHAR(10)

BEGIN
    SET NOCOUNT ON
    SET @return = 0
    
    TRUNCATE TABLE RFCardPrint

	IF (@piPrivilege = 0) SET @privilegeCheck = ' AND NOT '
	ELSE SET @privilegeCheck = ' AND '

	EXECUTE
	   ('INSERT INTO RFCardPrint ' +
	    '       (Title, ' +
        '        FirstName, ' +
        '        Name, ' +
        '        CustId, ' +
        '        RFCreditLimit , ' +
        '        RFCardSeqNo , ' +
        '        CusAddr1, ' +
        '        CusAddr2, ' +
        '        CusAddr3, ' +
        '        CusPoCode, ' +
        '        ToDate) ' +
	    ' SELECT c.Title, ' +
        '        c.FirstName, ' +
        '        c.Name, ' +
        '        c.CustId, ' +
        '        c.RFCreditLimit , ' +
        '        c.RFCardSeqNo + 1 , ' +
        '        cu.CusAddr1, ' +
        '        cu.CusAddr2, ' +
        '        cu.CusAddr3, ' +
        '        cu.CusPoCode, ' +
        '        CONVERT(VARCHAR(10), GETDATE(), 103) AS ToDate ' +
        ' FROM Customer c, CustAddress cu ' +
        ' WHERE c.RFCreditLimit > 0 ' +
        ' AND   c.RFCardPrinted = ''N'' ' +
        ' AND   cu.CustId = c.CustId ' +
        ' AND   cu.AddType = ''H'' ' +
        ' AND   cu.DateMoved IS NULL ' +
        ' AND EXISTS (SELECT * FROM Proposal p, Acct a ' +
        '             WHERE  p.CustId = c.CustId ' +
        '             AND    p.PropResult = ''A'' ' +
        '             AND    a.Acctno = p.Acctno ' +
        '             AND    a.AcctType = ''R'' ) ' +
        ' AND EXISTS (SELECT * FROM CustAcct ca, Acct a ' +
        '             WHERE  ca.custid = c.custid ' +
        '             AND    ca.HldOrJnt = ''H'' ' +
        '             AND    a.Acctno = ca.Acctno ' +
        '             AND    a.accttype = ''R'' ' +
        '             AND    a.currstatus != ''S'') ' +
        @privilegeCheck +
        ' EXISTS (SELECT * FROM CustCatCode ct ' +
        '         WHERE  ct.CustId = c.CustId ' +
        '         AND    ct.Code = ''CLAC'' ' +
        '         AND    ct.DateDeleted IS NULL) ')


	-- Return list of customer details
	SELECT ISNULL(Title,' ') + ',' +
		   ISNULL(FirstName,' ') + ',' +
		   ISNULL(Name,' ') + ',' +
		   ISNULL(CustId,' ') + ',' +
		   ISNULL(CAST(RFCreditLimit AS VARCHAR),' ') + ',' +
		   ISNULL(CAST(RFCardSeqNo AS VARCHAR),' ') + ',' +
		   ISNULL(CusAddr1,' ') + ',' +
		   ISNULL(CusAddr2,' ') + ',' +
		   ISNULL(CusAddr3,' ') + ',' +
		   ISNULL(CusPoCode,' ') + ',' +
		   ToDate AS RecordLine
	FROM RFCardPrint


    SET NOCOUNT OFF
    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_RFCardPrintListSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
