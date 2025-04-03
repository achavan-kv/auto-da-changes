SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_StockItemIsInstantReplacementSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_StockItemIsInstantReplacementSP]
GO

CREATE PROCEDURE 	dbo.DN_StockItemIsInstantReplacementSP
			@itemId int,
			@branchno smallint,
			@instant smallint OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@instant = count(*)
	FROM	stockitem
	WHERE	itemId = @itemId
	AND		stocklocn = @branchno
	AND		(WarrantyType = 'I' 
			or	LEFT(refcode,2) = 'ZZ')

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

