

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_GetWorkListDataSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_GetWorkListDataSP]
GO


CREATE PROCEDURE  dbo.CM_GetWorkListDataSP
				@return	int	OUTPUT

AS

    SET @return = 0    --initialise return code

	SELECT	worklist as worklist, 
			empeetype as empeetype, 
			'' as worklistaction, 
			'' as action
	FROM CMWorkList 
	UNION 
	SELECT	'', 
			'', 
			worklist as worklistaction, 
			action as action
	FROM CMWorkListActions  
    
	IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

