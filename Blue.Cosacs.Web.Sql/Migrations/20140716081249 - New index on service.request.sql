-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE NONCLUSTERED INDEX [IX_ServiceRequest_Account_IsClosed]
ON [Service].[Request] ([Account],[IsClosed])