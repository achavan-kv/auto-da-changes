-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS( SELECT Id FROM Payments.PaymentMethod WHERE Id = 11 AND [Description] = 'Optical Postings' ) BEGIN
	INSERT INTO [Payments].[PaymentMethod] (Id, [Description], Active, IsReturnAllowed, IsCashReturnAllowed)
	VALUES (11, 'Optical Postings', 1, 1, 0)
END