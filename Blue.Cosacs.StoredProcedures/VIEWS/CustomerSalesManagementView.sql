IF OBJECT_ID('CustomerSalesManagementView') IS NOT NULL
	DROP VIEW CustomerSalesManagementView
GO
 
CREATE VIEW CustomerSalesManagementView
AS
	SELECT 
		l.Empeenochange AS SalesPerson,
		c.custid AS CustomerId,
		c.firstname AS FirstName,
		c.name AS LastName,
		NULLIF(LTRIM(RTRIM(m.telno)), '') AS MobileNumber,
		NULLIF(ISNULL(NULLIF(LTRIM(RTRIM(CONVERT(VarChar, cu.DialCode))), '') + '-', '') + LTRIM(RTRIM(cu.telno)), '') AS LandLinePhone,
		ca.acctno AS CustomerAccount,
		NULLIF(LTRIM(RTRIM(Cadh.Email)), '') AS Email,
		acct.agrmttotal TotalAmount, 
		ISNULL(c.ResieveSms, 1) AS ReceiveSms
	FROM
		LineitemAudit l
		INNER JOIN custacct ca
			ON l.acctno = ca.acctno
			AND l.agrmtno = 1
			AND ca.hldorjnt = 'H'
		INNER JOIN customer c
			ON ca.custid = c.custid
		INNER JOIN acct 
			ON ca.acctno = acct.acctno
		INNER JOIN branch 
			ON CONVERT(SmallInt,LEFT(ca.acctno, 3)) = branch.branchno
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