-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'FinYear' AND [object_id] = OBJECT_ID(N'financialweeks'))
BEGIN
	ALTER TABLE financialweeks
		ADD FinYear INT NULL
END
GO

update financialweeks 
	set FinYear=Case when quarter<4 then Year else Year-1 end

