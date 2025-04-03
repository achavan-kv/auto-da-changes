SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_PCCustomerTiersUpdateSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_PCCustomerTiersUpdateSP
END
GO


CREATE PROCEDURE DN_PCCustomerTiersUpdateSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_PCCustomerTiersUpdateSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Promote or demote customers in Privilege Club Tier1/2
-- Author       : D Richardson
-- Date         : 9 Feb 2006
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @return         INTEGER OUTPUT

AS  DECLARE
    -- Local variables
    @DateToday      SMALLDATETIME,
    @EmpeeNoCode    INTEGER,
    @Reference      VARCHAR(10)

BEGIN
    SET @return = 0
    SET @DateToday = GETDATE()
    SET @EmpeeNoCode = 0
    SET @Reference = ''

    -- Record old Privilege Club members
    INSERT INTO OldPCMember (Custid, DateRemoved)
    SELECT DISTINCT CustId, @DateToday
    FROM   CustCatCode cc
    WHERE  cc.Code = 'CLAC'
    AND    ISNULL(cc.DateDeleted,'') = ''
    AND    NOT EXISTS (SELECT 1 FROM CustCatCode cc2
                       WHERE  cc2.CustId = cc.CustId
                       AND    cc2.Code IN ('CLAS', 'CLAW')
                       AND    ISNULL(cc2.DateDeleted,'') = '')
    AND    NOT EXISTS (SELECT 1 FROM OldPCMember opcm
                       WHERE  opcm.Custid = cc.CustId)

    -- Remove the old customer code
    UPDATE CustCatCode
    SET    DateDeleted = @DateToday
    WHERE  Code IN ('CLAC', 'CLAS', 'CLAW')
    AND    ISNULL(DateDeleted,'') = ''

    -- Add the new code for new Tier1 customers
    INSERT INTO CustCatCode
        (CustId, DateCoded, Code, DateDeleted, EmpeeNoCode, Reference)
    SELECT
        CustId, @DateToday, 'TIR1', Null, @EmpeeNoCode, @Reference
    FROM  vw_PCToQualifyTier1
    where not exists (select * from custcatcode C where c.code ='TIR1' AND c.custid = vw_PCToQualifyTier1.custid)
    group by custid
    -- Create letters for new Tier1 customers
    INSERT INTO Letter
        (AcctNo, DateAcctLttr, DateDue, LetterCode, AddToValue)
    SELECT
        AcctNo, @DateToday, @DateToday, LetterCode, 0
    FROM  vw_PCToQualifyTier1

    -- Add the new code for new Tier2 customers
    INSERT INTO CustCatCode
        (CustId, DateCoded, Code, DateDeleted, EmpeeNoCode, Reference)
    SELECT
        CustId, @DateToday, 'TIR2', Null, @EmpeeNoCode, @Reference
    FROM  vw_PCToQualifyTier2
    where not exists (select * from custcatcode C where c.code ='TIR2' AND c.custid = vw_PCToQualifyTier2.custid)
    		and custid not like '%,%'  and custid not like '%''%'  and custid !=''  
    group by custid

    -- Create letters for new Tier2 customers
    INSERT INTO Letter
        (AcctNo, DateAcctLttr, DateDue, LetterCode, AddToValue)
    SELECT
        AcctNo, @DateToday, @DateToday, LetterCode, 0
    FROM  vw_PCToQualifyTier2

    -- Mark the codes as deleted where there is a demotion
    UPDATE CustCatCode
    SET    DateDeleted = @DateToday
    FROM   vw_PCToDemoteCustomer dc
    WHERE  CustCatCode.Code IN ('TIR1','TIR2')
    AND    ISNULL(CustCatCode.DateDeleted,'') = ''
    AND    dc.CustId = CustCatCode.CustId

    -- Some demotions may be from Tier2 to Tier1
    INSERT INTO CustCatCode
        (CustId, DateCoded, Code, DateDeleted, EmpeeNoCode, Reference)
    SELECT
        CustId, @DateToday, 'TIR1', Null, @EmpeeNoCode, @Reference
    FROM   vw_PCToDemoteCustomer dc
    WHERE  dc.NewPC = 'TIR1'

    --
    -- Warning Letters
    --
    -- Mark as deleted warning customer codes without a current warning
    UPDATE CustCatCode
    SET    DateDeleted = @DateToday
    WHERE  Code = 'TIRW'
    AND    NOT EXISTS (SELECT 1 FROM PCCustomerTiers t
                       WHERE  t.CustId = CustCatCode.CustId
                       AND    t.LetterCode = 'TIERWARN')

    -- Remove new warnings already with a current warning customer code
    -- (do not repeat letters for the same warning)
    UPDATE PCCustomerTiers
    SET    LetterCode = ''
    WHERE  LetterCode = 'TIERWARN'
    AND    EXISTS (SELECT 1 FROM CustCatCode cc
                   WHERE  cc.CustId = PCCustomerTiers.CustId
                   AND    cc.Code = 'TIRW'
                   AND    ISNULL(cc.DateDeleted,'') = '')

    -- Add new warning customer codes for all new warnings
    INSERT INTO CustCatCode
        (CustId, DateCoded, Code, DateDeleted, EmpeeNoCode, Reference)
    SELECT
        CustId, @DateToday, 'TIRW', Null, @EmpeeNoCode, @Reference
    FROM   PCCustomerTiers
    WHERE  LetterCode = 'TIERWARN'

    -- Create the new warning letters
    INSERT INTO Letter
        (AcctNo, DateAcctLttr, DateDue, LetterCode, AddToValue)
    SELECT
        AcctNo, @DateToday, @DateToday, LetterCode, 0
    FROM   PCCustomerTiers
    WHERE  LetterCode = 'TIERWARN'


    SET @return = @@ERROR
    RETURN @return
END

GO
GRANT EXECUTE ON DN_PCCustomerTiersUpdateSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

