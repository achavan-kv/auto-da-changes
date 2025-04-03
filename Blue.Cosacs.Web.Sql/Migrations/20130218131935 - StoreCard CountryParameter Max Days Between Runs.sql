-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #12341 - CR11571


IF NOT EXISTS(SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'StoreCardMaxDaysEODRun')
BEGIN
	INSERT INTO dbo.CountryMaintenance
	        ( 
			  CountryCode ,
	          ParameterCategory ,
	          Name ,
	          Value ,
	          Type ,
	          Precision ,
	          OptionCategory ,
	          OptionListName ,
	          Description ,
	          CodeName
	        )
	SELECT CountryCode,
		   33,
		   'Store Card Maximum Days Between End Of Day Runs',
		   5,
		   'numeric',
		   0,
		   '',
		   '',
		   'Store Card Statements End Of Day routine will run automatically if the number of days since the last run has exceeded the value set in this parameter',
		   'StoreCardMaxDaysEODRun'
	FROM dbo.country
	
END

IF EXISTS(SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'StoreCardBatchPrint')
BEGIN
	UPDATE dbo.CountryMaintenance
	SET Name = 'Store Card Number of Statements per Batch Print', Description = 'Maximum number of statements per batch print'
	WHERE CodeName = 'StoreCardBatchPrint'
END

