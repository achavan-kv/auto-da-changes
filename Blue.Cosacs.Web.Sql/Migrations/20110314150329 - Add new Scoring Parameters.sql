-- transaction: true

-- put your SQL code here
SELECT * FROM codecat WHERE category='LXR' 

INSERT INTO codecat (
	origbr,	category,	catdescript,
	codelgth,	forcenum,	forcenumdesc,
	usermaint,	CodeHeaderText,	DescriptionHeaderText,
	SortOrderHeaderText,	ReferenceHeaderText,	AdditionalHeaderText,
	ToolTipText
) VALUES ( 
 0,	 'LXS', N'Limit Exceeded referral rules',
	4, 'N',	 'N', 
	'Y', 'Code', 'Description',
	'Months Applicable', 'Max Value', 'Increase Allowed Percent',
	/* ToolTipText - varchar(300) */ '' ) 

IF NOT EXISTS (SELECT * FROM code WHERE code = 'HMAC1' AND category = 'LXR')
INSERT INTO code (origbr,category,codedescript,code,statusflag,sortorder,reference,additional)
VALUES (0,'LXR','Highest Months Arrears Current','HMAC1','L',0,2,20)

IF NOT EXISTS (SELECT * FROM code WHERE code = 'HMAC2' AND category = 'LXR')
INSERT INTO code (origbr,category,codedescript,code,statusflag,sortorder,reference,additional)
VALUES (0,'LXR','Highest Months Arrears Current 2','HMAC2','L',0,3,10)

IF NOT EXISTS (SELECT * FROM code WHERE code = 'HMAS1' AND category = 'LXR')
INSERT INTO code (origbr,category,codedescript,code,statusflag,sortorder,reference,additional)
VALUES (0,'LXR','Highest Months Arrears Settled 1','HMAS1','L',15,2,20)

IF NOT EXISTS (SELECT * FROM code WHERE code = 'HMAS2' AND category = 'LXR')
INSERT INTO code (origbr,category,codedescript,code,statusflag,sortorder,reference,additional)
VALUES (0,'LXR','Highest Months Arrears Settled 1','HMAS2','L',15,3,10)

-- put your SQL code here


IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'MobPhoneRefer')
INSERT INTO CountryMaintenance
(CountryCode,ParameterCategory,
NAME,Value,[Type],PRECISION,
OptionCategory,OptionListName,Description,
CodeName)

SELECT CountryCode,'07',
'Mobile Phone refer for existing customers','0','checkbox','0',
'','','Determines whether existing customers get referred if they only have a mobile and no work/home telephone number',
'MobPhoneRefer' FROM dbo.Country

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'MobPhoneRefer')
INSERT INTO CountryMaintenance
(CountryCode,ParameterCategory,
NAME,Value,[Type],PRECISION,
OptionCategory,OptionListName,Description,
CodeName)
SELECT CountryCode,'07',
'Mobile Phone refer for existing customers','True','checkbox','0',
'','','Determines whether existing customers get referred if they only have a mobile and no work/home telephone number',
'MobPhoneRefer' FROM dbo.Country

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'StatExistsRefer')
INSERT INTO CountryMaintenance
(CountryCode,ParameterCategory,
NAME,Value,[Type],PRECISION,
OptionCategory,OptionListName,Description,
CodeName)
SELECT CountryCode,'07',
'Status code do not refer if subsequent','True','checkbox','0',
'','','Determines whether customers with bad status get referred if have opened subsequent good account',
'StatExistsRefer' FROM dbo.Country

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'StatExistsRefer')
INSERT INTO CountryMaintenance
(CountryCode,ParameterCategory,
NAME,Value,[Type],PRECISION,
OptionCategory,OptionListName,Description,
CodeName)
SELECT CountryCode,'07',
'Status code do not refer if subsequent','True','checkbox','0',
'','','Determines whether customers with bad status get referred if have opened subsequent good account',
'StatExistsRefer' FROM dbo.Country

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'WorstStatusPeriod')
INSERT INTO CountryMaintenance
(CountryCode,ParameterCategory,
NAME,Value,[Type],PRECISION,
OptionCategory,OptionListName,Description,
CodeName)
SELECT CountryCode,'07',
'Months over which to check worst status','120','numeric','0',
'','','Determines over what period to check worst account status when scoring',
'WorstStatusPeriod' FROM dbo.Country
-- Caribbean want this set to 24 months.
update countrymaintenance set value = '24' where countrycode not in ('P','M','C','Y')
and codename = 'WorstStatusPeriod' 

SELECT * FROM CountryMaintenance WHERE value = 'true'
IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'MinExpenseRefer')
INSERT INTO CountryMaintenance
(CountryCode,ParameterCategory,
NAME,Value,[Type],PRECISION,
OptionCategory,OptionListName,Description,
CodeName)
SELECT CountryCode,'07',
'Minimum Expense Referral for existing customers','True','checkbox' ,'0',
'','','Determines whether to apply rule to subsequent customers if they do not have a certain level of expenses',
'MinExpenseRefer' FROM dbo.Country

update countrymaintenance set value = 'False' where countrycode not in ('P','M','C','Y')
and codename = 'MinExpenseRefer' 

