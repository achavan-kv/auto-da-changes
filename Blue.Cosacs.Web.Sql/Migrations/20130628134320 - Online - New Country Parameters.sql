-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here



IF NOT EXISTS(select * from countrymaintenance where codename = 'OnlineDistCentre')
BEGIN
	INSERT INTO CountryMaintenance(CountryCode, ParameterCategory, Name, Value, [Type], [Precision], OptionCategory, OptionListName, [Description], CodeName)
	SELECT c.countrycode, '01', 'Online Distribution Centre', hobranchno, 'numeric', 0, '', '','This is the Online Distribution Centre branch number.', 'OnlineDistCentre'
	FROM Country c
END

IF NOT EXISTS(select * from countrymaintenance where codename = 'OnlineSBandIntRate')
BEGIN
	INSERT INTO CountryMaintenance(CountryCode, ParameterCategory, Name, Value, [Type], [Precision], OptionCategory, OptionListName, [Description], CodeName)
	SELECT c.countrycode, '01', 'Online Scoring Band', 'A', 'text', 0, '', '','This is the Scoring band (A, B or C) for the interest rate to be used when calculating weekly price for Online products.', 'OnlineSBandIntRate'
	FROM Country c
END

IF NOT EXISTS(select * from countrymaintenance where codename = 'OnlineTermsType')
BEGIN
	INSERT INTO CountryMaintenance(CountryCode, ParameterCategory, Name, Value, [Type], [Precision], OptionCategory, OptionListName, [Description], CodeName)
	SELECT c.countrycode, '01', 'Online Terms Type', '00', 'text', 0, '', '','This is the Terms type to be used when calculating weekly price for Online products.', 'OnlineTermsType'
	FROM Country c
END

IF NOT EXISTS(select * from countrymaintenance where codename = 'OnlineTermsLength')
BEGIN
	INSERT INTO CountryMaintenance(CountryCode, ParameterCategory, Name, Value, [Type], [Precision], OptionCategory, OptionListName, [Description], CodeName)
	SELECT c.countrycode, '01', 'Online Terms Length', 24, 'numeric', 0, '', '','This is the terms length (in months) to be used when calculating weekly price for Online products.', 'OnlineTermsLength'
	FROM Country c
END

