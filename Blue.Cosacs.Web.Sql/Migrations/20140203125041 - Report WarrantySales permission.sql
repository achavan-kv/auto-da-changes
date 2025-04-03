-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from Admin.Permission where id = 2002)
BEGIN

	INSERT INTO Admin.Permission 
	(id, Name, CategoryId, [Description])
	VALUES
	(
		2002, 
		'Report - Warranty Sales',
		20,
		'Warranty Sales Report'
	) 

END