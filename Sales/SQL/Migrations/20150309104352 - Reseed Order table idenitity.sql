-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
DECLARE @MaxId int

SELECT @MaxId = NextHi
FROM HiLo
WHERE Sequence = 'CashAndGoAgrmtNo'

-- Set current ID to "MaxId"
DBCC CHECKIDENT ('[Sales].[Order]', RESEED, @MaxId)