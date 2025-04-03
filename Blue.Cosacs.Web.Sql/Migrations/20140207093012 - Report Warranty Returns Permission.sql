-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from Admin.Permission where id = 2004)
BEGIN

	INSERT INTO Admin.Permission 
	(id, Name, CategoryId, [Description])
	VALUES
	(
		2004, 
		'Report - Warranty Returns',
		20,
		'Warranty Returns Report'
	) 

END