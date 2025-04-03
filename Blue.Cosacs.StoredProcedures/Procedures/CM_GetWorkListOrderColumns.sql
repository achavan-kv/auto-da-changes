SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_GetWorkListOrderColumns]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_GetWorkListOrderColumns]
GO

CREATE PROCEDURE  [dbo].[CM_GetWorkListOrderColumns]
				@return	int	OUTPUT

AS

    SET @return = 0    --initialise return code

    SELECT DISTINCT ColumnName 
    FROM    CMWorkListOrderColumns
    
    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO