-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #12509


UPDATE dbo.codecat
SET usermaint = 'N' 
WHERE category IN ('WDR', 'WLR', 'WPR')