-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE [Warehouse].[DriverPayment] 
WITH CHECK ADD CONSTRAINT [PK_DriverPayment]
PRIMARY KEY (Id)
