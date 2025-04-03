-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here



IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'FinMonth' AND [object_id] = OBJECT_ID(N'financialweeks'))
BEGIN
	ALTER TABLE financialweeks
		ADD FinMonth smallINT NULL
END
GO

update financialweeks 
	set FinMonth=Case when quarter<4 then Month(startDate)-3 else Month(startDate)+9 end


