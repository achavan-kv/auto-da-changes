-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Dropping the following foreign key as procedure: StoreCardBranchQualRulesSave.sql is required to delete from the StoreCardBranchQualRules table
--where the rules for a branch have been disabled.
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[fk_StoreCardBranchQualRulesAudit]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoreCardBranchQualRulesAudit]'))
ALTER TABLE [dbo].[StoreCardBranchQualRulesAudit] DROP CONSTRAINT [fk_StoreCardBranchQualRulesAudit]
GO