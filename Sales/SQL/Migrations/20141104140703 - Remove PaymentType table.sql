-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME='PaymentMethod' AND TABLE_SCHEMA = 'Sales' AND COLUMN_NAME='PaymentTypeId') BEGIN

	IF (OBJECT_ID(N'[Sales].[FK_PaymentMethod_PaymentType]') IS NOT NULL) BEGIN

		ALTER TABLE Sales.PaymentMethod
			DROP CONSTRAINT FK_PaymentMethod_PaymentType
	END



	ALTER TABLE Sales.PaymentMethod
		DROP COLUMN PaymentTypeId
END

GO

IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[Sales].[PaymentType]') AND type in (N'U')) BEGIN
	DROP TABLE [Sales].[PaymentType]
END

GO