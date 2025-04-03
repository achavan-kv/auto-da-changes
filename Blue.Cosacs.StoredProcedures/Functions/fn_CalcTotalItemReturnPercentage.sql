
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[fn_CalcTotalItemReturnPercentage]') AND xtype in ('FN', 'IF', 'TF'))
DROP FUNCTION [dbo].[fn_CalcTotalItemReturnPercentage]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 07-10-2008
-- Description:	Calculates the total percentage of returns over a year for a given product used in DN_SRReportServiceFailureSP
-- =============================================
CREATE FUNCTION fn_CalcTotalItemReturnPercentage
(
	-- Add the parameters for the function here
	@ItemNo			varchar(8),
	@WithinDays		int,
	@FromDate		datetime,
	@TotalItemsSold INT,
	@noOfDays       INT,
	@quarters       BIT
)
RETURNS float
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result float
	
	IF @TotalItemsSold = 0
		SELECT @Result = 0 
	ELSE
		SELECT @Result = cast(Count(*) as float) / cast(@TotalItemsSold as float) 
		FROM   SR_ServiceRequest
		WHERE ProductCode = @ItemNo AND DateDiff(d, @FromDate, DateLogged) BETWEEN 0 AND @WithinDays
		AND ServiceType = 'C'
		AND PurchaseDate >= @FromDate AND PurchaseDate < CASE WHEN @quarters = 0 THEN DATEADD(DAY,@noOfDays,@FromDate)
		ELSE DateAdd(YEAR, 1, @FromDate) END
		
	

	-- Return the result of the function
	RETURN @Result

END
GO

