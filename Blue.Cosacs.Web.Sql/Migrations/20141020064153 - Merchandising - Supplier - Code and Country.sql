-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE Merchandising.Supplier ADD Code varchar(100) NULL
ALTER TABLE Merchandising.Supplier ADD Country varchar(100) NOT NULL