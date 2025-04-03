SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DDDueDayListSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DDDueDayListSP]
GO


CREATE PROCEDURE DN_DDDueDayListSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDDueDayListSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Return the option list of Due Days
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

    @Return INT OUTPUT

AS --DECLARE
    -- Local variables
    
BEGIN

    SET NOCOUNT ON
    SET @Return = 0

    -- Include a blank entry at the top of the drop down list
    SELECT ''  AS DueDay,
           0    AS DueDayId
    UNION
    SELECT ISNULL(CAST(DueDay as VARCHAR(2)),'') AS DueDay,
           ISNULL(DueDayId,0)                      AS DueDayId
    FROM   DDDueDay
    WHERE  DATEDIFF(Day, StartDate, GETDATE()) >= 0
    AND    DATEDIFF(Day, ISNULL(EndDate,GETDATE()), GETDATE()) <= 0
    ORDER BY DueDay

    SET @Return = @@error

    RETURN @Return
END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

