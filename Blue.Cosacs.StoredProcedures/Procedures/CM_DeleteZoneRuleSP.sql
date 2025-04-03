SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_DeleteZoneRuleSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_DeleteZoneRuleSP]
GO

--Author : Mohamed Nasmi

CREATE PROCEDURE [dbo].[CM_DeleteZoneRuleSP] 
	@return INTEGER OUT, 
	@Zone VARCHAR (4),
	@All bit

AS 
	SET @return = 0 
	
	DELETE CmZoneAllocation
	WHERE Zone = @Zone or @All = 1

	SET @return = @@error
		
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO