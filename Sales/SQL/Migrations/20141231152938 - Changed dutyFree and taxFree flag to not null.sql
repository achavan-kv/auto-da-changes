-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
UPDATE Sales.[Order] SET IsTaxFreeSale = 0 WHERE IsTaxFreeSale IS NULL
ALTER TABLE Sales.[Order] ALTER COLUMN IsTaxFreeSale bit NOT NULL

UPDATE Sales.[Order] SET IsDutyFreeSale = 0 WHERE IsDutyFreeSale IS NULL
ALTER TABLE Sales.[Order] ALTER COLUMN IsDutyFreeSale bit NOT NULL