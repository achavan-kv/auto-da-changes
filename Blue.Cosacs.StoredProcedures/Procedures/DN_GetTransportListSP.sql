SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetTransportListSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetTransportListSP]
GO
CREATE PROCEDURE 	dbo.DN_GetTransportListSP
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT 	origbr,
		truckid,
		drivername,
		phoneno
	  FROM  transport 

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


