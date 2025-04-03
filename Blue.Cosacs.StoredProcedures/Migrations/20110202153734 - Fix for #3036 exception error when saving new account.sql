-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

	ALTER TABLE InstantCreditParameters ALTER COLUMN addressmonths INT 
	ALTER TABLE InstantCreditParameters ALTER COLUMN employmonths INT 
	ALTER TABLE InstantCreditParameters ALTER COLUMN settledmonths INT 
	ALTER TABLE InstantCreditParameters ALTER COLUMN settledlength INT 
	ALTER TABLE InstantCreditParameters ALTER COLUMN jointqual VARCHAR(5)