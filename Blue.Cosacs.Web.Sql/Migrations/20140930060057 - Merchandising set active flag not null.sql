-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
UPDATE Merchandising.Location SET Active=1 WHERE Active IS NULL


ALTER TABLE Merchandising.Location ALTER COLUMN Active BIT NOT NULL