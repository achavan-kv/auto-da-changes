-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

Update Warranty.WarrantySale
	set WarrantyLength=w.Length
from Warranty.WarrantySale s inner  join warranty.Warranty w on s.WarrantyId =w.Id
where s.warrantyLength > w.Length
and s.WarrantyLength>=24





