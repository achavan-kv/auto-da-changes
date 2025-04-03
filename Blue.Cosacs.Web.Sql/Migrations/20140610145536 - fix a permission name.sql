-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
UPDATE Admin.Permission 
SET NAME = 'Report - Service Request Financial',
Description = 'Service Request Financial Report'
WHERE id = 2015