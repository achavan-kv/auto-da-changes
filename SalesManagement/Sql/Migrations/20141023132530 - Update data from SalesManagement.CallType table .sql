-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
 Update SalesManagement.CallType
 Set Name = 'Branch Manager custom' where Id=2

  Update SalesManagement.CallType
 Set Name = 'Callback' where Id=3

  Update SalesManagement.CallType
 Set Name = 'CSR custom' where Id=4
