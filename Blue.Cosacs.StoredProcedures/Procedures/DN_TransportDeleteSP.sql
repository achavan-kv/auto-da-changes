SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TransportDeleteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TransportDeleteSP]
GO


CREATE PROCEDURE 	dbo.DN_TransportDeleteSP
			@truckid varchar(26),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DELETE FROM	Transport
	WHERE	truckid = @truckid

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

