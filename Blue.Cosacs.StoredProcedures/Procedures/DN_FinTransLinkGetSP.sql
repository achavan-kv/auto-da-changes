SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FinTransLinkGetSP]')
                                          and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FinTransLinkGetSP]
GO


CREATE PROCEDURE DN_FinTransLinkGetSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_FinTransLinkGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Retrieve a link between two accounts for a Financial Transaction
-- Author       : D Richardson
-- Date         : 7 January 2005
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piAcctNo         CHAR(12),
    @piDateTrans      DATETIME,
    @piBranchNo       INT,
    @piTransRefNo     INT,

    @poLinkedAcctNo   CHAR(12) OUTPUT,
    @Return           INT OUTPUT

AS --DECLARE
    -- Local variables

BEGIN

    SET NOCOUNT ON
    SET @Return = 0

    SELECT @poLinkedAcctNo = LinkedAcctNo
    FROM   FinTransAccount
    WHERE  AcctNo     = @piAcctNo
    AND    DateTrans  = @piDateTrans
    AND    BranchNo   = @piBranchNo
    AND    TransRefNo = @piTransRefNo

    SET @Return = @@error

    RETURN @Return
END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

