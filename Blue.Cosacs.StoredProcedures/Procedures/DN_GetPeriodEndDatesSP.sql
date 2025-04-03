

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetPeriodEndDatesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetPeriodEndDatesSP]
GO

CREATE PROCEDURE 	dbo.DN_GetPeriodEndDatesSP
			@nextperiodend varchar(12) OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE @tmpDate datetime

	SELECT	@tmpDate = min(enddate)
	FROM	RebateForecast_PeriodEndDates
	WHERE	rundate = '1/1/1900'
	AND	enddate != '1/1/1900'

	SET	@nextperiodend = CONVERT(VARCHAR(11), @tmpDate, 113)
	SET	@nextperiodend = REPLACE(@nextperiodend, ' ', '-')

	SELECT	--TOP 12 /* KEF 68360 - removed as couldn't see some period ends */
        position, 
		CONVERT(VARCHAR(11), rundate, 113) as rundate, 
		CONVERT(VARCHAR(11), enddate, 113) as enddate
	INTO	#period
	FROM 	RebateForecast_PeriodEndDates
	WHERE	rundate > '1/1/1900'
	ORDER BY enddate desc

	UPDATE	#period 
	SET	enddate = REPLACE(enddate, ' ', '-'),
		rundate = REPLACE(rundate, ' ', '-')

	SELECT	position,
		enddate,
		rundate
	FROM	#period
	ORDER BY position desc

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO