IF OBJECT_ID('NewCustomerAquisition') IS NOT NULL
	DROP PROCEDURE NewCustomerAquisition 
GO

CREATE PROCEDURE NewCustomerAquisition 
	@Today			Date
AS 

	--Calculate 11 weeks ago
	DECLARE @threeMonthsAgo DATE = DATEADD(WEEK, -11, @Today)
	DECLARE @FirstWeek DATE 

	--find the first day of that week
	select @FirstWeek = f.StartDate from FinancialWeeks f where @threeMonthsAgo BETWEEN f.StartDate AND f.EndDate

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
			ag.empeenosale as CSR,
			@FirstWeek as FirstWeek,
			COUNT(1) as Total
		FROM 
			acct a
			LEFT JOIN agreement ag 
				ON a.acctno = ag.acctno
			LEFT JOIN custacct c
				ON ag.acctno = c.acctno
				AND ag.agrmtno = 1
				AND c.hldorjnt = 'H'
			LEFT JOIN FinancialWeeks f
				ON CONVERT(DATE, a.dateacctopen)  BETWEEN f.StartDate AND f.EndDate
		WHERE 
			a.accttype in ('C', 'R', 'O')
			AND a.dateacctopen >= @FirstWeek
			AND CONVERT(date, a.dateacctopen) <= @Today
		GROUP BY 
				ag.empeenosale,
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