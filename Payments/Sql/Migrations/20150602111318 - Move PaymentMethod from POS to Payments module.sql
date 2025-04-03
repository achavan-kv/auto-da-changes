-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'PaymentMethod'
               AND TABLE_SCHEMA = 'Payments')
BEGIN
    
    CREATE TABLE Payments.PaymentMethod
    (
        Id TINYINT PRIMARY KEY,
        [Description] VARCHAR (64) NOT NULL,
        [Active] BIT NOT NULL,
    )

END

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'PaymentMethod'
               AND TABLE_SCHEMA = 'Sales')
BEGIN

    INSERT INTO 
        Payments.PaymentMethod (id, [Description], Active)
    SELECT
        s.Id, s.[Description], s.Active
    FROM
        Sales.PaymentMethod s


    IF (OBJECT_ID(N'[Sales].[FK_OrderReturnPayment_PaymentMethod]') IS NOT NULL) 
    BEGIN
	    ALTER TABLE 
           Sales.OrderReturnPayment
	    DROP CONSTRAINT 
            FK_OrderReturnPayment_PaymentMethod

        ALTER TABLE 
            Sales.OrderReturnPayment 
        ADD CONSTRAINT
	        FK_OrderReturnPayment_PaymentMethod FOREIGN KEY
	        (
	            PaymentMethodId
	        ) REFERENCES Payments.PaymentMethod
	        (
	            Id
	        )
    END

    DROP TABLE Sales.PaymentMethod
        
END
ELSE
BEGIN
    
    IF NOT EXISTS( SELECT Id FROM Payments.PaymentMethod WHERE Id = 1 AND [Description] = 'Cash' ) 
    BEGIN
	    INSERT INTO Payments.PaymentMethod (Id, [Description], Active)
	    VALUES (1, 'Cash', 1)
    END	


    IF NOT EXISTS( SELECT Id FROM Payments.PaymentMethod WHERE Id = 2 AND [Description] = 'Foreign Cash' ) 
    BEGIN
	    INSERT INTO Payments.PaymentMethod (Id, [Description], Active)
	    VALUES (2, 'Foreign Cash', 1)
    END	


    IF NOT EXISTS( SELECT Id FROM Payments.PaymentMethod WHERE Id = 3 AND [Description] = 'Store Card' ) 
    BEGIN
	    INSERT INTO Payments.PaymentMethod (Id, [Description], Active)
	    VALUES (3, 'Store Card', 1)
    END	


    IF NOT EXISTS( SELECT Id FROM Payments.PaymentMethod WHERE Id = 4 AND [Description] = 'Gift Voucher' )
    BEGIN
	    INSERT INTO Payments.PaymentMethod (Id, [Description], Active)
	    VALUES (4, 'Gift Voucher', 1)
    END	


    IF NOT EXISTS( SELECT Id FROM Payments.PaymentMethod WHERE Id = 5 AND [Description] = 'Cheque' ) 
    BEGIN
	    INSERT INTO Payments.PaymentMethod (Id, [Description], Active)
	    VALUES (5, 'Cheque', 1)
    END	


    IF NOT EXISTS( SELECT Id FROM Payments.PaymentMethod WHERE Id = 6 AND [Description] = 'Debit/Credit Card' ) 
    BEGIN
	    INSERT INTO Payments.PaymentMethod (Id, [Description], Active)
	    VALUES (6, 'Debit/Credit Card', 1)
    END	


    IF NOT EXISTS( SELECT Id FROM Payments.PaymentMethod WHERE Id = 7 AND [Description] = 'Standing Order' ) 
    BEGIN
	    INSERT INTO Payments.PaymentMethod (Id, [Description], Active)
	    VALUES (7, 'Standing Order', 1)
    END	


    IF NOT EXISTS( SELECT Id FROM Payments.PaymentMethod WHERE Id = 8 AND [Description] = 'Travellers Cheque' ) 
    BEGIN
	    INSERT INTO Payments.PaymentMethod (Id, [Description], Active)
	    VALUES (8, 'Travellers Cheque', 1)
    END	

    IF NOT EXISTS( SELECT Id FROM Payments.PaymentMethod WHERE Id = 9 AND [Description] = 'Direct Debit' ) 
    BEGIN
	    INSERT INTO Payments.PaymentMethod (Id, [Description], Active)
	    VALUES (9, 'Direct Debit', 1)
    END	

END
