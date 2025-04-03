-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
DELETE FROM transtype WHERE transtypecode ='SIN'

SELECT * INTO #TT FROM transtype WHERE transtypecode ='FEE'

UPDATE #TT SET transtypecode ='STA',description = 'Store Card Annual Fee'

IF NOT EXISTS (SELECT * FROM transtype WHERE transtypecode ='STA')
INSERT INTO transtype SELECT * FROM #TT

UPDATE #TT SET transtypecode ='STF',description = 'Store Card Replacement Fee'
IF NOT EXISTS (SELECT * FROM transtype WHERE transtypecode ='STF')
INSERT INTO transtype SELECT * FROM #TT

UPDATE #TT SET transtypecode ='SSF',description = 'Store Card Statement Fee'
IF NOT EXISTS (SELECT * FROM transtype WHERE transtypecode ='SSF')
INSERT INTO transtype SELECT * FROM #TT
GO 
