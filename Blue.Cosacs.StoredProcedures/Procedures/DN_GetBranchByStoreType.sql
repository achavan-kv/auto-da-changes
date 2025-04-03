SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetBranchByStoreType]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetBranchByStoreType]
GO


CREATE PROCEDURE [dbo].[DN_GetBranchByStoreType] 
		@storeType char(1),
		@return int output
AS  
	SET @return = 0    --initialise return code	
	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT  * FROM Branch 
	WHERE storeType = @storeType

	SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO