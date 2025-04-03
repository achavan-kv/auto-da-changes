-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
--Related to issue: #11243



IF EXISTS (SELECT * FROM ADMIN.Permission WHERE id = 380)
BEGIN
	UPDATE admin.Permission
	SET Description = 'Allow telephone caller operations in Worklist Setup & Collection Commission screens'
	WHERE id = 380
END


IF EXISTS (SELECT * FROM ADMIN.Permission WHERE id = 381)
BEGIN
	UPDATE admin.Permission
	SET Description = 'Allow collector operations in Zone Automated Allocation, Worklist Setup & Collection Commission screens'
	WHERE id = 381
END


