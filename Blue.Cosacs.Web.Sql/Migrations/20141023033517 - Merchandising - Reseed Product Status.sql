-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here



update [Merchandising].[ProductStatus]
set Name = 'Active Current'
where Name = 'Current'

Update [Merchandising].[ProductStatus]
set Name = 'Active New'
, IsAutomatic  = 0
, IsSystem = 1
where Name = 'New'

update [Merchandising].[ProductStatus]
set IsSystem = 1
Where name in ('Aged', 'Deleted', 'Inactive')

update [Merchandising].[ProductStatus]
set IsAutomatic = 1
where name = 'Deleted'