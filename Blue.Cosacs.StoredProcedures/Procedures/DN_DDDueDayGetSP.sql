SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DDDueDayGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DDDueDayGetSP]
GO


CREATE PROCEDURE DN_DDDueDayGetSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDDueDayGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Retrieve the Due Day for the ID
-- Author       : D Richardson
-- Date         : 18 Aug 2003
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piDueDayId   INT,

    @Return       INT OUTPUT

AS --DECLARE
    -- Local variables

BEGIN

    SET NOCOUNT ON
    SET @Return = 0

    SELECT ISNULL(CAST(DueDay AS VARCHAR(2)),'') AS DueDay,
           ISNULL(DueDayId,0)                      AS DueDayId,
           ISNULL(EndDate,'')                     AS EndDate,
           ISNULL(StartDate,'')                   AS StartDate
    FROM   DDDueDay
    WHERE  DueDayId = @piDueDayId

    SET @Return = @@error

    RETURN @Return
END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

