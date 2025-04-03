-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

Alter TABLE PurchaseOrderOutstanding Alter column itemno VARCHAR(18) not null

