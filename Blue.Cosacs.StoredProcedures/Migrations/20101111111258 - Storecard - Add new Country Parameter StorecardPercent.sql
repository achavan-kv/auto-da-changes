-- put your SQL code here
IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE NAME = 'Revolving Credit Split Percentage')
INSERT INTO CountryMaintenance
(CountryCode,ParameterCategory,
NAME,Value,[Type],PRECISION,
OptionCategory,OptionListName,Description,
CodeName)

SELECT CountryCode,'33',
'Revolving Credit Split Percentage','0','numeric','2',
'','','Maximum percentage of Total Credit allocated to Storecard',
'StorecardPercent' FROM dbo.Country

