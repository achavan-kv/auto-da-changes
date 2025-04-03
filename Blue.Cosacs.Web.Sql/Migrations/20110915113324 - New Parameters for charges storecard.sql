-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/*--Late Payment Fees – applied if payment made after due date
--Missed Payment Fee – applied if no payment made in statement period
--Annual Fee – Applied on application and anniversary of application
--Over the limit fees – applied when balance of card exceeds limit
--Reprinted Statement Fee – applied when manual statement printed
--Replacement Card Fee – applied when card is lost/stolen and replacement issued
--Penalty Interest (%) - if more than x instalments missed*/

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'SCLatePaymentFees')
BEGIN

--Late Payment Fees – applied if payment made after due date
INSERT INTO CountryMaintenance
(CountryCode,ParameterCategory,
NAME,Value,[Type],PRECISION,
OptionCategory,OptionListName,Description,
CodeName)
SELECT CountryCode,'33',
'Late Payment Fees ','0','numeric','2',
'','','Late Payment Fees – applied if payment made after due date',
'SCLatePaymentFees' FROM dbo.Country

--Annual Fee – Applied on application and anniversary of application
INSERT INTO CountryMaintenance
(CountryCode,ParameterCategory,
NAME,Value,[Type],PRECISION,
OptionCategory,OptionListName,Description,
CodeName)
SELECT CountryCode,'33',
'Annual Card Fee','0','numeric','2',
'','','Annual Fee – Applied on Activation and anniversary of Activation',
'SCAnnualFee' FROM dbo.Country

--Reprinted Statement Fee – applied when manual statement printed
INSERT INTO CountryMaintenance
(CountryCode,ParameterCategory,
NAME,Value,[Type],PRECISION,
OptionCategory,OptionListName,Description,
CodeName)
SELECT CountryCode,'33',
'Statement Fee','0','numeric','2',
'','','Statement Fee on Reprint of Statement',
'SCStatementFee' FROM dbo.Country

INSERT INTO CountryMaintenance
(CountryCode,ParameterCategory,
NAME,Value,[Type],PRECISION,
OptionCategory,OptionListName,Description,
CodeName)
SELECT CountryCode,'33',
'Replacement Card Fee','0','numeric','2',
'','','Replacement Card Fee applied when card is lost/stolen and replacement issued',
'SCReplacementFee' FROM dbo.Country
--Replacement Card Fee – applied when card is lost/stolen and replacement issued

END  