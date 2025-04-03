-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE Merchandising.ComboProductPrice
ADD Fascia VARCHAR(100) Null

ALTER TABLE Merchandising.ComboProductPrice
ALTER COLUMN LocationId INT NULL