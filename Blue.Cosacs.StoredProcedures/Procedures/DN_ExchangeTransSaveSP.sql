SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ExchangeTransSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ExchangeTransSaveSP]
GO


CREATE PROCEDURE DN_ExchangeTransSaveSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_ExchangeTransSaveSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save a Foreign Exchange Transaction
-- Author       : D Richardson
-- Date         : 15 December 2003
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piAcctNo         CHAR(12),
    @piTransRefNo     INT,
    @piDateTrans      DATETIME,
    @piPayMethod      SMALLINT,
    @piForeignTender  MONEY,
    @piLocalChange    MONEY,
    @piBranchNo		  SMALLINT,	

    @Return           INT OUTPUT

AS --DECLARE
    -- Local variables

BEGIN

    SET NOCOUNT ON
    SET @Return = 0

    INSERT INTO FinTransExchange
        (AcctNo, TransRefNo, DateTrans, PayMethod, ForeignTender, LocalChange, BranchNo)
    VALUES
        (@piAcctNo, @piTransRefNo, @piDateTrans, @piPayMethod, @piForeignTender, @piLocalChange, @piBranchNo)

    SET @Return = @@error

    RETURN @Return
END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

