-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


ALTER TABLE [Warehouse].[DriverPayment]  
WITH CHECK ADD  CONSTRAINT [FK_DriverPayment_SendingBranch] 
FOREIGN KEY([SendingBranch])
REFERENCES [dbo].[branch] ([branchno])
GO

ALTER TABLE [Warehouse].[DriverPayment] CHECK CONSTRAINT [FK_DriverPayment_SendingBranch]
GO

ALTER TABLE [Warehouse].[DriverPayment]  
WITH CHECK ADD  CONSTRAINT [FK_DriverPayment_ReceivingBranch] 
FOREIGN KEY([ReceivingBranch])
REFERENCES [dbo].[branch] ([branchno])
GO

ALTER TABLE [Warehouse].[DriverPayment] CHECK CONSTRAINT [FK_DriverPayment_ReceivingBranch]
GO



