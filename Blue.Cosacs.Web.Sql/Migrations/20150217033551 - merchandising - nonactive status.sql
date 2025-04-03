-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
update Merchandising.ProductStatus
set name = 'Non Active'
Where name = 'Inactive'
