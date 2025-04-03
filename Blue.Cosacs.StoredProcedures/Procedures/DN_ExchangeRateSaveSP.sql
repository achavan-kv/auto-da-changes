SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ExchangeRateSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ExchangeRateSaveSP]
GO


CREATE PROCEDURE DN_ExchangeRateSaveSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_ExchangeRateSaveSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save a new exchange rate
-- Author       : D Richardson
-- Date         : 14 October 2003
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters

    @piCurrency     VARCHAR(4),
    @piRate         FLOAT,
    @piEmpeeNo      INT,
    
    @Return         INT OUTPUT

AS --DECLARE
    -- Local variables

BEGIN

    SET NOCOUNT ON
    SET @Return = 0

    -- First make sure that any existing record has a history status
    UPDATE ExchangeRate
    SET    Status = 'H'
    WHERE  Currency = @piCurrency
    AND    Status = 'C'

    -- Add the new rate effective from now
    INSERT INTO ExchangeRate
        (Currency, DateFrom, Rate, EmpeeNo, Status)
    VALUES
        (@piCurrency, GETDATE(), @piRate, @piEmpeeNo, 'C')
    
    SET @Return = @@error

    RETURN @Return
END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

