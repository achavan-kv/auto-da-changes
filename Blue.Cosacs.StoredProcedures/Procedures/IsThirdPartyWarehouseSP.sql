SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[IsThirdPartyWarehouseSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[IsThirdPartyWarehouseSP]
GO

-- ============================================================================================
-- Author:		Ilyas Parker
-- Create date: 09/04/2010
-- Description:	Procedure which checks if a branch is a Third Party Warehouse
-- ============================================================================================

CREATE PROCEDURE 	dbo.IsThirdPartyWarehouseSP
			@branchno smallint,
			@isthirdpartywarehouse int OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@isthirdpartywarehouse = count(*)
	FROM	branch
	WHERE	branchno = @branchno
	AND	thirdpartywarehouse = 'Y'

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO