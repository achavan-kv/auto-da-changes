IF OBJECT_ID('SalesManagement.CallsFromUnavailableCSRView') IS NOT NULL
	DROP VIEW SalesManagement.CallsFromUnavailableCSRView
GO

CREATE VIEW SalesManagement.CallsFromUnavailableCSRView
As

Select 
	ISNULL(cust.CustomerBranch, c.Branch) AS BranchNo,
	c.Id AS CallId
From 
	SalesManagement.CsrUnavailable csr
	INNER JOIN SalesManagement.Call c
		ON csr.SalesPersonId = c.SalesPersonId
		AND c.CallClosedReasonId IS NULL
	LEFT JOIN SalesManagement.CustomerSalesPerson cust
		ON c.CustomerId = cust.CustomerId
WHERE 
	CONVERT(Date, c.ToCallAt) BETWEEN csr.BeggingUnavailable AND csr.EndUnavailable