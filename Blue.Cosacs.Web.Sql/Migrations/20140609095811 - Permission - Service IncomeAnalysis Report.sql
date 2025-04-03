-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists(select * from Admin.Permission where Id=2021)
begin
INSERT INTO Admin.Permission
	(Id, Name, CategoryId, Description)
VALUES   
	(2021, 'Report - Service Income Analysis', 20, 'Service Income and Margin Analysis Report')

end

