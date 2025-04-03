-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns
WHERE column_name = 'BehaveScoreFrom'
AND table_name ='storeCardRateDetails')
BEGIN
	EXEC sp_RENAME 'storeCardRateDetails.ScoreFrom' , 'AppScoreFrom', 'COLUMN'
	EXEC sp_RENAME 'storeCardRateDetails.ScoreTo' , 'AppScoreTo', 'COLUMN'

    
	ALTER TABLE storeCardRateDetails ADD BehaveScoreFrom SMALLINT ,
	BehaveScoreTo SMALLINT 

END