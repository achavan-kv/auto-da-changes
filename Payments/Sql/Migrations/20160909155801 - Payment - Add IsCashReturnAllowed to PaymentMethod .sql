-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'IsCashReturnAllowed' AND [object_id] = OBJECT_ID(N'Payments.PaymentMethod'))BEGIN
	ALTER TABLE [Payments].[PaymentMethod] ADD
		IsCashReturnAllowed bit NOT NULL CONSTRAINT DF_PaymentMethod_IsCashReturnAllowed DEFAULT 0
END
GO