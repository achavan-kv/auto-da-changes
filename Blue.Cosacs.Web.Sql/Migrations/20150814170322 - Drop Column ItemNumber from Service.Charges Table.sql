-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
           WHERE table_schema='Service' AND table_name='Charge' AND column_name='ItemNumber')
BEGIN
	ALTER TABLE Service.Charge
	DROP COLUMN ItemNumber
END
