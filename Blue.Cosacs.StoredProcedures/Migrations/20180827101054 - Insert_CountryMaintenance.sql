
-- Script Comment : Insert Equifax_Variable into CountryMaintenance
-- Script Name : Insert_CountryMaintenance.sql
-- Created For	: BB/BZ/TT
-- Created By	: Nilesh
-- Created On	: CR
-- Modified On	Modified By	Comment

	Declare @Country char(2)
	select  @Country= countrycode from country 
	--Select @Country
IF(@Country='B')
BEGIN
	IF NOT EXISTS (SELECT * FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxAInterceptSign')
	BEGIN
	   INSERT into CountryMaintenance	(CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
	Values (@Country,'07','Equifax Intercept value sign for Applicant','-','text',0,'','','This parameter sets the Equifax Intercept value sign for Applicant score card. It Can be + or - only','EquifaxAInterceptSign');
	END	
	IF NOT EXISTS (SELECT * FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxBInterceptSign')
	BEGIN
	  INSERT into CountryMaintenance	(CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
	Values (@Country,'07','Equifax Intercept value sign for Behavioural','-','text',0,'','','This parameter sets the Equifax Intercept value sign for Behavioural score card. It Can be + or - only','EquifaxBInterceptSign');
	END
	
	IF EXISTS (SELECT * FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxInterceptSign')
	BEGIN
	DELETE FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxInterceptSign'
	END	
	IF NOT EXISTS (SELECT * FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxALogValue')
	BEGIN
	  INSERT into CountryMaintenance	(CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
	Values (@Country,'07','Equifax Log value for Applicant','1','text',0,'','','This parameter sets the Equifax scorecard final formula calucuation value for Applicant','EquifaxALogValue');
	END

	IF EXISTS (SELECT * FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxLogValue')
	BEGIN
	DELETE FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxLogValue'
	END	
	IF NOT EXISTS (SELECT * FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxBLogValue')
	BEGIN
	  INSERT into CountryMaintenance	(CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
	Values (@Country,'07','Equifax Log value for Behavioural','1','text',0,'','','This parameter sets the Equifax scorecard final formula calucuation value for Behavioural','EquifaxBLogValue');
	END
END	
----------------------------------------------------------------------------------------------------
ELSE IF(@Country='Z')  ---for belize
BEGIN
	IF NOT EXISTS (SELECT * FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxAInterceptSign')
	BEGIN
	   INSERT into CountryMaintenance	(CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
	Values (@Country,'07','Equifax Intercept value sign for Applicant','-','text',0,'','','This parameter sets the Equifax Intercept value sign for Applicant score card. It Can be + or - only','EquifaxAInterceptSign');
	END	
	IF NOT EXISTS (SELECT * FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxBInterceptSign')
	BEGIN
	  INSERT into CountryMaintenance	(CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
	Values (@Country,'07','Equifax Intercept value sign for Behavioural','+','text',0,'','','This parameter sets the Equifax Intercept value sign for Behavioural score card. It Can be + or - only','EquifaxBInterceptSign');
	END
	
	IF EXISTS (SELECT * FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxInterceptSign')
	BEGIN
	DELETE FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxInterceptSign'
	END	
	IF NOT EXISTS (SELECT * FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxALogValue')
	BEGIN
	  INSERT into CountryMaintenance	(CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
	Values (@Country,'07','Equifax Log value for Applicant','0.25','text',0,'','','This parameter sets the Equifax scorecard final formula calucuation value for Applicant','EquifaxALogValue');
	END

	IF EXISTS (SELECT * FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxLogValue')
	BEGIN
	DELETE FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxLogValue'
	END	
	IF NOT EXISTS (SELECT * FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxBLogValue')
	BEGIN
	  INSERT into CountryMaintenance	(CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
	Values (@Country,'07','Equifax Log value for Behavioural','1.25','text',0,'','','This parameter sets the Equifax scorecard final formula calucuation value for Behavioural','EquifaxBLogValue');
	END
END
------------------------------------------------------------------------------------------------
ELSE  ----FOR TT AND all other country
BEGIN
	IF NOT EXISTS (SELECT * FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxAInterceptSign')
	BEGIN
	   INSERT into CountryMaintenance	(CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
	Values (@Country,'07','Equifax Intercept value sign for Applicant','+','text',0,'','','This parameter sets the Equifax Intercept value sign for Applicant score card. It Can be + or - only','EquifaxAInterceptSign');
	END	
	IF NOT EXISTS (SELECT * FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxBInterceptSign')
	BEGIN
	  INSERT into CountryMaintenance	(CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
	Values (@Country,'07','Equifax Intercept value sign for Behavioural','+','text',0,'','','This parameter sets the Equifax Intercept value sign for Behavioural score card. It Can be + or - only','EquifaxBInterceptSign');
	END
	
	IF EXISTS (SELECT * FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxInterceptSign')
	BEGIN
	DELETE FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxInterceptSign'
	END	
	IF NOT EXISTS (SELECT * FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxALogValue')
	BEGIN
	  INSERT into CountryMaintenance	(CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
	Values (@Country,'07','Equifax Log value for Applicant','0.25','text',0,'','','This parameter sets the Equifax scorecard final formula calucuation value for Applicant','EquifaxALogValue');
	END

	IF EXISTS (SELECT * FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxLogValue')
	BEGIN
	DELETE FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxLogValue'
	END	
	IF NOT EXISTS (SELECT * FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='EquifaxBLogValue')
	BEGIN
	  INSERT into CountryMaintenance	(CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
	Values (@Country,'07','Equifax Log value for Behavioural','1','text',0,'','','This parameter sets the Equifax scorecard final formula calucuation value for Behavioural','EquifaxBLogValue');
	END
END
-----------------------------------------------------------------------------------------------COMMON	
	IF EXISTS (SELECT * FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='IsOldScoreRunWithEquifax')
	BEGIN
	DELETE FROM [CountryMaintenance] WHERE [ParameterCategory]='07' and CodeName='IsOldScoreRunWithEquifax'
	END	
	INSERT into CountryMaintenance	(CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,
	OptionListName,[Description],CodeName)
		Values (@Country,'07','Is Old Score Run With new Equifax Parallel','False','checkbox',0,'','',
		'This parameter sets if the old score card and new equifax score card run parallel','IsOldScoreRunWithEquifax');
	