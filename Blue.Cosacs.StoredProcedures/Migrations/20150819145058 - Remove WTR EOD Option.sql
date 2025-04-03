-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

DELETE FROM EodConfigurationOption WHERE OptionCode = 'WKLYTRAD'
DELETE FROM code WHERE code = 'WKLYTRAD' AND category = 'EDC'