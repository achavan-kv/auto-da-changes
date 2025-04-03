-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(SELECT * FROM transtype WHERE transtypecode = 'COW')
BEGIN

	SELECT * INTO #transtype
	FROM dbo.transtype
	WHERE transtypecode = 'COS'
	
	UPDATE #transtype
	SET transtypecode = 'COW',
		[description] = 'Broker Cost of Warranty Return'
	
	
	INSERT INTO dbo.transtype
	SELECT * FROM #transtype

END