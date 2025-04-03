-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #12208

IF NOT EXISTS(SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'StoreCardBatchPrint')
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
		   'Store Card Number of Accounts per Batch Print',
		   50,
		   'numeric',
		   0,
		   '',
		   '',
		   'Maximum number of accounts per batch print of Statements',
		   'StoreCardBatchPrint'
	FROM dbo.country
	
END