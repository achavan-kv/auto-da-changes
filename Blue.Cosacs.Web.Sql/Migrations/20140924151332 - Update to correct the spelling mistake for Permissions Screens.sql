-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- 

update Admin.Permission
SET Name = 'Create/Delete Warranty Return Percentage',
Description = 'Allow user to create new Warranty Return Percentage and delete existing ones.'
where Name = 'Create/Delete Warranty Return Pecentage'

update Admin.Permission
SET Name = 'View Warranty Return Percentage',
Description = 'Allow user to view Warranty Return Percentage.'
where Name = 'View Warranty Return Pecentage'