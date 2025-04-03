-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'IsDutyFree' AND [object_id] = OBJECT_ID(N'Sales.Order'))BEGIN
	ALTER TABLE Sales.[Order] ADD
		IsDutyFree bit NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'IsTaxFree' AND [object_id] = OBJECT_ID(N'Sales.Order'))BEGIN
	ALTER TABLE Sales.[Order] ADD
		IsTaxFree bit NULL
END
GO

IF (OBJECT_ID(N'[Sales].[DF_Order_IsDutyFree]') IS NOT NULL) BEGIN
	ALTER TABLE Sales.[Order] ADD CONSTRAINT
		DF_Order_IsDutyFree DEFAULT 0 FOR IsDutyFree
END
GO

IF (OBJECT_ID(N'[Sales].[DF_Order_IsTaxFree]') IS NOT NULL) BEGIN
	ALTER TABLE Sales.[Order] ADD CONSTRAINT
		DF_Order_IsTaxFree DEFAULT 0 FOR IsTaxFree
END
GO
