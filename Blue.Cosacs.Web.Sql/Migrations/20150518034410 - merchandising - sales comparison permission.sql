-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
insert into admin.Permission (CategoryId, Id, Name, Description) values (20, 2045, 'Report - Sales Comparison', 'Grants access to view the Sales Comparison Report')
