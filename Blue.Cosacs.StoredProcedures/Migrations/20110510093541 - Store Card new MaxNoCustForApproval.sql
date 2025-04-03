-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Add new column to hold the maximum number of customers for pre-approval 

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.Columns
           WHERE  Table_Name = 'StoreCardBranchQualRules'
           AND    Column_Name = 'MaxNoCustForApproval')
BEGIN
    ALTER TABLE StoreCardBranchQualRules ADD MaxNoCustForApproval INT NULL
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.Columns
           WHERE  Table_Name = 'StoreCardBranchQualRulesAudit'
           AND    Column_Name = 'MaxNoCustForApproval')
BEGIN
    ALTER TABLE StoreCardBranchQualRulesAudit ADD MaxNoCustForApproval INT NULL
END
GO

