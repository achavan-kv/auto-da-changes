IF OBJECT_ID('dbo.AllocateCustomersToCSR') IS NOT NULL
	DROP VIEW dbo.AllocateCustomersToCSR
GO

CREATE VIEW  dbo.AllocateCustomersToCSR
AS		
	WITH Contacts (CostomerId, DateChange, ContactType) AS
	(
		SELECT CustId AS CostomerId, MAX(datechange) datechange, tellocn AS ContactType 
		FROM custtel
		WHERE datediscon is null 
		GROUP BY custid, tellocn
	)
	SELECT 
		u.Id AS SalesPersonId,
		c.custid AS CustomerId,
		l.Datechange AS DateChanged,
		NULLIF(LTRIM(RTRIM(m.telno)), '') AS MobileNumber,
		NULLIF(ISNULL(NULLIF(LTRIM(RTRIM(CONVERT(VarChar, cu.DialCode))), '') + '-', '') + LTRIM(RTRIM(cu.telno)), '') AS LandLinePhone,
		NULLIF(LTRIM(RTRIM(Cadh.Email)), '') AS Email,
		branch.branchno AS CustomerBranch
	FROM 
	(
		Select 
			MAX(i.LineitemAuditId) AS LineItemAuditId
		From 
			LineitemAudit i
			INNER JOIN custacct c
				ON i.acctno = c.acctno
				AND i.agrmtno = 1
				AND c.hldorjnt = 'H'
		WHERE 
			LTRIM(RTRIM(i.[source])) In ('NewAccount', 'Revise') 
		GROUP BY 
			c.custid
	) AS Data
		INNER JOIN LineitemAudit l
			ON l.LineItemAuditID = Data.LineItemAuditId
		INNER JOIN custacct c
			ON l.acctno = c.acctno
			AND c.hldorjnt = 'H'
		INNER JOIN acct ac
			ON ac.acctno = c.acctno
			and ac.acctno = l.acctno
		INNER JOIN customer cust
			ON c.custid = cust.custid
		INNER JOIN branch 
			ON CONVERT(SmallInt, LEFT(ac.acctno, 3)) = branch.branchno
		INNER JOIN Admin.[User] u
			ON u.id = l.Empeenochange
		LEFT JOIN 
		(
			SELECT w.custid, w.telno, DialCode
			FROM custtel w INNER JOIN Contacts cont ON cont.CostomerId = w.custid AND cont.datechange = w.datechange and w.tellocn = cont.ContactType WHERE w.tellocn = 'H' and w.datediscon is null
		) cu
			ON cu.custid = cust.custid
		LEFT JOIN 
		(
			SELECT w.custid, w.telno, DialCode
			FROM custtel w INNER JOIN Contacts cont ON cont.CostomerId = w.custid AND cont.datechange = w.datechange and w.tellocn = cont.ContactType WHERE w.tellocn = 'M' and w.datediscon is null
		) m
			ON m.custid = cust.custid
		LEFT JOIN custaddress cadh
			ON cust.custid = cadh.custid
			AND cadh.addtype = 'H'
			AND cadh.datemoved IS NULL
	WHERE
		COALESCE(NULLIF(LTRIM(RTRIM(cu.telno)), ''), NULLIF(LTRIM(RTRIM(m.telno)), '')) IS NOT NULL