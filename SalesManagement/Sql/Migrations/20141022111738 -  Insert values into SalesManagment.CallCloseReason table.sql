-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT INTO SalesManagement.CallCloseReason
Values(1, 'Spoke to Customer'), 
	  (2, 'Closed by Store Manager'), 
	  (3, 'Closed in Bulk By CSR')
