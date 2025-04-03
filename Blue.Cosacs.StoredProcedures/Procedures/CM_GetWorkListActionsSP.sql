SET ANSI_NULLS OFF 
GO 
SET QUOTED_IDENTIFIER ON 
GO 
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_GetWorkListActionsSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1) 
DROP PROCEDURE [dbo].[CM_GetWorkListActionsSP]
GO
--exec CM_GetWorkListActionsSP 0
CREATE PROCEDURE [dbo].[CM_GetWorkListActionsSP]				
@return int OUTPUT 
AS 
SET 	@return = 0			
--initialise return code	

SELECT WorkList,Action,WorkListEffect AS 'Exit'    
FROM CMWorkListActions 
IF (@@error != 0)	
BEGIN		
SET @return = @@error 
END 
GO