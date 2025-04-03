-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

update merchandising.setlocation 
set EffectiveDate = '2015-01-01'

ALTER TABLE Merchandising.SetLocation
ALTER COLUMN EffectiveDate DATE NOT NULL