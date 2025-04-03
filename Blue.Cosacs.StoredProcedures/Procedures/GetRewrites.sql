IF OBJECT_ID('GetRewrites') IS NOT NULL
	DROP PROCEDURE GetRewrites 
GO

CREATE PROCEDURE GetRewrites 
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
		ISNULL(data.RewriteValue, 0) as Total
	FROM 
		(
		SELECT 
			f.Week,
			Ret.Empeenochange as CSR,
			@FirstWeekNumber as FirstWeekNo,
			@FirstWeek as FirstWeek, 
			SUM( Exchange.Value + Ret.Value) as RewriteValue
		FROM
		(
		-- get returns
			SELECT  
				l.acctno, 
				cast(l.datechange as date) as ReturnDate, 
				l.Empeenochange,
				SUM(ISNULL(l.ValueAfter -l.ValueBefore, 0)) as Value
			FROM LineitemAudit l
			WHERE  
				l.acctno <> '' 
				AND (l.ValueAfter < l.ValueBefore)  
				AND l.[Source] = 'GRTExchange    ' 
			GROUP BY l.acctno, cast(l.datechange as date), l.Empeenochange
		) Ret
		JOIN 
		(
		-- get exchange
			SELECT  
				Exch.acctno, 
				cast(Exch.datechange as date) as ExchDate, 
				Exch.Empeenochange,
				SUM(ISNULL(Exch.ValueAfter - Exch.ValueBefore, 0)) as Value
			FROM LineitemAudit Exch
			WHERE  
				Exch.acctno <> '' 
				AND Exch.Source =  'Revise         '
				AND (Exch.ValueAfter > Exch.ValueBefore) 
				AND (Exch.ValueAfter - Exch.ValueBefore) <> 0
			GROUP BY Exch.acctno, cast(Exch.datechange as date), Exch.Empeenochange
		) Exchange
		ON Ret.acctno = Exchange.acctno
	LEFT JOIN FinancialWeeks f
		ON CONVERT(DATE, Ret.ReturnDate)  BETWEEN f.StartDate AND f.EndDate
	WHERE	 
		cast(Ret.ReturnDate as date) = cast(Exchange.ExchDate as date) 
		 AND Ret.ReturnDate <= Exchange.ExchDate
		 AND Ret.ReturnDate >= @FirstWeek 
		 AND CONVERT(date, Ret.ReturnDate) <= @Today
	GROUP BY
			Ret.Empeenochange,
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