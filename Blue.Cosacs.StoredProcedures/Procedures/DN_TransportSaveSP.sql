SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TransportSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TransportSaveSP]
GO


CREATE PROCEDURE 	[dbo].[DN_TransportSaveSP]
			@truckid varchar(26),
			@drivername varchar(50),		
			@phoneno varchar(20),
			@carrierNumber varchar(20),
			@return int OUTPUT
AS

	SET 	@return = 0			--initialise return code

	UPDATE	Transport
	SET		drivername = @drivername,
			phoneno = @phoneno,
			carrierNumber = @carrierNumber,
			lastUpdatedDate = getdate()
	WHERE	truckid = @truckid

	IF(@@rowcount = 0)
	BEGIN
		INSERT
		INTO	Transport
				(origbr, truckid, drivername, phoneno, carrierNumber)
		VALUES	(0,@truckid, @drivername, @phoneno, @carrierNumber)
	END	

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

