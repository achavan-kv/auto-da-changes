-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'MaxSpendLimitRefer')
INSERT INTO CountryMaintenance
(CountryCode,ParameterCategory,
NAME,Value,[Type],PRECISION,
OptionCategory,OptionListName,Description,
CodeName)
SELECT CountryCode,'07',
'Max Spend limit before referral','100000000','numeric','0',
'','','This will be automatically updated from the current score card ',
'MaxSpendLimitRefer' FROM dbo.Country
 
UPDATE code SET codedescript = 'Spend Limit Exceeds Referral Limit'
WHERE code = 'SL' AND category = 'SN1'