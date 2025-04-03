-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE Merchandising.GoodsOnLoan DROP COLUMN AddresssLine1
ALTER TABLE Merchandising.GoodsOnLoan ADD AddressLine1 varchar(100) NOT NULL
