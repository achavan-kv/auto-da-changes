-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
UPDATE code SET codedescript = 'Highest Months Arrears Settled 2' WHERE code = 'HMAS2'
AND category='lxr'