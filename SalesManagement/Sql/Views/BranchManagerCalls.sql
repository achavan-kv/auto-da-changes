IF OBJECT_ID('SalesManagement.BranchManagerCall') IS NOT NULL
	DROP VIEW SalesManagement.BranchManagerCall
GO

CREATE VIEW SalesManagement.BranchManagerCall
AS
	SELECT 
		c.Id AS CallId, 
		c.SalesPersonId, 
		c.CustomerFirstName, 
		c.CustomerLastName, 
		c.ReasonToCall AS ReasonForCalling,
		c.ToCallAt, 
		c.CallTypeId, 
		csp.CustomerId, 
		ISNULL(C.Branch, csp.CustomerBranch) AS CustomerBranch,
		csp.DoNotCallAgain AS DoNotCallAgain
	FROM  
		SalesManagement.Call AS c
		LEFT JOIN SalesManagement.CustomerSalesPerson csp	
			ON C.CustomerId = csp.CustomerId
	WHERE
		c.CallClosedReasonId IS NULL