-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns 
	WHERE table_name ='SR_Allocation' AND column_name = 'ReAssignedBy')
Alter TABLE SR_Allocation add ReAssignedBy INT null

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns 
	WHERE table_name ='SR_AllocationAudit' AND column_name = 'ReAssignedBy')
Alter TABLE SR_AllocationAudit add ReAssignedBy INT null
