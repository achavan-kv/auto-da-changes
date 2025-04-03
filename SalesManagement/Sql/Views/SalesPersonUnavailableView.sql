IF OBJECT_ID('SalesManagement.SalesPersonUnavailableView') IS NOT NULL
	DROP VIEW SalesManagement.SalesPersonUnavailableView
GO

CREATE VIEW SalesManagement.SalesPersonUnavailableView
AS 

	SELECT DISTINCT
		csr.BeggingUnavailable,
		csr.EndUnavailable,
		csr.SalesPersonId,
		csr.Id,
		cust.CustomerBranch AS SalesPersonBranch
	FROM 
		SalesManagement.CsrUnavailable csr
		INNER JOIN SalesManagement.CustomerSalesPerson cust
			ON csr.SalesPersonId = cust.SalesPersonId