-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'Bank'
               AND TABLE_SCHEMA = 'Payments')
BEGIN
    CREATE TABLE Payments.Bank
    (
        Id int IDENTITY(1,1) PRIMARY KEY,
        BankName VARCHAR(40) not null,
        Active bit not null
    )
END
GO