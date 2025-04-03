-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


--Move the Enable StoreCard Country Parameter to correct category
UPDATE CountryMaintenance 
SET ParameterCategory = 33 
WHERE parametercategory = 34 
AND codename = 'StoreCardEnabled'
go

--Delete duplicate entry for Store Card in Code table.
DELETE FROM Code 
WHERE category = 'CMC' 
AND code = 34 
AND codedescript = 'StoreCard'
go

--Add new Country Parameter for maximum value of an item when purchasing the item on a Store Card
IF NOT EXISTS(SELECT * FROM CountryMaintenance WHERE CodeName = 'MaxItemValStoreCard')
INSERT INTO CountryMaintenance
(
	CountryCode,
	ParameterCategory,
	Name,
	Value,
	[Type],
	[PRECISION],
	OptionCategory,
	OptionListName,
	[Description],
	CodeName
)
SELECT CountryCode, 
	   '33',
	   'Maximum Item Value',
	   '0',
	   'numeric',
	    0,
	   '',
	   '',
	   'This is the maximum value that an item can have when purchasing the item using a Store Card',
	   'MaxItemValStoreCard'
FROM Country
go