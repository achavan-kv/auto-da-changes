-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Rename existing columns on the StoreCardBranchQualRules and StoreCardBranchQualRulesAudit tables
IF EXISTS(select * from information_schema.columns
			where table_name = 'StoreCardBranchQualRules' and Column_Name = 'MinMthsAcctHist')
BEGIN
	EXEC sp_rename
		@objname = 'StoreCardBranchQualRules.MinMthsAcctHist',
		@newname = 'MinMthsAcctHistX',
		@objtype = 'COLUMN' 
END
GO

IF EXISTS(select * from information_schema.columns
			where table_name = 'StoreCardBranchQualRules' and Column_Name = 'MaxPrevMthsInArrs')
BEGIN
	EXEC sp_rename
		@objname = 'StoreCardBranchQualRules.MaxPrevMthsInArrs',
		@newname = 'MaxPrevMthsInArrsX',
		@objtype = 'COLUMN' 
END
GO

IF EXISTS(select * from information_schema.columns
			where table_name = 'StoreCardBranchQualRules' and Column_Name = 'MinAvailRFLimit')
BEGIN
	EXEC sp_rename
		@objname = 'StoreCardBranchQualRules.MinAvailRFLimit',
		@newname = 'PcentInitRFLimit',
		@objtype = 'COLUMN' 
END
GO

IF EXISTS(select * from information_schema.columns
			where table_name = 'StoreCardBranchQualRulesAudit' and Column_Name = 'MinMthsAcctHist')
BEGIN
	EXEC sp_rename
		@objname = 'StoreCardBranchQualRulesAudit.MinMthsAcctHist',
		@newname = 'MinMthsAcctHistX',
		@objtype = 'COLUMN' 
END
GO

IF EXISTS(select * from information_schema.columns
			where table_name = 'StoreCardBranchQualRulesAudit' and Column_Name = 'MaxPrevMthsInArrs')
BEGIN
	EXEC sp_rename
		@objname = 'StoreCardBranchQualRulesAudit.MaxPrevMthsInArrs',
		@newname = 'MaxPrevMthsInArrsX',
		@objtype = 'COLUMN' 
END
GO

IF EXISTS(select * from information_schema.columns
			where table_name = 'StoreCardBranchQualRulesAudit' and Column_Name = 'MinAvailRFLimit')
BEGIN
	EXEC sp_rename
		@objname = 'StoreCardBranchQualRulesAudit.MinAvailRFLimit',
		@newname = 'PcentInitRFLimit',
		@objtype = 'COLUMN' 
END
GO

--Add new columns to the StoreCardBranchQualRules and StoreCardBranchQualRulesAudit tables

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.Columns
           WHERE  Table_Name = 'StoreCardBranchQualRules'
           AND    Column_Name = 'MinMthsAcctHistY')
BEGIN
    ALTER TABLE StoreCardBranchQualRules ADD MinMthsAcctHistY INT NULL
END
GO
 
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.Columns
           WHERE  Table_Name = 'StoreCardBranchQualRules'
           AND    Column_Name = 'MaxPrevMthsInArrsY')
BEGIN
    ALTER TABLE StoreCardBranchQualRules ADD MaxPrevMthsInArrsY INT NULL
END
GO


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.Columns
           WHERE  Table_Name = 'StoreCardBranchQualRulesAudit'
           AND    Column_Name = 'MinMthsAcctHistY')
BEGIN
    ALTER TABLE StoreCardBranchQualRulesAudit ADD MinMthsAcctHistY INT NULL
END
GO
 
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.Columns
           WHERE  Table_Name = 'StoreCardBranchQualRulesAudit'
           AND    Column_Name = 'MaxPrevMthsInArrsY')
BEGIN
    ALTER TABLE StoreCardBranchQualRulesAudit ADD MaxPrevMthsInArrsY INT NULL
END
GO

--Change the type of column PcentInitRFLimit from money to float
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Columns
           WHERE  Table_Name = 'StoreCardBranchQualRules'
           AND    Column_Name = 'PcentInitRFLimit')
BEGIN
	ALTER TABLE StoreCardBranchQualRules ALTER COLUMN PcentInitRFLimit FLOAT NULL
END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Columns
           WHERE  Table_Name = 'StoreCardBranchQualRulesAudit'
           AND    Column_Name = 'PcentInitRFLimit')
BEGIN
	ALTER TABLE StoreCardBranchQualRulesAudit ALTER COLUMN PcentInitRFLimit FLOAT NULL
END
GO