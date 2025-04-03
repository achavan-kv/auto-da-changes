-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
UPDATE codecat 
SET ToolTipText = 'Months Applicable for current Account enter the min months the account has been running (since delivery). The system works off whole months. For the settled account Months applicable should be based on when account was settled - the min length of agreement will be the same as current account'
WHERE category = 'lxr'
