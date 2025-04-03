SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[GetAllRepairCentre]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[GetAllRepairCentre]
GO

CREATE PROCEDURE  GetAllRepairCentre  
	@return int OUTPUT  
  
AS  
  
 SET  @return = 0   --initialise return code  
  
 SELECT branchno, branchname, storetype, ServiceRepairCentre  
 FROM  branch  
 WHERE ServiceRepairCentre = 1
  
IF(@@error != 0)   
	SET @return = @@error  
 
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



  
  
  
  
  
  
  