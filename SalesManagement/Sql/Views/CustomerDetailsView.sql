IF OBJECT_ID('SalesManagement.CustomerDetailsView') IS NOT NULL
	DROP VIEW SalesManagement.CustomerDetailsView
GO

CREATE VIEW SalesManagement.CustomerDetailsView
As

Select 
    distinct ISNULL(csp.CustomerBranch, c.Branch) AS BranchNo,
	csp.DoNotCallAgain,
	csp.SalesPersonId,
	c.Id
From 
	SalesManagement.CustomerSalesPerson csp
	INNER JOIN SalesManagement.Call c
		ON csp.SalesPersonId = c.SalesPersonId