-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
SELECT * INTO #t FROM transtype WHERE transtypecode ='INT'

UPDATE #t SET transtypecode = 'SIN', description = 'Store Card Interest'

GO
INSERT INTO transtype SELECT * FROM #t
GO 