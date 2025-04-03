-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE Admin.[User] ADD CONSTRAINT
	FK_User_branch FOREIGN KEY
	(
		BranchNo
	) REFERENCES dbo.branch
	(
		branchno
	) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
	
GO
