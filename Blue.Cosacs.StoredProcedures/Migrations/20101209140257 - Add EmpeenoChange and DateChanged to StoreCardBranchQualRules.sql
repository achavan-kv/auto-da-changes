-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


ALTER TABLE StoreCardBranchQualRules
ADD EmpeenoChange int NULL
go

ALTER TABLE StoreCardBranchQualRules
ADD DateChanged datetime NOT NULL
go

