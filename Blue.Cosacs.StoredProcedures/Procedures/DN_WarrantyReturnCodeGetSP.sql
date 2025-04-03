SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_WarrantyReturnCodeGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_WarrantyReturnCodeGetSP]
GO

CREATE PROCEDURE 	dbo.DN_WarrantyReturnCodeGetSP
			@elapsedMonths int,
			@returnItem varchar(8) OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@returnItem = returnItemNo
	FROM		WarrantyReturnCode
	WHERE	elapsedMonths = @elapsedMonths

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

