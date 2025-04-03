-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #11989

IF NOT EXISTS(SELECT * FROM control WHERE taskid = 1100)
BEGIN
	INSERT INTO control
	SELECT 1100, 'MainForm', 'menuService', 1, 1, ''
	UNION 
	SELECT 1100, 'MainForm', 'menuBERRep',1,1, 'menuService'
END


IF EXISTS(SELECT * FROM control WHERE taskid = 279)
BEGIN
	DELETE FROM control WHERE TaskID = 279
END