-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 

IF EXISTS (SELECT 1 FROM country WHERE countrycode = 'M')
  UPDATE courtsperson
  SET datepasschge = '1900-01-01'
