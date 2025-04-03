-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
UPDATE dbo.branch
SET StoreType = 'N', AshleyStore = NULL, LuckyDollarStore = NULL, RadioShackStore = 1
WHERE branchno IN (750, 751) AND CountryCode = 'T' AND BranchName LIKE '%RADIOSHACK%';