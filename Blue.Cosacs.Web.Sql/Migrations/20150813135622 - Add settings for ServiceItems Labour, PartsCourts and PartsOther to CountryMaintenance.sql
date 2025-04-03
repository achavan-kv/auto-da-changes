-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

DECLARE @ServiceItemLabour VARCHAR(20) = (SELECT TOP(1) [Value] FROM CountryMaintenance WHERE CodeName='ServiceItemLabour')
DECLARE @ServiceItemPartsCourts VARCHAR(20) = (SELECT TOP(1) [Value] FROM CountryMaintenance WHERE CodeName='ServiceItemPartsCourts')
DECLARE @ServiceItemPartsOther VARCHAR(20) = (SELECT TOP(1) [Value] FROM CountryMaintenance WHERE CodeName='ServiceItemPartsOther')

DELETE FROM CountryMaintenance WHERE CodeName='ServiceItemLabour';
DELETE FROM CountryMaintenance WHERE CodeName='ServiceItemPartsCourts';
DELETE FROM CountryMaintenance WHERE CodeName='ServiceItemPartsOther';


DECLARE @countryCode CHAR = (SELECT TOP 1 countrycode FROM country)

INSERT INTO CountryMaintenance (CountryCode, ParameterCategory, Name, Value, [Type], [Precision], OptionCategory, OptionListName, [Description], CodeName) VALUES
    (@countryCode, 28, 'Service Item Labour', COALESCE(@ServiceItemLabour, '7L01'), 'text', 0, '', '', 'The generic item number for Labour Charge To', 'ServiceItemLabour'),
    (@countryCode, 28, 'Service Item Parts Courts', COALESCE(@ServiceItemPartsCourts, '7C01'), 'text', 0, '', '', 'The generic item number for Courts Parts Charge To', 'ServiceItemPartsCourts'),
    (@countryCode, 28, 'Service Item Parts Other', COALESCE(@ServiceItemPartsOther, '7L02'), 'text', 0, '', '', 'The generic item number for Other Parts Charge To', 'ServiceItemPartsOther')
