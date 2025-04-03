-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE CountryMaintenance SET NAME ='Refer Existing No home/Work Phone with Mobile' ,
Description = 'When set to true existing customers without home and work phones but with a mobile will get referred on scoring. When set to False existing customers will not get referred if they have a mobile. Customers without any phone at all will always get referred'
WHERE CodeName= 'MobPhoneRefer'


IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'MobPhoneReferNew')
INSERT INTO CountryMaintenance
(CountryCode,ParameterCategory,
NAME,Value,[Type],PRECISION,
OptionCategory,OptionListName,Description,
CodeName)
SELECT CountryCode,'07',
'Refer New No home/Work Phone with Mobile','True','checkbox','0',
'','','When set to true new customers without home and work phones but with a mobile will get referred on scoring. When set to False new customers will not get referred if they have a mobile. Customers without any phone at all will always get referred',
'MobPhoneReferNew' FROM dbo.Country
