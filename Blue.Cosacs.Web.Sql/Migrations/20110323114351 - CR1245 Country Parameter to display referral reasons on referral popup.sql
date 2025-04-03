-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here



IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'ReasonsReferPopup')
BEGIN
	INSERT INTO CountryMaintenance(CountryCode,
								   ParameterCategory,
								   NAME,
								   Value,
								   [Type],
								   [PRECISION],
								   OptionCategory,
								   OptionListName,
								   [Description],
								   CodeName)

	SELECT CountryCode,
		   '07',
		   'Display reasons on referral/rejection popup',
		   'True',
		   'checkbox',
		   '0',
		   '',
		   '',
		   'If TRUE, reasons for referral/rejection will be displayed on the referral/rejection popup',
		   'ReasonsReferPopup' 
	FROM dbo.Country
END

