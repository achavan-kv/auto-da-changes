
/****** Object:  UserDefinedFunction [dbo].[fn_CalcItemReturnPercentage]    Script Date: 10/24/2006 18:04:13 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[fn_CalcItemReturnPercentage]') AND xtype in ('FN', 'IF', 'TF'))
DROP FUNCTION [dbo].[fn_CalcItemReturnPercentage]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 24--Oct-2006
-- Description:	Calculates the percentage of returns within a certain number of days for a given product used in DN_SRReportServiceFailureSP
-- =============================================
CREATE FUNCTION fn_CalcItemReturnPercentage
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
		AND PurchaseDate >= @FromDate AND PurchaseDate < DateAdd(Q, 1, @FromDate)
		-- not sure this is required
		--CASE WHEN @quarters = 0 THEN DATEADD(DAY,@noOfDays,@FromDate)
		--ELSE  END
	

	-- Return the result of the function
	RETURN @Result

END
GO

