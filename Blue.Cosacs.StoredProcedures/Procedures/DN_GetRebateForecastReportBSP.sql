SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetRebateForecastReportBSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetRebateForecastReportBSP]
GO

CREATE PROCEDURE 	dbo.DN_GetRebateForecastReportBSP
			@periodend varchar(12) OUT,
			@return int OUTPUT

AS

	DECLARE @endperiod datetime

	SET 	@return = 0			--initialise return code

	SELECT @endperiod = CONVERT(datetime, @periodend, 102)

--	--IP - 04/04/08 -  CR931 - Sum all the values for the periodend selected to give company total.
--	SELECT	ArrearsLevel as 'Arrears Level', SUM(P1) AS P1, SUM(P2) AS P2, SUM(P3) AS P3, SUM(P4) AS P4, SUM(P5) AS P5,
--		SUM(P6) AS P6, SUM(P7) AS P7, SUM(P8) AS P8, SUM(P9) AS P9, SUM(P10) AS P10, SUM(P11) AS P11, SUM(P12) AS P12
--	FROM	RebateforecastB
--	WHERE	periodend = @endperiod
--	GROUP BY ArrearsLevel, sequence
--	ORDER BY sequence asc

SELECT	ArrearsLevel as 'Arrears Level', SUM(P1) AS P1, SUM(P2) AS P2, SUM(P3) AS P3, SUM(P4) AS P4, SUM(P5) AS P5,
		SUM(P6) AS P6, SUM(P7) AS P7, SUM(P8) AS P8, SUM(P9) AS P9, SUM(P10) AS P10, SUM(P11) AS P11, SUM(P12) AS P12
	FROM	RebateforecastB
	WHERE	periodend = @endperiod
AND ARREARSLEvel NOT LIKE 'avg%'
GROUP BY ArrearsLevel, sequence
	
UNION
SELECT	ArrearsLevel as 'Arrears Level', SUM(P1) AS P1, SUM(P2) AS P2, SUM(P3) AS P3, SUM(P4) AS P4, SUM(P5) AS P5,
		SUM(P6) AS P6, SUM(P7) AS P7, SUM(P8) AS P8, SUM(P9) AS P9, SUM(P10) AS P10, SUM(P11) AS P11, SUM(P12) AS P12
	FROM	RebateforecastB
	WHERE	periodend = @endperiod AND BranchNo = 0
AND ARREARSLEvel LIKE 'avg%'
GROUP BY ArrearsLevel, sequence

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO