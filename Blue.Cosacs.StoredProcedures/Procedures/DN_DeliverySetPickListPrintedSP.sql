SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliverySetPickListPrintedSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliverySetPickListPrintedSP]
GO

CREATE PROCEDURE 	dbo.DN_DeliverySetPickListPrintedSP
			@picklistno int,
			@type bit,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	IF(@type = 1)
	BEGIN
		UPDATE	schedule
		SET	datepicklistprinted = GETDATE()
		WHERE 	picklistnumber = @picklistno
	END
	ELSE
	BEGIN
		UPDATE	schedule
		SET	datetranschednoprinted = GETDATE()
		WHERE 	transchedno = @picklistno
	END

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO