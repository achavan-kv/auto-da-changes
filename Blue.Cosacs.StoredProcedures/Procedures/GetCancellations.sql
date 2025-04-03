IF OBJECT_ID('GetCancellations') IS NOT NULL
	DROP PROCEDURE GetCancellations 
GO

CREATE PROCEDURE GetCancellations 
	@Today			Date
AS 
	
	DECLARE @threeMonthsAgo DATE = DATEADD(WEEK, -11, @Today)
	DECLARE @FirstWeek DATE 	
	DECLARE @FirstWeekNumber TinyInt

		--find the first day of that week
	select @FirstWeek = f.StartDate, @FirstWeekNumber = f.Week from FinancialWeeks f where @threeMonthsAgo BETWEEN f.StartDate AND f.EndDate

	SELECT
		ROW_NUMBER() OVER (PARTITION BY finweek.id ORDER BY finweek.StartDate) AS WeekNo, 
		@FirstWeek as FirstWeek,
		finweek.Id as CSR,
		finweek.Week as Week, 
		finweek.BranchNo,
		ISNULL(Total, 0) as Total
	FROM 
		(
			SELECT 
				f.Week, 
				l.EmpeenoChange as CSR,
				@FirstWeekNumber WN, 
				@FirstWeek as FirstWeek,
				Count(1) as Total,
				Max(d.datetrans) as x
			FROM delivery d
			INNER JOIN LineitemAudit l 
				ON d.acctno = l.acctno AND
				d.agrmtno = l.agrmtno AND
				d.ItemID = l.ItemID AND
				d.stocklocn = l.stocklocn AND
				d.contractno = l.contractno
			LEFT JOIN FinancialWeeks f
					ON CONVERT(DATE, d.datetrans)  BETWEEN f.StartDate AND f.EndDate
			INNER JOIN Admin.[User] u ON l.EmpeenoChange = u.Id
			WHERE d.delorcoll = 'C' AND
				  l.source = 'GRTCancel' AND
				  d.datetrans >= @FirstWeek AND 
				  CONVERT(date, d.datetrans) <= @Today
			Group By 
				l.Empeenochange,
				f.Week
		) data
		RIGHT JOIN 
		(
			SELECT 
				usr.Id, f.Week, usr.BranchNo, f.StartDate
			FROM 
				FinancialWeeks f
				CROSS JOIN 
				(
					SELECT Id, BranchNo FROM Admin.[User]
				) usr
			WHERE 
				f.StartDate >= (select f.StartDate from FinancialWeeks f where (@threeMonthsAgo BETWEEN f.StartDate AND f.EndDate) )
				AND f.EndDate <= (select f.EndDate from FinancialWeeks f where (@Today BETWEEN f.StartDate AND f.EndDate) )
		) finweek
			ON finweek.Week = data.Week
			AND data.CSR = finweek.Id
		ORDER BY 
			finweek.Id,
			WeekNo
			