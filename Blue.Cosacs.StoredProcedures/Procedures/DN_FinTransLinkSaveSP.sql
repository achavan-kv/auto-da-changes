SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FinTransLinkSaveSP]')
                                          and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FinTransLinkSaveSP]
GO


CREATE PROCEDURE DN_FinTransLinkSaveSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_FinTransLinkSaveSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save a link between two accounts for a Financial Transaction
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
    @piLinkedAcctNo   CHAR(12),
    @piDateTrans      DATETIME,
    @piBranchNo       INT,
    @piTransRefNo     INT,

    @Return           INT OUTPUT

AS --DECLARE
    -- Local variables

BEGIN

    SET NOCOUNT ON
    SET @Return = 0

    INSERT INTO FinTransAccount
        (AcctNo, LinkedAcctNo, DateTrans, BranchNo, TransRefNo)
    VALUES
        (@piAcctNo, @piLinkedAcctNo, @piDateTrans, @piBranchNo, @piTransRefNo)

    SET @Return = @@error

    RETURN @Return
END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

