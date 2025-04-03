-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #12361


IF NOT EXISTS(SELECT * FROM admin.Permission WHERE id = 57)
BEGIN
	INSERT INTO admin.Permission
	SELECT * FROM adminPermission_removed WHERE id = 57
END


IF NOT EXISTS(SELECT * FROM admin.Permission WHERE id = 59)
BEGIN
	INSERT INTO admin.Permission
	SELECT * FROM adminPermission_removed WHERE id = 59
END