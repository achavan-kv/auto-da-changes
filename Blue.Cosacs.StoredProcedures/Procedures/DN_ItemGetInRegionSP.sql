SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ItemGetInRegionSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ItemGetInRegionSP]
GO

CREATE PROCEDURE 	dbo.DN_ItemGetInRegionSP
			@itemId INT,
			@branchno smallint,
			@return int OUTPUT

AS

	DECLARE @region varchar(3)

	SET 	@return = 0			--initialise return code

	SELECT	@region = warehouseregion
	FROM	branch
	WHERE	branchno = @branchno
	
	SELECT	s.stocklocn,
		s.stockfactavailable as availablestock
	FROM	stockitem s, branch b, branchregion br
	WHERE	s.ItemID = @itemId
	AND	s.stocklocn = b.branchno
	AND	b.branchno = br.branchno
	AND	br.region = @region
	AND	s.stockfactavailable > 0

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

