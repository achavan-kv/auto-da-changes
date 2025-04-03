SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetNextTemporaryReceptNoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetNextTemporaryReceptNoSP]
GO

CREATE PROCEDURE 	dbo.DN_GetNextTemporaryReceptNoSP
			@nextrecpno int out,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT  @nextrecpno = MAX (receiptno) 
	  FROM  tempreceipt  

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


GO
