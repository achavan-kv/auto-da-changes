-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

Insert Into SalesManagement.IconTypes
VALUES
(1, 'Inactive customer/ account settlement Calls', null),
(2, 'Customer Instalment Ending Calls', null)

