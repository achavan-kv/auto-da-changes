-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
 

insert into CountryMaintenance
 ( CountryCode, ParameterCategory, Name, Value,
  Type, Precision, OptionCategory, OptionListName, 
  Description, CodeName)
select    countrycode,33 , 'Store Card Default Months',48, 
'numeric',0,'','',
'Default Expiry Months for new Cards','StoreCardDefaultCardMonths'
from Country
where not exists (select 'x' from CountryMaintenance where codename='StoreCardDefaultCardMonths')
 