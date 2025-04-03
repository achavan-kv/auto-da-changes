-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Changes Done for Strategy Job Optimization

-- **********************************************************************
-- Version		: 001
-- Title: AddCountryParameterToDoZoneBasedAllocation.sql
-- Developer: Suvidha
-- Date: August 2019
-- Purpose: To Create Country Based Flag To Do Zone Based Allocation

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/08/19  SH  Strategy Job Optimization => Added Flag in CountryMaintenance To Do Zone Based Allocation

-- **********************************************************************

Declare @Country nvarchar(2)
Select @Country = countrycode from country
IF NOT EXISTS (SELECT 1 FROM [CountryMaintenance] WHERE [ParameterCategory] = '01' and CodeName = 'ZoneBasedAllocation')
	BEGIN
		
	    INSERT into CountryMaintenance	(CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
		Values (@Country,'01', 'Zone Based Allocation', 'N','text', 0, '', '', 'This parameter sets the flag to do Bailiff Allocation based on Zones.', 'ZoneBasedAllocation');
	END
