-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
insert into [admin].[permission] (Id,Name,CategoryId,[Description]) 
values(2167, 'Top Sku Report View', 21, 'View the Top Sku report')
