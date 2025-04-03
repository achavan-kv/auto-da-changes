SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ItemIsDiscountSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ItemIsDiscountSP]
GO


CREATE PROCEDURE DN_ItemIsDiscountSP
    -- Parameters
    @itemno            VARCHAR(18),
    @isdiscount		   INTEGER OUTPUT,
    @return            INTEGER OUTPUT

AS --DECLARE
    -- Local variables

BEGIN
    SELECT	@isdiscount = COUNT(*)
    FROM	stockitem
    WHERE	IUPC = @itemno
	AND	   	category in ( select code from code where category = 'PCDIS')

    SET @return = @@ERROR
END

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

