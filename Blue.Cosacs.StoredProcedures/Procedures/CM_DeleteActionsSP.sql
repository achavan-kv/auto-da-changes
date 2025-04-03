
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_DeleteActionsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_DeleteActionsSP]
GO


CREATE PROCEDURE  dbo.CM_DeleteActionsSP
				@worklist varchar(10),
				@return	int	OUTPUT

AS

    SET @return = 0    --initialise return code

	DELETE	
	FROM	CMWorkListActions
	WHERE	WorkList = @worklist

    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

