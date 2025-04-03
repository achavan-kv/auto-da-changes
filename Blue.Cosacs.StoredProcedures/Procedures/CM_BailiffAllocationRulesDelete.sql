SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_BailiffAllocationRulesDelete]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_BailiffAllocationRulesDelete]
GO

--Author : Mohamed Nasmi

CREATE PROCEDURE [dbo].[CM_BailiffAllocationRulesDelete] 
	@return INTEGER OUT, 
	@EmpeeNo int,
	@All bit

AS 
	SET @return = 0 
	
	DELETE CMBailiffAllocationRules
	WHERE empeeno = @EmpeeNo or @All = 1

	SET @return = @@error
		
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO