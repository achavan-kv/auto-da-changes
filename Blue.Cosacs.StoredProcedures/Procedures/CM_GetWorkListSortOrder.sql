
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_GetWorkListSortOrder]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_GetWorkListSortOrder]
GO
-- =============================================
-- Author:		?
-- Create date: 09/05/2009
-- Description:	Returns details regarding employee types, column names and sort order
--				to determine the sort order of accounts in the Telephone Actions screen.
-- =============================================


CREATE PROCEDURE  [dbo].[CM_GetWorkListSortOrder]
				@return	int	OUTPUT

AS

    SET @return = 0    --initialise return code

    SELECT  '' as EmpeeType, 
			'' as Name,
			CMSO.SortColumnName as ColumnName, 
			CMSO.SortOrder,
			CMSO.AscDesc
    FROM    CMWorkListSortOrder CMSO
    ORDER BY CMSO.SortOrder
    
    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO


