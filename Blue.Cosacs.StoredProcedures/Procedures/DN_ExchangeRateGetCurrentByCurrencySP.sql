SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ExchangeRateGetCurrentByCurrencySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ExchangeRateGetCurrentByCurrencySP]
GO


CREATE PROCEDURE DN_ExchangeRateGetCurrentByCurrencySP
				@currency varchar(4),
				@rate float OUT,
			    @Return       INT OUTPUT

AS --DECLARE
    -- Local variables

BEGIN

    SET NOCOUNT ON
    SET @Return = 0


	SELECT	TOP 1
			@rate = rate
	FROM		exchangerate
	WHERE	currency = @currency
	ORDER BY 	datefrom DESC

    SET @Return = @@error

    RETURN @Return
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

