

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetAllPeriodEndDatesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetAllPeriodEndDatesSP]
GO

CREATE PROCEDURE 	dbo.DN_GetAllPeriodEndDatesSP
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	position, CONVERT(VARCHAR(11), enddate, 113) as enddate
	INTO	#period
	FROM 	RebateForecast_PeriodEndDates

	UPDATE	#period 
	SET	enddate = REPLACE(enddate, ' ', '-')

	SELECT	position,
		enddate
	FROM	#period
	ORDER BY position asc

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO