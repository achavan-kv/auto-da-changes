-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


declare @maxWarId int

select @maxWarId= max(Id) from warranty.Warranty


update warranty.WarrantySale
set WarrantyId= w.id
from warranty.WarrantySale ws, warranty.Warranty w
where ws.WarrantyId is not null
	and ws.WarrantyId>@maxWarId
	and ws.WarrantyNumber=w.Number

 
