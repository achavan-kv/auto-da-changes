SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ExchangeRateGetCurrentSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ExchangeRateGetCurrentSP]
GO


CREATE PROCEDURE DN_ExchangeRateGetCurrentSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_ExchangeRateGetCurrentSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Retrieve the list of current exchange rates
-- Author       : D Richardson
-- Date         : 13 October 2003
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters

    @Return       INT OUTPUT

AS --DECLARE
    -- Local variables

BEGIN

    SET NOCOUNT ON
    SET @Return = 0

    SELECT c.Code,
           c.CodeDescript AS Currency,
           e.Rate,
           e.DateFrom,
           e.EmpeeNo,
           u.FullName AS EmployeeName,
           e.Status
    FROM   Code c, ExchangeRate e
    LEFT OUTER JOIN Admin.[User] u ON u.id = e.EmpeeNo
    WHERE  c.Category = 'FPM'
    AND    e.Currency = c.Code
    AND    e.Status = 'C'
    ORDER BY c.Code ASC

    SET @Return = @@error

    RETURN @Return
END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

