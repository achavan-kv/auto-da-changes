-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from Admin.Permission where id = 2023 and categoryid = 20)
BEGIN
	INSERT INTO Admin.Permission
		(Id, Name, CategoryId, Description)
	VALUES
		(2023, 'Report - Delivery Performance Summary', 20, 'Delivery Performance Summary Report')
END


