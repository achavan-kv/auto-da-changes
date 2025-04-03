-- This parameter will determine whether storecard accounts will be auto activated by default.
-- 
-- 
-- 
-- Put your SQL code here
IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE NAME = 'Activate Card by Default')
INSERT INTO CountryMaintenance
(CountryCode,ParameterCategory,
NAME,Value,[Type],PRECISION,
OptionCategory,OptionListName,Description,
CodeName)

SELECT CountryCode,'33',
'Activate Card by Default','False','checkbox','0',
'','','New Store Cards issued will be activated by default',
'StoreCardActivate' FROM dbo.Country


