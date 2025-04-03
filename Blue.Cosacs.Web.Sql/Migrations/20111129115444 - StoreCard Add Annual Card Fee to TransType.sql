-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

DELETE FROM transtype where transtypecode = 'STA'

SELECT * INTO #TT FROM transtype WHERE transtypecode ='FEE'

UPDATE #TT SET transtypecode ='STA',description = 'Store Card Annual Fee'

INSERT INTO transtype SELECT * FROM #TT