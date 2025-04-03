-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE StoreCardBranchQualRules ALTER COLUMN MaxCurrMthsInArrs DECIMAL (5,2)
ALTER TABLE StoreCardBranchQualRules ALTER COLUMN MaxPrevMthsInArrsX DECIMAL (5,2)
ALTER TABLE StoreCardBranchQualRulesAudit ALTER COLUMN MaxCurrMthsInArrs DECIMAL (5,2)
ALTER TABLE StoreCardBranchQualRulesAudit ALTER COLUMN MaxPrevMthsInArrsX DECIMAL (5,2)
 