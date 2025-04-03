SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ExchangeRateGetHistorySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ExchangeRateGetHistorySP]
GO


CREATE PROCEDURE DN_ExchangeRateGetHistorySP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_ExchangeRateGetHistorySP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Retrieve the list of current and historical exchange rates
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

    @piCurrency     VARCHAR(4),
    @piDateFrom     SMALLDATETIME,
    @piDateTo       SMALLDATETIME,
    
    @Return         INT OUTPUT

AS DECLARE
    -- Local variables

    @SQLStr         VARCHAR(1400)

BEGIN

    SET NOCOUNT ON
    SET @Return = 0

    SET @SQLStr =
        ' SELECT c.Code, ' +
        '        c.CodeDescript AS Currency, ' +
        '        e.Rate, ' +
        '        e.DateFrom, ' +
        '        e.EmpeeNo, ' +
        '        cp.FullName as Empeename, ' +
        '        e.Status ' +
        ' FROM   Code c, ExchangeRate e ' +
        ' LEFT OUTER JOIN Admin.[User] cp ON cp.Id = e.EmpeeNo ' +
        ' WHERE  c.Category = ''FPM'' ' +
        ' AND    e.Currency = c.Code '



    -- May filter by currency
    IF @piCurrency != ''
    BEGIN
        SET @SQLStr = @SQLStr + ' AND e.Currency = ''' + @piCurrency + '''';
    END

    -- May filter by earliest DateFrom
    IF @piDateFrom > CONVERT(SMALLDATETIME,'01 Jan 1900',106)
    BEGIN
        SET @SQLStr = @SQLStr + ' AND e.DateFrom >= CONVERT(SMALLDATETIME,''' + CONVERT(VARCHAR,@piDateFrom,106) + ''',106)';
    END

    -- May filter by latest DateFrom
    IF @piDateTo > CONVERT(SMALLDATETIME,'01 Jan 1900',106)
    BEGIN
        SET @SQLStr = @SQLStr + ' AND e.DateFrom <= CONVERT(SMALLDATETIME,''' + CONVERT(VARCHAR,@piDateTo,106) + ''',106)';
    END


    SET @SQLStr = @SQLStr + ' ORDER BY c.Code ASC, e.DateFrom DESC '

    EXECUTE (@SQLStr)

    SET @Return = @@error

    RETURN @Return
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

