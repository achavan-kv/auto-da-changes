-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

update warranty.WarrantySale
set EffectiveDate = case when WarrantyIsFree = 1 then ItemDeliveredOn
					when WarrantyIsFree = 0 then 
						isnull((select dateadd(month, ws1.WarrantyLength, ws1.ItemDeliveredOn)  from warranty.WarrantySale ws1
							where ws1.CustomerAccount = ws.CustomerAccount
							and ws1.InvoiceNumber = ws.InvoiceNumber
							and ws1.ItemId = ws.ItemId
							and ws1.StockLocation = ws.StockLocation
							and ws1.WarrantyGroupId = ws.WarrantyGroupId
							and ws1.WarrantyIsFree != ws.WarrantyIsFree), ws.ItemDeliveredOn) end
from warranty.WarrantySale ws
where EffectiveDate is null