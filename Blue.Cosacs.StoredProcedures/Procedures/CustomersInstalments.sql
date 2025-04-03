--this use to be a view
IF EXISTS(SELECT 1 FROM SYS.VIEWS s WHERE s.name = 'CustomersInstalments')
	DROP VIEW CustomersInstalments
GO

IF OBJECT_ID('CustomersInstalments') IS NOT NULL
	DROP PROCEDURE CustomersInstalments
GO

CREATE PROCEDURE CustomersInstalments
	@From	Date,
	@To		Date
AS 

	SET NOCOUNT ON

	IF OBJECT_ID(N'tempdb..#Data', N'U') IS NOT NULL 
		DROP TABLE #Data

	SELECT 
		acctno,
		datelast
	INTO #Data
	FROM 
		instalplan
	WHERE 
		CONVERT(Date, datelast) >= @From 
		AND CONVERT(Date, datelast) < @To 
		AND agrmtno = 1

	SELECT 
		c.custid AS CustomerId,
		c.FirstName AS FirstName,
		c.name AS LastName,
		CONVERT(Date, i.datelast) AS LastInstalmentDate,
		la.Empeenochange  AS SalesPerson,
		NULLIF(LTRIM(RTRIM(m.telno)), '') AS MobileNumber,
		NULLIF(ISNULL(NULLIF(LTRIM(RTRIM(CONVERT(VarChar, cu.DialCode))), '') + '-', '') + LTRIM(RTRIM(cu.telno)), '') AS LandLinePhone,
		NULLIF(LTRIM(RTRIM(Cadh.Email)), '') AS Email,
		branch.branchno as CustomerBranch,
		a.acctno AS AccountNumber
	FROM 
		#Data i
		INNER JOIN custacct a
			ON i.acctno = a.acctno
			AND a.hldorjnt = 'H'
		INNER JOIN customer c
			ON a.custid = c.custid
		INNER JOIN acct ac
			ON ac.acctno = a.acctno
			AND currstatus != 'S'
		INNER JOIN 
		(
			SELECT 
				ca.custid, MAX(la.LineItemAuditID) AS LineItemAuditID
			FROM 
				custacct ca 
				INNER JOIN LineitemAudit la ON ca.acctno = la.acctno
				INNER JOIN Admin.[User] u ON la.Empeenochange = u.Id
			WHERE 
				ca.hldorjnt = 'H'
			GROUP BY 
				ca.custid
		) LastSalesPerson
			ON c.custid = LastSalesPerson.custid
		INNER JOIN LineitemAudit la
			ON la.LineItemAuditID = LastSalesPerson.LineItemAuditID
		LEFT JOIN custtel cu 
			ON c.custid = cu.custid
			AND (cu.datediscon is null) 
			AND cu.tellocn ='H'
		LEFT JOIN custtel m
			ON  m.custid = c.custid 
			and m.tellocn ='M'  
			AND (m.datediscon is null)
		INNER JOIN branch 
			ON CONVERT(SmallInt,LEFT(ac.acctno, 3)) = branch.branchno
		LEFT JOIN custaddress cadh
			ON c.custid = cadh.custid
			AND cadh.addtype = 'H'
			AND cadh.datemoved IS NULL
			
	IF OBJECT_ID(N'tempdb..#Data', N'U') IS NOT NULL 
		DROP TABLE #Data
