-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'RIInterfaceOptions')
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
		   '25',
		   'RI Interface Options',
		   'FACT',
		   'dropdown',
		   '0',
		   'RIO',
		   'RIInterfaceOptions',
		   'The following options determine how items/parts are interfaced. FACT - All items will interface to FACT2000. Parts - Parts will interface to FACT2000 and all other items to RI. RI - All items including parts will interface to RI.',
		   'RIInterfaceOptions' 
	FROM dbo.Country
END

IF NOT EXISTS(select * from code where category = 'RIO')
BEGIN

	INSERT INTO CodeCat(category,
						catdescript,
						codelgth,
						forcenum,
						forcenumdesc,
						usermaint)
	SELECT 'RIO',
			'RI Interface Options',
			12,
			'N',
			'N',
			'N'
						
	
	INSERT INTO Code(category,
					 code,
					 codedescript,
					 statusflag,
					 sortorder,
					 reference)
	SELECT  'RIO',
			'01',
			'FACT',
			'L',
			 1,
			 0
	union
	SELECT  'RIO',
			'02',
			'Parts',
			'L',
			1,
			0
	union
	SELECT 'RIO',
			'03',
			'RI',
			'L',
			1,
			0
			
		   
END


IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'RIDispCourtsCode')
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
		   '13',
		   'Display Courts Code',
		   'false',
		   'checkbox',
		   '0',
		   '',
		   '',
		   'If true, Courts item number will be displayed as well as the IUPC. If false only IUPC will be displayed.',
		   'RIDispCourtsCode' 
	FROM dbo.Country
END