-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

DELETE FROM Payments.PaymentMethod

INSERT INTO Payments.PaymentMethod (Id, [Description], Active, IsReturnAllowed) VALUES
    (1, 'Cash', 1, 1),
    (2, 'Foreign Cash', 1, 0),
    (3, 'Store Card', 1, 1),
    (4, 'Gift Voucher', 1, 0),
    (5, 'Cheque', 1, 1),
    (6, 'Debit Card', 1, 0),
    (7, 'Standing Order', 1, 0),
    (8, 'Travellers Cheque', 1, 1),
    (9, 'Direct Debit', 1, 1),
    (10, 'Credit Card', 1, 1)
