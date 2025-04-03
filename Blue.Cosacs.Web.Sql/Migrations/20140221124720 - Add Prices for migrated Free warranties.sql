-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

insert into warranty.WarrantyPrice (
WarrantyId, BranchType, BranchNumber, CostPrice, RetailPrice, EffectiveDate)
select id,null,null,0,0,getdate()
from warranty.Warranty 
where free=1 and number like '%M'
