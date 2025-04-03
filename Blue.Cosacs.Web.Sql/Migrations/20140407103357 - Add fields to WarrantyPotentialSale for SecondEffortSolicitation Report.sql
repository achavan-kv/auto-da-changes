
IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'CustomerAccount' AND [object_id] = OBJECT_ID(N'Warranty.WarrantyPotentialSale'))
BEGIN
    ALTER TABLE Warranty.WarrantyPotentialSale
    ADD CustomerAccount CHAR(12) NULL
END
GO

-- FILL CustomerAccount accounts...
UPDATE Warranty.WarrantyPotentialSale
SET CustomerAccount = Accts.AccountNumber
FROM Warranty.WarrantyPotentialSale wp
INNER JOIN (
    SELECT Id, SUBSTRING(InvoiceNumber, 1, 12) AS AccountNumber
    FROM Warranty.WarrantyPotentialSale
) Accts ON wp.Id=Accts.Id
GO

-- MAKE FIELD NO NULL
ALTER TABLE Warranty.WarrantyPotentialSale
ALTER COLUMN CustomerAccount CHAR(12) NOT NULL
GO

-- ADD FOREIGN KEY
ALTER TABLE Warranty.WarrantyPotentialSale
ADD CONSTRAINT FK_WarrantyPotentialSale_CustomerAccount__acct_acctno
FOREIGN KEY (CustomerAccount)
REFERENCES acct(acctno)
GO
