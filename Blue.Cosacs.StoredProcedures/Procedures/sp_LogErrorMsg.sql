SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[sp_LogErrorMsg]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[sp_LogErrorMsg]
GO



CREATE PROCEDURE sp_LogErrorMsg

--------------------------------------------------------------------------------
--
-- Project      : CoSACS Transact r 2002 Strategic Thought Ltd.
-- File Name    : LogErrorMsg.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Log errors (from stored procedures)
-- Author       : D Richardson
-- Date         : 14 May 2002
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piErrNo         INTEGER,
    @piMsg           TEXT,
    @piObjectName    VARCHAR(50)

AS -- DECLARE
    -- Local variables

BEGIN

    INSERT INTO AppErrorLog (
        Err_date,
        Err_no,
        Err_txt,
        Err_Obj,
        Empeename,
        Empeeno)
    VALUES (
        GETDATE(),
        @piErrNo,
        @piMsg,
        @piObjectName,
        USER,
        0);

    RETURN @@ERROR
END



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

