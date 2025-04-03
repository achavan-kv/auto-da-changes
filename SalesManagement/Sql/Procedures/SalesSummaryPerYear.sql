IF OBJECT_ID('SalesManagement.SalesSummaryPerYear') IS NOT NULL
	DROP PROCEDURE SalesManagement.SalesSummaryPerYear
GO

CREATE PROCEDURE SalesManagement.SalesSummaryPerYear
	@TodaysDate Date 
AS
	SET NOCOUNT ON;

	DECLARE @Week DATE = (SELECT DATEADD(DAY, -DATEPART(WEEKDAY, @TodaysDate), @TodaysDate))

	SELECT 
		SUM(CASE	
			WHEN CONVERT(Date, i.Date) = CONVERT(Date, @TodaysDate) 
				THEN i.Amount
			ELSE 0
		END)													AS Today,
		SUM(CASE	
			WHEN CONVERT(Date, i.Date) >= @Week
				THEN i.Amount
			ELSE 0
		END)													AS Week,
		SUM(CASE	
			WHEN MONTH(i.Date) = MONTH(@TodaysDate) 
				THEN i.Amount
			ELSE 0
		END)													AS Month,
		SUM(i.Amount)											AS Year,
		i.SalesPersonId,
		i.Matrix,
		MAX(Branch.BranchId)									AS BranchNo
	FROM 
		SalesManagement.SummaryTable i
		CROSS APPLY 
		(
			SELECT TOP 1 BranchId
			FROM SalesManagement.SummaryTable s
			WHERE i.SalesPersonId = s.SalesPersonId
			ORDER BY Date Desc
		) Branch
	WHERE
		YEAR(i.Date) = YEAR(@TodaysDate) 
	GROUP BY 
		i.SalesPersonId,
		i.Matrix
	ORDER BY 
		i.SalesPersonId,
		i.Matrix