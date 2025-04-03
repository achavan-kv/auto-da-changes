SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalRefListSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalRefListSP]
GO


CREATE PROCEDURE dbo.DN_ProposalRefListSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_ProposalRefListSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get sanction references by Cust Id
-- Author       : D Richardson
-- Date         : 21 Oct 2003
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    
    @custId             VARCHAR(20),
    
    @Return             INTEGER  OUTPUT

AS DECLARE

    -- Local variables

    @RowCount           INT

BEGIN

    SET NOCOUNT ON
    SET QUOTED_IDENTIFIER ON
    SET @Return = 0

    -- Retrieve from all accounts for this customer
    SELECT DISTINCT c.CodeDescript AS RefRelationText,
           pr.Name                AS RefFirstName,
           pr.Surname             AS RefLastName,
           pr.Relation            AS RefRelation,
           pr.YrsKnown            AS YrsKnown,
           pr.Address1            AS RefAddress1,
           pr.Address2            AS RefAddress2,
           pr.City                AS RefCity,
           pr.PostCode            AS RefPostCode,
           pr.WAddress1           AS RefWAddress1,
           pr.WAddress2           AS RefWAddress2,
           pr.WCity               AS RefWCity,
           pr.WPostCode           AS RefWPostCode,
           pr.Tel                 AS RefPhoneNo,
           pr.TelCode             AS RefDialCode,
           pr.WTel                AS RefWPhoneNo,
           pr.WTelCode            AS RefWDialCode,
           pr.MTel                AS RefMPhoneNo,
           pr.MTelCode            AS RefMDialCode,
           pr.Directions          AS RefDirections,
           pr.Comment             AS RefComment,
           pr.DateChange          AS DateChange,
           pr.EmpeeNoChange       AS EmpeeNoChange
    FROM   CustAcct ca, Acct a, ProposalRef pr, Code c
    WHERE  ca.CustId = @custId
    AND    ca.HldOrJnt = 'H'
    AND    a.AcctNo  = ca.AcctNo
    AND    pr.AcctNo = ca.AcctNo
    AND    c.Category = 'RL1'
    AND    c.Code = pr.Relation

    SET @Return = @@ERROR
    
    RETURN @Return
END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

