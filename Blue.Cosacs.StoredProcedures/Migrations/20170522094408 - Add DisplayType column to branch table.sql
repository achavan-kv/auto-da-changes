-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'Branch' AND  Column_Name = 'DisplayType')
BEGIN
	ALTER TABLE Branch ADD DisplayType VARCHAR(20) 
END

GO

--This section is mainly for testing but does what we need. We need to decide on a definitive way to store this data though.
-- Will we use one letter to define the store type? An acronym? The full name like I've just added, etc.
UPDATE dbo.branch
SET DisplayType = (SELECT CASE WHEN LuckyDollarStore = 1 AND StoreType = 'N' THEN 'LuckyDollar'
					ELSE 
					CASE WHEN AshleyStore = 1 AND StoreType = 'N' THEN 'Ashley'
					ELSE
					CASE WHEN RadioShackStore = 1 AND StoreType = 'N' THEN 'RadioShack'
					ELSE
					CASE WHEN StoreType = 'C' THEN 'Courts'
					END END END END)

GO