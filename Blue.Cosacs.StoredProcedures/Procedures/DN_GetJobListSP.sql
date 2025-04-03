SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetJobListSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetJobListSP]
GO


CREATE PROCEDURE DN_GetJobListSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_GetJobListSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Return the list of of jobs for EOD (SQL Server Agent)
-- Author       : D Richardson
-- Date         : 1 Dec 2003
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

    SELECT CoSACSId,
           RunSeq,
           JobId,
           JobName,
           Description,
           Enabled,
           FreqType,
           FreqInterval
    FROM   JobList
    ORDER BY RunSeq

    SET @Return = @@error

    RETURN @Return
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

