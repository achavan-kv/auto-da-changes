-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


Alter Table 
	dbo.StoreCardStatus 
		add EmpeeNo int
		
Alter Table 
	dbo.StoreCardStatus 
		add BranchNo smallint
Go

Update dbo.StoreCardStatus 
	Set		
		EmpeeNo = 99999, 
		BranchNo = 700

Alter Table dbo.StoreCardStatus 
	Alter Column 
		EmpeeNo int Not Null
		
Alter Table dbo.StoreCardStatus 
	Alter Column 
		BranchNo smallint Not Null

ALTER TABLE dbo.StoreCardStatus 
	ADD FOREIGN KEY (EmpeeNo)
		REFERENCES dbo.courtsperson(empeeno)

ALTER TABLE dbo.StoreCardStatus 
	ADD FOREIGN KEY (BranchNo)
		REFERENCES dbo.branch(branchno)

