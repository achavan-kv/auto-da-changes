-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

Alter TABLE WarrantyReturnCodes add WarrantyMonths INT not null default 0

Alter TABLE WarrantyReturnCodes add ManufacturerMonths INT not null default 12

go
UPDATE WarrantyReturnCodes 
	set WarrantyMonths=WarrantyLength*12


