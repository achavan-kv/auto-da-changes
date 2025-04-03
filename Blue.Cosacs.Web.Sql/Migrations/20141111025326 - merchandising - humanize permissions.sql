-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

update admin.Permission
set name = 'Tax Rate View'
where name = 'TaxRateView'


update admin.Permission
set name = 'Tax Rate Edit'
where name = 'TaxRateEdit'


update admin.Permission
set name = 'Associated Products View'
where name = 'AssociatedProductsView'


update admin.Permission
set name = 'Associated Products Edit'
where name = 'AssociatedProductsEdit'

update admin.Permission
set name = 'Promotion View'
where name = 'PromotionView'

update admin.Permission
set name = 'Promotion Edit'
where name = 'PromotionEdit'

update admin.Permission
set name = 'Retail Price View'
where name = 'RetailPriceView'

update admin.Permission
set name = 'Retail Price Edit'
where name = 'RetailPriceEdit'

update admin.Permission
set name = 'Cost Price View'
where name = 'CostPriceView'

update admin.Permission
set name = 'Cost Price Edit'
where name = 'CostPriceEdit'