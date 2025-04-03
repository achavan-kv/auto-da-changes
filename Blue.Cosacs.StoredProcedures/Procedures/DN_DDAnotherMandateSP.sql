SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DDAnotherMandateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DDAnotherMandateSP]
GO


CREATE PROCEDURE DN_DDAnotherMandateSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDAnotherMandateSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Check whether another mandate is current for this Account Number
-- Author       : D Richardson
-- Date         : 20 Aug 2003
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piMandateId    INTEGER,
    @piAcctNo       CHAR(12),

    @poMandateId    INTEGER OUTPUT,
    @poStartDate    SMALLDATETIME OUTPUT,
    @poEndDate      SMALLDATETIME OUTPUT,
    @Return         INTEGER OUTPUT

AS -- DECLARE
    -- Local variables

BEGIN

    SET NOCOUNT ON
    SET @Return = 0

    -- Check whether another mandate is current for this Account Number
    
    SELECT @poMandateId = MandateId,
           @poStartDate = StartDate,
           @poEndDate   = EndDate
    FROM   DDMandate
    WHERE  Status = 'C'
    AND    AcctNo     = @piAcctNo
    AND    MandateId != @piMandateId;

    SET @Return = @@ERROR
    RETURN @Return
END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

