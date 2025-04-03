-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @taxrate money 
select  @taxrate=value from CountryMaintenance where codename='taxrate'

update service.Charge
	set tax=cast(value-(value* 100/(100+@taxrate)) as Decimal(12,2)),Value=value-cast(value-(value* 100/(100+@taxrate)) as Decimal(12,2)) 
from service.Charge c 
where value!=0

