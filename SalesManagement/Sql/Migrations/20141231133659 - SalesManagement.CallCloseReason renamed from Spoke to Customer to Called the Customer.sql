-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

Update SalesManagement.CallCloseReason
SET Name = 'Called the Customer' 
Where Id = 1