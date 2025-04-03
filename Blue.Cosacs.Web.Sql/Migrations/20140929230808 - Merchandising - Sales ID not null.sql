-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE Merchandising.Location SET SalesId=LocationId WHERE SalesId IS NULL


ALTER TABLE Merchandising.Location ALTER COLUMN SalesId VARCHAR(100) NOT NULL