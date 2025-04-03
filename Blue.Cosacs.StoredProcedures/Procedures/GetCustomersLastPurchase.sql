IF OBJECT_ID('dbo.GetCustomersLastPurchase') IS NOT NULL
	DROP PROCEDURE dbo.GetCustomersLastPurchase
GO

CREATE PROCEDURE dbo.GetCustomersLastPurchase
	@howOldCash					Date,
	@beginningOfRangeCash		Date, 
	@howOldCredit				Date,
	@beginningOfRangeCredit		Date,
	@numberOfRecordsToReturn	Int
AS

	IF OBJECT_ID('tempdb..#tmp') IS NOT NULL 
		DROP TABLE #tmp

	SELECT 
		Data.CustomerId,
		DateLastPaid,
		MAX(l.Empeenochange) AS SalesPerson,
		MAX(c.firstname) AS FirstName,
		MAX(c.name) AS LastName,
		CONVERT(BIT, MAX(Data.CashAccount)) AS CashAccount,
		MAX(NULLIF(LTRIM(RTRIM(m.telno)), '')) AS MobileNumber,
		MAX(NULLIF(ISNULL(NULLIF(LTRIM(RTRIM(CONVERT(VarChar, cu.DialCode))), '') + '-', '') + LTRIM(RTRIM(cu.telno)), '')) AS LandLinePhone,
		Data.CustomerBranch,
		MAX(NULLIF(LTRIM(RTRIM(Cadh.Email)), '')) AS Email
	INTO #tmp
	FROM
	(
		SELECT 
			ca.custid AS CustomerId, 
			MAX(acct.datelastpaid) AS DateLastPaid, 
			MAX(l.LineItemAuditID) AS LineId,
			MAX(CASE 
					WHEN acct.accttype = 'C' THEN 0
					ELSE 1
				END) AS CashAccount,
			MAX(branch.branchno) as CustomerBranch
		FROM 
			LineitemAudit l
			INNER JOIN custacct ca
				ON l.acctno = ca.acctno
				AND l.agrmtno = 1
				AND ca.hldorjnt = 'H'
			INNER JOIN acct
				ON ca.acctno = acct.acctno
				AND acct.accttype IN ('C', 'R', 'O')
			INNER JOIN branch 
				ON CONVERT(SmallInt,LEFT(acct.acctno, 3)) = branch.branchno
		GROUP BY 
			ca.custid
	) AS Data 
		INNER JOIN LineitemAudit l
			ON l.LineItemAuditID = Data.LineId
		INNER JOIN customer c
			ON Data.CustomerId = c.custid
		LEFT JOIN custtel cu 
			ON c.custid = cu.custid
			AND (cu.datediscon is null) 
			AND cu.tellocn ='H'
		LEFT JOIN custtel m
			ON  m.custid = c.custid and m.tellocn ='M'  
			AND (m.datediscon is null)
		LEFT JOIN custaddress cadh
			ON c.custid = cadh.custid
			AND cadh.addtype = 'H'
			AND cadh.datemoved IS NULL
		LEFT JOIN custaddress cadW
			ON c.custid = cadW.custid
			AND cadW.addtype = 'W'
			AND cadW.datemoved IS NULL
		LEFT JOIN custaddress cadD
			ON c.custid = cadD.custid
			AND cadD.addtype = 'D'
			AND cadD.datemoved IS NULL
	WHERE
		Data.[DateLastPaid] >= 
			CASE
				WHEN @beginningOfRangeCash > @beginningOfRangeCredit THEN @beginningOfRangeCash
				else @beginningOfRangeCredit
			END
		AND Data.[DateLastPaid] < 
			CASE
				WHEN @howOldCash > @howOldCredit THEN @howOldCredit
				else @howOldCash
			END
	GROUP BY 
		Data.CustomerId,
		Data.DateLastPaid,
		Data.CustomerBranch

	SELECT DISTINCT TOP (@numberOfRecordsToReturn) 
		Data.[CustomerId], 
		Data.[DateLastPaid], 
		Data.[SalesPerson], 
		Data.[FirstName], 
		Data.[LastName],
		Data.[CashAccount],
		Data.MobileNumber,
		Data.LandLinePhone,
		Data.CustomerBranch,
		Data.Email
	FROM 
	(
		SELECT 
			[CustomerId], [DateLastPaid], [SalesPerson], [FirstName], [LastName], [CashAccount], MobileNumber, LandLinePhone, [CustomerBranch], Email
		FROM #tmp 
		WHERE 
			[DateLastPaid] < @howOldCash AND [DateLastPaid] >= @beginningOfRangeCash AND [CashAccount] = 1
		UNION ALL
		SELECT 
			[CustomerId], [DateLastPaid], [SalesPerson], [FirstName], [LastName], [CashAccount], MobileNumber, LandLinePhone, [CustomerBranch], Email
		FROM #tmp
		WHERE 
			[DateLastPaid] < @howOldCredit AND [DateLastPaid] >= @beginningOfRangeCredit AND [CashAccount] != 1
	) AS Data
	INNER JOIN Admin.[User] u
		ON u.id = Data.SalesPerson

	IF OBJECT_ID('tempdb..#tmp') IS NOT NULL 
		DROP TABLE #tmp
go
