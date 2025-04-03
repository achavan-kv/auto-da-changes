-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- *************************************************************************************************
-- Developer:    Rahul Sonawane
-- Date:         25 Aug 2020
-- Purpose:      Added New Column in instalplanaudit table for keep change history for the Preference day for first instalment			
-- *************************************************************************************************

IF NOT EXISTS(SELECT 1 FROM sys.columns
          WHERE Name = N'PrefInstalmentDay'
          AND Object_ID = Object_ID(N'dbo.instalplanaudit'))
BEGIN
    ALTER TABLE instalplanaudit
	ADD PrefInstalmentDay TINYINT;
END

IF NOT EXISTS(SELECT 1 FROM sys.columns
          WHERE Name = N'OldPrefInstalmentDay'
          AND Object_ID = Object_ID(N'dbo.instalplanaudit'))
BEGIN
    ALTER TABLE instalplanaudit
	ADD OldPrefInstalmentDay TINYINT;
END

