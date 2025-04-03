-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE Merchandising.Product ADD RequiresLabel bit NULL
ALTER TABLE Merchandising.Product ADD NumberOfBoxes int NULL