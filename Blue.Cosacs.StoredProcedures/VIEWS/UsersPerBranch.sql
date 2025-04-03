IF OBJECT_ID('UsersPerBranch') IS NOT NULL
	DROP VIEW UsersPerBranch
GO
CREATE VIEW UsersPerBranch
AS
	SELECT 
		u.Id, 
		u.BranchNo
	FROM 
		Admin.[User] u
		INNER JOIN branch b
			ON u.BranchNo = b.branchno

