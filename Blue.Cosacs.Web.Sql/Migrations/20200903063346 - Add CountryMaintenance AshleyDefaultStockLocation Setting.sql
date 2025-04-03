-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


-- Script Comment	: Add CountryMaintenance AshleyDefaultStockLocation Setting

IF NOT EXISTS(select * from CountryMaintenance where codename = 'AshleyDefaultStockLocation')
BEGIN

    INSERT INTO CountryMaintenance (CountryCode, ParameterCategory, Name, Value, Type, Precision, OptionCategory, OptionListName, Description, CodeName)
    SELECT CountryCode, '01', 'Ashley Default Stock Location', '', 'text', 0, '', '', 'Enter details AshleyDefaultStockLocation', 'AshleyDefaultStockLocation'
    FROM Country

END
GO