SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryIsDotNetWarehouseSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryIsDotNetWarehouseSP]
GO

CREATE PROCEDURE 	dbo.DN_DeliveryIsDotNetWarehouseSP
			@branchno smallint,
			@isdotnetwarehouse int OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@isdotnetwarehouse = count(*)
	FROM	branch
	WHERE	branchno = @branchno
	AND	dotnetforwarehouse = 'Y'

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO