

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_GetWorkListEmployeesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_GetWorkListEmployeesSP]
GO


CREATE PROCEDURE  dbo.CM_GetWorkListEmployeesSP
				@return	int	OUTPUT

AS

    SET @return = 0    --initialise return code

    SELECT WorkList,EmpeeType
    FROM CMWorkList

    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

