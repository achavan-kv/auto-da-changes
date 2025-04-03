-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- new Store Card reversal transaction... 
-- Put your SQL code here

SELECT * INTO #t FROM transtype WHERE transtypecode ='SCT'

UPDATE #t SET transtypecode='STR', description = 'Store Card Refund'
IF NOT EXISTS (SELECT * FROM transtype WHERE transtypecode ='STR')
INSERT INTO transtype 
SELECT * FROM #t

