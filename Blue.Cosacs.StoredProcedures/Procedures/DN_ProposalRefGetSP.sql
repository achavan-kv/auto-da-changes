SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalRefGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalRefGetSP]
GO


CREATE PROCEDURE dbo.DN_ProposalRefGetSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_ProposalRefGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get sanction references by Account No
-- Author       : D Richardson
-- Date         : 15 Oct 2003
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    
    @acctNo             VARCHAR(12),
    
    @Return             INTEGER  OUTPUT

AS DECLARE

    -- Local variables

    @RowCount           INT,
    @AltAcctNo          VARCHAR(12),
    @DateAcctOpen       DATETIME

BEGIN

    SET NOCOUNT ON
    SET QUOTED_IDENTIFIER ON
    SET @Return = 0

    --
    -- If there are no references on this account then:
    -- 1) For an RF account copy from latest non 'C'/'S' account;
    -- 2) For a non 'C'/'S'/'R' account copy only if Country.CopyReferences = 1
    --
    
    -- Prepare the AcctNo of the latest account with references
    SELECT TOP 1
           @AltAcctNo = a.AcctNo,
           @DateAcctOpen = a.DateAcctOpen
    FROM   CustAcct ca1, CustAcct ca2, Acct a, ProposalRef pr
    WHERE  ca1.AcctNo = @acctno
    AND    ca1.HldOrJnt = 'H'
    AND    ca2.Custid = ca1.CustId
    AND    ca2.HldOrJnt = 'H'
    AND    a.AcctNo = ca2.AcctNo
    AND    a.AcctType NOT IN ('C','S')
    AND    pr.AcctNo = a.AcctNo
    ORDER BY a.DateAcctOpen DESC, a.AcctNo DESC


    -- Retrieve either for this AcctNo or the alternative AcctNo
    SELECT RefNo               AS RefNo,
           Name                AS RefFirstName,
           Surname             AS RefLastName,
           Relation            AS RefRelation,
           YrsKnown            AS YrsKnown,
           Address1            AS RefAddress1,
           Address2            AS RefAddress2,
           City                AS RefCity,
           PostCode            AS RefPostCode,
           WAddress1           AS RefWAddress1,
           WAddress2           AS RefWAddress2,
           WCity               AS RefWCity,
           WPostCode           AS RefWPostCode,
           Tel                 AS RefPhoneNo,
           TelCode             AS RefDialCode,
           WTel                AS RefWPhoneNo,
           WTelCode            AS RefWDialCode,
           MTel                AS RefMPhoneNo,
           MTelCode            AS RefMDialCode,
           Directions          AS RefDirections,
           Comment             AS RefComment,
           DateChange          AS DateChange,
           EmpeeNoChange       AS EmpeeNoChange,
		   ''				   AS NewComment 			
    FROM   ProposalRef
    WHERE  Acctno = @acctno
    UNION
    SELECT pr.RefNo            AS RefNo,
           pr.Name             AS RefFirstName,
           pr.Surname          AS RefLastName,
           pr.Relation         AS RefRelation,
           pr.YrsKnown         AS YrsKnown,
           pr.Address1         AS RefAddress1,
           pr.Address2         AS RefAddress2,
           pr.City             AS RefCity,
           pr.PostCode         AS RefPostCode,
           pr.WAddress1        AS RefWAddress1,
           pr.WAddress2        AS RefWAddress2,
           pr.WCity            AS RefWCity,
           pr.WPostCode        AS RefWPostCode,
           pr.Tel              AS RefPhoneNo,
           pr.TelCode          AS RefDialCode,
           pr.WTel             AS RefWPhoneNo,
           pr.WTelCode         AS RefWDialCode,
           pr.MTel             AS RefMPhoneNo,
           pr.MTelCode         AS RefMDialCode,
           pr.Directions       AS RefDirections,
           pr.Comment          AS RefComment,
           pr.DateChange       AS DateChange,
           pr.EmpeeNoChange    AS EmpeeNoChange,
		   ''				   AS NewComment 			
    FROM   Country c, Acct a1, ProposalRef pr
    WHERE  NOT EXISTS (SELECT AcctNo FROM ProposalRef WHERE AcctNo = @acctNo)
    AND    a1.AcctNo = @acctno
    AND    (c.CopyReferences = 1 OR a1.AcctType = 'R')
    AND    pr.AcctNo = @AltAcctNo
    ORDER BY RefNo


    SET @Return = @@ERROR
    
    RETURN @Return
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

