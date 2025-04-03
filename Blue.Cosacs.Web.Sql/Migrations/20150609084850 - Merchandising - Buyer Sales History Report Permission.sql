-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

delete from admin.permission where name = 'Report - Buyer Sales History Report'
insert into admin.Permission (CategoryId, Id, Name, Description) 
values (20, 2053, 'Report - Buyer Sales History Report', 'Grants access to view the Buyer Sales History Report')
