-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

SELECT * INTO #tt FROM transtype WHERE transtypecode ='fee'

UPDATE #tt SET transtypecode = 'SRF', description = 'Store Card Replacement Fee'

IF NOT EXISTS (SELECT * FROM transtype WHERE transtypecode ='SRF')
 INSERT INTO transtype SELECT * FROM #tt
