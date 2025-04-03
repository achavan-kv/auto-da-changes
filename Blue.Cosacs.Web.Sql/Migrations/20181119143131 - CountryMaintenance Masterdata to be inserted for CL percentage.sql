 
 IF EXISTS (SELECT * FROM code WHERE category = 'CMC' AND codedescript LIKE 'Credit Sanctioning')
BEGIN
INSERT  INTO CountryMaintenance (CountryCode, ParameterCategory, Name, Value, [Type], [Precision], OptionCategory, OptionListName, [Description], CodeName)
 VALUES ((select countrycode from country),(select code from code WHERE codedescript='Credit Sanctioning'),'Percentage of expenditure1',30,'numeric',0,'',
 '','Tick this field to enable Percentage','CL_Percentageavailable')
END