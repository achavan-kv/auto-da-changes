
IF NOT EXISTS ( SELECT * FROM CountryMaintenance
		        WHERE CodeName = 'Culture')
BEGIN
	INSERT INTO CountryMaintenance
			( CountryCode ,
			  ParameterCategory ,
			  Name ,
			  Value ,
			  Type ,
			  Precision ,
			  OptionCategory ,
			  OptionListName ,
			  Description ,
			  CodeName
			)
	SELECT  CountryCode ,
			'01' ,
			'Culture' ,
			'English (Jamaica)',
			'dropdown' ,
			Precision ,
			OptionCategory ,
			OptionListName ,
			'Sets culture settings for the CoSaCS server.' ,
			'Culture' FROM CountryMaintenance
	WHERE Name LIKE '%Curreny Symbol for print%'
END
