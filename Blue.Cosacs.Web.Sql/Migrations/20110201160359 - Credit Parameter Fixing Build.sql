-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns WHERE table_name ='InstantCreditParameters'
AND column_name = 'settledlength')
BEGIN

ALTER TABLE InstantCreditParameters
DROP COLUMN settledlength 
END
GO 
ALTER TABLE InstantCreditParameters
ADD settledmonths varchar