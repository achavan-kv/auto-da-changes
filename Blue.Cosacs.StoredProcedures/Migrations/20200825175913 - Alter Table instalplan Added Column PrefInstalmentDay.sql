-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


-- *************************************************************************************************
-- Developer:    Rahul Sonawane
-- Date:         25 Aug 2020
-- Purpose:      Added New Column in instalplan table to save the Preference day for first installment			
-- *************************************************************************************************

IF NOT EXISTS(SELECT 1 FROM sys.columns
          WHERE Name = N'PrefInstalmentDay'
          AND Object_ID = Object_ID(N'dbo.instalplan'))
BEGIN
    ALTER TABLE instalplan
	ADD PrefInstalmentDay TINYINT;
END
