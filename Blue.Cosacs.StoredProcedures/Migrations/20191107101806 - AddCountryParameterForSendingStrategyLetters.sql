-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Changes Done for Strategy Job Optimization

-- **********************************************************************
-- Version		: 001
-- Title: AddCountryParameterForSendingStrategyLetters.sql
-- Developer: Suvidha
-- Date: August 2019
-- Purpose: To Create Country Based Flag to Send Letters

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/08/19  SH  Strategy Job Optimization => Added Flag in CountryMaintenance To Send Letters And for Zone Based Allocation

-- **********************************************************************

Declare @Country nvarchar(2)
Select @Country = countrycode from country
IF NOT EXISTS (SELECT 1 FROM [CountryMaintenance] WHERE [ParameterCategory]='12' and CodeName='LettersApplicable')
	BEGIN
		
	    INSERT into CountryMaintenance	(CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
		Values (@Country,'12','Send Letters','N','text',0,'','','This parameter sets the flag to send Letters when strategy job is run.','LettersApplicable');
	END

