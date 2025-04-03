SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_CheckValidDecimalQtySP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_CheckValidDecimalQtySP
END
GO

CREATE PROCEDURE 	dbo.DN_CheckValidDecimalQtySP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_CheckValidDecimalQtySP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Check Item is valid for Decimal Quantity
-- Author       : John Croft
-- Date         : 11 September 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------
    -- Parameters
            @itemId int,
            @decimal int output,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	set @decimal = (select TOP 1 reference
					from dbo.StockInfo s, code c
					where s.ID = @itemId and 
						convert(varchar(2), s.category) = c.code and 
						c.category like 'PC%')

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- end end end end end end end end end end end end end end end end end end end end end end end 
