-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'IsReturnAllowed' AND [object_id] = OBJECT_ID(N'[Payments].[PaymentMethod]')) 
	BEGIN
	ALTER TABLE [Payments].[PaymentMethod]
	ADD IsReturnAllowed bit NOT NULL DEFAULT 1
END
GO


IF EXISTS( SELECT Id FROM Payments.PaymentMethod WHERE Id = 6 AND [Description] = 'Debit/Credit Card' ) 
BEGIN
	UPDATE Payments.PaymentMethod 
	SET [Description] = 'Debit Card',
	IsReturnAllowed = 0
	WHERE Id=6
END	

IF NOT EXISTS( SELECT Id FROM Payments.PaymentMethod WHERE Id = 10 AND [Description] = 'Credit Card' ) 
BEGIN
	INSERT INTO Payments.PaymentMethod (Id, [Description], Active, IsReturnAllowed)
	VALUES (10, 'Credit Card', 1,1)
END	

UPDATE Payments.PaymentMethod
SET 
IsReturnAllowed = 0
WHERE Id in (2,4,6,7)
